using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace EchoCity
{
    public class AudioContext
    {
        public readonly SOSoundEmittedEvent NewAudioSphereEvent;
        public readonly IEventSender Sender;
        public AudioContext(IEventSender sender, SOSoundEmittedEvent newAudioSphereEvent)
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

        private static PlayerController playerController; //cached player controller

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
        /// Defaults to the player's AudioSource if none is provided.
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
        /// Defaults to the player's AudioSource if none is provided.
        /// </summary>
        public static void PlayInAudioSource(AudioClip clip, float volume, AudioSource aSource, MixerGroupEnum mixerGroup = MixerGroupEnum.Master)
        {
            if (playerController == null)
            {
                playerController = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerController>();
            }
            if (aSource == null)
            {
                aSource = playerController.PlayerAudioSource;
            }
            aSource.outputAudioMixerGroup = GetMixerGroup(mixerGroup);
            aSource.clip = clip;
            aSource.volume = volume;
            aSource.Play();
        }

        /// <summary>
        /// Plays a SOSoundSource in the given AudioSource and raises the context event.
        /// Defaults to the player's AudioSource if none is provided.
        /// </summary>
        public static void PlayInAudioSource(SOSoundSource soundSource, AudioSource aSource, AudioContext audioContext, MixerGroupEnum mixerGroup = MixerGroupEnum.Master)
        {
            audioContext.NewAudioSphereEvent?.RaiseEvent(audioContext.Sender, new SoundEmissionData(aSource.transform.position, soundSource));
            PlayInAudioSource(soundSource.AudioClip, soundSource.Volume, aSource, mixerGroup);
        }

        #region Voice Lines
        //#####################################
        //## IN GAME VOICE LINES MANAGEMENT  ##
        //#####################################

        private static Queue<DialogLine> _voiceMainPlayQueue = new();
        private static DialogLine _currentMainVoiceLine;
        private static Queue<DialogLine> _voiceSecondaryPlayQueue = new();
        private static SOShowUIEvent _showUIEvent;
        private static Coroutine _voiceMainCoroutine;
        private static Coroutine _voiceSecondaryCoroutine;
        private static bool _paused = false;
        private static bool _stopMainVoice = false;
        private static bool AddInVoiceQueueChecks(SODialogContainer container, SOShowUIEvent @event, int lineIndex)
        {
            if (!container || !@event || container.DialogLines.Length == 0)
                return false;
            if (playerController == null)
                playerController = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerController>();
            if (_showUIEvent == null)
                _showUIEvent = @event;
            if (lineIndex < 0 || lineIndex >= container.DialogLines.Length)
                return false;
            if (container.DialogLines[lineIndex].AudioClip == null)
                return false;
            return true;
        }
        public static void AddInVoicePlayQueue(SODialogContainer container, SOShowUIEvent @event, int lineIndex, bool @override = false)
        {
            if (!AddInVoiceQueueChecks(container, @event, lineIndex))
                return;
            if (@override && _voiceMainCoroutine != null)
            {
                playerController.StopCoroutine(_voiceMainCoroutine);
                playerController.PlayerVoiceAudioSource.AudioSource.Stop();
                _voiceMainCoroutine = null;
                _voiceMainPlayQueue.Clear();
            }
            if (_voiceMainCoroutine == null)
            {
                _voiceMainPlayQueue.Enqueue(container.DialogLines[lineIndex]);
                _voiceMainCoroutine = playerController.StartCoroutine(PlayVoiceMainQueueCoroutine());
            }
            else
                _voiceMainPlayQueue.Enqueue(container.DialogLines[lineIndex]);
        }

        public static void AddInSecondaryVoicePlayQueue(SODialogContainer container, SOShowUIEvent @event, int lineIndex, bool @override = false)
        {
            if (!AddInVoiceQueueChecks(container, @event, lineIndex))
                return;
            if (@override && _voiceSecondaryCoroutine != null)
            {
                playerController.StopCoroutine(_voiceSecondaryCoroutine);
                playerController.PlayerSecondaryVoiceAudioSource.AudioSource.Stop();
                _voiceSecondaryCoroutine = null;
                _voiceSecondaryPlayQueue.Clear();
            }
            if (_voiceSecondaryCoroutine == null)
            {
                _voiceSecondaryPlayQueue.Enqueue(container.DialogLines[lineIndex]);
                _voiceSecondaryCoroutine = playerController.StartCoroutine(PlayVoiceSecondaryQueueCoroutine());
            }
            else
                _voiceSecondaryPlayQueue.Enqueue(container.DialogLines[lineIndex]);
        }

        private static IEnumerator PlayVoiceMainQueueCoroutine()
        {
            var aSource = playerController.PlayerVoiceAudioSource.AudioSource;
            while (_voiceMainPlayQueue.Count > 0)
            {
                Debug.Log($"Playing Main voice line. Queue count: {_voiceMainPlayQueue.Count}");
                _currentMainVoiceLine = _voiceMainPlayQueue.Dequeue();
                PlayInAudioSource(_currentMainVoiceLine.AudioClip, 1f, aSource, MixerGroupEnum.Voice);
                _showUIEvent?.RaiseEvent(playerController, ShowableUIEnum.Subtitles, new SubtitleParams(_currentMainVoiceLine.SpeakerName, _currentMainVoiceLine.DialogText, _currentMainVoiceLine.AudioClip.length)); //FIX add timer to stop printing subtitles
                yield return new WaitWhile(() => aSource.isPlaying || _paused || _stopMainVoice);
            }
            aSource.clip = null;
            _voiceMainCoroutine = null;
        }

        private static IEnumerator PlayVoiceSecondaryQueueCoroutine()
        {
            _stopMainVoice = true;
            playerController.PlayerVoiceAudioSource.AudioSource.Pause();
            var aSource = playerController.PlayerSecondaryVoiceAudioSource.AudioSource;
            while (_voiceSecondaryPlayQueue.Count > 0)
            {
                Debug.Log($"Playing Secondary voice line. Queue count: {_voiceSecondaryPlayQueue.Count}");
                var line = _voiceSecondaryPlayQueue.Dequeue();
                PlayInAudioSource(line.AudioClip, 1f, aSource, MixerGroupEnum.Voice);
                _showUIEvent?.RaiseEvent(playerController, ShowableUIEnum.Subtitles, new SubtitleParams(line.SpeakerName, line.DialogText, line.AudioClip.length));
                yield return new WaitWhile(() => aSource.isPlaying || _paused);
            }
            aSource.clip = null;
            _stopMainVoice = false;
            playerController.PlayerVoiceAudioSource.AudioSource.UnPause();
            _showUIEvent?.RaiseEvent(playerController, ShowableUIEnum.Subtitles, new SubtitleParams(_currentMainVoiceLine.SpeakerName, _currentMainVoiceLine.DialogText, _currentMainVoiceLine.AudioClip.length));
            _voiceSecondaryCoroutine = null;
        }

        //##########################
        //## NARRATION MANAGEMENT ##
        //##########################

        private static SODialogContainer _currentContainer;
        private static SOTimerEvent _timerEvent;
        private static int _narrationIndex = 0;
        private static Coroutine _narrationCoroutine;
        private static IEventSender _narrationSender;

        public static void PlayNarration(SODialogContainer container, SOTimerEvent @event, IEventSender sender, float eventTime = 0f)
        {
            if (playerController == null)
                playerController = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerController>();
            _narrationSender = sender;
            _currentContainer = container;
            _narrationIndex = 0;
            if (_timerEvent == null)
                _timerEvent = @event;
            if (_voiceMainCoroutine != null)
            {
                playerController.StopCoroutine(_voiceMainCoroutine);
                _voiceMainCoroutine = null;
                playerController.PlayerVoiceAudioSource.AudioSource.Stop();
                _voiceMainPlayQueue.Clear();
            }
            if (_narrationCoroutine == null)
            {
                _timerEvent = @event;
                _narrationCoroutine = playerController.StartCoroutine(PlayNarrationCoroutine(0, eventTime));
            }
            PlayNarrationLine(_narrationIndex, eventTime);
        }

        public static void PlayNarrationLine(int index, float eventTime = 0f)
        {
            if (_currentContainer == null || index < 0 || index >= _currentContainer.DialogLines.Length)
                return;
            _narrationIndex = index;
            if (_narrationCoroutine == null)
                _narrationCoroutine = playerController.StartCoroutine(PlayNarrationCoroutine(index, eventTime));
            else
            {
                playerController.StopCoroutine(_narrationCoroutine);
                _narrationCoroutine = playerController.StartCoroutine(PlayNarrationCoroutine(index, eventTime));
            }
        }

        private static IEnumerator PlayNarrationCoroutine(int startIndex = 0, float eventTime = 0f)
        {
            var aSource = playerController.PlayerVoiceAudioSource.AudioSource;
            for (int i = startIndex; i < _currentContainer.DialogLines.Length; i++)
            {
                var audio = _currentContainer.DialogLines[i].AudioClip;
                var eTime = audio.length - eventTime;
                PlayInAudioSource(audio, 1f, aSource, MixerGroupEnum.Voice);
                yield return new WaitForSeconds(eTime);
                _timerEvent?.RaiseEvent(_narrationSender, TimerEventEnum.NarrationLineHalfway);
                yield return new WaitWhile(() => aSource.isPlaying || _paused);
                _timerEvent?.RaiseEvent(_narrationSender, TimerEventEnum.NarrationLineEnded);
            }
            _narrationCoroutine = null;
        }

        public static void StopNarration()
        {
            if (_narrationCoroutine != null)
            {
                playerController.StopCoroutine(_narrationCoroutine);
                _narrationCoroutine = null;
            }
            playerController.PlayerVoiceAudioSource.AudioSource.Stop();
        }

        public static void PauseAllPlayingAudioSources()
        {
            if (playerController == null)
                playerController = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerController>();
            _paused = true;
            playerController.PlayerAudioSource.Pause();
            playerController.PlayerVoiceAudioSource.AudioSource.Pause();
        }

        public static void UnPauseAllPlayingAudioSources()
        {
            if (playerController == null)
                playerController = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerController>();
            _paused = false;
            playerController.PlayerAudioSource.UnPause();
            playerController.PlayerVoiceAudioSource.AudioSource.UnPause();
        }

        #endregion

        public static void StopAllVoices()
        {
            if (playerController == null)
                playerController = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerController>();
            if (_voiceMainCoroutine != null)
            {
                playerController.StopCoroutine(_voiceMainCoroutine);
                _voiceMainCoroutine = null;
                playerController.PlayerVoiceAudioSource.AudioSource.Stop();
                _voiceMainPlayQueue.Clear();
            }
            if (_voiceSecondaryCoroutine != null)
            {
                playerController.StopCoroutine(_voiceSecondaryCoroutine);
                _voiceSecondaryCoroutine = null;
                playerController.PlayerSecondaryVoiceAudioSource.AudioSource.Stop();
                _voiceSecondaryPlayQueue.Clear();
            }
        }
    }
}