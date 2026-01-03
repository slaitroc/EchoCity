using System.Collections;
using UnityEngine;
using static EchoCity.EchoCitySound;

namespace EchoCity
{
    [RequireComponent(typeof(Collider))]
    public abstract class Pickable : PlainInteractable
    {
        [Header("Pickable Data")]
        [SerializeField] public SOPickable PickableData;

        protected override void ResolveInteraction(bool outcome)
        {
            if (outcome)
                PickUp();
        }

        public void PickUp()
        {
            PlayAtPosition(transform.position, PickableData.PickUpSound, _audioContext, MixerGroupEnum.SFX);
            Destroy(gameObject);
        }
    }
}