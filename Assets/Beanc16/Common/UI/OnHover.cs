using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;



namespace Beanc16.Common.UI
{
    [System.Serializable]
    public class AcceptHoverEnterIfNoErrorIsThrownEvent : UnityEvent<PointerEventData> { }

    [System.Serializable]
    public class HoverEnterEvent : UnityEvent<PointerEventData> { }

    [System.Serializable]
    public class HoverExitEvent : UnityEvent<PointerEventData> { }



    public class OnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public AcceptHoverEnterIfNoErrorIsThrownEvent AcceptHoverEnterIfNoErrorIsThrown;
        public HoverEnterEvent OnHoverEnter;
        public HoverExitEvent OnHoverExit;



        public void OnPointerEnter(PointerEventData eventData)
        {
            TryCallHoverEnterEvent(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TryCallHoverExitEvent(eventData);
        }



        private void TryCallHoverEnterEvent(PointerEventData eventData)
        {
            if (OnHoverEnter != null)
            {
                OnHoverEnter.Invoke(eventData);
            }
        }

        private void TryCallHoverExitEvent(PointerEventData eventData)
        {
            if (OnHoverExit != null)
            {
                OnHoverExit.Invoke(eventData);
            }
        }
    }
}
