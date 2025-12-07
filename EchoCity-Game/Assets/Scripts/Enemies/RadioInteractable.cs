using EchoCity;
using UnityEngine;

/// <summary>
/// Example implementation of an interactable radio that activates/deactivates a ConfusingSoundSource.
/// When the player interacts with this radio, it activates the confusing sound, which will attract enemies.
/// The radio automatically turns OFF after a configurable duration (default: 6 seconds).
/// 
/// Setup:
/// 1. Add this component to a GameObject (e.g., a radio model)
/// 2. Add ConfusingSoundSource component to the same GameObject
/// 3. Add a Collider (set to layer 6 - Interactable)
/// 4. Configure ConfusingSoundSource settings (Distraction Strength, etc.)
/// 5. Configure Auto Off Duration (default: 6 seconds, set to 0 for no auto-off)
/// 
/// Behavior:
/// - Player interacts → Radio turns ON → Stays ON for Auto Off Duration → Auto-OFF
/// - Player can manually turn OFF by interacting again while radio is ON
/// - Enemy will be distracted while radio is active (within detection range)
/// </summary>
[RequireComponent(typeof(ConfusingSoundSource))]
public class RadioInteractable : Interactable
{
    #region Constants
    protected override string _TYPE_LOG_TAG => "RADIO";
    protected override string _LOG_TAG => "RadioInteractable";
    #endregion

    [Header("Radio Settings")]
    [Tooltip("Whether the radio starts active or inactive")]
    [SerializeField] private bool startActive = false;
    
    [Tooltip("Duration in seconds before radio automatically turns off (0 = never auto-off)")]
    [Min(0f)]
    [SerializeField] private float autoOffDuration = 6f;
    
    [Tooltip("Optional: AudioSource to play radio sound when active")]
    [SerializeField] private AudioSource radioAudioSource;
    
    [Tooltip("Optional: AudioClip to play when radio is on")]
    [SerializeField] private AudioClip radioSoundClip;

    private ConfusingSoundSource _confusingSoundSource;
    private bool _isOn = false;
    private float _turnOnTime = 0f;
    private bool _hasAutoOffTimer = false;

    protected override void Awake()
    {
        base.Awake();
        _confusingSoundSource = GetComponent<ConfusingSoundSource>();
        
        if (_confusingSoundSource == null)
        {
            Log.E("RadioInteractable requires ConfusingSoundSource component!", _LOG_COLOR, _LOG_TAG_FULL);
            enabled = false;
            return;
        }

        // Set initial state
        _isOn = startActive;
        if (_isOn)
        {
            TurnOn();
        }
        else
        {
            _confusingSoundSource.Deactivate();
            StopRadioSound();
        }
    }

    void Update()
    {
        // Check if auto-off timer has elapsed
        if (_isOn && _hasAutoOffTimer && autoOffDuration > 0f)
        {
            if (Time.time - _turnOnTime >= autoOffDuration)
            {
                // Auto-off timer expired
                TurnOff();
                Log.D("Radio automatically turned OFF after " + autoOffDuration + " seconds", _LOG_COLOR, _LOG_TAG_FULL);
            }
        }
    }

    /// <summary>
    /// Called when player interacts with the radio (presses Interact key)
    /// </summary>
    public override void Interact()
    {
        if (_isOn)
        {
            // Radio is on - turn it off manually (player override)
            TurnOff();
            Log.D("Radio turned OFF (manual)", _LOG_COLOR, _LOG_TAG_FULL);
        }
        else
        {
            // Radio is off - turn it on
            TurnOn();
            Log.D($"Radio turned ON (will auto-off in {autoOffDuration} seconds)", _LOG_COLOR, _LOG_TAG_FULL);
        }
    }

    private void TurnOn()
    {
        _isOn = true;
        _confusingSoundSource.Activate();
        PlayRadioSound();
        
        // Start auto-off timer if duration is set
        if (autoOffDuration > 0f)
        {
            _turnOnTime = Time.time;
            _hasAutoOffTimer = true;
        }
        else
        {
            _hasAutoOffTimer = false;
        }
        
        // Optional: Add visual feedback (e.g., light, animation)
        // You can add particle effects, light components, etc. here
    }

    private void TurnOff()
    {
        _isOn = false;
        _confusingSoundSource.Deactivate();
        StopRadioSound();
        _hasAutoOffTimer = false; // Cancel timer
        
        // Optional: Remove visual feedback
    }

    private void PlayRadioSound()
    {
        if (radioAudioSource != null && radioSoundClip != null)
        {
            radioAudioSource.clip = radioSoundClip;
            radioAudioSource.loop = true;
            radioAudioSource.Play();
        }
    }

    private void StopRadioSound()
    {
        if (radioAudioSource != null && radioAudioSource.isPlaying)
        {
            radioAudioSource.Stop();
        }
    }

    public override void CheckTagsHandler(PuzzleTagEnum[] tagsToCheck)
    {
        throw new System.NotImplementedException();
    }

    public override void SetTagsHandler(PuzzleTagEnum[] tagsToSet)
    {
        throw new System.NotImplementedException();
    }

    public override void InteractionOutcomeHandler(bool outcome)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Public method to check if radio is currently on
    /// </summary>
    public bool IsOn => _isOn;
}

