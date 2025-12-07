using System.Collections;
using UnityEngine;

namespace EchoCity.Interactables
{
    [RequireComponent(typeof(Collider))]
    public class Pickable : Interactable
    {
        protected override string _TYPE_LOG_TAG => "PICKABLE";
        protected override string _LOG_TAG => pickableData != null ? pickableData.name : "NO_DATA";

        [Header("Invoking Events")]
        [SerializeField] private SOPickableDataGameObjectEvent itemPickedEvent;
        [SerializeField] private SOSoundEmissionDataEvent newAudioSphereEvent;
        [Header("Observing Events")]
        [SerializeField] private SOBoolEvent canBePickedEvent;
        [Header("Pickable Data")]
        [SerializeField] private SOPickable pickableData;

        private bool _canBePicked = false;


        public override void Interact()
        {
            _canBePicked = true;
            itemPickedEvent?.RaiseEvent(new PickableData(pickableData), pickableData.PickablePrefab);
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

        public override void CheckTagsHandler(PuzzleTagEnum[] tagsToCheck)
        {
            throw new System.NotImplementedException();
        }

        public override void SetTagsHandler(PuzzleTagEnum[] tagsToSet)
        {
            throw new System.NotImplementedException();
        }

        public override void InteractionOutcomeHandler(bool outcome)
        {
        }


    }
}