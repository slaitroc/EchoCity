using UnityEngine;

public enum Frequency
{
    Low,
    Mid,
    High
}

public struct SoundClass
{
    private float rangeFactor;
    private float intensityFactor;
    private float decay;
    private Frequency frequency;

    public float RangeFactor => rangeFactor;
    public float IntensityFactor => intensityFactor;
    public float Decay => decay;
    public Frequency Frequency => frequency;
    public SoundClass(float rangeF, float intensityF, float dec, Frequency frequency)
    {
        rangeFactor = rangeF;
        intensityFactor = intensityF;
        decay = dec;
        this.frequency = frequency;
    }
}

public struct SoundEmissionData
{
    private Vector3 position;
    private float radius;
    private float intensity;
    private float duration;
    private SoundClass soundClass;

    public Vector3 Position => position;
    public float Radius => radius;
    public float Intensity => intensity;
    public float Duration => duration;
    public SoundClass SoundClass => soundClass;


    public SoundEmissionData(Vector3 pos, SOSoundSource soundSource)
    {
        position = pos;
        radius = soundSource.Radius;
        intensity = soundSource.Intensity;
        duration = soundSource.AudioClip.length;
        soundClass = new SoundClass(
            soundSource.SoundClass.RangeFactor,
            soundSource.SoundClass.IntensityFactor,
            soundSource.SoundClass.Decay,
            soundSource.SoundClass.Frequency
        );
    }

    public SoundEmissionData(Vector3 pos, SOSoundSource soundSource, AudioClip clip)
    {
        position = pos;
        radius = soundSource.Radius;
        intensity = soundSource.Intensity;
        duration = clip.length;
        soundClass = new SoundClass(
            soundSource.SoundClass.RangeFactor,
            soundSource.SoundClass.IntensityFactor,
            soundSource.SoundClass.Decay,
            soundSource.SoundClass.Frequency
        );
    }
}

