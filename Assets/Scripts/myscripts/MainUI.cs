using UnityEngine;
using UnityEngine.UI;

namespace Homework8
{
    public class MainUI : MonoBehaviour
    {
        public DogSpawner Ds => DogSpawner.DS;

        [SerializeField]
        private Button startBtn;
        [SerializeField]
        private Button resetBtn;
        [SerializeField]
        private Button InfoBtn;
        [SerializeField]
        private GameObject InfoObj;
        private bool isShowInfo;
        private bool isFreeze;

        void Start()
        {
            startBtn.onClick.AddListener(() => StartBtnOnClick());
            resetBtn.onClick.AddListener(() => ResetSpawner());
            InfoBtn.onClick.AddListener(() => ShowInfo());
        }
        void StartBtnOnClick()
        {
            string text = isFreeze ? "Запустить" : "Остановить";
            GetComponentInChildren<Text>().text = text;
            Ds.StartSpawner(isFreeze);
            isFreeze = !isFreeze;
        }
        void ResetSpawner()
        {
            Ds.ResetSpawner();
        }
        void ShowInfo()
        {
            isShowInfo = !isShowInfo;
            InfoObj.SetActive(isShowInfo);
        }

    }
}

