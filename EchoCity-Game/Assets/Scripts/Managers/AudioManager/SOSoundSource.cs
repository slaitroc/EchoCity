using System;
using UnityEngine;


[CreateAssetMenu(fileName = "SoundSourceSO", menuName = "ECHO CITY/Sound/SoundSourceSO")]
public class SOSoundSource : ScriptableObject
{
    [SerializeField] private SOSoundClass soundClass;
    #region Serialized Fields
    [Header("Echo Settings")]
    [SerializeField] private AudioClip audioClip;
    [Range(0f, 1f)][SerializeField] private float intensity;
    [SerializeField, Min(0f)] private float radius;
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
    public AudioClip AudioClip => audioClip;
    public float Intensity => intensity;
    public float Radius => radius;
    public float GapBetweenSounds => gapBetweenSounds;
    public float MinimumDuration => minimumDuration;
    public bool EmitOnStart => emitOnStart;
    public AudioClip[] RandomAudioClips => randomAudioClips;
    #endregion


}
