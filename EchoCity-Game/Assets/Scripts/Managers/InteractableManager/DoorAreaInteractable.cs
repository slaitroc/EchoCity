using UnityEngine;

namespace EchoCity
{
    [RequireComponent(typeof(Animator))]
    public class DoorAreaInteractable : PlayerAreaInteractable
    {
        [Header("Door Settings")]

        [SerializeField] private Animator doorAnimator;
        private readonly int _hashIsOpen = Animator.StringToHash("isOpen");
        [SerializeField] private bool _isOpen = false;

        private bool isOpen
        {
            get { return _isOpen; }
            set
            {
                if (doorAnimator.IsInTransition(0)) return;
                _isOpen = value;
                doorAnimator?.SetBool(_hashIsOpen, _isOpen);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            TryGetComponent(out doorAnimator);
        }

        public override void Interact()
        {
            isOpen = !isOpen;
        }

        public override void InteractionOutcomeHandler(IEventSender sender, bool outcome)
        {
            throw new System.NotImplementedException();
        }
    }
}
