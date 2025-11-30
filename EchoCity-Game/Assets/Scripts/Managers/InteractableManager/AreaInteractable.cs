using UnityEngine;

public abstract class AreaInteractable : Interactable
{
    protected override string _TYPE_LOG_TAG => "AREA";
    [SerializeField] protected InteractionArea interactableArea;
    [SerializeField] protected Collider _playerInRange;
    private Collider _rangeCollider;

    protected override void Awake()
    {
        base.Awake();
        if (!interactableArea)
        {
            Log.E($"No InteractableArea assigned to Interactable on {gameObject.name}", _LOG_COLOR, _LOG_TAG_FULL);
            return;
        }
        _rangeCollider = interactableArea.GetComponent<Collider>();
        _rangeCollider.isTrigger = true;
    }

    public abstract void OnEnteringRangeArea(Collider other);
    public abstract void OnExitingRangeArea(Collider other);
}