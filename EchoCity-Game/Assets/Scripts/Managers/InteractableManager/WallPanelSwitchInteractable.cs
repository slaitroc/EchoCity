using UnityEngine;
using static EchoCity.EchoCitySound;

namespace EchoCity
{

    [RequireComponent(typeof(Animator))]

    public class WallPanelSwitchInteractable : LinkableInteractable
    {
        [Header("Wall Panel Switch Settings")]
        [SerializeField] private GameObject switchLinkedObject;
        [SerializeField] private SOSoundSource switchOnSound;
        [SerializeField] private SOSoundSource switchOffSound;

        [SerializeField] private Animator wallPanelSwitchAnimator;
        private readonly int _hashIsSwitchedOn = Animator.StringToHash("isSwitchedOn");
        [SerializeField] private bool _isSwitchedOn = true;
        private bool isSwitchedOn
        {
            get { return _isSwitchedOn; }
            set
            {
                if (wallPanelSwitchAnimator.IsInTransition(0)) return;
                if (_isSwitchedOn)
                {
                    PlayAtPosition(transform.position, switchOnSound, _audioContext, MixerGroupEnum.SFX);
                }
                else
                {
                    PlayAtPosition(transform.position, switchOffSound, _audioContext, MixerGroupEnum.SFX);
                }
                if (switchLinkedObject)
                {
                    switchLinkedObject.SetActive(isSwitchedOn);
                }
                _isSwitchedOn = value;
                wallPanelSwitchAnimator?.SetBool(_hashIsSwitchedOn, _isSwitchedOn);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            TryGetComponent(out wallPanelSwitchAnimator);
            if (switchLinkedObject)
            {
                switchLinkedObject.SetActive(isSwitchedOn);
            }
            Debug.Assert(switchLinkedObject != null, $"WallPanelSwitchInteractable: No switchLinkedObject assigned on {gameObject.name}.");
        }

        public override void InteractionOutcomeHandler(bool outcome)
        {
            if (_waitForInteractionOutcome)
            {
                if (!outcome)
                {
                    isSwitchedOn = !isSwitchedOn;
                }

            }
            _waitForInteractionOutcome = false;
        }
    }
}

