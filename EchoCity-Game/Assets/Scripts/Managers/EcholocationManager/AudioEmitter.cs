using UnityEngine;
using System.Collections.Generic;
// using Managers.AudioManager;
using UnityEngine.UIElements;

[RequireComponent(typeof(AudioSource))]
public class AudioEmitter : MonoBehaviour
{
    #region Constants
    private const string LOG_TAG = "AUDIO EMITTER";
    private const string LOG_COLOR = "#39e8b4ff";
    #endregion

    #region Serialized Fields

    [Header("Invoking Events")]
    [SerializeField] SOSoundEmissionDataEvent newAudioSphereEvent;

    [Header("Audio Settings")]
    [SerializeField] public SOSoundSource soundSource;

    private List<AudioClip> _validClips = new();
    #endregion

    #region Private Fields
    private AudioSource audioSource;
    private float nextAutoEmit;
    #endregion


    void Awake() => RebuildValidClips();

    void Start()
    {
        if (soundSource == null)
        {
            Log.E("AudioEmitter is missing a SOSoundSource reference!");
            enabled = false;
            return;
        }

        TryGetComponent(out audioSource);
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D sound

        if (soundSource.IsAutoEmit)
        {
            EmitSound();
        }

        if (soundSource.GapBetweenSounds > 0f)
        {
            nextAutoEmit = Time.time + soundSource.GapBetweenSounds;
        }
    }

    void Update()
    {
        if (soundSource.GapBetweenSounds > 0f && Time.time >= nextAutoEmit)
        {
            EmitSound();
        }
    }

    public void EmitSound()
    {
        float audioDuration = 0f;
        AudioClip clipToPlay = GetAudioClip();
        if (!clipToPlay)
        {
            Log.W("AudioEmitter has no AudioClip to play.", LOG_COLOR, LOG_TAG);
            return;
        }
        else
        {
            audioDuration = clipToPlay.length <= soundSource.MinimumDuration ? soundSource.MinimumDuration : clipToPlay.length;

            nextAutoEmit = Time.time + audioDuration + soundSource.GapBetweenSounds;
            audioSource.PlayOneShot(clipToPlay);
        }

        newAudioSphereEvent?.RaiseEvent(new SoundEmissionData(transform.position, soundSource.Radius, soundSource.Intensity, soundSource.AudioClip.length, soundSource.Frequency));
    }

    // If randomAudioClips has valid clips, return one at random; otherwise return the main audioClip
    // This means that:
    // - If both randomAudioClips and audioClip are null, returns null
    // - If randomAudioClips is empty or has only nulls, returns audioClip
    // - If randomAudioClips has valid clips, returns one of them at random regardless of audioClip
    private AudioClip GetAudioClip()
    {
        if (_validClips.Count > 0) return _validClips[Random.Range(0, _validClips.Count)];
        return soundSource.AudioClip;
    }

    private void RebuildValidClips()
    {
        _validClips.Clear();
        if (soundSource.RandomAudioClips != null)
            foreach (var sound in soundSource.RandomAudioClips)
            {
                if (sound) _validClips.Add(sound);
            }
    }

    // private void OnDrawGizmos()
    // {
    //     if (soundSource == null)
    //     {
    //         Log.E("OnDrawGizmos | AudioEmitter is missing a SOSoundSource reference!");
    //         return;
    //     }
    //     Gizmos.color = Color.cyan;
    //     Gizmos.DrawWireSphere(transform.position, soundSource.Radius);
    // }
}
