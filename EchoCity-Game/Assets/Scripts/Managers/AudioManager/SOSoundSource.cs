using System;
using UnityEngine;


[CreateAssetMenu(fileName = "SoundSourceSO", menuName = "ECHO CITY/Sound/SoundSourceSO")]
public class SOSoundSource : ScriptableObject
{
    public Frequency Frequency;
    public AudioClip AudioClip;
    public float Intensity;
    public float Radius;
    // public bool IsLoop;
    public float GapBetweenSounds = 0f;
}
