using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Homework8
{
    public abstract class Creature : MonoBehaviour
    {
        public DogSpawner Ds => DogSpawner.DS;
        protected AnimationCurve _curve;

        public Sprite sprite;
        public string voice;
        public Color color;

        protected Vector3 currentDirection;
        protected Vector2 savedDirection;

        //скорость дл€ всех - наследуетс€
        protected float _baseSpeed;
        //скорость зависит от типа и поведени€
        public float localSpeed;
        //коэффециент дл€ плавного дижени€ - наследуетс€
        public float _dumpMult;
        //относительные границы скорости по отношению к длине перемещени€
        //дл€ SmoothDump нужно помен€ть min и max. Ќе разобралс€ почему дл€ Lerp наоборот
        public float speedMax = 20;
        public float speedMin = 80;

        protected bool isFreeze;
        protected Bounds _boundsScreen;
        protected Bounds boundsSprite;

        protected Vector2 targetPos;
        protected Vector2 randomOffset;
        public enum CreatureType
        {
            Character,
            Dog,
            Cat
        }
        public enum CreatureBehavior
        {
            Happy,
            Sad,
            Angry,
            Zombie
        }
        public CreatureBehavior behavior { get; set; }
        public CreatureType type { get; set; }

        private void Start()
        {
            _curve = Ds.curve;
            _baseSpeed = Ds.baseSpeed;
            _dumpMult = Ds.dumpMult;
            _boundsScreen = Ds.boundsScreen;

            Ds.FreezeAll += FreezeCreature;
            Ds.UnFreezeAll += UnFreezeCreature;
            Ds.BaseMotion += BaseMotion;
            Ds.CustomMotion += CustomMotion;
            Ds.Speaking += Speaking;
            Ds.ChangeColor += ChangeColor;

            Initiate(true);
        }
        private void OnDestroy()
        {
            Ds.FreezeAll -= FreezeCreature;
            Ds.UnFreezeAll -= UnFreezeCreature;
            Ds.BaseMotion -= BaseMotion;
            Ds.CustomMotion -= CustomMotion;
            Ds.Speaking -= Speaking;
            Ds.ChangeColor -= ChangeColor;
        }
        private void FreezeCreature()
        {
            StopAllCoroutines();
            isFreeze = true;
        }
        private void UnFreezeCreature()
        {
            isFreeze = false;
            Initiate(true);
        }
        protected void BaseMotion()
        {
            currentDirection = savedDirection;
            StopAllCoroutines();
            Initiate(true);
        }
        protected void CustomMotion()
        {
            savedDirection = currentDirection;
            StopAllCoroutines();

            randomOffset = Random.onUnitSphere * 2;
            Initiate(false);
        }

        protected void Speaking()
        {
            float distance = ((Vector2)(transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition))).magnitude;
            if (distance < 2)
            {
                bool isSpeak = Ds.isSpeaking;
                transform.GetChild(0).gameObject.SetActive(isSpeak);
            }
        }

        protected void ChangeColor()
        {
            bool colorIsChanging = Ds.colorIsChanging;
            var sprite = GetComponent<SpriteRenderer>();
            Color baseCol = sprite.color;
            if (colorIsChanging)
            {
                sprite.color = color;
                color = baseCol;
            }
            else
            {
                sprite.color = color;
                color = baseCol;
            }
        }
        protected void ChangeSettings(Creature creatureSource, Creature creatureTarget)
        {
            creatureTarget.type = creatureSource.type;
            creatureTarget.behavior = creatureSource.behavior;
            creatureTarget.sprite = creatureTarget.GetComponent<SpriteRenderer>().sprite = creatureSource.sprite;
            creatureTarget.voice = creatureTarget.transform.GetComponentInChildren<Text>(true).text = creatureSource.voice;
            creatureTarget.name = creatureSource.name;
            creatureTarget.localSpeed = creatureSource.localSpeed;
            creatureTarget.color = creatureSource.color;
            creatureTarget.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
        protected virtual void Initiate(bool basemotion)
        {
            if (basemotion == true)
            {
                //базовое движение через Coroutine
                if (currentDirection == Vector3.zero)
                    currentDirection = RotateTowardsUp(transform.position, Random.Range(0f, 360f));
                GetBaseMotion();
            }
            else
            {
                //кастомное движение через Coroutine
                GetCustomMotion();
            }
        }
        protected virtual void GetBaseMotion()
        {
        }
        protected virtual void GetCustomMotion()
        {
        }
        protected virtual void Update()
        {
        }
        //поворот на заданный угол
        protected static Vector3 RotateTowardsUp(Vector3 start, float angle)
        {
            start.Normalize();
            Vector3 axis = Vector3.Cross(start, Vector3.up);
            if (axis == Vector3.zero) axis = Vector3.right;

            return Quaternion.AngleAxis(angle, axis) * start;
        }
        //плавное движение
        protected Vector2 SmoothDamp(Vector2 position, Vector2 target, float dumpFactor)
        {
            //Vector2 velocity = Vector2.zero;
            //position = Vector2.SmoothDamp(position, target, ref velocity, dumpFactor / _baseSpeed * Time.deltaTime);
            position = Vector2.LerpUnclamped(position, target, dumpFactor * _baseSpeed * .01f * Time.deltaTime);
            return position;
        }
        //коэффециент изменени€ скорости от рассто€ни€ до цели
        protected float DumpFactor()
        {
            float dirLength = Mathf.Clamp(Vector2.Distance(transform.position, targetPos), 1, 25);
            float speedClamped = Mathf.Clamp01((dirLength - 1) / 25);
            float speedCurved = _curve.Evaluate(speedClamped);
            
            //return speedMin / speedMax * speedCurved * _dumpMult;
            return ((speedMax - speedMin) * speedCurved + speedMin) * _dumpMult;
        }
        protected static Vector2 RandomPos(Bounds bounds, Bounds sprite)
        {
            float x;
            float y;
            bool xFirst = Random.Range(0, 2) == 0;
            if (xFirst)
            {
                x = Random.Range(bounds.min.x, bounds.max.x);
                bool yIsUp = Random.Range(0, 2) == 0;
                y = yIsUp ? bounds.max.y - sprite.size.y : bounds.min.y + sprite.size.y;
            }
            else
            {
                y = Random.Range(bounds.min.y, bounds.max.y);
                bool xIsRight = Random.Range(0, 2) == 0;
                x = xIsRight ? bounds.max.x - sprite.size.x : bounds.min.x + sprite.size.x;
            }
            return new Vector2(x, y);
        }
    }
}
