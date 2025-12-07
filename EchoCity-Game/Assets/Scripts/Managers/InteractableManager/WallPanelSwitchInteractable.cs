using UnityEngine;

namespace EchoCity
{

    [RequireComponent(typeof(Animator))]

    public class WallPanelSwitchInteractable : LinkableInteractable
    {
        #region Constants
        protected override string _LOG_TAG => "WALL_PANEL_SWITCH";
        protected override string _TYPE_LOG_TAG => "GENERAL";
        #endregion

        #region Serialized Fields
        [Header("Invoking Events")]
        [SerializeField] private SOSoundEmissionDataEvent newAudioSphereEvent;
        [Header("Wall Panel Switch Settings")]
        [SerializeField] private GameObject switchLinkedObject;
        [SerializeField] private SOSoundSource switchOnSound;
        [SerializeField] private SOSoundSource switchOffSound;
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
                if (_isSwitchedOn)
                {
                    ECSound.PlayAtPosition(switchOnSound, transform.position, newAudioSphereEvent, "SFX");
                }
                else
                {
                    ECSound.PlayAtPosition(switchOffSound, transform.position, newAudioSphereEvent, "SFX");
                }
                if (switchLinkedObject)
                {
                    switchLinkedObject.SetActive(isSwitchedOn);
                }
                _isSwitchedOn = value;
                wallPanelSwitchAnimator?.SetBool(_hashIsSwitchedOn, _isSwitchedOn);
            }
        }

        #endregion

        protected override void Awake()
        {
            base.Awake();
            TryGetComponent(out wallPanelSwitchAnimator);
            if (switchLinkedObject)
            {
                switchLinkedObject.SetActive(isSwitchedOn);
            }
            else
            {
                Log.W($"No linked object assigned to WallPanelSwitchInteractable on {gameObject.name}", _LOG_COLOR, _LOG_TAG_FULL);
            }
        }

        public override void Interact()
        {
            isSwitchedOn = !isSwitchedOn;
        }

        public override void CheckTags(PuzzleTagEnum[] tagsToCheck)
        {
            throw new System.NotImplementedException();
        }

        public override void SetTags(PuzzleTagEnum[] tagsToSet)
        {
            throw new System.NotImplementedException();
        }

        public override void InteractionOutcomeHandler(bool outcome)
        {
            throw new System.NotImplementedException();
        }
    }
}

