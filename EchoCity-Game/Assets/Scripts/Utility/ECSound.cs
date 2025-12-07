using UnityEngine;
using UnityEngine.Audio;

namespace EchoCity
{
    public static class ECSound
    {
        private const string _LOG_TAG = "SOUND_SOURCE";
        private const string _LOG_COLOR = "yellow";

        private static AudioMixer _mixer;
        public static AudioMixer Mixer => _mixer;

        static ECSound()
        {
            _mixer = Resources.Load<AudioMixer>("EchoCity-AudioMixer");

        }

        public static void PlayAtPosition(SOSoundSource soundSource, Vector3 position, SOSoundEmissionDataEvent newAudioSphereEvent = null, string mixerGroup = null)
        {
            if (soundSource == null || soundSource.AudioClip == null)
            {
                Log.W("SoundSource or AudioClip is null. Cannot play sound.", _LOG_COLOR, _LOG_TAG);
                return;
            }
            newAudioSphereEvent?.RaiseEvent(new SoundEmissionData(position, soundSource));
            PlayAtPosition(soundSource.AudioClip, position, soundSource.Volume, mixerGroup);
        }

        public static void PlayRandomAtPosition(SOSoundSource soundSource, Vector3 position, SOSoundEmissionDataEvent newAudioSphereEvent = null, string mixerGroup = null)
        {
            if (soundSource?.RandomAudioClips == null || soundSource.RandomAudioClips.Length == 0)
            {
                if (soundSource?.AudioClip != null)
                {
                    // Fallback to main audio clip if random clips are not available
                    PlayAtPosition(soundSource, position, newAudioSphereEvent, mixerGroup);
                    return;
                }
                Log.W("SoundSource RandomAudioClips is null or empty. Cannot play random sound.", _LOG_COLOR, _LOG_TAG);
                return;
            }

            var index = Random.Range(0, soundSource.RandomAudioClips.Length);
            AudioClip clip = soundSource.RandomAudioClips[index];

            newAudioSphereEvent?.RaiseEvent(new SoundEmissionData(position, soundSource, clip));
            PlayAtPosition(clip, position, soundSource.Volume, mixerGroup);

        }


        public static void PlayRandomInAudioSource(SOSoundSource soundSource, SOSoundEmissionDataEvent newAudioSphereEvent = null, string mixerGroup = null, AudioSource audioSource = null)
        {
            if (soundSource?.RandomAudioClips == null || soundSource.RandomAudioClips.Length == 0)
            {
                if (soundSource?.AudioClip != null)
                {
                    // Fallback to main audio clip if random clips are not available
                    newAudioSphereEvent?.RaiseEvent(new SoundEmissionData(audioSource.transform.position, soundSource));
                    PlayInAudioSource(soundSource.AudioClip, soundSource.Volume, mixerGroup, audioSource);
                    return;
                }
                Log.W("SoundSource RandomAudioClips is null or empty. Cannot play random sound.", _LOG_COLOR, _LOG_TAG);
                return;
            }

            var index = Random.Range(0, soundSource.RandomAudioClips.Length);
            AudioClip clip = soundSource.RandomAudioClips[index];

            newAudioSphereEvent?.RaiseEvent(new SoundEmissionData(audioSource.transform.position, soundSource, clip));
            PlayInAudioSource(clip, soundSource.Volume, mixerGroup, audioSource);
        }



        public static void PlayAtPosition(AudioClip clip, Vector3 position, float volume, string mixerGroup)
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


        // public static void PlayAtPositionSchedule(AudioClip clip, Vector3 position, float volume, string mixerGroup, double scheduledTime)
        // {
        //     GameObject tempGO = new GameObject("TempAudio");
        //     tempGO.transform.position = position;
        //     AudioSource aSource = tempGO.AddComponent<AudioSource>();
        //     aSource.spatialBlend = 1.0f; // 3D sound
        //     aSource.clip = clip;
        //     aSource.volume = volume;
        //     aSource.outputAudioMixerGroup = mixerGroup == null ? _mixer.FindMatchingGroups("Master")[0] : _mixer.FindMatchingGroups(mixerGroup)[0];
        //     aSource.Play();
        //     Object.Destroy(tempGO, clip.length);
        // }

        private static void PlayInAudioSource(AudioClip clip, float volume, string mixerGroup, AudioSource aSource = null)
        {
            aSource.outputAudioMixerGroup = mixerGroup == null ? _mixer.FindMatchingGroups("Master")[0] : _mixer.FindMatchingGroups(mixerGroup)[0];
            aSource.clip = clip;
            aSource.volume = volume;
            aSource.Play();
        }




    }
}