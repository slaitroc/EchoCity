using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class EnemyAI : MonoBehaviour
{
    #region Constants 
    private string _LOG_TAG = "ENEMY AI";
    private string _LOG_COLOR = "#ff0000ff";
    #endregion
    public enum State { Patrol, Chase }

    #region  Serialized Fields

    [Header("Invoking Events")]
    [SerializeField] private SOEnemyIAEvent playerHitEvent;
    [SerializeField] private SOEnemyNoiseUIEvent noiseUIEvent;
    public SOEnemyInvestigationEvent investigationEvent;

    [Header("Observed Events")]
    [SerializeField] private SOSoundEmissionDataEvent newAudioSphereEvent;

    [Header("References")]
    public NavMeshAgent agent;
    public Animator animator;
    public AudioSource audioSource;
    public SOEnemyData enemyData;
    public Transform player;
    public Transform[] waypoints;
    public AttackRangeDetector attackRangeDetector;

    [Header("FSM")]
    private EnemyFSM _fsm;
    public EnemyStatesEnum CurrentState;
    #endregion

    #region Noise/Annoyance System
    private float _currentNoiseLevel = 0f;
    private bool _wasShowingUI = false;
    private Vector3 _lastNoisePosition = Vector3.zero;
    private bool _hasLastNoisePosition = false;
    
    // List of active sounds contributing to noise
    private struct ActiveSound
    {
        public Vector3 position;
        public float intensity;
        public float frequency;
        public float radius;
        public float timeRemaining;
        
        public ActiveSound(SoundEmissionData soundData)
        {
            position = soundData.position;
            intensity = soundData.intensity;
            frequency = soundData.frequency;
            radius = soundData.radius;
            timeRemaining = soundData.duration;
        }
    }
    
    private System.Collections.Generic.List<ActiveSound> _activeSounds = new System.Collections.Generic.List<ActiveSound>();
    #endregion

    void Awake()
    {
        TryGetComponent(out agent);
        TryGetComponent(out animator);
        TryGetComponent(out audioSource);
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (!p) Log.D("No player found for EnemyAI on " + gameObject.name, _LOG_COLOR, _LOG_TAG);
        else player = p.transform;

        if (enemyData == null)
        {
            Log.E("No SOEnemyData assigned to EnemyAI on " + gameObject.name, _LOG_COLOR, _LOG_TAG);
        }
        if (waypoints == null || waypoints.Length == 0)
        {
            Log.E("No waypoints assigned to EnemyAI on " + gameObject.name, _LOG_COLOR, _LOG_TAG);
        }
        if (attackRangeDetector == null)
        {
            Log.E("No AttackRangeDetector assigned to EnemyAI on " + gameObject.name, _LOG_COLOR, _LOG_TAG);
        }
        
        _fsm = new EnemyFSM(this);
        _fsm.Initialize();
    }

    void OnEnable()
    {
        if (newAudioSphereEvent != null)
        {
            newAudioSphereEvent.OnEventRaised += OnSoundEmitted;
        }
    }

    void OnDisable()
    {
        if (newAudioSphereEvent != null)
        {
            newAudioSphereEvent.OnEventRaised -= OnSoundEmitted;
        }
    }

    void Update()
    {
        if (enemyData == null)
        {
            _fsm.Update(Vector3.Distance(transform.position, player.position));
            return;
        }

        // Remove expired sounds
        for (int i = _activeSounds.Count - 1; i >= 0; i--)
        {
            var sound = _activeSounds[i];
            sound.timeRemaining -= Time.deltaTime;
            
            if (sound.timeRemaining <= 0f)
            {
                _activeSounds.RemoveAt(i);
            }
            else
            {
                _activeSounds[i] = sound;
            }
        }

        // Accumulate noise from active sounds (continuous accumulation)
        float noiseAccumulation = 0f;
        Vector3 strongestNoisePosition = Vector3.zero;
        float strongestNoiseContribution = 0f;
        
        foreach (var sound in _activeSounds)
        {
            float distance = Vector3.Distance(sound.position, transform.position);
            
            // Skip if sound is too far away
            if (distance > enemyData.MaxNoisePerceptionDistance) continue;

            // Calculate noise contribution using Attraction formula
            float frequencyMultiplier = GetFrequencyMultiplier(sound.frequency);
            float distanceMultiplier = GetDistanceMultiplier(distance, sound.radius);
            
            // Continuous accumulation per frame (like Attraction.cs)
            float contribution = sound.intensity * frequencyMultiplier * distanceMultiplier * Time.deltaTime;
            noiseAccumulation += contribution;
            
            // Track position of strongest contributing sound (for investigation)
            if (contribution > strongestNoiseContribution)
            {
                strongestNoiseContribution = contribution;
                strongestNoisePosition = sound.position;
            }
        }
        
        // Update last noise position with strongest sound position (only if we have active sounds)
        if (strongestNoiseContribution > 0f)
        {
            _lastNoisePosition = strongestNoisePosition;
            _hasLastNoisePosition = true;
        }

        // Add accumulated noise
        _currentNoiseLevel += noiseAccumulation;

        // Decay noise over time
        _currentNoiseLevel = Mathf.Max(0f, _currentNoiseLevel - enemyData.NoiseDecayRate * Time.deltaTime);

        // Check if UI should be shown/hidden and notify
        UpdateNoiseUI();

        _fsm.Update(Vector3.Distance(transform.position, player.position));
    }

    private void OnSoundEmitted(SoundEmissionData soundData)
    {
        if (enemyData == null) return;

        // Add sound to active sounds list (will accumulate continuously in Update)
        _activeSounds.Add(new ActiveSound(soundData));
    }

    private float GetFrequencyMultiplier(float frequency)
    {
        // Frequency enum: Low = 0, Mid = 1, High = 2
        // Convert to multiplier: Low = 1x, Mid = 1.5x, High = 2x
        if (frequency <= 0.5f) return 1f;      // Low
        if (frequency <= 1.5f) return 1.5f;     // Mid
        return 2f;                               // High
    }

    private float GetDistanceMultiplier(float distance, float soundRadius)
    {
        if (enemyData == null) return 0f;

        // Use Attraction.cs formula: intensityFactor / Pow((distance + 0.01) * rangeFactor, decay)
        // Adapted to include soundRadius as rangeFactor if needed, or use enemyData parameters
        float rangeFactor = enemyData.NoiseRangeFactor;
        float decay = enemyData.NoiseDistanceDecay;
        float intensityFactor = enemyData.NoiseIntensityFactor;

        // Calculate using inverse power law (like Attraction.cs)
        float distMult = intensityFactor / Mathf.Pow((distance + 0.01f) * rangeFactor, decay);

        // Clamp to 0 if beyond max perception distance
        if (distance > enemyData.MaxNoisePerceptionDistance)
        {
            return 0f;
        }

        return distMult;
    }

    private void UpdateNoiseUI()
    {
        if (enemyData == null || noiseUIEvent == null) return;

        bool shouldShowUI = _currentNoiseLevel >= enemyData.NoiseUIThreshold && 
                           _currentNoiseLevel < enemyData.NoiseThreshold;

        // Notify UI if state changed (show or hide)
        if (shouldShowUI != _wasShowingUI)
        {
            var noiseData = new EnemyNoiseData(
                this,
                _currentNoiseLevel,
                enemyData.NoiseUIThreshold,
                enemyData.NoiseThreshold
            );
            noiseUIEvent.RaiseEvent(noiseData);
            _wasShowingUI = shouldShowUI;
        }
        // Also update if already showing (for continuous updates)
        else if (shouldShowUI)
        {
            var noiseData = new EnemyNoiseData(
                this,
                _currentNoiseLevel,
                enemyData.NoiseUIThreshold,
                enemyData.NoiseThreshold
            );
            noiseUIEvent.RaiseEvent(noiseData);
        }
    }

    /// <summary>
    /// Get the current noise/annoyance level for this enemy
    /// </summary>
    public float GetNoiseLevel()
    {
        return _currentNoiseLevel;
    }

    /// <summary>
    /// Reset noise level (useful for special events)
    /// </summary>
    public void ResetNoiseLevel()
    {
        _currentNoiseLevel = 0f;
        _wasShowingUI = false;
        _activeSounds.Clear(); // Clear active sounds when resetting
    }

    /// <summary>
    /// Set noise level directly (used when entering chase to ensure minimum threshold)
    /// </summary>
    public void SetNoiseLevel(float level)
    {
        _currentNoiseLevel = Mathf.Max(0f, level);
    }

    /// <summary>
    /// Get the last known noise position (where the enemy heard the sound that triggered chase)
    /// </summary>
    public Vector3 GetLastNoisePosition()
    {
        return _hasLastNoisePosition ? _lastNoisePosition : transform.position;
    }

    /// <summary>
    /// Check if there's a valid last noise position
    /// </summary>
    public bool HasLastNoisePosition()
    {
        return _hasLastNoisePosition;
    }

    /// <summary>
    /// Set the last known noise position (called when starting to chase)
    /// </summary>
    public void SetLastNoisePosition(Vector3 position)
    {
        _lastNoisePosition = position;
        _hasLastNoisePosition = true;
    }

    /// <summary>
    /// Clear the last noise position
    /// </summary>
    public void ClearLastNoisePosition()
    {
        _hasLastNoisePosition = false;
    }

    public void OnPlayerHit()
    {
        if (_fsm.CurrentState.OnPlayerHit())
            playerHitEvent?.RaiseEvent(this);
    }

    /// <summary>
    /// Play a random investigation phrase audio clip and raise the investigation event
    /// </summary>
    public void PlayInvestigationPhrase()
    {
        if (enemyData == null || enemyData.InvestigationPhrases == null || enemyData.InvestigationPhrases.Length == 0)
            return;

        // Filter out null clips
        System.Collections.Generic.List<AudioClip> validClips = new System.Collections.Generic.List<AudioClip>();
        foreach (var clip in enemyData.InvestigationPhrases)
        {
            if (clip != null) validClips.Add(clip);
        }

        if (validClips.Count == 0) return;

        // Pick a random phrase from the array
        int randomIndex = Random.Range(0, validClips.Count);
        AudioClip selectedClip = validClips[randomIndex];

        // Play the audio clip
        if (audioSource != null && selectedClip != null)
        {
            audioSource.PlayOneShot(selectedClip);
        }

        // Raise investigation event
        if (investigationEvent != null)
        {
            var investigationData = new EnemyInvestigationData(this, selectedClip);
            investigationEvent.RaiseEvent(investigationData);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (enemyData == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemyData.ChaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyData.LoseRange);

        // Gizmos.color = Color.magenta;
        // Gizmos.DrawWireSphere(transform.position, enemyData.KillRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, enemyData.AttackRange);
    }
}