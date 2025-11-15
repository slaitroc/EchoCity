using UnityEngine;

public struct SoundEmissionData
{
    public Vector3 position;
    public float radius;
    public float intensity;
    public float duration;

    public SoundEmissionData(Vector3 pos, float rad, float intens, float dur)
    {
        position = pos;
        radius = rad;
        intensity = intens;
        duration = dur;
    }
}