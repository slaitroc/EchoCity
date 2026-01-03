using UnityEngine;

namespace EchoCity
{
    [RequireComponent(typeof(Collider))]
    public class InteractionArea : MonoBehaviour
    {
        [SerializeField] private AreaInteractable interactable;

#pragma warning disable CS0414
        [SerializeField] private bool isPlayerInRange = false; //DEBUG
#pragma warning restore CS0414

        void Awake()
        {
            Debug.Assert(interactable, $"No Interactable assigned to InteractableRange on {gameObject.name}", this);
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            isPlayerInRange = true;
            interactable.OnEnteringRangeArea(other);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            isPlayerInRange = false;
            interactable.OnExitingRangeArea(other);
        }

        void OnValidate()
        {
            gameObject.layer = 6;
        }
    }
}
