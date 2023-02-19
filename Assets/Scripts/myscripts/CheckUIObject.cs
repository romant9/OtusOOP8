using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Homework8
{ 
    public class CheckUIObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public DogSpawner Ds => DogSpawner.DS;

        public void OnPointerEnter(PointerEventData eventData)
        {
            Ds.isUI = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Ds.isUI = false;
        }
    }
}
