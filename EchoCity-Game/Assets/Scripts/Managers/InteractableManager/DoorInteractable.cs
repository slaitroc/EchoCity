using EchoCity;
using UnityEngine;

public class DoorInteractable : LinkableInteractable
{
    protected override string _LOG_TAG => "DOOR";

    #region Private Fields
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private SOSoundEmissionDataEvent newAudioSphereEvent;
    [SerializeField] private SOSoundSource openSound;
    [SerializeField] private SOSoundSource closeSound;
    private readonly int _hashIsOpen = Animator.StringToHash("isOpen");
    [SerializeField] private bool _isOpen = false;

    private bool isOpen
    {
        get { return _isOpen; }
        set
        {
            if (doorAnimator.IsInTransition(0)) return;
            if (_isOpen)
            {
                ECSound.PlaySoundAtPosition(closeSound, transform.position, newAudioSphereEvent, "SFX");
            }
            else
            {
                ECSound.PlaySoundAtPosition(openSound, transform.position, newAudioSphereEvent, "SFX");
            }

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
        isOpen = !isOpen;
    }
}