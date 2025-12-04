using UnityEngine;

/// <summary>
/// Data structure for enemy investigation events (when enemy arrives at noise position and finds nothing)
/// </summary>
public struct EnemyInvestigationData
{
    public EnemyAI enemy;
    public AudioClip audioClip;

    public EnemyInvestigationData(EnemyAI enemy, AudioClip audioClip)
    {
        this.enemy = enemy;
        this.audioClip = audioClip;
    }
}

