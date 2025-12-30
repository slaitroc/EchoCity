// using System;
// using NUnit.Framework;
// using Unity.VisualScripting;
// using UnityEngine;

// namespace EchoCity
// {

//     public class EventsLogger : MonoBehaviour
//     {

//         #region Constants
//         private string _LOG_TAG = "EVENTS LOGGER";
//         private string _LOG_COLOR = "#ff8800ff";
//         #endregion

//         #region Serialized Fields
//         [Header("Observing Events From")]
//         [Space(5)]
//         [Header("Game Manager")]
//         [SerializeField] private SOEventDoubleParam<GameStatesEnum, GameStatesEnum> switchGameStateEvent;

//         [Header("Enemies")]
//         [SerializeField] private SOEnemyAIEvent playerHitEvent;

//         [Header("Echolocation")]
//         [SerializeField] private SOSoundEmissionDataEvent newAudioSphereEvent;

//         [Header("Input")]
//         [SerializeField] private SOEventVoid pauseGameEvent;
//         [SerializeField] private SOEventVoid interactEvent;
//         [SerializeField] private SOIntEvent intEventExample;
//         [SerializeField] private SOStringEvent stringEventExample;
//         [SerializeField] private SOEventVoid voidEventExample;
//         [SerializeField] private SOEventVoid enablePlayerActionMapEvent;
//         [SerializeField] private SOEventVoid disablePlayerActionMapEvent;
//         [SerializeField] private SOEventVoid enableUIActionMapEvent;
//         [SerializeField] private SOEventVoid disableUIActionMapEvent;
//         [SerializeField] private SOHudEnumEvent switchToHudStateEvent;
//         [SerializeField] private SOStringColorEvent spawnWarningEvent;



//         [Header("Interactables")]
//         [SerializeField] private SOAreaInteractableEvent enterInteractionAreaEvent;
//         [SerializeField] private SOAreaInteractableEvent exitInteractionAreaEvent;
//         [SerializeField] private SOPickableDataGameObjectEvent pickedPickableEvent;

//         [Header("UI")]

//         [Header("Observed Events From GM")]
//         [SerializeField] private SOEventVoid titleMenuEvent;
//         [SerializeField] private SOHudEnumEvent hudMenuEvent;
//         [SerializeField] private SOEventVoid pauseMenuEvent;
//         [SerializeField] private SODialogDataEvent dialogMenuEvent;
//         [SerializeField] private SOEventVoid deathMenuEvent;
//         [SerializeField] private SOEventVoid enterLoadingScreenEvent;
//         [SerializeField] private SOEventVoid exitLoadingScreenEvent;

//         [Header("Misc")]
//         [SerializeField] private SOIntStringEvent feedbackSubmittedEvent;
//         #endregion

//         void OnEnable()
//         {
//             if (switchGameStateEvent != null) switchGameStateEvent.OnEventRaised += OnSwitchGameStateEvent;
//             if (playerHitEvent != null) playerHitEvent.OnEventRaised += OnPlayerHit;
//             if (newAudioSphereEvent != null) newAudioSphereEvent.OnEventRaised += OnNewAudioSphereEvent;
//             if (interactEvent != null) interactEvent.OnEventRaised += OnInteractEvent;
//             if (intEventExample != null) intEventExample.OnEventRaised += OnIntEventExample;
//             if (stringEventExample != null) stringEventExample.OnEventRaised += OnStringEventExample;
//             if (voidEventExample != null) voidEventExample.OnEventRaised += OnVoidEventExample;
//             if (enablePlayerActionMapEvent != null) enablePlayerActionMapEvent.OnEventRaised += OnEnablePlayerActionMapEvent;
//             if (disablePlayerActionMapEvent != null) disablePlayerActionMapEvent.OnEventRaised += OnDisablePlayerActionMapEvent;
//             if (enableUIActionMapEvent != null) enableUIActionMapEvent.OnEventRaised += OnEnableUIActionMapEvent;
//             if (disableUIActionMapEvent != null) disableUIActionMapEvent.OnEventRaised += OnDisableUIActionMapEvent;
//             if (switchToHudStateEvent != null) switchToHudStateEvent.OnEventRaised += OnSwitchToHudStateEvent;
//             if (spawnWarningEvent != null) spawnWarningEvent.OnEventRaised += OnSpawnWarningEvent;

//             if (enterInteractionAreaEvent != null) enterInteractionAreaEvent.OnEventRaised += OnEnterInteractionRangeEvent;
//             if (exitInteractionAreaEvent != null) exitInteractionAreaEvent.OnEventRaised += OnExitInteractionRangeEvent;
//             if (pickedPickableEvent != null) pickedPickableEvent.OnEventRaised += OnPickedPickableEvent;
//             if (pauseGameEvent != null) pauseGameEvent.OnEventRaised += OnPauseEvent;
//             if (titleMenuEvent != null) titleMenuEvent.OnEventRaised += OnTitleMenuEvent;
//             if (hudMenuEvent != null) hudMenuEvent.OnEventRaised += OnHudMenuEvent;
//             if (pauseMenuEvent != null) pauseMenuEvent.OnEventRaised += OnPauseMenuEvent;
//             if (dialogMenuEvent != null) dialogMenuEvent.OnEventRaised += OnDialogMenuEvent;
//             if (deathMenuEvent != null) deathMenuEvent.OnEventRaised += OnDeathMenuEvent;
//             if (enterLoadingScreenEvent != null) enterLoadingScreenEvent.OnEventRaised += OnEnterLoadingScreenEvent;
//             if (exitLoadingScreenEvent != null) exitLoadingScreenEvent.OnEventRaised += OnExitLoadingScreenEvent;

//             if (feedbackSubmittedEvent != null) feedbackSubmittedEvent.OnEventRaised += OnFeedbackSubmittedEvent;
//         }


//         void OnDisable()
//         {
//             if (switchGameStateEvent != null) switchGameStateEvent.OnEventRaised -= OnSwitchGameStateEvent;
//             if (pauseGameEvent != null) pauseGameEvent.OnEventRaised -= OnPauseEvent;
//             if (playerHitEvent != null) playerHitEvent.OnEventRaised -= OnPlayerHit;
//             if (newAudioSphereEvent != null) newAudioSphereEvent.OnEventRaised -= OnNewAudioSphereEvent;
//             if (interactEvent != null) interactEvent.OnEventRaised -= OnInteractEvent;
//             if (intEventExample != null) intEventExample.OnEventRaised -= OnIntEventExample;
//             if (stringEventExample != null) stringEventExample.OnEventRaised -= OnStringEventExample;
//             if (voidEventExample != null) voidEventExample.OnEventRaised -= OnVoidEventExample;
//             if (enablePlayerActionMapEvent != null) enablePlayerActionMapEvent.OnEventRaised -= OnEnablePlayerActionMapEvent;
//             if (disablePlayerActionMapEvent != null) disablePlayerActionMapEvent.OnEventRaised -= OnDisablePlayerActionMapEvent;
//             if (enableUIActionMapEvent != null) enableUIActionMapEvent.OnEventRaised -= OnEnableUIActionMapEvent;
//             if (disableUIActionMapEvent != null) disableUIActionMapEvent.OnEventRaised -= OnDisableUIActionMapEvent;
//             if (switchToHudStateEvent != null) switchToHudStateEvent.OnEventRaised -= OnSwitchToHudStateEvent;
//             if (spawnWarningEvent != null) spawnWarningEvent.OnEventRaised -= OnSpawnWarningEvent;

//             if (enterInteractionAreaEvent != null) enterInteractionAreaEvent.OnEventRaised -= OnEnterInteractionRangeEvent;
//             if (exitInteractionAreaEvent != null) exitInteractionAreaEvent.OnEventRaised -= OnExitInteractionRangeEvent;
//             if (pickedPickableEvent != null) pickedPickableEvent.OnEventRaised -= OnPickedPickableEvent;
//             if (titleMenuEvent != null) titleMenuEvent.OnEventRaised -= OnTitleMenuEvent;
//             if (hudMenuEvent != null) hudMenuEvent.OnEventRaised -= OnHudMenuEvent;
//             if (pauseMenuEvent != null) pauseMenuEvent.OnEventRaised -= OnPauseMenuEvent;
//             if (dialogMenuEvent != null) dialogMenuEvent.OnEventRaised -= OnDialogMenuEvent;
//             if (deathMenuEvent != null) deathMenuEvent.OnEventRaised -= OnDeathMenuEvent;
//             if (enterLoadingScreenEvent != null) enterLoadingScreenEvent.OnEventRaised -= OnEnterLoadingScreenEvent;
//             if (exitLoadingScreenEvent != null) exitLoadingScreenEvent.OnEventRaised -= OnExitLoadingScreenEvent;

//             if (feedbackSubmittedEvent != null) feedbackSubmittedEvent.OnEventRaised -= OnFeedbackSubmittedEvent;
//         }

//         #region Game Manager Events
//         private void OnSwitchGameStateEvent(GameStatesEnum from, GameStatesEnum to) => Log.DLazy(() => $"Switch Game State Event Raised from {from} to {to}", _LOG_TAG, _LOG_COLOR);
//         //ui
//         private void OnPauseEvent() => Log.DLazy(() => "Pause Event Raised", _LOG_TAG, _LOG_COLOR);
//         #endregion

//         #region Enemies
//         private void OnPlayerHit(EnemyAI enemyAI) => Log.DLazy(() => $"Player Hit Event Raised by Enemy: {enemyAI.gameObject.name}", _LOG_TAG, _LOG_COLOR);
//         #endregion

//         #region Echolocation 
//         private void OnNewAudioSphereEvent(SoundEmissionData soundEmissionData) => Log.DLazy(() => $"New Audio Sphere Event Raised at Position: {soundEmissionData.Position}, Radius: {soundEmissionData.Radius}, Intensity: {soundEmissionData.Intensity}", _LOG_TAG, _LOG_COLOR);
//         #endregion

//         #region Input 
//         private void OnInteractEvent() => Log.DLazy(() => "Interact Event Raised", _LOG_TAG, _LOG_COLOR);
//         private void OnIntEventExample(int value) => Log.DLazy(() => $"Int Event Raised with Value: {value}", _LOG_TAG, _LOG_COLOR);
//         private void OnStringEventExample(string value) => Log.DLazy(() => $"String Event Raised with Value: {value}", _LOG_TAG, _LOG_COLOR);
//         private void OnVoidEventExample() => Log.DLazy(() => "Void Event Raised", _LOG_TAG, _LOG_COLOR);
//         private void OnEnablePlayerActionMapEvent() => Log.DLazy(() => "Enable Player Action Map Event Raised", _LOG_TAG, _LOG_COLOR);
//         private void OnDisablePlayerActionMapEvent() => Log.DLazy(() => "Disable Player Action Map Event Raised", _LOG_TAG, _LOG_COLOR);
//         private void OnEnableUIActionMapEvent() => Log.DLazy(() => "Enable UI Action Map Event Raised", _LOG_TAG, _LOG_COLOR);
//         private void OnDisableUIActionMapEvent() => Log.DLazy(() => "Disable UI Action Map Event Raised", _LOG_TAG, _LOG_COLOR);
//         private void OnSwitchToHudStateEvent(HudEnum hud) => Log.DLazy(() => $"Switch to HUD Menu Event Raised for HUD: {hud}", _LOG_TAG, _LOG_COLOR);
//         private void OnSpawnWarningEvent(string message, Color color) => Log.DLazy(() => $"Spawn Warning Event Raised with Message: {message}, Color: {color}", _LOG_TAG, _LOG_COLOR);
//         #endregion

//         #region Interactable
//         private void OnEnterInteractionRangeEvent(Interactable interactable) => Log.DLazy(() => $"Enter Interaction Range Event Raised for Interactable: {interactable.gameObject.name}", _LOG_TAG, _LOG_COLOR);
//         private void OnExitInteractionRangeEvent(Interactable interactable) => Log.DLazy(() => $"Exit Interaction Range Event Raised for Interactable: {interactable.gameObject.name}", _LOG_TAG, _LOG_COLOR);
//         private void OnPickedPickableEvent(PickableData data, GameObject prefab) => Log.DLazy(() => $"Picked Pickable Event Raised for Pickable Data: {data.Name}", _LOG_TAG, _LOG_COLOR);
//         #endregion

//         #region UI
//         private void OnTitleMenuEvent() => Log.DLazy(() => "Title Menu Event Raised", _LOG_TAG, _LOG_COLOR);
//         private void OnHudMenuEvent(HudEnum hud) => Log.DLazy(() => $"HUD Menu Event Raised for HUD: {hud}", _LOG_TAG, _LOG_COLOR);
//         private void OnPauseMenuEvent() => Log.DLazy(() => "Pause Menu Event Raised", _LOG_TAG, _LOG_COLOR);
//         private void OnDialogMenuEvent(DialogData dialogData) => Log.DLazy(() => $"Dialog Menu Event Raised", _LOG_TAG, _LOG_COLOR);
//         private void OnDeathMenuEvent() => Log.DLazy(() => "Death Menu Event Raised", _LOG_TAG, _LOG_COLOR);
//         private void OnEnterLoadingScreenEvent() => Log.DLazy(() => "Enter Loading Screen Event Raised", _LOG_TAG, _LOG_COLOR);
//         private void OnExitLoadingScreenEvent() => Log.DLazy(() => "Exit Loading Screen Event Raised", _LOG_TAG, _LOG_COLOR);

//         #endregion

//         #region Misc
//         private void OnFeedbackSubmittedEvent(int rating, string feedback) => Log.DLazy(() => $"Feedback Submitted Event Raised with Rating: {rating}, Feedback: {feedback}", _LOG_TAG, _LOG_COLOR);
//         #endregion
//     }
// }