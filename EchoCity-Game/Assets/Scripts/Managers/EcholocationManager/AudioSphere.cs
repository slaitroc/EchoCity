using UnityEngine;

[System.Serializable]
public struct AudioSphere
{
    [SerializeField] private Vector3 position;
    [SerializeField] private float radius;
    [SerializeField] private float frequency;
    [SerializeField] private float maxIntensity;
    [SerializeField] private float currentIntensity;
    [SerializeField] private float timeRemaining;
    [SerializeField] private float totalDuration;
    [SerializeField] private float audioClipDuration;
    [SerializeField] private float fadeInDuration;
    [SerializeField] private float fadeOutDuration;

    public Vector3 Position => position;
    public float Radius => radius;
    public float Frequency => frequency;
    public float CurrentIntensity { get => currentIntensity; set => currentIntensity = value; }
    public float TimeRemaining { get => timeRemaining; set => timeRemaining = value; }

    public AudioSphere(Vector3 pos, float rad, float freq, float intens, float audioDuration, float totalDur, float fadeIn = 0.1f, float fadeOut = 0.3f)
    {
        position = pos;
        radius = rad;
        frequency = freq;
        maxIntensity = intens;
        currentIntensity = 0f;
        timeRemaining = totalDur;
        totalDuration = totalDur;
        audioClipDuration = audioDuration;
        fadeInDuration = fadeIn;
        fadeOutDuration = fadeOut;
    }

    public bool IsExpired => timeRemaining <= 0f;

    public float GetCurrentIntensity()
    {
        float elapsedTime = totalDuration - timeRemaining;

        if (elapsedTime < fadeInDuration)
        {
            return Mathf.Lerp(0f, maxIntensity, elapsedTime / fadeInDuration);
        }
        else if (elapsedTime < audioClipDuration)
        {
            return maxIntensity;
        }
        else
        {
            float fadeOutProgress = (elapsedTime - audioClipDuration) / fadeOutDuration;
            return Mathf.Lerp(maxIntensity, 0f, fadeOutProgress);
        }
    }
}
