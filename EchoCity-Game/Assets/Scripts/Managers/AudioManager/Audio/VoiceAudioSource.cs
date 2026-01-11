using UnityEngine;

namespace EchoCity
{
    public class VoiceAudioSource : MonoBehaviour
    {
        private AudioSource _audioSource;
        public AudioSource AudioSource => _audioSource;

        void Awake()
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.outputAudioMixerGroup = EchoCitySound.GetMixerGroup(EchoCitySound.MixerGroupEnum.Voice);
            _audioSource.spatialBlend = 1f; // Make it 3D
        }
    }
}