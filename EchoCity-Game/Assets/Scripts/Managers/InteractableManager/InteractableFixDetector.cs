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
}