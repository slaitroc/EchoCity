using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DoorAreaInteractable : PlayerAreaInteractable
{
    #region Constants
    protected override string _LOG_TAG => "DOOR";
    #endregion

    #region Serialized Fields
    [Header("Door Settings")]
    #endregion

    #region Private Fields
    [SerializeField] private Animator doorAnimator;
    private readonly int _hashIsOpen = Animator.StringToHash("isOpen");
    [SerializeField] private bool _isOpen = false;

    private bool isOpen
    {
        get { return _isOpen; }
        set
        {
            if (doorAnimator.IsInTransition(0)) return;
            _isOpen = value;
            doorAnimator?.SetBool(_hashIsOpen, _isOpen);
        }
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        TryGetComponent(out doorAnimator);
    }

    public override void Interact()
    {
        //Log.D("Interaction!", "#ff5733ff", $"{_INTERACTABLE_LOG_TAG}+{_LOG_TAG}");
        isOpen = !isOpen;
    }
}
