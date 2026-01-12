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

        public override InteractionEnum InteractionCode => InteractionEnum.DoorArea;

        protected override void Awake()
        {
            base.Awake();
            TryGetComponent(out doorAnimator);
        }

        //skips puzzle interaction to just toggle door open/close
        public override bool Interact()
        {
            isOpen = !isOpen;
            return true;
        }

        protected override void ResolveInteraction(bool outcome) { }

    }
}
