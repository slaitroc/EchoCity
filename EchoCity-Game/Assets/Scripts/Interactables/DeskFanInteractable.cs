using UnityEngine;
using static EchoCity.EchoCitySound;

namespace EchoCity
{
    public class DeskFanInteractable : PlainInteractable
    {
        [Header("Desk Fan Data")]
        [SerializeField] private SOSoundSource deskFanSoundSource;
        [SerializeField] private SOSoundSource deskFanStopSoundSource;
        public override InteractionEnum InteractionCode => InteractionEnum.DeskFan;
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
                if (_audioSource.isPlaying)
                {
                }
                else
                {
                    PlayRandomInAudioSource(deskFanSoundSource, _audioContext, _audioSource, MixerGroupEnum.SFX);
                }
            }
        }

    }
}