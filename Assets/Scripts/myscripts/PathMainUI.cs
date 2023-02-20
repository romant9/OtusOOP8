using UnityEngine;
using UnityEngine.UI;

namespace Homework8
{
    public class PathMainUI : MonoBehaviour
    {
        public OrderManager Om => OrderManager.OM;

        [SerializeField]
        private Button SortBtn;
        [SerializeField]
        private Button GenPath;
        [SerializeField]
        private Button InvertBtn;       
        [SerializeField]
        private Button InfoBtn;
        [SerializeField]
        private GameObject InfoObj;
        private bool isShowInfo;

        void Start()
        {
            SortBtn.onClick.AddListener(() => Om.SortAction.Invoke());
            GenPath.onClick.AddListener(() => Om.PathFindingAction.Invoke());
            InvertBtn.onClick.AddListener(() => Om.InvertNormalsAction.Invoke());

            InfoBtn.onClick.AddListener(() => ShowInfo());
        }
       
      
        void ShowInfo()
        {
            isShowInfo = !isShowInfo;
            InfoObj.SetActive(isShowInfo);
        }

    }
}

