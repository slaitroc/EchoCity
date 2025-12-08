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
        if (soundSource.AudioClip != null)
            duration = soundSource.AudioClip.length;
        else if (soundSource.RandomAudioClips != null && soundSource.RandomAudioClips.Length > 0)
            duration = soundSource.RandomAudioClips[0].length; // Approximate with first clip
        else
            duration = 0f;

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

    /// <summary>
    /// Constructor for creating SoundEmissionData directly with parameters.
    /// Frequency is a float: 0=Low, 1=Mid, 2=High
    /// </summary>
    public SoundEmissionData(Vector3 pos, float rad, float intens, float dur, float freq)
    {
        position = pos;
        radius = rad;
        intensity = intens;
        duration = dur;

        // Convert float frequency to Frequency enum
        Frequency frequencyEnum;
        if (freq <= 0f)
            frequencyEnum = Frequency.Low;
        else if (freq <= 1f)
            frequencyEnum = Frequency.Mid;
        else
            frequencyEnum = Frequency.High;

        // Create SoundClass with default values (these are typically from SOSoundSource)
        // Using reasonable defaults for investigation sounds
        soundClass = new SoundClass(
            rangeF: 1f,           // Default range factor
            intensityF: 1f,       // Default intensity factor
            dec: 0.1f,            // Default decay
            frequency: frequencyEnum
        );
    }
}

