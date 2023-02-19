using UnityEngine;

namespace Delegates
{
    public class EventConsumer : MonoBehaviour
    {
        private int health;

        public int Health
        {
            get => health;
            set
            {
                // if ...
                
                health = value;
            }
        }

        protected void Start()
        {
            if (DelegatesExample.Instance.IsReady)
            {
                Initialize();
            }
            else
            {
                DelegatesExample.Instance.OnDataReady += OnDataReady;
            }
        }

        private void OnDataReady()
        {
            Initialize();
        }

        private void Initialize()
        {
            Debug.Log("EventConsumer.Initialize: ");
        }

        private void OnDestroy()
        {
            DelegatesExample.Instance.OnDataReady -= OnDataReady;
        }
    }
}