using System;
using UnityEngine;

namespace Homework
{
    public class InputController : MonoBehaviour
    {
        public static InputController Instance;

        public Action OnColorChanged;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1"))
            {
                OnColorChanged?.Invoke();
            }
        }
    }
}