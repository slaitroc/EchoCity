using EchoCity;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteractableFixDetector : Interactable
{
    protected override string _TYPE_LOG_TAG => "";

    protected override string _LOG_TAG => "FIX DETECTOR";
    [SerializeField] private Interactable linkedInteractable;

    public override void Interact()
    {
        linkedInteractable?.Interact();
    }

    public override void CheckTags(PuzzleTagEnum[] tagsToCheck)
    {
        throw new System.NotImplementedException();
    }

    public override void SetTags(PuzzleTagEnum[] tagsToSet)
    {
        throw new System.NotImplementedException();
    }

    public override void InteractionOutcomeHandler(bool outcome)
    {
        throw new System.NotImplementedException();
    }
}