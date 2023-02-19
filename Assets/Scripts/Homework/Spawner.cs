using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Homework
{
    /**
     * TODO:
     * 1. Найти примеры полиморфизма в уже написанном коде и в том, что будет написан вами.
     * 2. Реализовать удаление объектов из коллекции _spawnedObjects.
     * 3. Заменить тип коллекции на более подходящий к данному случаю. Объяснить, если замена не требуется.
     */
    public class Spawner : MonoBehaviour
    {
        [SerializeField]
        private int _totalAmount;

        [SerializeField]
        private float _spawnDelay;

        [SerializeField]
        private List<GameObject> _objectsToSpawn;

        private readonly List<GameObject> _spawnedObjects = new List<GameObject>();


        void Start()
        {
            StartCoroutine(SpawnNext());
        }

        //private IEnumerator SpawnNext()
        //{
        //    var random = new System.Random();
        //    int i;

        //    while (true)
        //    {
        //        yield return new WaitForSeconds(_spawnDelay);

        //        Debug.Log("Next");
        //        if (_spawnedObjects.Count < _totalAmount)
        //        {
        //            i = random.Next(_objectsToSpawn.Count);

        //            var spawnedObject = Instantiate(_objectsToSpawn[i], transform);

        //            _spawnedObjects.Add(spawnedObject);
        //        }
        //    }
        //}

        private IEnumerator SpawnNext()
        {
            var random = new System.Random();
            int i;

             

                while (_spawnedObjects.Count < _totalAmount)
                {
                    Debug.Log("Next");

                    i = random.Next(_objectsToSpawn.Count);

                    var spawnedObject = Instantiate(_objectsToSpawn[i], transform);

                    _spawnedObjects.Add(spawnedObject);
                    yield return new WaitForSeconds(_spawnDelay);
                }
            
        }
    }
}