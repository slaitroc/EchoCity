using EchoCity;
using UnityEngine;
using UnityEngine.AI;
using static EchoCity.EchoCitySound;

public class DoorInteractable : LinkableInteractable
{

    [SerializeField] private Animator doorAnimator;
    [SerializeField] private SOSoundSource openSound;
    [SerializeField] private SOSoundSource closeSound;
    private readonly int _hashIsOpen = Animator.StringToHash("isOpen");
    [SerializeField] private bool _isOpen = false;
    [Header("Nav Mesh Obstacle")]
    [SerializeField] private NavMeshObstacle navMeshObstacle;

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

    protected override void ResolveInteraction(bool outcome)
    {
        if (outcome)
        {
            isOpen = !isOpen;
            navMeshObstacle.enabled = !isOpen;
        }

    }

}