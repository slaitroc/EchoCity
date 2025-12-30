using UnityEngine;

namespace EchoCity
{
    [RequireComponent(typeof(Collider))]
    public class InteractionArea : MonoBehaviour, IEventSender
    {
        [Header("Invoking Events")]
        [SerializeField] private SOAreaInteractableEvent enterInteractableAreaEvent;
        [SerializeField] private SOAreaInteractableEvent exitInteractableAreaEvent;
        [SerializeField] private AreaInteractable interactable;

        public string SenderName => gameObject.name;
        public int SenderID => GetInstanceID();
        public bool IsManager => false;
        public EventSenderCategoriesEnum[] SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.Interactable };


#pragma warning disable CS0414
        #region Debug Fields
        [SerializeField] private bool isPlayerInRange = false; //DEBUG
#if UNITY_EDITOR
        [TextArea][SerializeField] private string notes = "InteractableArea's colliders must not intersect with each other otherwise the OnTriggerEnter and OnTriggerExit events will misbehave.";
#endif
        #endregion
#pragma warning restore CS0414


        void Awake()
        {
            Debug.Assert(interactable, "No Interactable assigned to InteractableRange on {gameObject.name}", this);
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            isPlayerInRange = true;
            interactable.OnEnteringRangeArea(other);
            enterInteractableAreaEvent?.RaiseEvent(this, interactable);

            //Log.D($"Player entered interactable range", _LOG_COLOR, _LOG_TAG);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            isPlayerInRange = false;
            interactable.OnExitingRangeArea(other);
            exitInteractableAreaEvent?.RaiseEvent(this, interactable);
            //Log.D($"Player exited interactable range", _LOG_COLOR, _LOG_TAG);
        }

        void OnValidate()
        {
            gameObject.layer = 6;
        }
    }
}
