using UnityEngine;

namespace Homework.Dogs
{
    public class HappyDog : Dog
    {
        private SpriteRenderer _spriteRenderer;

        private SpriteRenderer GetSpriteRenderer()
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            return _spriteRenderer;
        }


        public override void ChangeColor()
        {
            var random = new System.Random();
            var red = (float)random. NextDouble();
            Debug.Log(red);
            GetSpriteRenderer().color = new Color(0.5f + red / 2, 0.1f, 0.1f);
        }
    }
}