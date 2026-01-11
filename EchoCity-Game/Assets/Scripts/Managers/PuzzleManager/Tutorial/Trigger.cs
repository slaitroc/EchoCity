using System;
using UnityEngine;

namespace EchoCity
{
    [RequireComponent(typeof(Collider))]
    public class Trigger : PlainInteractable
    {
        [SerializeField] private Collider triggerCollider;

        public override string SenderName => gameObject.name;
        public override int SenderID => GetInstanceID();
        public override bool IsManager => false;

        public override bool HasRaycastDescription => false;
        public override bool IsInteractable => false;

        public override EventSenderCategoriesEnum[] SenderCategory => new EventSenderCategoriesEnum[] {
            EventSenderCategoriesEnum.Puzzle,
            EventSenderCategoriesEnum.ColliderTrigger
        };

        public override InteractionEnum InteractionCode => InteractionEnum.TutorialTrigger;

        protected override void Start()
        {
            base.Start();
            if (triggerCollider == null)
                TryGetComponent(out triggerCollider);
            triggerCollider.isTrigger = true;
        }
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            Interact();
        }

        protected override void ResolveInteraction(bool outcome)
        {
            if (outcome)
            {
                triggerCollider.enabled = false;
            }
        }
    }
}
