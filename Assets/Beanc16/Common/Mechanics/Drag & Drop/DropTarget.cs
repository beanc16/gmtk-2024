using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Beanc16.Common.Mechanics.DragAndDrop;

namespace Beanc16.Common.Mechanics.DragAndDrop
{
    [System.Serializable]
    public class AcceptDropIfNoErrorIsThrownEvent : UnityEvent<Draggable> { }

    [System.Serializable]
    public class DropSuccessEvent : UnityEvent<Draggable> { }

    [System.Serializable]
    public class DropFailedEvent : UnityEvent<Draggable?> { }



    public class DropTarget : MonoBehaviour, IDropHandler
    {
        public AcceptDropIfNoErrorIsThrownEvent AcceptDropIfNoErrorIsThrown;
        public DropSuccessEvent OnSuccessfulDrop;
        public DropFailedEvent OnFailedDrop;



        public void OnDrop(PointerEventData eventData)
        {
            GameObject objBeingHoveredOver = eventData.pointerDrag;

            if (objBeingHoveredOver != null)
            {
                Draggable droppedObject = objBeingHoveredOver.GetComponent<Draggable>();
                
                if (ShouldAcceptDroppedObject(droppedObject))
                {
                    UpdateDraggablesPosition(objBeingHoveredOver, droppedObject);
                }

                else
                {
                    TryCallDropFailedEvent(droppedObject);
                }
            }

            else
            {
                TryCallDropFailedEvent(null);
            }
        }

        private void UpdateDraggablesPosition(GameObject objBeingHoveredOver, Draggable droppedObject)
        {
            Transform transformBeingHoveredOver = 
                objBeingHoveredOver.GetComponent<Transform>();

            transformBeingHoveredOver.SetParent(this.transform, false);
            transformBeingHoveredOver.position = this.transform.position;

            TryCallDropSuccessEvent(droppedObject);
        }



        private bool TryCallAcceptDropIfEvent(Draggable draggable)
        {
            // There ARE functions to test
            if (AcceptDropIfNoErrorIsThrown != null)
            {
                try
                {
                    AcceptDropIfNoErrorIsThrown.Invoke(draggable);
                    return true;
                }
                catch (Exception error)
                {
                    return false;
                }
            }

            // There ARE NOT functions to test, so accept all drops
            return true;
        }

        private bool ShouldAcceptDroppedObject(Draggable draggable)
        {
            return TryCallAcceptDropIfEvent(draggable);
        }

        private void TryCallDropSuccessEvent(Draggable droppedObject)
        {
            if (OnSuccessfulDrop != null)
            {
                OnSuccessfulDrop.Invoke(droppedObject);
            }
        }

        private void TryCallDropFailedEvent(Draggable? droppedObject)
        {
            if (OnFailedDrop != null)
            {
                OnFailedDrop.Invoke(droppedObject);
            }
        }



        protected void DeleteDroppedDraggable(Draggable draggable)
        {
            Destroy(draggable.gameObject);
        }
    }
}
