using System.Collections;
using UnityEngine;
using static EchoCity.EchoCitySound;

namespace EchoCity
{
    public class SecondAreaIDCheckerInteractable : PlainInteractable
    {
        [SerializeField] private SOSoundSource successSoundSource;
        [SerializeField] private SOSwitchToGameStateEvent switchToGameStateEvent;
        [SerializeField] private SOQuest extraTagTriggerQuest;
        public override InteractionEnum InteractionCode => InteractionEnum.SecondAreaIDChecker;
        private AudioSource _audioSource;

        protected override void Start()
        {
            base.Start();
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.spatialBlend = 1.0f; // 3D sound
        }

        protected override void ResolveInteraction(bool outcome)
        {
            if (outcome)
            {
                PlayAtPosition(transform.position, successSoundSource, _audioContext, MixerGroupEnum.SFX);
                switchToGameStateEvent.RaiseEvent(this, GameStatesEnum.Win, null);
            }
            else
            {
                if (extraTagTriggerQuest != null)
                {
                    PuzzleManager.AddToTagsTriggeredQuests(extraTagTriggerQuest);
                    PuzzleManager.CheckQuests();
                }
            }
        }

    }
}