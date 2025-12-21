using UnityEngine;

public enum Frequency
{
    Low,
    Mid,
    High
}

public struct SoundClass
{
    private readonly float _rangeFactor;
    private readonly float _intensityFactor;
    private readonly float _decay;
    private readonly float _persistence;
    private readonly Frequency _frequency;
    private readonly bool _isConfusing;
    private readonly bool _isEnemy;
    private readonly bool _isPlayerBodySound;

    public readonly float RangeFactor => _rangeFactor;
    public readonly float IntensityFactor => _intensityFactor;
    public readonly float Decay => _decay;
    public readonly float Persistence => _persistence;
    public readonly Frequency Frequency => _frequency;
    public readonly bool IsConfusing => _isConfusing;
    public readonly bool IsEnemy => _isEnemy;
    public readonly bool IsPlayerBodySound => _isPlayerBodySound;

    public SoundClass(SOSoundClass soundClass)
    {
        _rangeFactor = soundClass.RangeFactor;
        _intensityFactor = soundClass.IntensityFactor;
        _decay = soundClass.Decay;
        _persistence = soundClass.Persistence;
        _frequency = soundClass.Frequency;
        _isConfusing = soundClass.IsConfusing;
        _isEnemy = soundClass.IsEnemy;
        _isPlayerBodySound = soundClass.IsPlayerBodySound;
    }
}

public struct SoundEmissionData
{
    private Vector3 _position;
    private  readonly float _radius;
    private readonly float _intensity;
    private readonly float _duration;
    private readonly SoundClass _soundClass;
    private readonly float _soundClassIntensityFactorMultiplier;
    
    public readonly Vector3 Position => _position;
    public readonly float Radius => _radius;
    public readonly float Intensity => _intensity;
    public readonly float Duration => _duration;
    public readonly SoundClass SoundClass => _soundClass;
    public readonly float SoundClassIntensityFactorMultiplier => _soundClassIntensityFactorMultiplier;


    public SoundEmissionData(Vector3 pos, SOSoundSource soundSource)
    {
        _position = pos;
        _radius = soundSource.Radius;
        _intensity = soundSource.Intensity;
        _soundClassIntensityFactorMultiplier = soundSource.SoundClassIntensityFactorMultiplier;
        _soundClass = new SoundClass(soundSource.SoundClass);
        _duration = soundSource.AudioLengthOverride;
    }
}

