using System;
using System.Collections;
using EchoCity;
using Unity.VisualScripting;
using UnityEngine;

namespace EchoCity
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioClip[] titleMusicClips;
        [SerializeField] private AudioClip playingMusicClip;
        private GameObject _audioSourceGO;
        private AudioSource _audioSource1;
        private AudioSource _audioSource2;

        [Header("Observing Events")]
        [SerializeField] private SOGameManagerStateTransitionEvent gameManagerStateTransitionEvent;

        void OnEnable()
        {
            _audioSourceGO = new GameObject("AudioManager_AudioSource");
            _audioSource1 = _audioSourceGO.AddComponent<AudioSource>();
            _audioSource2 = _audioSourceGO.AddComponent<AudioSource>();
            _audioSource1.spatialBlend = 0.0f; // 2D sound
            _audioSource2.spatialBlend = 0.0f; // 2D sound
            _audioSource1.volume = 0.5f;
            _audioSource2.volume = 0.5f;
            _audioSource1.outputAudioMixerGroup = ECSound.Mixer.FindMatchingGroups("BackgroundMusic")[0];
            _audioSource2.outputAudioMixerGroup = ECSound.Mixer.FindMatchingGroups("BackgroundMusic")[0];

            gameManagerStateTransitionEvent.OnEventRaised += StateTransitionHandler;
            PlayTitleMusic();
        }

        void OnDisable()
        {
            gameManagerStateTransitionEvent.OnEventRaised -= StateTransitionHandler;
            Destroy(_audioSourceGO);
        }

        private void StateTransitionHandler(GameStatesEnum currentState, GameStatesEnum newState)
        {
            if (newState == GameStatesEnum.Playing)
            {
                _audioSource1.Stop();
                _audioSource2.Stop();
                PlayPlayingMusic();
            }
            if (newState == GameStatesEnum.Title)
            {
                _audioSource1.Stop();
                _audioSource2.Stop();
                PlayTitleMusic();
            }
        }


        private void PlayTitleMusic()
        {
            _audioSource1.clip = titleMusicClips[0];
            _audioSource2.clip = titleMusicClips[1];
            _audioSource1.loop = false;
            _audioSource2.loop = true;
            var startTime = AudioSettings.dspTime + 0.1f;
            _audioSource1.PlayScheduled(startTime);
            _audioSource2.PlayScheduled(startTime + _audioSource1.clip.length + 0.3f);
        }

        private void PlayPlayingMusic()
        {
            _audioSource1.clip = playingMusicClip;
            _audioSource1.loop = true;
            _audioSource1.Play();
        }

    }


}