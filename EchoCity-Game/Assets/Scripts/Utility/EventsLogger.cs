using System;
using Unity.VisualScripting;
using UnityEngine;

namespace EchoCity
{

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
        [SerializeField] private SOEventVoid enablePlayerActionMapEvent;
        [SerializeField] private SOEventVoid disablePlayerActionMapEvent;
        [SerializeField] private SOEventVoid enableUIActionMapEvent;
        [SerializeField] private SOEventVoid disableUIActionMapEvent;



        [Header("Interactables")]
        [SerializeField] private SOAreaInteractableEvent enterInteractionAreaEvent;
        [SerializeField] private SOAreaInteractableEvent exitInteractionAreaEvent;
        [SerializeField] private SOPickableDataGameObjectEvent pickedPickableEvent;

        [Header("UI")]
        [SerializeField] private SOEventVoid quitToTitleEvent;
        [SerializeField] private SOEventVoid settingsEvent;
        [SerializeField] private SOEventVoid openRadialMenuEvent;
        [SerializeField] private SOEventVoid closeRadialMenuEvent;
        [SerializeField] private SOEventVoid rebuildRadialMenuEvent;
        [SerializeField] private SODialogDataEvent dialogDataEvent;
        [SerializeField] private SOEventVoid startGameEvent;
        #endregion

        void OnEnable()
        {
            if (switchGameStateEvent != null) switchGameStateEvent.OnEventRaised += OnSwitchGameStateEvent;
            if (playerHitEvent != null) playerHitEvent.OnEventRaised += OnPlayerHit;
            if (newAudioSphereEvent != null) newAudioSphereEvent.OnEventRaised += OnNewAudioSphereEvent;
            if (interactEvent != null) interactEvent.OnEventRaised += OnInteractEvent;
            if (intEventExample != null) intEventExample.OnEventRaised += OnIntEventExample;
            if (stringEventExample != null) stringEventExample.OnEventRaised += OnStringEventExample;
            if (voidEventExample != null) voidEventExample.OnEventRaised += OnVoidEventExample;
            if (enablePlayerActionMapEvent != null) enablePlayerActionMapEvent.OnEventRaised += OnEnablePlayerActionMapEvent;
            if (disablePlayerActionMapEvent != null) disablePlayerActionMapEvent.OnEventRaised += OnDisablePlayerActionMapEvent;
            if (enableUIActionMapEvent != null) enableUIActionMapEvent.OnEventRaised += OnEnableUIActionMapEvent;
            if (disableUIActionMapEvent != null) disableUIActionMapEvent.OnEventRaised += OnDisableUIActionMapEvent;
            if (enterInteractionAreaEvent != null) enterInteractionAreaEvent.OnEventRaised += OnEnterInteractionRangeEvent;
            if (exitInteractionAreaEvent != null) exitInteractionAreaEvent.OnEventRaised += OnExitInteractionRangeEvent;
            if (pickedPickableEvent != null) pickedPickableEvent.OnEventRaised += OnPickedPickableEvent;
            if (pauseGameEvent != null) pauseGameEvent.OnEventRaised += OnPauseEvent;
            if (quitToTitleEvent != null) quitToTitleEvent.OnEventRaised += OnQuitToTitleEvent;
            if (settingsEvent != null) settingsEvent.OnEventRaised += OnSettingsEvent;
            if (openRadialMenuEvent != null) openRadialMenuEvent.OnEventRaised += OnOpenRadialMenuEvent;
            if (closeRadialMenuEvent != null) closeRadialMenuEvent.OnEventRaised += OnCloseRadialMenuEvent;
            if (rebuildRadialMenuEvent != null) rebuildRadialMenuEvent.OnEventRaised += OnRebuildRadialMenuEvent;
            if (dialogDataEvent != null) dialogDataEvent.OnEventRaised += OnDialogDataEvent;
            if (startGameEvent != null) startGameEvent.OnEventRaised += OnStartGameEvent;
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
            if (enablePlayerActionMapEvent != null) enablePlayerActionMapEvent.OnEventRaised -= OnEnablePlayerActionMapEvent;
            if (disablePlayerActionMapEvent != null) disablePlayerActionMapEvent.OnEventRaised -= OnDisablePlayerActionMapEvent;
            if (enableUIActionMapEvent != null) enableUIActionMapEvent.OnEventRaised -= OnEnableUIActionMapEvent;
            if (disableUIActionMapEvent != null) disableUIActionMapEvent.OnEventRaised -= OnDisableUIActionMapEvent;
            if (enterInteractionAreaEvent != null) enterInteractionAreaEvent.OnEventRaised -= OnEnterInteractionRangeEvent;
            if (exitInteractionAreaEvent != null) exitInteractionAreaEvent.OnEventRaised -= OnExitInteractionRangeEvent;
            if (pickedPickableEvent != null) pickedPickableEvent.OnEventRaised -= OnPickedPickableEvent;
            if (quitToTitleEvent != null) quitToTitleEvent.OnEventRaised -= OnQuitToTitleEvent;
            if (settingsEvent != null) settingsEvent.OnEventRaised -= OnSettingsEvent;
            if (openRadialMenuEvent != null) openRadialMenuEvent.OnEventRaised -= OnOpenRadialMenuEvent;
            if (closeRadialMenuEvent != null) closeRadialMenuEvent.OnEventRaised -= OnCloseRadialMenuEvent;
            if (rebuildRadialMenuEvent != null) rebuildRadialMenuEvent.OnEventRaised -= OnRebuildRadialMenuEvent;
            if (dialogDataEvent != null) dialogDataEvent.OnEventRaised -= OnDialogDataEvent;
            if (startGameEvent != null) startGameEvent.OnEventRaised -= OnStartGameEvent;
        }

        #region Game Manager Events
        private void OnSwitchGameStateEvent(GameStatesEnum from, GameStatesEnum to) => Log.D($"Switch Game State Event Raised from {from} to {to}", _LOG_COLOR, _LOG_TAG);
        //ui
        private void OnPauseEvent() => Log.D("Pause Event Raised", _LOG_COLOR, _LOG_TAG);
        #endregion

        #region Enemies
        private void OnPlayerHit(EnemyAI enemyAI) => Log.D($"Player Hit Event Raised by Enemy: {enemyAI.gameObject.name}", _LOG_COLOR, _LOG_TAG);
        #endregion

        #region Echolocation 
        private void OnNewAudioSphereEvent(SoundEmissionData soundEmissionData) => Log.D($"New Audio Sphere Event Raised at Position: {soundEmissionData.Position}, Radius: {soundEmissionData.Radius}, Intensity: {soundEmissionData.Intensity}", _LOG_COLOR, _LOG_TAG);
        #endregion

        #region Input 
        private void OnInteractEvent() => Log.D("Interact Event Raised", _LOG_COLOR, _LOG_TAG);
        private void OnIntEventExample(int value) => Log.D($"Int Event Raised with Value: {value}", _LOG_COLOR, _LOG_TAG);
        private void OnStringEventExample(string value) => Log.D($"String Event Raised with Value: {value}", _LOG_COLOR, _LOG_TAG);
        private void OnVoidEventExample() => Log.D("Void Event Raised", _LOG_COLOR, _LOG_TAG);
        private void OnEnablePlayerActionMapEvent() => Log.D("Enable Player Action Map Event Raised", _LOG_COLOR, _LOG_TAG);
        private void OnDisablePlayerActionMapEvent() => Log.D("Disable Player Action Map Event Raised", _LOG_COLOR, _LOG_TAG);
        private void OnEnableUIActionMapEvent() => Log.D("Enable UI Action Map Event Raised", _LOG_COLOR, _LOG_TAG);
        private void OnDisableUIActionMapEvent() => Log.D("Disable UI Action Map Event Raised", _LOG_COLOR, _LOG_TAG);

        #endregion

        #region Interactable
        private void OnEnterInteractionRangeEvent(Interactable interactable) => Log.D($"Enter Interaction Range Event Raised for Interactable: {interactable.gameObject.name}", _LOG_COLOR, _LOG_TAG);
        private void OnExitInteractionRangeEvent(Interactable interactable) => Log.D($"Exit Interaction Range Event Raised for Interactable: {interactable.gameObject.name}", _LOG_COLOR, _LOG_TAG);
        private void OnPickedPickableEvent(PickableData data, GameObject prefab) => Log.D($"Picked Pickable Event Raised for Pickable Data: {data.Name}", _LOG_COLOR, _LOG_TAG);
        #endregion

        #region UI
        private void OnQuitToTitleEvent() => Log.D("Quit To Title Event Raised", _LOG_COLOR, _LOG_TAG);
        private void OnSettingsEvent() => Log.D("Settings Event Raised", _LOG_COLOR, _LOG_TAG);
        private void OnOpenRadialMenuEvent() => Log.D("Open Radial Menu Event Raised", _LOG_COLOR, _LOG_TAG);
        private void OnCloseRadialMenuEvent() => Log.D("Close Radial Menu Event Raised", _LOG_COLOR, _LOG_TAG);
        private void OnRebuildRadialMenuEvent() => Log.D("Rebuild Radial Menu Event Raised", _LOG_COLOR, _LOG_TAG);
        private void OnDialogDataEvent(DialogData dialogData) => Log.D($"Dialog Data Event Raised with: {dialogData.DialogLines.Length} lines", _LOG_COLOR, _LOG_TAG);
        private void OnStartGameEvent() => Log.D("Start Game Event Raised", _LOG_COLOR, _LOG_TAG);
        #endregion
    }
}