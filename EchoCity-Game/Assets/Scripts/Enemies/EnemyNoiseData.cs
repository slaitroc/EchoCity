using UnityEngine;

/// <summary>
/// Data structure containing noise/annoyance information for UI display
/// </summary>
public struct EnemyNoiseData
{
    public EnemyAI enemy;
    public float currentNoiseLevel;
    public float uiThreshold;
    public float chaseThreshold;
    public bool shouldShowUI;

    public EnemyNoiseData(EnemyAI enemy, float currentNoise, float uiThreshold, float chaseThreshold)
    {
        this.enemy = enemy;
        this.currentNoiseLevel = currentNoise;
        this.uiThreshold = uiThreshold;
        this.chaseThreshold = chaseThreshold;
        this.shouldShowUI = currentNoise >= uiThreshold && currentNoise < chaseThreshold;
    }

    /// <summary>
    /// Returns normalized noise level (0-1) relative to UI threshold and chase threshold
    /// </summary>
    public float GetNormalizedNoise()
    {
        if (uiThreshold >= chaseThreshold) return 0f;
        float range = chaseThreshold - uiThreshold;
        return Mathf.Clamp01((currentNoiseLevel - uiThreshold) / range);
    }
}

