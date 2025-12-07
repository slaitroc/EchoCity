using System.Collections;
using UnityEngine;

namespace EchoCity
{
    [RequireComponent(typeof(Collider))]
    public abstract class Pickable : Interactable
    {
        protected override string _TYPE_LOG_TAG => "PICKABLE";
        protected override string _LOG_TAG => pickableData != null ? pickableData.name : "NO_DATA";

        [Header("Invoking Events")]
        [SerializeField] protected SOPickableDataGameObjectEvent itemPickedEvent;
        [SerializeField] protected SOSoundEmissionDataEvent newAudioSphereEvent;
        [Header("Observing Events")]
        [SerializeField] protected SOBoolEvent canBePickedEvent;
        [Header("Pickable Data")]
        [SerializeField] protected SOPickable pickableData;

        protected bool _canBePicked = false;

        public override void InteractionOutcomeHandler(bool outcome)
        {
            base.InteractionOutcomeHandler(outcome);
            if (outcome)
            {
                _canBePicked = true;
                itemPickedEvent?.RaiseEvent(new PickableData(pickableData), pickableData.PickablePrefab);
            }
        }

        public void InventoryHandler(bool canPickUp)
        {
            if (canPickUp && _canBePicked)
            {
                ECSound.PlayAtPosition(pickableData.PickUpSound, transform.position, newAudioSphereEvent, "SFX");
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