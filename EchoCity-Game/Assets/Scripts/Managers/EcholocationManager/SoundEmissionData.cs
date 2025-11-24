using UnityEngine;

public enum Frequency
{
    Low,
    Mid,
    High
}

public struct SoundEmissionData
{
    public Vector3 position;
    public float radius;
    public float intensity;
    public float duration;
    public Frequency frequency;

    public SoundEmissionData(Vector3 pos, float rad, float intens, float dur, Frequency objFreq)
    {
        position = pos;
        radius = rad;
        intensity = intens;
        duration = dur;
        frequency = objFreq;
    }
}

