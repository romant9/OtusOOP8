using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class OrderManager : MonoBehaviour
{
    public static OrderManager OM;
    //[Multiline]
    //может браться из файла и делиться построчно \n
    public string sortString;
    //сортировать все из unsordedList или только те, что в списке sortString
    public bool sortAllObjects;
    public List<GameObject> unsordedList = new List<GameObject>();
    public List<GameObject> sortedList = new List<GameObject>();

    public MeshFilter invertMeshObject;
    public bool invert;
    
    public Transform LookingForObject;
    public GameObject markersGroup;
    public List<Transform> markers = new List<Transform>();
    public List<Transform> wayMarkers = new List<Transform>();

    #region бонусный поиск MaterialID
    private bool mouseRaycast;
    private bool doRaycast;
    public LayerMask raycastMask;
    private GameObject _raycastObject;
    Ray ray;
    RaycastHit hit;
    Camera cam;
    private bool colorChange;
    Color newCol;
    #endregion

    public float speed = 1;
    private bool pathIsGenerated;

    public Action SortAction;
    public Action InvertNormalsAction;
    public Action PathFindingAction;


#if UNITY_2017_1_OR_NEWER
    private bool oldMethod = false;
#else
    private bool oldMethod = true;
#endif

    void Start()
    {
        OM = this;
        cam = Camera.main;       

        markers = markersGroup.transform.GetComponentsInChildren<Transform>().Where(x => x.parent == markersGroup.transform).ToList();

        InvertNormalsAction += InvertNormalsInit;
        PathFindingAction += PathFindingInit;
        SortAction += SortInit;
    }
    private void OnDisable()
    {
        InvertNormalsAction -= InvertNormalsInit;
        PathFindingAction -= PathFindingInit;
        SortAction -= SortInit;
    }
    void Update()
    {        
        if (doRaycast)
        {
            EyeRaycast();
            if (Input.GetMouseButtonDown(1))
            {
                colorChange = !colorChange;
                ChangeColor(colorChange);
            }
        }
    }
    void SortInit()
    {
        unsordedList.Clear();
        unsordedList = GetComponentsInChildren<Transform>().Where(x => x.parent == transform).Select(x => x.gameObject).ToList();
        sortedList = SortedList(sortString, unsordedList);
    }
    //сортировка любой коллекции по строке со списком имен
    public List<GameObject> SortedList(string _sortString, List<GameObject> _unsortedList)
    {
        List<string> OrderByName = _sortString.Split(' ').ToList();
        List<GameObject> sortedList = new List<GameObject>();
        int repeat = 0;
        foreach (var name in OrderByName)
        {
            var obj = _unsortedList.Find(x => x.name == name);
            if (obj != null)
            {
                if (sortedList.Contains(obj)) repeat++;
                sortedList.Add(obj);
            }
        }
        if ((sortedList.Count != _unsortedList.Count || repeat > 0) && sortAllObjects)
        {
            //лишние объекты сортируются просто по имени, но можно придумать свое условие
            sortedList.AddRange(_unsortedList.Except(sortedList).OrderBy(x => x.name));
        }
        return sortedList;
    }

    void InvertNormalsInit()
    {
        InvertNormals(invertMeshObject);
    }
    //Метод, реализующий сортировку индексов трианглов для инвертирования нормалей Меша
    void InvertNormals(MeshFilter filter)
    {
        Mesh mesh = filter.mesh;
        Vector3[] normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -normals[i];
        }
        mesh.normals = normals;

        if (oldMethod)
        {
            int[] triangles = mesh.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int t = triangles[i];
                triangles[i] = triangles[i + 2];
                triangles[i + 2] = t;
            }
            mesh.triangles = triangles;
        }
        else mesh.triangles = mesh.triangles.Reverse().ToArray();

    }
  
    //Поиск пути. Ищется ближайший waypoint к камере. Затем идет поиск ближайших waypoints из заданной области вокруг последнего waypoint
    //Из ближайших waypoints выбирается один ближайший к цели
    void PathFindingInit()
    {
        wayMarkers.Clear();
        Transform nearestFirst = NearestMarkerToCam(markers, cam.transform.position);
        wayMarkers.Add(nearestFirst);
        StartCoroutine(AddWayPoints());
    }
    private IEnumerator MoveByPath()
    {
        float t = 0;
        int index = 0;
        Vector3 velocityCam = Vector3.zero;
        Vector3 velocityTarget = Vector3.zero;

        Transform cameraTarget = new GameObject("cameraTarget").transform;
        cameraTarget.position = wayMarkers[1].position;
        while (index < wayMarkers.Count && pathIsGenerated)
        {

            t += Time.deltaTime;
            cameraTarget.position = Vector3.SmoothDamp(cameraTarget.position, index + 1 >= wayMarkers.Count ? LookingForObject.transform.position : wayMarkers[index + 1].position, ref velocityTarget, .75f/speed);
            cam.transform.LookAt(cameraTarget.position);
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, wayMarkers[index].position, ref velocityCam, 1/speed);

            if (t > 1/speed)
            {
                index += 1;
                t = 0;
            }
            yield return null;
        }
        Destroy(cameraTarget.gameObject);
        pathIsGenerated = false;
    }
    private IEnumerator AddWayPoints()
    {
        while (Vector3.Distance(wayMarkers.Last().position, LookingForObject.transform.position) > .6f) 
        {
            Transform lastPos = wayMarkers.Last();
            Transform nearestNext = NearestMarkerToTarget(markers.Except(wayMarkers).ToList(), lastPos.position, LookingForObject.transform.position);
            Debug.DrawLine(nearestNext.position, lastPos.position, Color.blue,10);
            wayMarkers.Add(nearestNext);
            yield return null;
        }
        pathIsGenerated = true;
        StartCoroutine(MoveByPath());
    }
    //Поиск первого waypoint
    public Transform NearestMarkerToCam(List<Transform> markers, Vector3 cam)
    {
        Transform nearest = markers[0];
        int nearestIndex = 0; 
        float distanceCurrent = 0;
        float distanceNext;
        for (int i = 0; i < markers.Count-1; i++)
        {
            nearest = markers[nearestIndex];
            if (distanceCurrent == 0) distanceCurrent = Vector3.Distance(markers[nearestIndex].position, cam);
            distanceNext = Vector3.Distance(markers[i + 1].position, cam);

            if (distanceNext < distanceCurrent)
            {
                nearest = markers[i + 1];
                nearestIndex = i + 1;
            }
        }       
        return nearest;
    }
    //поиск всех остальных waypoints
    public Transform NearestMarkerToTarget(List<Transform> markers, Vector3 cam, Vector3 target)
    {
        List<Transform> nearestInArea = new List<Transform>();

        for (int i = 0; i < markers.Count; i++)
        {
            if (Vector3.Distance(markers[i].position, cam) < 1.2f)
            {
                nearestInArea.Add(markers[i]);
            }
        }
        Transform nearest = nearestInArea[0];
        int nearestIndex = 0;
        float distanceCurrent = 0;
        float distanceNext;
        for (int i = 0; i < nearestInArea.Count - 1; i++)
        {
            nearest = nearestInArea[nearestIndex];
            if (distanceCurrent == 0) distanceCurrent = Vector3.Distance(nearestInArea[nearestIndex].position, target);
            distanceNext = Vector3.Distance(nearestInArea[i + 1].position, target);

            if (distanceNext < distanceCurrent)
            {
                nearest = nearestInArea[i + 1];
                nearestIndex = i + 1;
            }
        }
        return nearest;
    }


    public void DoRaycast(Toggle tg)
    {
        doRaycast = tg.isOn;
        mouseRaycast = doRaycast;
    }
    //бонус. Изолирование всех объектов по референсному цвету, взятого из raycast объекта.
    //Реализован поиск materialID
    //Для корректной работы следует брать raycast MousePosition 
    public void ChangeColor(bool sign)
    {
        var changeCutObjs = FindObjectsOfType<Renderer>().ToList();
        string mayName = "";
        if (_raycastObject != null)
        {
            var materialIdx = GetMatID();
            mayName = _raycastObject.GetComponent<Renderer>().sharedMaterials[materialIdx].name;
        }

        foreach (var obj in changeCutObjs)
        {
            var mat = obj.sharedMaterials;
            for (int i = 0; i < mat.Length; i++)
            {
                if (sign && !string.IsNullOrEmpty(mayName))
                {
                    if (mat[i].name != mayName)
                    {
                        newCol = mat[i].color;
                        mat[i].SetFloat("_Mode", 2f);
                        mat[i].SetInt("_ZWrite", 1);
                        mat[i].EnableKeyword("_ALPHATEST_ON");
                        mat[i].DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        newCol.a = 0.0f;
                        mat[i].color = newCol;
                    }
                }
                else
                {
                    newCol = mat[i].color;
                    newCol.a = 1f;
                    mat[i].color = newCol;
                    mat[i].SetFloat("_Mode", 0f);
                    mat[i].DisableKeyword("_ALPHAPREMULTIPLY_ON");
                }
            }
        }
        changeCutObjs.Clear();
    }
    public int GetMatID()
    {
        int materialIdx;
        var triangleIdx = hit.triangleIndex;
        var mesh = _raycastObject.GetComponent<MeshFilter>().mesh;
        var subMeshesNr = mesh.subMeshCount;
        var lookupIdx1 = mesh.triangles[triangleIdx * 3];
        var lookupIdx2 = mesh.triangles[triangleIdx * 3 + 1];
        var lookupIdx3 = mesh.triangles[triangleIdx * 3 + 2];
        materialIdx = -1;

        for (int i = 0; i < subMeshesNr; i++)
        {
            var tr = mesh.GetTriangles(i);
            for (int j = 0; j < tr.Length; j += 3)
            {
                if (tr[j] == lookupIdx1 && tr[j + 1] == lookupIdx2 && tr[j + 2] == lookupIdx3)
                {
                    materialIdx = i;
                    break;
                }
            }
            if (materialIdx != -1) break;
        }
        return materialIdx;
    }
    private void EyeRaycast()
    {
        if (mouseRaycast)
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
        }
        else ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out hit, 100, raycastMask))
        {
            _raycastObject = hit.transform.gameObject;

        }
        
    }
}
