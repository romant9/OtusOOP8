using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Homework8
{
    public class Dog : Creature
    {
        //Базовое движение. Отскок от стенок под углом из рандомного диапазона
        //Для всех собак отличается только скорость
        private IEnumerator MoveDog(float localSpeed)
        {
            Vector2 normal;
            Vector2 normalCurrent = Vector2.zero;
            while (!isFreeze)
            {
                boundsSprite = GetComponent<SpriteRenderer>().bounds;
                if (DogSpawner.IsIntersectDirection(boundsSprite, _boundsScreen, out normal))
                {
                    if (normalCurrent != normal)
                    {
                        currentDirection = Vector2.Reflect(currentDirection, normal);
                        float randomAngle = 90 - Vector2.Angle(currentDirection, normal);
                        currentDirection = Quaternion.AngleAxis(.8f * Random.Range(-randomAngle, randomAngle), Vector3.forward) * currentDirection;
                        normalCurrent = normal;
                    }
                }
                transform.position += currentDirection * Time.deltaTime * localSpeed * _baseSpeed;

                yield return null;
            }
        }
        //доп движение. Собаки слетаются к игроку и следуют за ним
        private IEnumerator AnimateMoveHappy()
        {
            while (!isFreeze)
            {
                targetPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 targetPosCorrected = targetPos + randomOffset;
                transform.position = SmoothDamp(transform.position, targetPosCorrected, DumpFactor());

                yield return null;
            }
        }
        //доп движение. Собаки шухеряться по разным углам
        private IEnumerator AnimateMoveSad()
        {
            float startTime = 0;
            targetPos = RandomPos(_boundsScreen, boundsSprite);
            Vector2 velocity = Vector2.zero;
            Vector2 randomPos = targetPos;
            while (!isFreeze)
            {
                startTime += Time.deltaTime;
                if (startTime >= 1.5f)
                {
                    randomPos = RandomPos(_boundsScreen, boundsSprite);
                    startTime = 0;
                }
                targetPos = Vector2.SmoothDamp(targetPos, randomPos, ref velocity, .75f);
                transform.position = SmoothDamp(transform.position, targetPos, DumpFactor());

                yield return null;
            }
        }
        //доп движение. Собаки-зомби ищут блидайшую цель-не зомби и следуют за ней
        //поймав цель - превращают в зомби
        //если целей нет - то используют базовое движение
        private IEnumerator AnimateMoveZombie()
        {
            List<Transform> aliveDogs = Ds.spawnedObjects.Where(x => x.behavior != CreatureBehavior.Zombie).Select(x => x.transform).ToList();
            Transform target = aliveDogs.OrderBy(x => (x.position - transform.position).sqrMagnitude).FirstOrDefault();
            if (target != null)
            {
                while (!isFreeze)
                {
                    transform.position = SmoothDamp(transform.position, target.position, DumpFactor());
                    if (Vector2.Distance(transform.position, target.position) < 1)
                        ChangeSettings(this, target.GetComponent<Creature>());

                    yield return null;
                }
            }
            else
            {
                StartCoroutine(MoveDog(_baseSpeed));
            }
        }

        protected override void GetBaseMotion()
        {
            StartCoroutine(MoveDog(localSpeed));
        }
        protected override void GetCustomMotion()
        {
            IEnumerator customMove = CurrentCoroutine(behavior);
            StartCoroutine(customMove);
        }
        private IEnumerator CurrentCoroutine(CreatureBehavior behavior)
        {
            switch (behavior)
            {
                case CreatureBehavior.Happy:
                    return AnimateMoveHappy();
                case CreatureBehavior.Sad:
                    return AnimateMoveSad();
                case CreatureBehavior.Zombie:
                    return AnimateMoveZombie();
                default:
                    return AnimateMoveHappy();
            }
        }
    }
}
