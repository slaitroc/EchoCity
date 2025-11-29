using UnityEngine;

public enum Frequency
{
    Low,
    Mid,
    High
}

public struct SoundClass
{
    public float rangeFactor;
    public float intensityFactor;
    public float decay;

    public SoundClass(float rangeF, float intensityF, float dec)
    {
        rangeFactor = rangeF;
        intensityFactor = intensityF;
        decay = dec;
    }
}

public struct SoundEmissionData
{
    public Vector3 position;
    public float radius;
    public float intensity;
    public float duration;
    public Frequency frequency;
    public SoundClass soundClass;

    public SoundEmissionData(Vector3 pos, float rad, float intens, float dur, Frequency objFreq, SoundClass soundClass)
    {
        position = pos;
        radius = rad;
        intensity = intens;
        duration = dur;
        frequency = objFreq;
        this.soundClass = soundClass;

    }
}

