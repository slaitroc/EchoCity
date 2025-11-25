using UnityEngine;


public class EventsLogger : MonoBehaviour
{

    #region Constants
    private string _LOG_TAG = "EVENTS LOGGER";
    private string _LOG_COLOR = "#ff8800ff";
    #endregion

    #region Serialized Fields
    [Header("Observing Events From")]
    [Space(5)]
    [Header("Game Manager")]
    [SerializeField] private SOEventDoubleParam<GameStatesEnum, GameStatesEnum> switchGameStateEvent;

    [Header("Enemies")]
    [SerializeField] private SOEnemyAIEvent playerHitEvent;

    [Header("Echolocation")]
    [SerializeField] private SOSoundEmissionDataEvent newAudioSphereEvent;

    [Header("Input")]
    [SerializeField] private SOEventVoid pauseGameEvent;
    [SerializeField] private SOEventVoid interactEvent;
    [SerializeField] private SOIntEvent intEventExample;
    [SerializeField] private SOStringEvent stringEventExample;
    [SerializeField] private SOEventVoid voidEventExample;

    [Header("Interactables")]
    [SerializeField] private SOAreaInteractableEvent enterInteractionAreaEvent;
    [SerializeField] private SOAreaInteractableEvent exitInteractionAreaEvent;

    [Header("UI")]
    [SerializeField] private SOEventVoid quitToTitleEvent;
    [SerializeField] private SOEventVoid settingsEvent;
    #endregion

    void OnEnable()
    {
        if (switchGameStateEvent != null)
        {
            switchGameStateEvent.OnEventRaised -= OnSwitchGameStateEvent;
            switchGameStateEvent.OnEventRaised += OnSwitchGameStateEvent;
        }
        if (playerHitEvent != null)
        {
            playerHitEvent.OnEventRaised -= OnPlayerHit;
            playerHitEvent.OnEventRaised += OnPlayerHit;
        }

        if (newAudioSphereEvent != null)
        {
            newAudioSphereEvent.OnEventRaised -= OnNewAudioSphereEvent;
            newAudioSphereEvent.OnEventRaised += OnNewAudioSphereEvent;
        }
        if (interactEvent != null)
        {
            interactEvent.OnEventRaised -= OnInteractEvent;
            interactEvent.OnEventRaised += OnInteractEvent;
        }
        if (intEventExample != null)
        {
            intEventExample.OnEventRaised -= OnIntEventExample;
            intEventExample.OnEventRaised += OnIntEventExample;
        }
        if (stringEventExample != null)
        {
            stringEventExample.OnEventRaised -= OnStringEventExample;
            stringEventExample.OnEventRaised += OnStringEventExample;
        }
        if (voidEventExample != null)
        {
            voidEventExample.OnEventRaised -= OnVoidEventExample;
            voidEventExample.OnEventRaised += OnVoidEventExample;
        }
        if (enterInteractionAreaEvent != null)
        {
            enterInteractionAreaEvent.OnEventRaised -= OnEnterInteractionRangeEvent;
            enterInteractionAreaEvent.OnEventRaised += OnEnterInteractionRangeEvent;
        }
        if (exitInteractionAreaEvent != null)
        {
            exitInteractionAreaEvent.OnEventRaised -= OnExitInteractionRangeEvent;
            exitInteractionAreaEvent.OnEventRaised += OnExitInteractionRangeEvent;
        }
        if (pauseGameEvent != null)
        {
            pauseGameEvent.OnEventRaised -= OnPauseEvent;
            pauseGameEvent.OnEventRaised += OnPauseEvent;
        }
        if (quitToTitleEvent != null)
        {
            quitToTitleEvent.OnEventRaised -= OnQuitToTitleEvent;
            quitToTitleEvent.OnEventRaised += OnQuitToTitleEvent;
        }
        if (settingsEvent != null)
        {
            settingsEvent.OnEventRaised -= OnSettingsEvent;
            settingsEvent.OnEventRaised += OnSettingsEvent;
        }



    }

    void OnDisable()
    {
        if (switchGameStateEvent != null) switchGameStateEvent.OnEventRaised -= OnSwitchGameStateEvent;
        if (pauseGameEvent != null) pauseGameEvent.OnEventRaised -= OnPauseEvent;
        if (playerHitEvent != null) playerHitEvent.OnEventRaised -= OnPlayerHit;
        if (newAudioSphereEvent != null) newAudioSphereEvent.OnEventRaised -= OnNewAudioSphereEvent;
        if (interactEvent != null) interactEvent.OnEventRaised -= OnInteractEvent;
        if (intEventExample != null) intEventExample.OnEventRaised -= OnIntEventExample;
        if (stringEventExample != null) stringEventExample.OnEventRaised -= OnStringEventExample;
        if (voidEventExample != null) voidEventExample.OnEventRaised -= OnVoidEventExample;
        if (enterInteractionAreaEvent != null) enterInteractionAreaEvent.OnEventRaised -= OnEnterInteractionRangeEvent;
        if (exitInteractionAreaEvent != null) exitInteractionAreaEvent.OnEventRaised -= OnExitInteractionRangeEvent;
        if (quitToTitleEvent != null) quitToTitleEvent.OnEventRaised -= OnQuitToTitleEvent;
        if (settingsEvent != null) settingsEvent.OnEventRaised -= OnSettingsEvent;

    }

    #region Game Manager Events
    private void OnSwitchGameStateEvent(GameStatesEnum from, GameStatesEnum to)
    {
        Log.D($"Switch Game State Event Raised from {from} to {to}", _LOG_COLOR, _LOG_TAG);
    }

    //ui
    private void OnPauseEvent()
    {
        Log.D("Pause Event Raised", _LOG_COLOR, _LOG_TAG);
    }
    #endregion

    #region Enemies
    private void OnPlayerHit(EnemyAI enemyAI)
    {
        Log.D($"Player Hit Event Raised by Enemy: {enemyAI.gameObject.name}", _LOG_COLOR, _LOG_TAG);
    }
    #endregion

    #region Echolocation 
    private void OnNewAudioSphereEvent(SoundEmissionData soundEmissionData)
    {
        Log.D($"New Audio Sphere Event Raised at Position: {soundEmissionData.position}, Radius: {soundEmissionData.radius}, Intensity: {soundEmissionData.intensity}", _LOG_COLOR, _LOG_TAG);
    }
    #endregion

    #region Input 
    private void OnInteractEvent()
    {
        Log.D("Interact Event Raised", _LOG_COLOR, _LOG_TAG);
    }

    private void OnIntEventExample(int value)
    {
        Log.D($"Int Event Raised with Value: {value}", _LOG_COLOR, _LOG_TAG);
    }

    private void OnStringEventExample(string value)
    {
        Log.D($"String Event Raised with Value: {value}", _LOG_COLOR, _LOG_TAG);
    }

    private void OnVoidEventExample()
    {
        Log.D("Void Event Raised", _LOG_COLOR, _LOG_TAG);
    }
    #endregion

    #region Interactable
    private void OnEnterInteractionRangeEvent(Interactable interactable)
    {
        Log.D($"Enter Interaction Range Event Raised for Interactable: {interactable.gameObject.name}", _LOG_COLOR, _LOG_TAG);
    }

    private void OnExitInteractionRangeEvent(Interactable interactable)
    {
        Log.D($"Exit Interaction Range Event Raised for Interactable: {interactable.gameObject.name}", _LOG_COLOR, _LOG_TAG);
    }
    #endregion

    #region UI
    private void OnQuitToTitleEvent()
    {
        Log.D("Quit To Title Event Raised", _LOG_COLOR, _LOG_TAG);
    }

    private void OnSettingsEvent()
    {
        Log.D("Settings Event Raised", _LOG_COLOR, _LOG_TAG);
    }
    #endregion


}