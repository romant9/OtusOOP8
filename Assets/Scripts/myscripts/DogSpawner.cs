using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System;
using Random = UnityEngine.Random;
using System.Linq;

namespace Homework8
{
    public class DogSpawner : MonoBehaviour
    {
        public static DogSpawner DS;
        public AnimationCurve curve;

        [SerializeField] private int totalAmount;
        internal int currentAmount;

        [SerializeField] private float spawnDelay;
        //ќбща€ скорость
        public float baseSpeed;
        //коэффециент дл€ плавного движени€
        public float dumpMult;
        [SerializeField] private List<Sprite> creatureSprites;
        [SerializeField] private Sprite character;

        private bool isSpawning;
        private bool isSpawnZombies = true;

        [HideInInspector] public bool isUI;

        [SerializeField][Range(0, 100)]       
        private int maxDistance;

        public List<Creature> spawnedObjects { get; private set; }

        public Action FreezeAll;
        public Action UnFreezeAll;
        public Action BaseMotion;
        public Action CustomMotion;
        public Action ChangeColor;
        public bool colorIsChanging { get; private set; }

        public Action Speaking;
        public bool isSpeaking { get; private set; }
        //sound prefab
        [SerializeField] private GameObject speakPrefab;
 
        internal Bounds boundsScreen;

        public void Start()
        {
            DS = this;
            
            float screenHalfHeight = Camera.main.orthographicSize;
            float screenHalfWidth = screenHalfHeight * Screen.width / Screen.height;
            boundsScreen = new Bounds(
                Vector2.zero,
                new Vector2(screenHalfWidth, screenHalfHeight) * 2);
            
            currentAmount = totalAmount;
            spawnedObjects = new List<Creature>();

            //SetValues();
        }

        private void SetValues()
        {
            totalAmount = 6;
            spawnDelay = .2f;
            baseSpeed = 4;
            dumpMult = 1;
            maxDistance = 50;
            curve = new AnimationCurve(
                new Keyframe(0, 0),
                new Keyframe(1, 1));
            //curve = new AnimationCurve(
            //    new Keyframe(0, 0, 1.5f ,1.5f),
            //    new Keyframe(.85f, .72f, 1, 1),
            //    new Keyframe(1, 1, 2.83f, 2.83f));
        }

        public void StartSpawner(bool isFreeze)
        {
            if (!isFreeze)
            {
                isSpawning = true;
                UnFreezeAll?.Invoke();
                if (spawnedObjects.Count == 0) StartCoroutine(CreaturesSpawner(currentAmount));
            }
            else
            {
                isSpawning = false;
                StopAllCoroutines();
                FreezeAll.Invoke();
            }
        }

        public void ResetSpawner()
        {
            foreach (var go in spawnedObjects)
            {
                Destroy(go.gameObject);
            }
            spawnedObjects.Clear();
            StartSpawner(false);
        }

        public void SpawnZombies()
        {
            isSpawnZombies = !isSpawnZombies;
        }
        private void Update()
        {
            if (currentAmount != totalAmount)
            {
                int creatureRange = Math.Abs(totalAmount - currentAmount);
                if (totalAmount > currentAmount)
                {
                    StartCoroutine(CreaturesSpawner(creatureRange));
                }
                else
                {
                    if (spawnedObjects.Count > 0)
                    {
                        foreach (var c in spawnedObjects.GetRange(spawnedObjects.Count - creatureRange, creatureRange))
                        {
                            GameObject g = c.gameObject;
                            spawnedObjects.Remove(c);
                            Destroy(g);
                        }
                    }
                }
                currentAmount = totalAmount;
            }

            if (!isUI)
            {
                //кастомное движение
                if (Input.GetMouseButtonDown(0))
                {
                    CustomMotion?.Invoke();
                }
                //фоновое движение
                if (Input.GetMouseButtonUp(0))
                {
                    BaseMotion?.Invoke();
                }
            }
            if (Input.GetMouseButton(1)) 
            {
                isSpeaking = true;
                Speaking?.Invoke();     
            }
            if (Input.GetMouseButtonDown(1))
            {
                colorIsChanging = true;
                ChangeColor?.Invoke();
            }
            if (Input.GetMouseButtonUp(1))
            {
                isSpeaking = false;
                Speaking?.Invoke();

                colorIsChanging = true;
                ChangeColor?.Invoke();
            }
        }     

        private IEnumerator CreaturesSpawner(int amount)
        {
            int i = 0;
            while (isSpawning && i < amount)
            {
                Transform obj = new GameObject().transform;
                obj.parent = transform;
                var maxDistanceNormalized = maxDistance * Camera.main.orthographicSize / 100;
                obj.position = new Vector3(
                    Random.Range(0f, maxDistanceNormalized * Screen.width / Screen.height),
                    Random.Range(0f, maxDistanceNormalized));

                int id = Random.value > .3f ? 0 : 1;
                Creature creature = SwitchCreature(obj.gameObject, id);
                SwitchBehaviour(Random.Range(0, isSpawnZombies ? 4 : 3), creature);
                string prefix = creature.type.ToString().ToLower();
                string postfix = creature.behavior.ToString().ToLower();
                string creatureName = $"{prefix}_{postfix}";
                obj.name = creatureName;

                Sprite creatureSprite = creatureSprites.Find(x => x.name == creatureName);
                if (creatureSprite == null)
                {
                    Destroy(obj.gameObject);
                    continue;
                }
               
                creature.sprite = obj.gameObject.AddComponent<SpriteRenderer>().sprite = creatureSprite;
                if (creature.behavior == Creature.CreatureBehavior.Zombie)
                    creature.GetComponent<SpriteRenderer>().sortingOrder = 1;

                GameObject creatureVoice = Instantiate(speakPrefab, obj);
                creature.voice = creatureVoice.GetComponentInChildren<Text>().text = creature.voice;
                creatureVoice.SetActive(false);
                spawnedObjects.Add(creature);
                i++;
                yield return new WaitForSeconds(spawnDelay);
            }
        }

        private Creature SwitchCreature(GameObject go, float id)
        {
            Creature comp;
            switch (id)
            {
                case 0:
                    comp = go.AddComponent<Dog>();
                    comp.type = Creature.CreatureType.Dog;
                    return comp;
                case 1:
                    comp = go.AddComponent<Cat>();
                    comp.type = Creature.CreatureType.Cat;
                    return comp;
                default:
                    return null;
            }
        }

        private void SwitchBehaviour(float id, Creature creature)
        {
            if (creature.type == Creature.CreatureType.Cat)
            {
                creature.behavior = Creature.CreatureBehavior.Happy;
                creature.localSpeed = 1.5f;
                creature.color = Color.grey;
                creature.voice = "м€у-м€у!";               
            }
            else
            {
                switch (id)
                {
                    case 0:
                        creature.behavior = Creature.CreatureBehavior.Happy;
                        creature.localSpeed = 2;
                        creature.color = Color.green;
                        creature.voice = "гав-гав!";
                        break;
                    case 1:
                        creature.behavior = Creature.CreatureBehavior.Sad;
                        creature.localSpeed = 1.5f;
                        creature.color = new Color(1, .5f, 1);
                        creature.voice = "ай-ай-ох!";
                        break;
                    case 2:
                        creature.behavior = Creature.CreatureBehavior.Angry;
                        creature.localSpeed = 2.5f;
                        creature.color = Color.yellow;
                        creature.voice = "рр––––ррафф!!!";
                        break;
                    case 3:
                        creature.behavior = Creature.CreatureBehavior.Zombie;
                        creature.localSpeed = .75f;
                        creature.color = new Color(1, 0, 0);
                        creature.voice = "ээ-аа-уу-хрр";                        
                        break;
                    default:
                        break;
                }
            }            
        }

       public static bool IsIntersectDirection(Bounds bSprite, Bounds bScreen, out Vector2 normal)
       {
            if (bSprite.min.x <= bScreen.min.x) { normal = Vector2.right; return true; }
            else if (bSprite.max.x >= bScreen.max.x) { normal = Vector2.left; return true; }
            else if (bSprite.min.y <= bScreen.min.y) { normal = Vector2.up; return true; }
            else if (bSprite.max.y >= bScreen.max.y) { normal = Vector2.down; return true; }
            else { normal = Vector2.zero; return false; }
       }
    }
}

