using Homework.Common;
using Homework.Movement;
using UnityEngine;

namespace Homework.Dogs
{
    /**
     * TODO:
     * 1. Добавить всем собакам способность гавкать: достаточно метода, пишущего в Unity-консоль строку с сообщение.
     * 2. HappyDog должен гавкать более радостно.
     * 3. (сложно) Пусть собаки гавкают только тогда, когда меняют направление движения.
     */
    public abstract class Dog : MonoBehaviour, IColorChangeable
    {
        public abstract void ChangeColor();

        protected Move Move;


        protected void Start()
        {
            Move = new Walk(this, -4, 4, 1);

            InputController.Instance.OnColorChanged += OnColorChanged;
        }

        protected void Update()
        {
            Move.Execute();
        }

        private void OnDestroy()
        {
            InputController.Instance.OnColorChanged -= OnColorChanged;
        }

        private void OnColorChanged()
        {
            ChangeColor();
        }
    }
}