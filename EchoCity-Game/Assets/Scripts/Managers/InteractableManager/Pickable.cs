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

        protected override void ResolveInteraction(bool outcome)
        {
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
                PlayAtPosition(transform.position, pickableData.PickUpSound, _audioContext, MixerGroupEnum.SFX);
                Destroy(gameObject);
            }
            _canBePicked = false;
        }

        protected void OnEnable()
        {
            if (canBePickedEvent)
                canBePickedEvent.OnEventRaised += InventoryHandler;
        }

        void OnDisable()
        {
            if (canBePickedEvent)
                canBePickedEvent.OnEventRaised -= InventoryHandler;
        }
    }
}