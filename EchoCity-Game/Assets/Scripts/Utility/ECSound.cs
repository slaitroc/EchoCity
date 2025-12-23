using UnityEngine;
using UnityEngine.Audio;

namespace EchoCity
{
    public static class ECSound
    {
        private const string _LOG_TAG = "EC-SOUND";
        private static AudioMixer _mixer;
        public static AudioMixer Mixer => _mixer;

        static ECSound()
        {
            _mixer = Resources.Load<AudioMixer>("EchoCity-AudioMixer");
        }

        public static void PlayAtPosition(SOSoundSource soundSource, Vector3 position, SOSoundEmissionDataEvent newAudioSphereEvent = null, string mixerGroup = null)
        {
            Debug.Assert(soundSource != null && soundSource.AudioClip != null, $"{_LOG_TAG}-PlayAtPosition: soundSource or AudioClip is null.");
            if (soundSource == null || soundSource.AudioClip == null) return;
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
                Debug.Assert(false, $"{_LOG_TAG}-PlayRandomAtPosition: SoundSource RandomAudioClips is null or empty. Cannot play random sound.", soundSource);
                return;
            }

            var index = Random.Range(0, soundSource.RandomAudioClips.Length);
            AudioClip clip = soundSource.RandomAudioClips[index];

            newAudioSphereEvent?.RaiseEvent(new SoundEmissionData(position, soundSource));
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
                Debug.Assert(false, $"{_LOG_TAG}-PlayRandomInAudioSource: SoundSource RandomAudioClips is null or empty. Cannot play random sound.", soundSource);
                return;
            }

            var index = Random.Range(0, soundSource.RandomAudioClips.Length);
            AudioClip clip = soundSource.RandomAudioClips[index];

            newAudioSphereEvent?.RaiseEvent(new SoundEmissionData(audioSource.transform.position, soundSource));
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

        //TODO
        private static void GetEnemiesInRange(Vector3 position, float radius, AudioSource aSource)
        {
            Collider[] enemies = new Collider[10];
            int numEnemies = Physics.OverlapSphereNonAlloc(position, radius, enemies, LayerMask.GetMask("Enemy"));
            for (int i = 0; i < numEnemies; i++)
            {
                // IConfusable confuseable = enemies[i].GetComponent<IConfusable>();
                // // if (confuseable != null)
                // {
                //     confuseable.Confuse(aSource);
                // }

            }


        }




    }
}