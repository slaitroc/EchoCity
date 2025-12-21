using System;
using UnityEngine;


[CreateAssetMenu(fileName = "SoundSourceSO", menuName = "ECHO CITY/Sound/SoundSourceSO")]
public class SOSoundSource : ScriptableObject
{
    [SerializeField] private SOSoundClass soundClass;
    [SerializeField] private float soundClassIntensityFactorMultiplier = 1f;
    #region Serialized Fields
    [Header("Echo Settings")]
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private float audioLengthReference ;
    [SerializeField] private float audioLengthOverride = 0f;
    [Range(0f, 1f)][SerializeField] private float intensity;
    [Range(0f, 1f)][SerializeField] private float volume;
    [Range(0f, 50f)][SerializeField, Min(0f)] private float radius;
    [Tooltip("Time from the end of a sound to the start of the next sound (0 = single emission)")]
    [SerializeField, Min(0f)] private float gapBetweenSounds = 0f;
    [Tooltip("Minimum visibility duration, use if audio is very short")]
    [Min(0.1f)][SerializeField] private float minimumDuration = 0.1f;
    [SerializeField] private bool emitOnStart = false;
    [Tooltip("Overrides audioClip")]
    [SerializeField] private AudioClip[] randomAudioClips;
    #endregion

    #region Public Properties
    public SOSoundClass SoundClass => soundClass;
    public float SoundClassIntensityFactorMultiplier => soundClassIntensityFactorMultiplier;
    public AudioClip AudioClip => audioClip;
    public float AudioLengthOverride => audioLengthOverride;
    public float Intensity => intensity;
    public float Volume => volume;
    public float Radius => radius;
    public float GapBetweenSounds => gapBetweenSounds;
    public float MinimumDuration => minimumDuration;
    public bool EmitOnStart => emitOnStart;
    public AudioClip[] RandomAudioClips => randomAudioClips;
    #endregion

    void OnValidate()
    {
        if (randomAudioClips != null && randomAudioClips.Length > 0)
        {
            float maxLength = 0f;
            foreach (var clip in randomAudioClips)
            {
                if (clip != null && clip.length > maxLength)
                {
                    maxLength = clip.length;
                }
            }
            audioLengthReference = Mathf.Max(audioLengthReference, maxLength);
        }
        else if (audioClip != null)
        {
            audioLengthReference = audioClip.length;
        }
    }

}
