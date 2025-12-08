using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using EchoCity;
using EchoCity;

[System.Serializable]
public class PatrolArea
{
    public string name;
    public Transform[] waypoints;

    /// <summary>
    /// Calculates the center position of this patrol area as the average of all waypoint positions.
    /// </summary>
    public Vector3 GetCenter()
    {
        if (waypoints == null || waypoints.Length == 0)
            return Vector3.zero;

        Vector3 center = Vector3.zero;
        int validWaypoints = 0;

        foreach (var waypoint in waypoints)
        {
            if (waypoint != null)
            {
                center += waypoint.position;
                validWaypoints++;
            }
        }

        return validWaypoints > 0 ? center / validWaypoints : Vector3.zero;
    }
}

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
    [SerializeField] public SOEnemyAIEvent playerHitEvent;
    [SerializeField] private SOEnemyNoiseUIEvent noiseUIEvent;
    public SOEnemyInvestigationEvent investigationEvent;
    [Tooltip("Event for emitting sounds from enemy (for echolocation system). Used by states.")]
    public SOSoundEmissionDataEvent enemySoundEmissionEvent;

    [Header("Observed Events")]
    [Tooltip("Event raised by InputManager when player performs an action (e.g., hitting object with item)")]
    [SerializeField] private SOPlayerActionEvent playerActionEvent;
    [SerializeField] private SOSoundEmissionDataVector3 playerEmittedSoundEvent;

    [Header("References")]
    public NavMeshAgent agent;
    public Animator animator;
    public AudioSource audioSource;
    public SOEnemyData enemyData;
    public Transform player;
    [Tooltip("Multiple patrol areas. Each area has its own set of waypoints. When returning to patrol, the enemy will choose the area closest to its current position.")]
    public PatrolArea[] patrolAreas;
    public AttackRangeDetector attackRangeDetector;

    [HideInInspector]
    /// <summary>
    /// Currently selected patrol area. Set automatically when returning to patrol from StandAndExamine or LostTarget states.
    /// </summary>
    public PatrolArea currentPatrolArea { get; private set; }

    [Header("FSM")]
    private EnemyFSM _fsm;
    public EnemyStatesEnum CurrentState;

    [HideInInspector]
    /// <summary>
    /// Flag indicating if the enemy has confirmed the player's existence (has chased the player).
    /// Used to differentiate between StandAndExamineState (suspicion before confirmation) 
    /// and LostTargetState (loss after a real chase).
    /// </summary>
    public bool HasConfirmedPlayer = false;

    /// <summary>
    /// True se il nemico sta inseguendo il player perché ha superato la soglia di rumore
    /// (NoiseThreshold) o è arrivato da un suono investigato. False se sta inseguendo solo
    /// per distanza (ChaseDistance/Attack senza threshold superata).
    /// </summary>
    public bool IsNoiseChaseActive { get; set; } = false;
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
    // Used by CheckSoundState to investigate the source
    private Vector3 _lastChaseActionPosition = Vector3.zero;
    private bool _hasLastChaseActionPosition = false;
    #endregion

    void Awake()
    {
        TryGetComponent(out agent);
        TryGetComponent(out animator);
        TryGetComponent(out audioSource);
        if (!TryAssignPlayer())
        {
            Log.D("No player found for EnemyAI on " + gameObject.name, _LOG_COLOR, _LOG_TAG);
        }

        if (enemyData == null)
        {
            Log.E("No SOEnemyData assigned to EnemyAI on " + gameObject.name, _LOG_COLOR, _LOG_TAG);
        }

        // Validate patrol areas
        if (patrolAreas == null || patrolAreas.Length == 0)
        {
            Log.E("No patrol areas assigned to EnemyAI on " + gameObject.name, _LOG_COLOR, _LOG_TAG);
        }

        // Initialize current patrol area if patrol areas are available
        if (patrolAreas != null && patrolAreas.Length > 0)
        {
            SelectClosestPatrolArea();
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
        if (playerEmittedSoundEvent != null)
        {
            playerEmittedSoundEvent.OnEventRaised += OnPlayerAction;
        }

        TryAssignPlayer();
    }

    void OnDisable()
    {
        if (playerEmittedSoundEvent != null)
        {
            playerEmittedSoundEvent.OnEventRaised -= OnPlayerAction;
        }
    }

    void Update()
    {
        if (!TryAssignPlayer())
        {
            return;
        }

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
    /// Sync vertical position between NavMeshAgent and model
    /// </summary>
    void LateUpdate()
    {
        if (agent == null) return;

        // Sync verticale tra NavMeshAgent e modello
        Vector3 pos = transform.position;
        pos.y = agent.nextPosition.y;
        transform.position = pos;
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
                // BUT keep the position if it triggered a chase (for CheckSoundState)
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
    void OnPlayerAction(Vector3 position, SoundEmissionData sound)
    {
        // Replace current action with new one (only one action active at a time)
        _activePlayerAction = new PlayerActionData(position, sound);
        _actionTimeRemaining = sound.Duration;
        _hasActiveAction = true;
        _actionPosition = position;

        // Save position for chase investigation (even if action expires later)
        _lastChaseActionPosition = position;
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
    /// Helper method to play a random phrase from a SOSoundSource using ECSound utility.
    /// Uses RandomAudioClips array if available, otherwise uses main AudioClip.
    /// Helper method to play a random phrase from a SOSoundSource using ECSound utility.
    /// Uses RandomAudioClips array if available, otherwise uses main AudioClip.
    /// </summary>
    public void PlayRandomPhrase(SOSoundSource soundSource)
    {
        if (enemyData == null || soundSource == null)
            if (enemyData == null || soundSource == null)
                return;

        // Use ECSound utility to play sound at enemy position
        // Pass null for echolocation event since voice lines don't need to emit sounds for echolocation
        // Use "SFX" mixer group (or null for Master)
        if (soundSource.RandomAudioClips != null && soundSource.RandomAudioClips.Length > 0)
        {
            // Use random clip from array
            ECSound.PlayRandomAtPosition(soundSource, transform.position, null, "SFX");
        }
        else if (soundSource.AudioClip != null)
        {
            // Use main audio clip
            ECSound.PlayAtPosition(soundSource, transform.position, null, "SFX");
        }
        else
        {
            Log.W("SOSoundSource has no AudioClip or RandomAudioClips. Cannot play phrase.", _LOG_COLOR, _LOG_TAG);
            return;
        }

        // Raise investigation event (optional: can decide if to raise for each type)
        if (investigationEvent != null)
        {
            AudioClip selectedClip = soundSource.RandomAudioClips != null && soundSource.RandomAudioClips.Length > 0
                ? soundSource.RandomAudioClips[Random.Range(0, soundSource.RandomAudioClips.Length)]
                : soundSource.AudioClip;

            if (selectedClip != null)
            {
                var investigationData = new EnemyInvestigationData(this, selectedClip);
                investigationEvent.RaiseEvent(investigationData);
            }
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
    /// Finds the nearest active confusing sound source within detection range.
    /// Returns null if no active confusing sound source is found.
    /// </summary>
    public IConfusingSoundSource FindNearestConfusingSoundSource()
    {
        if (enemyData == null) return null;

        IConfusingSoundSource nearest = null;
        float nearestDistance = float.MaxValue;
        float detectionRange = enemyData.ConfusingSoundDetectionRange;

        // Find all ConfusingSoundSource components in the scene
        ConfusingSoundSource[] allSources = FindObjectsOfType<ConfusingSoundSource>();

        foreach (var source in allSources)
        {
            if (!source.IsActive) continue;

            float distance = Vector3.Distance(transform.position, source.Position);

            if (distance <= detectionRange && distance < nearestDistance)
            {
                nearest = source;
                nearestDistance = distance;
            }
        }

        return nearest;
    }

    /// <summary>
    /// Checks if there's an active confusing sound source that should distract the enemy.
    /// Returns true if conditions are met (active source, in range, attraction not too high).
    /// Note: Player distance check is handled by global trigger in EnemyFSM (ChaseDistance if d <= 10m).
    /// </summary>
    public bool ShouldBeDistractedByConfusingSound()
    {
        if (enemyData == null) return false;

        IConfusingSoundSource source = FindNearestConfusingSoundSource();
        if (source == null) return false;

        // Check if attraction is too high (already in MandatoryChase territory)
        // If attraction >= 1.0, global trigger will switch to MandatoryChase anyway
        if (_attraction >= enemyData.NoiseThreshold) return false;

        return true;
    }

    /// <summary>
    /// Returns the position of the active player action (or last chase action position if expired)
    /// This is where the action occurred (e.g., where player hit an object)
    /// Used by CheckSoundState to investigate the source
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

    /// <summary>
    /// Selects the patrol area whose center is closest to the enemy's current position.
    /// Sets currentPatrolArea to the selected area.
    /// </summary>
    public void SelectClosestPatrolArea()
    {
        if (patrolAreas == null || patrolAreas.Length == 0)
        {
            currentPatrolArea = null;
            return;
        }

        Vector3 enemyPosition = transform.position;
        PatrolArea closestArea = null;
        float closestDistance = float.MaxValue;

        foreach (var area in patrolAreas)
        {
            if (area == null || area.waypoints == null || area.waypoints.Length == 0)
                continue;

            Vector3 center = area.GetCenter();
            float distance = Vector3.Distance(enemyPosition, center);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestArea = area;
            }
        }

        currentPatrolArea = closestArea;

        if (currentPatrolArea == null)
        {
            Log.W("No valid patrol area found. Ensure patrol areas have at least one waypoint.", _LOG_COLOR, _LOG_TAG);
        }
    }

    /// <summary>
    /// Returns the waypoints for the current patrol area.
    /// Used by PatrolEnemyState to get the waypoints to patrol.
    /// </summary>
    public Transform[] GetCurrentWaypoints()
    {
        if (currentPatrolArea != null && currentPatrolArea.waypoints != null && currentPatrolArea.waypoints.Length > 0)
        {
            return currentPatrolArea.waypoints;
        }

        return null;
    }
    #endregion

    #region Helper Methods (used internally)
    bool TryAssignPlayer()
    {
        if (player != null) return true;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p == null) return false;

        player = p.transform;
        return true;
    }

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

        // Check if enemy is currently chasing (any chase-related state)
        bool isChasing = CurrentState == EnemyStatesEnum.MandatoryChase ||
                         CurrentState == EnemyStatesEnum.Chase ||
                         CurrentState == EnemyStatesEnum.ChaseDistance ||
                         CurrentState == EnemyStatesEnum.Attack;

        // Pass attraction value and chase status - UI developer decides when to show/hide
        var noiseData = new EnemyNoiseData(this, _attraction, 0f, enemyData.NoiseThreshold, isChasing);
        noiseUIEvent.RaiseEvent(noiseData);
    }
    #endregion

    void OnDrawGizmosSelected()
    {
        if (enemyData == null) return;

        // D_enter: distance threshold for proximity chase (yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemyData.D_enter);

        // D_exit: distance threshold for losing player (red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyData.D_exit);

        // AttackRange: distance at which enemy can attack (cyan)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, enemyData.AttackRange);
    }
}
