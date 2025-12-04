using UnityEngine;
using UnityEngine.Audio;

namespace EchoCity
{
    public static class ECSound
    {
        private const string _LOG_TAG = "SOUND_SOURCE";
        private const string _LOG_COLOR = "yellow";

        private static AudioMixer _mixer;

        static ECSound()
        {
            _mixer = Resources.Load<AudioMixer>("EchoCity-AudioMixer");
        }

        public static void PlaySoundAtPosition(SOSoundSource soundSource, Vector3 position, SOSoundEmissionDataEvent newAudioSphereEvent = null, string mixerGroup = null)
        {
            if (soundSource == null || soundSource.AudioClip == null)
            {
                Log.W("SoundSource or AudioClip is null. Cannot play sound.", _LOG_COLOR, _LOG_TAG);
                return;
            }
            newAudioSphereEvent?.RaiseEvent(new SoundEmissionData(position, soundSource));
            PlayClipWithTemporaryAudioSource(soundSource.AudioClip, position, soundSource.Volume, mixerGroup);
        }

        public static void PlayRandomClipAtPosition(SOSoundSource soundSource, Vector3 position, SOSoundEmissionDataEvent newAudioSphereEvent = null, string mixerGroup = null)
        {
            if (soundSource?.RandomAudioClips == null || soundSource.RandomAudioClips.Length == 0)
            {
                Log.W("SoundSource RandomAudioClips is null or empty. Cannot play random sound.", _LOG_COLOR, _LOG_TAG);
                return;
            }

            var index = Random.Range(0, soundSource.RandomAudioClips.Length);
            AudioClip clip = soundSource.RandomAudioClips[index];

            newAudioSphereEvent?.RaiseEvent(new SoundEmissionData(position, soundSource, clip));
            PlayClipWithTemporaryAudioSource(clip, position, soundSource.Volume, mixerGroup);

        }

        private static void PlayClipWithTemporaryAudioSource(AudioClip clip, Vector3 position, float volume, string mixerGroup)
        {
            GameObject tempGO = new GameObject("TempAudio");
            tempGO.transform.position = position;
            AudioSource aSource = tempGO.AddComponent<AudioSource>();
            aSource.spatialBlend = 1.0f; // 3D sound
            aSource.clip = clip;
            aSource.volume = volume;
            aSource.outputAudioMixerGroup = mixerGroup == null ? _mixer.FindMatchingGroups("Master")[0] : _mixer.FindMatchingGroups(mixerGroup)[0];
            aSource.Play();
            Object.Destroy(tempGO, clip.length);
        }
    }
}