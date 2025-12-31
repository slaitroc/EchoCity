using UnityEngine;
using UnityEngine.Audio;

namespace EchoCity
{
    public class AudioContext
    {
        public readonly SOSoundEmissionDataEvent NewAudioSphereEvent;
        public readonly IEventSender Sender;
        public AudioContext(IEventSender sender, SOSoundEmissionDataEvent newAudioSphereEvent)
        {
            Sender = sender;
            NewAudioSphereEvent = newAudioSphereEvent;
        }
    }
    public static class EchoCitySound
    {
        public enum MixerGroupEnum
        {
            Master,
            Music,
            SFX,
            Voice,
            Ambience
        }

        public static string GetGroupString(MixerGroupEnum group)
        {
            switch (group)
            {
                case MixerGroupEnum.Master:
                    return "Master";
                case MixerGroupEnum.Music:
                    return "BackgroundMusic";
                case MixerGroupEnum.SFX:
                    return "SFX";
                case MixerGroupEnum.Voice:
                    return "Voice";
                case MixerGroupEnum.Ambience:
                    return "Ambience";
                default:
                    return "Master";
            }
        }

        public static AudioMixerGroup GetMixerGroup(MixerGroupEnum group)
        {
            return _mixer.FindMatchingGroups(GetGroupString(group))[0];
        }
        private const string _LOG_TAG = "ECHO CITY SOUND";
        private static AudioMixer _mixer;
        public static AudioMixer Mixer => _mixer;
        static EchoCitySound()
        {
            _mixer = Resources.Load<AudioMixer>("EchoCity-AudioMixer");
        }

        /// <summary>
        /// Gets a random AudioClip from the provided array.
        /// If the array is null or empty, returns null and logs an assertion.
        /// </summary>
        public static AudioClip GetRandomClip(AudioClip[] audioClips)
        {
            if (audioClips == null || audioClips.Length == 0)
            {
                Debug.Assert(false, $"{_LOG_TAG}-GetRandomClip: AudioClips array is null or empty. Cannot get random sound.");
                return null;
            }

            var index = Random.Range(0, audioClips.Length);
            return audioClips[index];
        }

        /// <summary>
        /// Plays an AudioClip at the given position with specified volume and mixer group.
        /// It does not raise any events.
        /// </summary>
        public static void PlayAtPosition(AudioClip clip, Vector3 position, float volume, MixerGroupEnum mixerGroup = MixerGroupEnum.Master) => AudioPooler.PlayPooledAudio(position, clip, volume, GetMixerGroup(mixerGroup), true);

        /// <summary>
        /// Plays a SOSoundSource at the given position and raises the context event.
        /// </summary>
        public static void PlayAtPosition(Vector3 position, SOSoundSource soundSource, AudioContext audioContext, MixerGroupEnum mixerGroup = MixerGroupEnum.Master)
        {
            Debug.Assert(soundSource != null && soundSource.AudioClip != null, $"{_LOG_TAG}-PlayAtPosition: soundSource or AudioClip is null.");
            if (soundSource == null || soundSource.AudioClip == null) return;
            PlayAtPosition(soundSource.AudioClip, position, soundSource.Volume, mixerGroup);
            audioContext.NewAudioSphereEvent?.RaiseEvent(audioContext.Sender, new SoundEmissionData(position, soundSource));
        }

        /// <summary>
        /// Plays a random AudioClip from the SOSoundSource at the given position and raises the context event.
        /// </summary>
        public static void PlayRandomAtPosition(Vector3 position, SOSoundSource soundSource, AudioContext audioContext, MixerGroupEnum mixerGroup = MixerGroupEnum.Master)
        {
            var clip = GetRandomClip(soundSource.RandomAudioClips);
            if (clip == null)
            {
                if (soundSource?.AudioClip != null)
                {
                    // Fallback to main audio clip if random clips are not available
                    PlayAtPosition(position, soundSource, audioContext, mixerGroup);
                    audioContext.NewAudioSphereEvent?.RaiseEvent(audioContext.Sender, new SoundEmissionData(position, soundSource));
                    return;
                }
                else
                {
                    Debug.Assert(false, $"{_LOG_TAG}-PlayRandomAtPosition: Both RandomAudioClips and AudioClip are null. Cannot play random sound.", soundSource);
                    return;
                }
            }
            else
            {
                PlayAtPosition(clip, position, soundSource.Volume, MixerGroupEnum.Master);
                audioContext.NewAudioSphereEvent?.RaiseEvent(audioContext.Sender, new SoundEmissionData(position, soundSource));
            }
        }

        /// <summary>
        /// Plays a random AudioClip from the SOSoundSource in the given AudioSource and raises the context event.
        /// </summary>
        public static void PlayRandomInAudioSource(SOSoundSource soundSource, AudioContext audioContext, AudioSource audioSource, MixerGroupEnum mixerGroup = MixerGroupEnum.Master)
        {
            var clip = GetRandomClip(soundSource.RandomAudioClips);
            if (clip == null)
            {
                if (soundSource?.AudioClip != null)
                {
                    // Fallback to main audio clip if random clips are not available
                    audioContext.NewAudioSphereEvent?.RaiseEvent(audioContext.Sender, new SoundEmissionData(audioSource.transform.position, soundSource));
                    PlayInAudioSource(soundSource.AudioClip, soundSource.Volume, audioSource, mixerGroup);
                    return;
                }
                else
                {
                    Debug.Assert(false, $"{_LOG_TAG}-PlayRandomInAudioSource: Both RandomAudioClips and AudioClip are null. Cannot play random sound.", soundSource);
                    return;
                }
            }
            audioContext.NewAudioSphereEvent?.RaiseEvent(audioContext.Sender, new SoundEmissionData(audioSource.transform.position, soundSource));
            PlayInAudioSource(clip, soundSource.Volume, audioSource, mixerGroup);
        }

        /// <summary>
        /// Plays an AudioClip in the given AudioSource with specified volume and mixer group.
        /// It does not raise any events.
        /// </summary>
        public static void PlayInAudioSource(AudioClip clip, float volume, AudioSource aSource, MixerGroupEnum mixerGroup = MixerGroupEnum.Master)
        {
            aSource.outputAudioMixerGroup = GetMixerGroup(mixerGroup);
            aSource.clip = clip;
            aSource.volume = volume;
            aSource.Play();
        }

        /// <summary>
        /// Plays a SOSoundSource in the given AudioSource and raises the context event.
        /// </summary>
        public static void PlayInAudioSource(SOSoundSource soundSource, AudioSource aSource, AudioContext audioContext, MixerGroupEnum mixerGroup = MixerGroupEnum.Master)
        {
            audioContext.NewAudioSphereEvent?.RaiseEvent(audioContext.Sender, new SoundEmissionData(aSource.transform.position, soundSource));
            PlayInAudioSource(soundSource.AudioClip, soundSource.Volume, aSource, mixerGroup);
        }
    }
}