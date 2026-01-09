using System.Collections;
using UnityEngine;
using static EchoCity.EchoCitySound;

namespace EchoCity
{
    public class RadioInteractable : PlainInteractable
    {
        [SerializeField] private SOSoundSource radioSoundSource;
        protected override InteractionEnum _interactionCode => InteractionEnum.Radio;
        private AudioSource _audioSource;

        protected override void Start()
        {
            base.Start();
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.loop = false;
            _audioSource.spatialBlend = 1.0f; // 3D sound
        }

        protected override void ResolveInteraction(bool outcome)
        {
            if (outcome)
            {
                if (_audioSource.isPlaying)
                {
                    _audioSource.Stop();
                }
                else
                {
                    PlayRandomInAudioSource(radioSoundSource, _audioContext, _audioSource, MixerGroupEnum.SFX);
                }
            }
        }
    }
}
