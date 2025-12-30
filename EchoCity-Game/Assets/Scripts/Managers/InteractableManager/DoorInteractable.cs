using EchoCity;
using UnityEngine;
using static EchoCity.EchoCitySound;

public class DoorInteractable : LinkableInteractable
{

    #region Private Fields
    [SerializeField] private Animator doorAnimator;
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
                PlayAtPosition(transform.position, closeSound, _audioContext, MixerGroupEnum.SFX);
            }
            else
            {
                PlayAtPosition(transform.position, openSound, _audioContext, MixerGroupEnum.SFX);
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


    public override void InteractionOutcomeHandler(IEventSender sender, bool outcome)
    {
        throw new System.NotImplementedException();
    }
}