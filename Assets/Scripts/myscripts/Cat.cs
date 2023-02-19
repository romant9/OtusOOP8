using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Homework8
{
    public class Cat : Creature
    {
        //Ѕазовое движение. ¬ращение вокруг сдвигаемого по Perlin Noise центра
        //ƒл€ всех кошек отличаетс€ только скорость
        private IEnumerator MoveCat(float localSpeed)
        {
            localSpeed *= Random.Range(.8f, 1.2f);
            float radius = Random.Range(8f, 12f);
            targetPos = _boundsScreen.center;

            while (!isFreeze)
            {
                targetPos = new Vector2(Mathf.PerlinNoise(Time.time / 5, 0) - .5f, Mathf.PerlinNoise(Time.time / 5, 10) - .5f) * radius;

                if ((targetPos - (Vector2)transform.position).magnitude > radius)
                    transform.position = SmoothDamp(transform.position, targetPos, DumpFactor());
                else
                {
                    transform.RotateAround(targetPos, Vector3.forward, localSpeed * 60 * Time.deltaTime);
                    transform.localEulerAngles = Vector3.zero;
                }

                yield return null;
            }
        }
        protected override void GetBaseMotion()
        {
            StartCoroutine(MoveCat(localSpeed));
        }
        //доп движение. ¬ данном случае ускоренное базовое движение
        protected override void GetCustomMotion()
        {
            StartCoroutine(MoveCat(localSpeed * 3));
        }
    }
}
