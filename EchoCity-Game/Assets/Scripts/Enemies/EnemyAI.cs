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
    [Tooltip("Event for emitting sounds from enemy (for echolocation system). Used by states.")]
    public SONewAudioSphereEvent enemySoundEmissionEvent;

    [Header("Observed Events")]
    [Tooltip("Event raised by InputManager when player performs an action (e.g., hitting object with item)")]
    [SerializeField] private SOPlayerActionEvent playerActionEvent;

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

    #region Noise/Annoyance System (Centralized)
    // Single active player action (only one action at a time)
    // Actions come from InputManager when player performs actions (e.g., hitting object with item)
    private PlayerActionData? _activePlayerAction = null;
    private float _actionTimeRemaining = 0f;
    
    // Attraction value (calculated and ready for states)
    private float _attraction = 0f;
    
    // Action position (for states to use - where the action occurred)
    private Vector3 _actionPosition = Vector3.zero;
    private bool _hasActiveAction = false;
    
    // Last action position that triggered chase (saved even after action expires)
    // Used by ChaseSoundState to investigate the source
    private Vector3 _lastChaseActionPosition = Vector3.zero;
    private bool _hasLastChaseActionPosition = false;
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
        if (playerActionEvent != null)
        {
            playerActionEvent.OnEventRaised += OnPlayerAction;
        }
    }

    void OnDisable()
    {
        if (playerActionEvent != null)
        {
            playerActionEvent.OnEventRaised -= OnPlayerAction;
        }
    }

    void Update()
    {
        if (enemyData == null)
        {
            _fsm.Update(_attraction);
            return;
        }

        // Update player action duration and remove if expired
        UpdateActiveAction();
        
        // Calculate attraction (centralized logic)
        CalculateAttraction();
        
        // Notify UI with current attraction value (UI developer handles thresholds)
        NotifyUI();
        
        // Update FSM with calculated attraction (states receive ready value)
        _fsm.Update(_attraction);
    }

    /// <summary>
    /// Updates active player action duration and removes it if expired
    /// </summary>
    void UpdateActiveAction()
    {
        if (_hasActiveAction && _activePlayerAction.HasValue)
        {
            _actionTimeRemaining -= Time.deltaTime;
            
            if (_actionTimeRemaining <= 0f)
            {
                // Action expired - clear it
                // BUT keep the position if it triggered a chase (for ChaseSoundState)
                _activePlayerAction = null;
                _hasActiveAction = false;
                // Don't reset _actionPosition here - it's used by GetSoundPosition()
                // It will be updated when a new action arrives or cleared when chase ends
            }
        }
    }

    /// <summary>
    /// Centralized attraction calculation using Attraction.cs formula
    /// Handles: accumulation when player action is active, decay when no action, clamp to 0
    /// Uses action properties (intensity, duration, frequency) from the object/item used by player
    /// </summary>
    void CalculateAttraction()
    {
        if (_hasActiveAction && _activePlayerAction.HasValue)
        {
            var action = _activePlayerAction.Value;
            
            // Update action position (where the action occurred)
            _actionPosition = action.position;
            
            // Calculate distance to action position 
            float distance = Vector3.Distance(transform.position, action.position);    
            
            // Calculate frequency multiplier
            float frequencyMultiplier = GetFrequencyMultiplier(action.frequency);
            
            // Calculate distance multiplier using Attraction.cs formula
            // Formula: intensity * intensityFactor / Pow((distance + 0.01) * rangeFactor, decay)
            float distanceMultiplier = 1f / Mathf.Pow((distance + 0.01f) * enemyData.NoiseRangeFactor, enemyData.NoiseDistanceDecay);
            
            // Accumulate attraction per frame (continuous accumulation)
            // Uses action intensity (from object/item used by player)
            float contribution = action.intensity 
                                * enemyData.NoiseIntensityFactor 
                                * frequencyMultiplier 
                                * distanceMultiplier 
                                * Time.deltaTime;
            
            _attraction += contribution;
        }
        else
        {
            // No active action - apply decay
            ApplyDecay();
        }
        
        // Clamp attraction to 0 (never negative)
        _attraction = Mathf.Max(0f, _attraction);
    }

    /// <summary>
    /// Applies decay to attraction when no player action is active or action is too far
    /// </summary>
    void ApplyDecay()
    {
        if (_attraction > 0f)
        {
            _attraction -= enemyData.NoiseDecayRate * Time.deltaTime;
        }
    }

    /// <summary>
    /// Called when player performs an action (e.g., hitting object with item)
    /// Raised by InputManager - replaces current action (only one action at a time)
    /// Action properties (intensity, duration, frequency) come from the object/item used
    /// </summary>
    void OnPlayerAction(PlayerActionData actionData)
    {
        // Replace current action with new one (only one action active at a time)
        _activePlayerAction = actionData;
        _actionTimeRemaining = actionData.duration;
        _hasActiveAction = true;
        _actionPosition = actionData.position;
        
        // Save position for chase investigation (even if action expires later)
        _lastChaseActionPosition = actionData.position;
        _hasLastChaseActionPosition = true;
    }

    /// <summary>
    /// Emits a sound from the enemy (for echolocation system).
    /// Called by states when enemy makes noise (e.g., investigation phrases).
    /// </summary>
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

    #region Public Getters for States
    /// <summary>
    /// Returns current attraction value (ready to use by states)
    /// </summary>
    public float GetAttraction()
    {
        return _attraction;
    }

    /// <summary>
    /// Returns the position of the active player action (or last chase action position if expired)
    /// This is where the action occurred (e.g., where player hit an object)
    /// Used by ChaseSoundState to investigate the source
    /// </summary>
    public Vector3 GetSoundPosition()
    {
        // If there's an active action, return its position
        if (_hasActiveAction && _actionPosition != Vector3.zero)
        {
            return _actionPosition;
        }
        
        // Otherwise, return the last known action position that triggered chase
        if (_hasLastChaseActionPosition)
        {
            return _lastChaseActionPosition;
        }
        
        // Fallback: return current action position (might be zero)
        return _actionPosition;
    }

    /// <summary>
    /// Returns whether there is an active player action
    /// </summary>
    public bool HasActiveSound()
    {
        return _hasActiveAction;
    }
    
    /// <summary>
    /// Clears the last chase action position (called when investigation is complete)
    /// </summary>
    public void ClearLastChaseActionPosition()
    {
        _hasLastChaseActionPosition = false;
        _lastChaseActionPosition = Vector3.zero;
    }

    /// <summary>
    /// Resets attraction to 0 (called by states when needed)
    /// </summary>
    public void ResetAttraction()
    {
        _attraction = 0f;
    }
    #endregion

    #region Helper Methods (used internally)
    /// <summary>
    /// Calculates frequency multiplier: Low=1x, Mid=1.5x, High=2x
    /// </summary>
    float GetFrequencyMultiplier(float frequency)
    {
        if (frequency <= 0f) return 1f;      // Low
        if (frequency <= 1f) return 1.5f;      // Mid
        return 2f;                             // High
    }

    /// <summary>
    /// Notifies UI with current attraction value (UI developer handles all thresholds and logic)
    /// </summary>
    void NotifyUI()
    {
        if (noiseUIEvent == null || enemyData == null) return;

        // Check if enemy is currently chasing (Chase or Attack state)
        bool isChasing = CurrentState == EnemyStatesEnum.Chase || CurrentState == EnemyStatesEnum.Attack;

        // Pass attraction value and chase status - UI developer decides when to show/hide
        var noiseData = new EnemyNoiseData(this, _attraction, 0f, enemyData.NoiseThreshold, isChasing);
        noiseUIEvent.RaiseEvent(noiseData);
    }
    #endregion

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