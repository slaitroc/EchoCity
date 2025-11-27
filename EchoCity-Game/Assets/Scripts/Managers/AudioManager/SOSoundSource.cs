using System;
using UnityEngine;


[CreateAssetMenu(fileName = "SoundSourceSO", menuName = "ECHO CITY/Sound/SoundSourceSO")]
public class SOSoundSource : ScriptableObject
{
    [Header("Echo Settings")]

    #region Serialized Fields

    [SerializeField] private Frequency frequency;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private float intensity;
    [SerializeField, Min(0f)] private float radius;

    [Tooltip("Time from the end of a sound to the start of the next sound (0 = single emission)")]
    [SerializeField, Min(0f)]
    private float gapBetweenSounds = 0f;


    [Tooltip("Minimum visibility duration, use if audio is very short")]
    [Min(0.1f)]
    [SerializeField]
    private float minimumDuration = 0.1f;

    [SerializeField] private bool isAutoEmit = false;

    [Tooltip("Overrides audioClip")]
    [SerializeField] private AudioClip[] randomAudioClips;

    #endregion



    #region Public Properties

    public Frequency Frequency => frequency;
    public AudioClip AudioClip => audioClip;
    public float Intensity => intensity;
    public float Radius => radius;
    public float GapBetweenSounds => gapBetweenSounds;
    public float MinimumDuration => minimumDuration;
    public bool IsAutoEmit => isAutoEmit;
    public AudioClip[] RandomAudioClips => randomAudioClips;

    #endregion


}
