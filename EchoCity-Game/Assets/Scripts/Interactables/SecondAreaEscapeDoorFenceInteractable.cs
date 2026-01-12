using System.Collections;
using UnityEngine;
using static EchoCity.EchoCitySound;

namespace EchoCity
{
    public class SecondAreaEscapeDoorFenceInteractable : PlainInteractable
    {
        [SerializeField] private SOSoundSource successSoundSource;
        public override InteractionEnum InteractionCode => InteractionEnum.SecondAreaEscapeDoorFence;
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
                if (successSoundSource != null)
                    PlayAtPosition(transform.position, successSoundSource, _audioContext, MixerGroupEnum.SFX);
                StartCoroutine(RemoveFenceCoroutine());
            }
        }

        private IEnumerator RemoveFenceCoroutine()
        {
            var delay = successSoundSource != null ? successSoundSource.AudioClip.length : 1f;
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
        }

    }
}