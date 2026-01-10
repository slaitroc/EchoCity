using EchoCity;
using UnityEngine;
using static EchoCity.EchoCitySound;

public class DoorInteractable : LinkableInteractable
{

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

    public override InteractionEnum InteractionCode => InteractionEnum.Door;

    protected override void Awake()
    {
        base.Awake();
        TryGetComponent(out doorAnimator);
    }

    //skips puzzle interaction to just toggle door open/close
    public override bool Interact()
    {
        isOpen = !isOpen;
        return true;
    }


    protected override void ResolveInteraction(bool outcome) { }

}