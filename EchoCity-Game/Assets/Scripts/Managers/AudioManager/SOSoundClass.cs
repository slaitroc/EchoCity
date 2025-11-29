using UnityEngine;


[CreateAssetMenu(fileName = "SoundClassSO", menuName = "ECHO CITY/Sound/SoundClassSO")]
public class SOSoundClass : ScriptableObject
{
    [SerializeField] private Frequency frequency;
    [SerializeField] private float rangeFactor;
    [SerializeField] private float intensityFactor;
    [SerializeField] private float decay;

    public Frequency Frequency => frequency;
    public float RangeFactor => rangeFactor;
    public float IntensityFactor => intensityFactor;
    public float Decay => decay;


}