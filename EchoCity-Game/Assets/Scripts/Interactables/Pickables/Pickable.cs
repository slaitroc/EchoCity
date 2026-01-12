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
        [SerializeField] private bool undoTagsOnDrop = true;

        public override InteractionEnum InteractionCode => InteractionEnum.Pickable;

        protected override void Awake() => gameObject.layer = 9; // Set to Pickable layer

        protected override void ResolveInteraction(bool outcome)
        {
            if (outcome)
                PickUp();
        }

        public virtual void PickUp()
        {
            if (PickableData != null)
                PlayAtPosition(transform.position, PickableData.PickUpSound, _audioContext, MixerGroupEnum.SFX);
            Destroy(gameObject);
        }

        public virtual void Drop()
        {
            if (!undoTagsOnDrop) return;
            if (PuzzleManager == null) return;
            PuzzleManager.SetTags(PuzzleManager.ConstructOppositeTags(setTags), true);
        }
    }
}