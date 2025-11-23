using UnityEngine;


[RequireComponent(typeof(Animator))]
public class WallPanelSwitchInteractable : Interactable
{
    #region Constants
    protected override string _LOG_TAG => "WALL_PANEL_SWITCH";
    protected override string _TYPE_LOG_TAG => "GENERAL";
    #endregion

    #region Serialized Fields
    [Header("Wall Panel Switch Settings")]
    [SerializeField] private GameObject linkedObject;
    #endregion

    #region Private Fields
    [SerializeField] private Animator wallPanelSwitchAnimator;
    private readonly int _hashIsSwitchedOn = Animator.StringToHash("isSwitchedOn");
    [SerializeField] private bool _isSwitchedOn = false;
    private bool isSwitchedOn
    {
        get { return _isSwitchedOn; }
        set
        {
            if (wallPanelSwitchAnimator.IsInTransition(0)) return;
            _isSwitchedOn = value;
            wallPanelSwitchAnimator?.SetBool(_hashIsSwitchedOn, _isSwitchedOn);
        }
    }

    #endregion

    protected void Awake()
    {
        TryGetComponent(out wallPanelSwitchAnimator);
        if (linkedObject)
        {
            linkedObject.SetActive(isSwitchedOn);
        }
        else
        {
            Log.W($"No linked object assigned to WallPanelSwitchInteractable on {gameObject.name}", _LOG_COLOR, _LOG_TAG_FULL);
        }
    }

    public override void Interact()
    {
        isSwitchedOn = !isSwitchedOn;
        if (linkedObject)
        {
            linkedObject.SetActive(isSwitchedOn);
        }
    }
}
