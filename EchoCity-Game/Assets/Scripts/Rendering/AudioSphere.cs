using UnityEngine;

[System.Serializable]
public struct AudioSphere
{
    public Vector3 position;
    public float radius;
    public float maxIntensity;
    public float currentIntensity;
    public float timeRemaining;
    public float totalDuration;
    public float audioClipDuration;
    public float fadeInDuration;
    public float fadeOutDuration;

    public AudioSphere(Vector3 pos, float rad, float intens, float audioDuration, float totalDur, float fadeIn = 0.1f, float fadeOut = 0.3f)
    {
        position = pos;
        radius = rad;
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
