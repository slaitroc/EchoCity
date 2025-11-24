using UnityEngine;
using System.Collections.Generic;
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

    [Header("Echo Settings")]
    [SerializeField] private Frequency objectFrequency = Frequency.Low;
    [Min(0f)]
    [SerializeField] private float echoRadius = 10f;
    [SerializeField] private bool emitOnStart = false;

    [Tooltip("Time from the end of a sound to the start of the next sound (0 = single emission)")]
    [Min(0f)]
    [SerializeField] private float gapBetweenSounds = 0f;

    [Tooltip("Minimum visibility duration, use if audio is very short")]
    [Min(0.1f)]
    [SerializeField]
    private float minimumDuration = 0.1f;

    [Min(0f)]
    [SerializeField] private float echoIntensity = 0.5f;


    [Header("Audio Settings")]
    public AudioClip audioClip;

    [Tooltip("Overrides audioClip")]
    [SerializeField] private AudioClip[] randomAudioClips;
    private List<AudioClip> _validClips = new();


    #endregion

    #region Private Fields
    private AudioSource audioSource;
    private float nextAutoEmit;
    #endregion


    void Awake() => RebuildValidClips();

    void Start()
    {
        TryGetComponent(out audioSource);
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D sound

        if (emitOnStart)
        {
            EmitSound();
        }

        if (gapBetweenSounds > 0f)
        {
            nextAutoEmit = Time.time + gapBetweenSounds;
        }
    }

    void Update()
    {
        if (gapBetweenSounds > 0f && Time.time >= nextAutoEmit)
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
            audioDuration = clipToPlay.length <= minimumDuration ? minimumDuration : clipToPlay.length;

            nextAutoEmit = Time.time + audioDuration + gapBetweenSounds;
            audioSource.PlayOneShot(clipToPlay);
        }

        newAudioSphereEvent?.RaiseEvent(new SoundEmissionData(transform.position, echoRadius, echoIntensity, audioDuration, objectFrequency));
    }

    // If randomAudioClips has valid clips, return one at random; otherwise return the main audioClip
    // This means that:
    // - If both randomAudioClips and audioClip are null, returns null
    // - If randomAudioClips is empty or has only nulls, returns audioClip
    // - If randomAudioClips has valid clips, returns one of them at random regardless of audioClip
    private AudioClip GetAudioClip()
    {
        if (_validClips.Count > 0) return _validClips[Random.Range(0, _validClips.Count)];
        return audioClip;
    }

    private void RebuildValidClips()
    {
        _validClips.Clear();
        if (randomAudioClips != null)
            foreach (var sound in randomAudioClips)
            {
                if (sound) _validClips.Add(sound);
            }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, echoRadius);
    }
}
