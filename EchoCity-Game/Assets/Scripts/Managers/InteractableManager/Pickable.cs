using System.Collections;
using UnityEngine;
using static EchoCity.EchoCitySound;

namespace EchoCity
{
    [RequireComponent(typeof(Collider))]
    public abstract class Pickable : Interactable
    {
        [Header("Invoking Events")]
        [SerializeField] protected SOPickableDataGameObjectEvent itemPickedEvent;
        [Header("Observing Events")]
        [SerializeField] protected SOBoolEvent canBePickedEvent;
        [Header("Pickable Data")]
        [SerializeField] protected SOPickable pickableData;

        protected bool _canBePicked = false;

        public override void InteractionOutcomeHandler(IEventSender sender, bool outcome)
        {
            if (!_waitForInteractionOutcome) return;
            if (outcome)
            {
                _canBePicked = true;
                itemPickedEvent?.RaiseEvent(this, new PickableData(pickableData), pickableData.PickablePrefab);
            }
        }

        public void InventoryHandler(IEventSender sender, bool canPickUp)
        {
            if (canPickUp && _canBePicked)
            {
                PlayAtPosition(transform.position, pickableData.PickUpSound, _audioContext, MixerGroupEnum.SFX);
                Destroy(gameObject);
            }
            _canBePicked = false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (canBePickedEvent)
                canBePickedEvent.OnEventRaised += InventoryHandler;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (canBePickedEvent)
                canBePickedEvent.OnEventRaised -= InventoryHandler;
        }
    }
}