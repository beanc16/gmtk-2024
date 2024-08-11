using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;



namespace Beanc16.Common.UI
{
    [System.Serializable]
    public class ClickStartEvent : UnityEvent<PointerEventData> { }

    [System.Serializable]
    public class ClickStopEvent : UnityEvent<PointerEventData> { }



    public class OnClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public ClickStartEvent OnClickStart;
        public ClickStopEvent OnClickStop;



        public void OnPointerDown(PointerEventData eventData)
        {
            TryCallClickStartEvent(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            TryCallClickStopEvent(eventData);
        }



        private void TryCallClickStartEvent(PointerEventData eventData)
        {
            if (OnClickStart != null)
            {
                OnClickStart.Invoke(eventData);
            }
        }

        private void TryCallClickStopEvent(PointerEventData eventData)
        {
            if (OnClickStop != null)
            {
                OnClickStop.Invoke(eventData);
            }
        }
    }
}
