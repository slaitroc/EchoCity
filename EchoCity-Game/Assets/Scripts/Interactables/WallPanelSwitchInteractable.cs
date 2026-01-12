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
        private bool _firstInteractionDone = false;
        [Header("Invoking Events")]
        [SerializeField] private SOSetMaterialEvent setMaterialEvent;
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
                _isSwitchedOn = value;
                if (switchLinkedObject)
                {
                    switchLinkedObject.SetActive(isSwitchedOn);
                }
                wallPanelSwitchAnimator?.SetBool(_hashIsSwitchedOn, _isSwitchedOn);
            }
        }

        [SerializeField] private SOQuest[] triggeredQuest;
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

        protected override void ResolveInteraction(bool outcome)
        {
            if (outcome)
            {
                isSwitchedOn = !isSwitchedOn;
                if (!_firstInteractionDone)
                {
                    _firstInteractionDone = true;
                }
                setMaterialEvent?.RaiseEvent(this, EchoMaterialCodeEnum.Toggle);
                foreach (var quest in triggeredQuest)
                {
                    puzzleManager?.AddQuest(quest);
                }
            }
        }
    }
}

