using UnityEngine;
using System.Collections;

public class CameraFlyController : MonoBehaviour
{
	public float speedMove = 4f;
    public float speedRotate = 4f;

    private Vector3 mpStart;
	private Vector3 originalRotation;

	//private float t = 0f;
	
	
	void Awake()
	{
		//t = Time.realtimeSinceStartup;
	}
	
	
	void Update()
	{
		// Movement
		float forward = 0f;
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) { forward += 1f; }
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) { forward -= 1f; }
		
		float right = 0f;
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) { right += 1f; }
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) { right -= 1f; }
		
		float up = 0f;
		if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Space)) { up += 1f; }
		if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.C)) { up -= 1f; }

		//float dT = Time.realtimeSinceStartup - t;
		//t = Time.realtimeSinceStartup;

		transform.position += transform.TransformDirection(new Vector3(right, up, forward) 
			* speedMove * (Input.GetKey(KeyCode.LeftShift) ? 2f : 1f) * Time.deltaTime);
		
		Vector3 mpEnd = Input.mousePosition;
		
		if (Input.GetMouseButtonDown(1))
		{
			originalRotation = transform.localEulerAngles;
			mpStart = mpEnd;
		}
		
		if (Input.GetMouseButton(1))
		{
			Vector2 offs = new Vector2((mpEnd.x - mpStart.x) / Screen.width, (mpStart.y - mpEnd.y) / Screen.height) * speedRotate;
			transform.localEulerAngles = originalRotation + new Vector3(offs.y * 360f, offs.x * 360f, 0f);
		}
	}
}
