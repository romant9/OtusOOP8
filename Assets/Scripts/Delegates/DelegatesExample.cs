using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Delegates
{
    public class DelegatesExample : MonoBehaviour
    {
        #region Simple delegates

        public delegate void SimpleDelegate();

        private SimpleDelegate Play = () => Debug.Log("Play");

        private SimpleDelegate Stop = delegate { Debug.Log("Stop"); };

        public SimpleDelegate Pause = DoPause;

        public static void DoPause()
        {
            Debug.Log("DoPause");
        }

        public Button Button;

        private void TestButton()
        {
            
            
        }


        [ContextMenu("Simple delegates")]
        public void TestSimpleDelegates()
        {
            // Play.Invoke();
            // Stop.Invoke();
            // Pause.Invoke();
            //
            // InvokeSimpleDelegate(Play);

            SimpleDelegate simple = Play;
            simple += Stop;
            simple += Pause;

            simple.Invoke();
        }

        private void InvokeSimpleDelegate(SimpleDelegate simple)
        {
            simple.Invoke();
        }

        #endregion


        #region Not-so-simple delegates

        private delegate string ComplexDelegate(int i, string s);

        private ComplexDelegate PlayAt = ((i, s) =>
        {
            Debug.Log($"DelegatesExample.PlayAt: = {i} - {s}");

            return $"Echo: {s}";
        });


        [ContextMenu("Delegates with parameters")]
        public void TestDelegatesWithParameters()
        {
            var result = PlayAt.Invoke(42, "Galaxy");
        }

        #endregion


        #region Actions and Func

        private Action SimpleAction;
        private Func<int, string> ComplexAction;

        [ContextMenu("Delegates with parameters 2")]
        public void TestAction()
        {
            SimpleAction += DoSomethingSimple;

            SimpleAction?.Invoke();

            ComplexAction += DoSomethingComplex;
            ComplexAction?.Invoke(3);
        }

        private void DoSomethingSimple()
        {
            Debug.Log("DelegatesExample.DoSomethingSimple: ");
        }

        private string DoSomethingComplex(int i)
        {
            Debug.Log($"DelegatesExample.DoSomethingComplex: {i}");

            return i.ToString();
        }

        #endregion


        #region Real-world example

        public static DelegatesExample Instance { get; private set; }

        [SerializeField]
        private float _delay = 3f;

        public bool IsReady { get; private set; }

        public event Action OnDataReady;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            // StartCoroutine(DelayAndFireEvent());
        }

        private IEnumerator DelayAndFireEvent()
        {
            Debug.Log("DelegatesExample.DelayAndFireEvent: delay started");

            yield return new WaitForSeconds(_delay);

            Debug.Log("DelegatesExample.DelayAndFireEvent: delay finished");

            IsReady = true;
            OnDataReady?.Invoke();
        }

        #endregion
    }
}