using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Beanc16.Common.Mechanics.DragAndDrop;

namespace Beanc16.Common.Mechanics.DragAndDrop
{
    [RequireComponent(typeof (CanvasGroup))]
    public class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private CanvasGroup canvasGroup;
        private Vector3 startPosition;
        private DropTarget startDropTarget;
        [SerializeField]
        [Tooltip("What action to take when a drop fails.")]
        private OnFailedDropEnum onFailedDrop = OnFailedDropEnum.SNAP_TO_START_POSITION;

        public UnityEvent OnDragStart;
        public UnityEvent OnDragging;
        public UnityEvent OnDragEnd;



        private void Awake()
        {
            InitializeComponents();
        }
        
        private void InitializeComponents()
        {
            this.canvasGroup = this.GetComponent<CanvasGroup>();
        }



        public void OnBeginDrag(PointerEventData eventData)
        {
            this.startPosition = this.transform.position;
            this.startDropTarget = this.GetComponentInParent<DropTarget>();

            // Allow detection of DropTargets beneath this Draggable object
            this.DetectThisObjectBeforeObjectsBeneath(false);

            TryCallDragStartEvent();
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Make it look like this object is being dragged
            this.transform.position = eventData.position;

            TryCallDraggingEvent();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (this.onFailedDrop == OnFailedDropEnum
                                    .SNAP_TO_START_POSITION)
            {
                TrySnapToStartPosition();
            }

            else if (onFailedDrop == OnFailedDropEnum
                                    .LEAVE_WHEREVER_DROPPED)
            {
                // Do nothing
            }

            // Detect this object before DropTargets beneath it
            this.DetectThisObjectBeforeObjectsBeneath(true);

            TryCallDragEndEvent();
        }



        private void TrySnapToStartPosition()
        {
            DropTarget dropTarget = this.GetComponentInParent<DropTarget>();

            // This object WAS NOT dropped into a new drop target
            if (dropTarget != null
                && StartingDropTargetEqualsEndingDropTarget(dropTarget))
            {
                // Reset position to where it was before getting dragged
                this.transform.position = this.startPosition;
            }
        }

        private void DetectThisObjectBeforeObjectsBeneath(bool shouldAllow)
        {
            // true = Allow detection of DropTargets beneath this
            // false = Don't allow detection of DropTargets beneath this
            this.canvasGroup.blocksRaycasts = shouldAllow;
        }

        public void ToggleInteractivity(bool shouldBeInteractive)
        {
            this.DetectThisObjectBeforeObjectsBeneath(shouldBeInteractive);
        }

        private bool StartingDropTargetEqualsEndingDropTarget(DropTarget endingDropTarget)
        {
            return Equals(this.startDropTarget, endingDropTarget);
        }



        private void TryCallDragStartEvent()
        {
            if (OnDragStart != null)
            {
                OnDragStart.Invoke();
            }
        }

        private void TryCallDraggingEvent()
        {
            if (OnDragging != null)
            {
                OnDragging.Invoke();
            }
        }

        private void TryCallDragEndEvent()
        {
            if (OnDragEnd != null)
            {
                OnDragEnd.Invoke();
            }
        }
    }



    /**
    * Enum: OnFailedDropEnum
    * Used to identify what to do when a <Draggable> is dropped outside of 
    * a <DropTarget>.
    * 
    * SNAP_TO_START_POSITION - Immediately set position back to where a 
    * 							<Draggable> was at when it started getting 
    * 							dragged.
    * LEAVE_WHEREVER_DROPPED - Leave a <Draggable> wherever it was dropped.
    */
    public enum OnFailedDropEnum
    {
        SNAP_TO_START_POSITION,
        LEAVE_WHEREVER_DROPPED
    }
}
