using System;
using UnityEngine;
using UnityEngine.EventSystems;
namespace NumeralSystem.Utils
{
    public class UITouchEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Action<PointerEventData> onTouchEnter;
        public Action<PointerEventData> onTouchExit;
        public Action<PointerEventData> onTouchDown;
        public Action<PointerEventData> onTouchUp;
        public Action<PointerEventData> onPointClick;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (onTouchDown != null && eventData.dragging == false)
            {
                onTouchDown(eventData);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (onTouchUp != null && eventData.dragging == false)
            {
                onTouchUp(eventData);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (onTouchEnter != null)
            {
                onTouchEnter(eventData);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (onTouchExit != null)
            {
                onTouchExit(eventData);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (onPointClick != null)
            {
                onPointClick(eventData);
            }
        }
    }
}