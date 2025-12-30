using UnityEngine;
using System.Collections.Generic;

namespace EchoCity
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioEmitter : MonoBehaviour, IEventSender
    {
        [Header("Invoking Events")]
        [SerializeField] SOSoundEmissionDataEvent newAudioSphereEvent;

        public string SenderName => gameObject.name;
        public int SenderID => GetInstanceID();
        public bool IsManager => false;
        public EventSenderCategoriesEnum[] SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.Emitter };

        [Header("Emitter Settings")]
        [SerializeField] private SOSoundSource soundSource;
        [SerializeField] private bool emitOnStart = true;
        [SerializeField] private float gapBetweenSounds = 0f;

        private AudioSource audioSource;
        private float nextAutoEmit;
        private List<AudioClip> _validClips = new List<AudioClip>();


        void Awake() => RebuildValidClips();

        void Start()
        {
            Debug.Assert(soundSource != null, "AudioEmitter requires a SOSoundSource reference.");
            if (soundSource == null)
            {
                enabled = false;
                return;
            }

            TryGetComponent(out audioSource);
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // 3D sound

            if (emitOnStart)
                EmitSound();
            if (gapBetweenSounds > 0f)
                nextAutoEmit = Time.time + gapBetweenSounds;
        }

        void Update()
        {
            if (gapBetweenSounds > 0f && Time.time >= nextAutoEmit)
                EmitSound();
        }

        public void EmitSound()
        {
            float audioDuration = 0f;
            AudioClip clipToPlay = GetAudioClip();
            if (!clipToPlay)
            {
                Log.WLazy(() => "No AudioClip to play.", this);
                return;
            }
            else
            {
                audioDuration = clipToPlay.length <= soundSource.MinEchoDuration ? soundSource.MinEchoDuration : clipToPlay.length;

                nextAutoEmit = Time.time + audioDuration + gapBetweenSounds;
                audioSource.PlayOneShot(clipToPlay);
            }

            newAudioSphereEvent?.RaiseEvent(this, new SoundEmissionData(transform.position, soundSource));
        }

        // If randomAudioClips has valid clips, return one at random; otherwise return the main audioClip
        // This means that:
        // - If both randomAudioClips and audioClip are null, returns null
        // - If randomAudioClips is empty or has only nulls, returns audioClip
        // - If randomAudioClips has valid clips, returns one of them at random regardless of audioClip
        private AudioClip GetAudioClip()
        {
            if (_validClips != null && _validClips.Count > 0) return _validClips[Random.Range(0, _validClips.Count)];
            else return soundSource.AudioClip;
        }

        private void RebuildValidClips() //TODO move this logic in ECSound
        {
            _validClips.Clear();
            if (soundSource.RandomAudioClips != null)
                foreach (var sound in soundSource.RandomAudioClips)
                {
                    if (sound) _validClips.Add(sound);
                }
        }
    }
}
