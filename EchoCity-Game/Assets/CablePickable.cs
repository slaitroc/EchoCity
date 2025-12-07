using EchoCity.Interactables;
using UnityEngine;

namespace EchoCity
{
    public class CablePickable : Pickable
    {
        [SerializeField] SODialogContainer dialogContainer;
        [SerializeField] SODialogDataEvent dialogDataEvent;
        [SerializeField] SOStringColorEvent spawnMessageEvent;

        protected override void OnEnable()
        {
            base.OnEnable();
            interactableTags = new PuzzleTagEnum[] { PuzzleTagEnum.CablePicked };
        }


        public override void CheckTags(PuzzleTagEnum[] tagsToCheck)
        {
        }

        public override void Interact()
        {
            
        }

        public override void InteractionOutcomeHandler(bool outcome)
        {
            if (outcome)
            {
                spawnMessageEvent?.RaiseEvent("Cable Picked Up!", new Color(1f, 0.5f, 0f, 1f));
                dialogDataEvent?.RaiseEvent(new DialogData(dialogContainer));
                Destroy(gameObject);
            }
        }

        public override void SetTags(PuzzleTagEnum[] tagsToSet)
        {
        }
    }
}
