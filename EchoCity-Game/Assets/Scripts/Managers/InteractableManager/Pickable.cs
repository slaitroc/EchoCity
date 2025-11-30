using UnityEngine;

namespace EchoCity.Interactables
{
    [RequireComponent(typeof(Collider))]
    public class Pickable : Interactable
    {

        [SerializeField] private SOSoundEmissionDataEvent newAudioSphereEvent;
        [SerializeField] private SOPickableData pickableData;
        [Header("Invoking Events")]
        [SerializeField] private SOPickableDataEvent pickedEvent;

        protected override string _TYPE_LOG_TAG => "PICKABLE";

        protected override string _LOG_TAG => pickableData != null ? pickableData.name : "NO_DATA";

        public override void Interact()
        {
            Log.D("Picked up " + gameObject.name, _LOG_COLOR, _LOG_TAG_FULL);
            pickedEvent?.RaiseEvent(pickableData);

            ECSound.PlaySoundAtPosition(pickableData.PickUpSound, transform.position, newAudioSphereEvent, "SFX");
            Destroy(gameObject);
        }
    }
}