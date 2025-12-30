using UnityEngine;

namespace EchoCity
{
    public abstract class AreaInteractable : Interactable
    {
        [SerializeField] protected InteractionArea interactableArea;
        [SerializeField] protected Collider _playerInRange;
        private Collider _rangeCollider;

        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(interactableArea != null, "Interaction Area is not assigned!", this);
            _rangeCollider = interactableArea.GetComponent<Collider>();
            _rangeCollider.isTrigger = true;
        }

        public abstract void OnEnteringRangeArea(Collider other);
        public abstract void OnExitingRangeArea(Collider other);
    }
}