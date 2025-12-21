using UnityEngine;


[CreateAssetMenu(fileName = "SoundClassSO", menuName = "ECHO CITY/Sound/SoundClassSO")]
public class SOSoundClass : ScriptableObject
{
    [Tooltip("Frequency band of the sound: impacts the echolocator color"), SerializeField] private Frequency frequency = Frequency.Low;
    [Tooltip("Base attraction strength. Higher = more immediate attention."), SerializeField] private float intensityFactor = 1f;
    [Tooltip("Distance in meters where attraction strength is reduced by 50% (if Decay = 1)"), SerializeField] private float rangeFactor = 1f;
    [Tooltip("How quickly the sound's influence diminishes with distance."), SerializeField] private float decay = 1f;
    [Tooltip("How long the sound is remembered by enemies. Higher values mean longer memory."), SerializeField] private float persistence = 1f;
    [Tooltip("If true, the sound might cause enemies to lose track or become stunned."), SerializeField] private bool isConfusing = false;
    [Tooltip("If true, other enemies will recognize this as a 'friendly' sound and ignore it."), SerializeField] private bool isEnemy = false;

    public Frequency Frequency => frequency;
    public float RangeFactor => rangeFactor;
    public float IntensityFactor => intensityFactor;
    public float Decay => decay;
    public float Persistence => persistence;
    public bool IsConfusing => isConfusing;
    public bool IsEnemy => isEnemy;

}