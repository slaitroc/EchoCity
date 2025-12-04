using UnityEngine;

/// <summary>
/// Interface for objects that can confuse/distract enemies (e.g., radio, speaker, machine).
/// Enemies will be attracted to these objects when active.
/// </summary>
public interface IConfusingSoundSource
{
    /// <summary>
    /// Whether the confusing sound is currently active
    /// </summary>
    bool IsActive { get; }
    
    /// <summary>
    /// Position of the confusing sound source
    /// </summary>
    Vector3 Position { get; }
    
    /// <summary>
    /// Distraction strength (S_conf) - how "strong" the distraction is compared to player sounds
    /// </summary>
    float DistractionStrength { get; }
}

/// <summary>
/// MonoBehaviour component for objects that can confuse/distract enemies.
/// Attach this to objects like radios, speakers, machines, etc.
/// </summary>
public class ConfusingSoundSource : MonoBehaviour, IConfusingSoundSource
{
    [Header("Confusing Sound Settings")]
    [Tooltip("Whether the confusing sound is currently active")]
    [SerializeField] private bool isActive = false;
    
    [Tooltip("Distraction strength (S_conf) - how 'strong' the distraction is compared to player sounds")]
    [Min(0f)]
    [SerializeField] private float distractionStrength = 1.0f;
    
    public bool IsActive => isActive;
    public Vector3 Position => transform.position;
    public float DistractionStrength => distractionStrength;
    
    /// <summary>
    /// Activate the confusing sound (e.g., when player turns on a radio)
    /// </summary>
    public void Activate()
    {
        isActive = true;
    }
    
    /// <summary>
    /// Deactivate the confusing sound (e.g., when player turns off a radio)
    /// </summary>
    public void Deactivate()
    {
        isActive = false;
    }
    
    /// <summary>
    /// Toggle the confusing sound on/off
    /// </summary>
    public void Toggle()
    {
        isActive = !isActive;
    }
}

