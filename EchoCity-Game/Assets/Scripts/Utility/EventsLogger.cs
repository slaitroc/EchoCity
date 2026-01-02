using System;
using UnityEngine;


namespace EchoCity
{
    public class EventsLogger : MonoBehaviour
    {

#if UNITY_EDITOR
        public static class SenderColorCache
        {
            // Classe interna generica: la vera "magia" della cache
            private static class Storage<T>
            {
                public static readonly string HexColor = GenerateColor(typeof(T).Name);

                private static string GenerateColor(string name)
                {
                    int hash = name.GetHashCode();
                    float r = (Mathf.Abs(hash & 0xFF0000) >> 16) / 255f;
                    float g = (Mathf.Abs(hash & 0x00FF00) >> 8) / 255f;
                    float b = Mathf.Abs(hash & 0x0000FF) / 255f;

                    // Mix con il bianco per la leggibilità (0.4f = 40% bianco)
                    UnityEngine.Color c = UnityEngine.Color.Lerp(new UnityEngine.Color(r, g, b), UnityEngine.Color.white, 0.4f);
                    return "#" + ColorUtility.ToHtmlStringRGB(c);
                }
            }

            // Metodo pubblico per ottenere il colore
            // Nota: qui usiamo un po' di Reflection (MakeGenericType) solo la PRIMA volta 
            // che un tipo viene loggato. Poi diventa un accesso diretto alla memoria.
            public static string GetColor(object sender)
            {
                Type type = sender.GetType();
                // Recuperiamo il campo statico 'HexColor' dalla classe Storage<IlTuoTipo>
                var storageType = typeof(Storage<>).MakeGenericType(type);
                return (string)storageType.GetField("HexColor").GetValue(null);
            }
        }
#pragma warning disable CS0414
        [TextArea(1, 2)][SerializeField] private string description = "Events are grouped under headers based on where their logic is executed";
#pragma warning restore CS0414

        #region GM
        [Header("GM")]
        //GM to any
        [SerializeField] private SOEventDoubleParam<GameStatesEnum, GameStatesEnum> switchGameStateEvent;
        [SerializeField] private bool logSwitchGameState = true;
        //Player & UI to GM
        [SerializeField] private SOEventVoid switchToTitleStateEvent;
        [SerializeField] private bool logSwitchToTitleState = true;
        [SerializeField] private SOSceneEnumEvent switchLevelEvent;
        [SerializeField] private bool logSwitchLevel = true;
        [SerializeField] private SOEventVoid switchToPlayingStateEvent;
        [SerializeField] private bool logSwitchToPlayingState = true;
        [SerializeField] private SOEventVoid switchToPauseStateEvent;
        [SerializeField] private bool logSwitchToPauseState = true;
        [SerializeField] private SOEventVoid switchToWinStateEvent;
        [SerializeField] private bool logSwitchToWinState = true;
        [SerializeField] private SOEventVoid switchToDeathStateEvent;
        [SerializeField] private bool logSwitchToDeathState = true;
        [SerializeField] private SODialogDataEvent switchToNarrationStateEvent;
        [SerializeField] private bool logSwitchToNarrationState = true;
        [SerializeField] private SOHudEnumEvent switchToHudStateEvent;
        [SerializeField] private bool logSwitchToHudState = true;
        // SceneLoader to GM
        [SerializeField] private SOEventVoid enterLoadingEvent;
        [SerializeField] private bool logEnterLoading = true;
        [SerializeField] private SOEventVoid exitLoadingEvent;
        [SerializeField] private bool logExitLoading = true;

        void RegisterGM()
        {
            if (switchGameStateEvent != null) switchGameStateEvent.OnEventRaised += OnSwitchGameState;
            if (switchToTitleStateEvent != null) switchToTitleStateEvent.OnEventRaised += OnSwitchToTitleState;
            if (switchLevelEvent != null) switchLevelEvent.OnEventRaised += OnSwitchLevel;
            if (switchToPlayingStateEvent != null) switchToPlayingStateEvent.OnEventRaised += OnSwitchToPlayingState;
            if (switchToPauseStateEvent != null) switchToPauseStateEvent.OnEventRaised += OnSwitchToPauseState;
            if (switchToWinStateEvent != null) switchToWinStateEvent.OnEventRaised += OnSwitchToWinState;
            if (switchToDeathStateEvent != null) switchToDeathStateEvent.OnEventRaised += OnSwitchToDeathState;
            if (switchToNarrationStateEvent != null) switchToNarrationStateEvent.OnEventRaised += OnSwitchToNarrationState;
            if (switchToHudStateEvent != null) switchToHudStateEvent.OnEventRaised += OnSwitchToHudState;
            if (enterLoadingEvent != null) enterLoadingEvent.OnEventRaised += OnEnterLoading;
            if (exitLoadingEvent != null) exitLoadingEvent.OnEventRaised += OnExitLoading;
        }

        void UnregisterGM()
        {
            if (switchGameStateEvent != null) switchGameStateEvent.OnEventRaised -= OnSwitchGameState;
            if (switchToTitleStateEvent != null) switchToTitleStateEvent.OnEventRaised -= OnSwitchToTitleState;
            if (switchLevelEvent != null) switchLevelEvent.OnEventRaised -= OnSwitchLevel;
            if (switchToPlayingStateEvent != null) switchToPlayingStateEvent.OnEventRaised -= OnSwitchToPlayingState;
            if (switchToPauseStateEvent != null) switchToPauseStateEvent.OnEventRaised -= OnSwitchToPauseState;
            if (switchToWinStateEvent != null) switchToWinStateEvent.OnEventRaised -= OnSwitchToWinState;
            if (switchToDeathStateEvent != null) switchToDeathStateEvent.OnEventRaised -= OnSwitchToDeathState;
            if (switchToNarrationStateEvent != null) switchToNarrationStateEvent.OnEventRaised -= OnSwitchToNarrationState;
            if (switchToHudStateEvent != null) switchToHudStateEvent.OnEventRaised -= OnSwitchToHudState;
            if (enterLoadingEvent != null) enterLoadingEvent.OnEventRaised -= OnEnterLoading;
            if (exitLoadingEvent != null) exitLoadingEvent.OnEventRaised -= OnExitLoading;
        }

        private void OnSwitchGameState(IEventSender sender, GameStatesEnum from, GameStatesEnum to) { if (logSwitchGameState) Log.DLazy(() => $"{GetColoredName(sender)}: Switch Game State Event Raised from {from} to {to}", this); }
        private void OnSwitchToTitleState(IEventSender sender) { if (logSwitchToTitleState) Log.DLazy(() => $"{GetColoredName(sender)}: Switch to Title State Event Raised", this); }
        private void OnSwitchLevel(IEventSender sender, SceneEnum scene) { if (logSwitchLevel) Log.DLazy(() => $"{GetColoredName(sender)}: Switch Level Event Raised for Scene: {scene}", this); }
        private void OnSwitchToPlayingState(IEventSender sender) { if (logSwitchToPlayingState) Log.DLazy(() => $"{GetColoredName(sender)}: Switch to Playing State Event Raised", this); }
        private void OnSwitchToPauseState(IEventSender sender) { if (logSwitchToPauseState) Log.DLazy(() => $"{GetColoredName(sender)}: Switch to Pause State Event Raised", this); }
        private void OnSwitchToWinState(IEventSender sender) { if (logSwitchToWinState) Log.DLazy(() => $"{GetColoredName(sender)}: Switch to Win State Event Raised", this); }
        private void OnSwitchToDeathState(IEventSender sender) { if (logSwitchToDeathState) Log.DLazy(() => $"{GetColoredName(sender)}: Switch to Death State Event Raised", this); }
        private void OnSwitchToNarrationState(IEventSender sender, DialogData dialogData) { if (logSwitchToNarrationState) Log.DLazy(() => $"{GetColoredName(sender)}: Switch to Narration State Event Raised", this); }
        private void OnSwitchToHudState(IEventSender sender, HudEnum hud) { if (logSwitchToHudState) Log.DLazy(() => $"{GetColoredName(sender)}: Switch to HUD State Event Raised for HUD: {hud}", this); }
        private void OnEnterLoading(IEventSender sender) { if (logEnterLoading) Log.DLazy(() => $"{GetColoredName(sender)}: Enter Loading Event Raised", this); }
        private void OnExitLoading(IEventSender sender) { if (logExitLoading) Log.DLazy(() => $"{GetColoredName(sender)}: Exit Loading Event Raised", this); }
        #endregion

        #region Player
        [Header("Player")]
        //GM to Player
        [SerializeField] private SOEventVoid enablePlayerActionMapEvent;
        [SerializeField] private bool logEnablePlayerActionMap = true;
        [SerializeField] private SOEventVoid disablePlayerActionMapEvent;
        [SerializeField] private bool logDisablePlayerActionMap = true;
        //Enemies to Player
        [SerializeField] private SOIAttractionEvent updatePlayerAttractionTargetEvent;
        [SerializeField] private bool logUpdatePlayerAttractionTarget = true;

        void RegisterPlayer()
        {
            if (enablePlayerActionMapEvent != null) enablePlayerActionMapEvent.OnEventRaised += OnEnablePlayerActionMap;
            if (disablePlayerActionMapEvent != null) disablePlayerActionMapEvent.OnEventRaised += OnDisablePlayerActionMap;
            if (updatePlayerAttractionTargetEvent != null) updatePlayerAttractionTargetEvent.OnEventRaised += OnUpdatePlayerAttractionTarget;
        }

        void UnregisterPlayer()
        {
            if (enablePlayerActionMapEvent != null) enablePlayerActionMapEvent.OnEventRaised -= OnEnablePlayerActionMap;
            if (disablePlayerActionMapEvent != null) disablePlayerActionMapEvent.OnEventRaised -= OnDisablePlayerActionMap;
            if (updatePlayerAttractionTargetEvent != null) updatePlayerAttractionTargetEvent.OnEventRaised -= OnUpdatePlayerAttractionTarget;
        }

        private void OnEnablePlayerActionMap(IEventSender sender) { if (logEnablePlayerActionMap) Log.DLazy(() => $"{GetColoredName(sender)}: Enable Player Action Map Event Raised", this); }
        private void OnDisablePlayerActionMap(IEventSender sender) { if (logDisablePlayerActionMap) Log.DLazy(() => $"{GetColoredName(sender)}: Disable Player Action Map Event Raised", this); }
        private void OnUpdatePlayerAttractionTarget(IEventSender sender, IAttraction attraction, Transform transform, bool isActive) { if (logUpdatePlayerAttractionTarget) Log.DLazy(() => $"{GetColoredName(sender)}: Update Player Attraction Target Event Raised, Position: {transform.position}, IsActive: {isActive}", this); }
        #endregion

        #region Scene Loader
        [Header("Scene Loader")]
        //GM to SceneLoader
        [SerializeField] private SOSceneEnumEvent loadLevelEvent;
        [SerializeField] private bool logLoadLevel = true;
        [SerializeField] private SOEventVoid reloadLevelEvent;
        [SerializeField] private bool logReloadLevel = true;
        [SerializeField] private SOEventVoid setPlayerOnSpawnEvent;
        [SerializeField] private bool logSetPlayerOnSpawn = true;
        [SerializeField] private SOEventVoid unloadCurrentLevelEvent;
        [SerializeField] private bool logUnloadCurrentLevel = true;

        void RegisterSceneLoader()
        {
            if (loadLevelEvent != null) loadLevelEvent.OnEventRaised += OnLoadLevel;
            if (reloadLevelEvent != null) reloadLevelEvent.OnEventRaised += OnReloadLevel;
            if (setPlayerOnSpawnEvent != null) setPlayerOnSpawnEvent.OnEventRaised += OnSetPlayerOnSpawn;
            if (unloadCurrentLevelEvent != null) unloadCurrentLevelEvent.OnEventRaised += OnUnloadCurrentLevel;
        }

        void UnregisterSceneLoader()
        {
            if (loadLevelEvent != null) loadLevelEvent.OnEventRaised -= OnLoadLevel;
            if (reloadLevelEvent != null) reloadLevelEvent.OnEventRaised -= OnReloadLevel;
            if (setPlayerOnSpawnEvent != null) setPlayerOnSpawnEvent.OnEventRaised -= OnSetPlayerOnSpawn;
            if (unloadCurrentLevelEvent != null) unloadCurrentLevelEvent.OnEventRaised -= OnUnloadCurrentLevel;
        }

        private void OnLoadLevel(IEventSender sender, SceneEnum scene) { if (logLoadLevel) Log.DLazy(() => $"{GetColoredName(sender)}: Load Level Event Raised for Scene: {scene}", this); }
        private void OnReloadLevel(IEventSender sender) { if (logReloadLevel) Log.DLazy(() => $"{GetColoredName(sender)}: Reload Level Event Raised", this); }
        private void OnSetPlayerOnSpawn(IEventSender sender) { if (logSetPlayerOnSpawn) Log.DLazy(() => $"{GetColoredName(sender)}: Set Player On Spawn Event Raised", this); }
        private void OnUnloadCurrentLevel(IEventSender sender) { if (logUnloadCurrentLevel) Log.DLazy(() => $"{GetColoredName(sender)}: Unload Current Level Event Raised", this); }
        #endregion

        #region Interactables
        [Header("Interactables")]
        //PuzzleManager to Interactables
        [SerializeField] private SOBoolEvent interactionOutcomeEvent;
        [SerializeField] private bool logInteractionOutcome = true;

        void RegisterInteractables()
        {
            if (interactionOutcomeEvent != null) interactionOutcomeEvent.OnEventRaised += OnInteractionOutcome;
        }

        void UnregisterInteractables()
        {
            if (interactionOutcomeEvent != null) interactionOutcomeEvent.OnEventRaised -= OnInteractionOutcome;
        }

        private void OnInteractionOutcome(IEventSender sender, bool success) { if (logInteractionOutcome) Log.DLazy(() => $"{GetColoredName(sender)}: Interaction Outcome Event Raised, Success: {success}", this); }
        #endregion

        #region UI
        [Header("UI")]
        //GM to UI
        [SerializeField] private SOEventVoid enableUIActionMapEvent;
        [SerializeField] private bool logEnableUIActionMap = true;
        [SerializeField] private SOEventVoid disableUIActionMapEvent;
        [SerializeField] private bool logDisableUIActionMap = true;
        [SerializeField] private SOEventVoid titleMenuEvent;
        [SerializeField] private bool logTitleMenu = true;
        [SerializeField] private SOHudEnumEvent hudMenuEvent;
        [SerializeField] private bool logHudMenu = true;
        [SerializeField] private SOEventVoid pauseMenuEvent;
        [SerializeField] private bool logPauseMenu = true;
        [SerializeField] private SOEventVoid deathMenuEvent;
        [SerializeField] private bool logDeathMenu = true;
        [SerializeField] private SOEventVoid winMenuEvent;
        [SerializeField] private bool logWinMenu = true;
        [SerializeField] private SOEventVoid enterLoadingScreenEvent;
        [SerializeField] private bool logEnterLoadingScreen = true;
        [SerializeField] private SOEventVoid exitLoadingScreenEvent;
        [SerializeField] private bool logExitLoadingScreen = true;
        //any to UI
        [SerializeField] private SODialogDataEvent dialogMenuEvent;
        [SerializeField] private bool logDialogMenu = true;
        [SerializeField] private SOStringColorEvent warningEvent;
        [SerializeField] private bool logWarningEvent = true;
        //Player to UI
        [SerializeField] private SOBoolStringEvent canInteractStartEvent;
        [SerializeField] private bool logCanInteractStart = true;
        [SerializeField] private SOEventVoid canInteractStopEvent;
        [SerializeField] private bool logCanInteractStop = true;
        [SerializeField] private SOEventVoid inventoryChangedEvent;
        [SerializeField] private bool logInventoryChanged = true;
        //QuestManager to UI
        [SerializeField] private SOIntIntEvent questsUpdatedEvent;
        [SerializeField] private bool logQuestsUpdated = true;

        void RegisterUI()
        {
            if (enableUIActionMapEvent != null) enableUIActionMapEvent.OnEventRaised += OnEnableUIActionMap;
            if (disableUIActionMapEvent != null) disableUIActionMapEvent.OnEventRaised += OnDisableUIActionMap;
            if (titleMenuEvent != null) titleMenuEvent.OnEventRaised += OnTitleMenu;
            if (hudMenuEvent != null) hudMenuEvent.OnEventRaised += OnHudMenu;
            if (pauseMenuEvent != null) pauseMenuEvent.OnEventRaised += OnPauseMenu;
            if (deathMenuEvent != null) deathMenuEvent.OnEventRaised += OnDeathMenu;
            if (winMenuEvent != null) winMenuEvent.OnEventRaised += OnWinMenu;
            if (enterLoadingScreenEvent != null) enterLoadingScreenEvent.OnEventRaised += OnEnterLoadingScreen;
            if (exitLoadingScreenEvent != null) exitLoadingScreenEvent.OnEventRaised += OnExitLoadingScreen;
            if (dialogMenuEvent != null) dialogMenuEvent.OnEventRaised += OnDialogMenu;
            if (warningEvent != null) warningEvent.OnEventRaised += OnWarningEvent;
            if (canInteractStartEvent != null) canInteractStartEvent.OnEventRaised += OnCanInteractStart;
            if (canInteractStopEvent != null) canInteractStopEvent.OnEventRaised += OnCanInteractStop;
            if (inventoryChangedEvent != null) inventoryChangedEvent.OnEventRaised += OnInventoryChanged;
            if (questsUpdatedEvent != null) questsUpdatedEvent.OnEventRaised += OnQuestsUpdated;
        }

        void UnregisterUI()
        {
            if (enableUIActionMapEvent != null) enableUIActionMapEvent.OnEventRaised -= OnEnableUIActionMap;
            if (disableUIActionMapEvent != null) disableUIActionMapEvent.OnEventRaised -= OnDisableUIActionMap;
            if (titleMenuEvent != null) titleMenuEvent.OnEventRaised -= OnTitleMenu;
            if (hudMenuEvent != null) hudMenuEvent.OnEventRaised -= OnHudMenu;
            if (pauseMenuEvent != null) pauseMenuEvent.OnEventRaised -= OnPauseMenu;
            if (deathMenuEvent != null) deathMenuEvent.OnEventRaised -= OnDeathMenu;
            if (winMenuEvent != null) winMenuEvent.OnEventRaised -= OnWinMenu;
            if (enterLoadingScreenEvent != null) enterLoadingScreenEvent.OnEventRaised -= OnEnterLoadingScreen;
            if (exitLoadingScreenEvent != null) exitLoadingScreenEvent.OnEventRaised -= OnExitLoadingScreen;
            if (dialogMenuEvent != null) dialogMenuEvent.OnEventRaised -= OnDialogMenu;
            if (warningEvent != null) warningEvent.OnEventRaised -= OnWarningEvent;
            if (canInteractStartEvent != null) canInteractStartEvent.OnEventRaised -= OnCanInteractStart;
            if (canInteractStopEvent != null) canInteractStopEvent.OnEventRaised -= OnCanInteractStop;
            if (inventoryChangedEvent != null) inventoryChangedEvent.OnEventRaised -= OnInventoryChanged;
            if (questsUpdatedEvent != null) questsUpdatedEvent.OnEventRaised -= OnQuestsUpdated;
        }

        private void OnEnableUIActionMap(IEventSender sender) { if (logEnableUIActionMap) Log.DLazy(() => $"{GetColoredName(sender)}: Enable UI Action Map Event Raised", this); }
        private void OnDisableUIActionMap(IEventSender sender) { if (logDisableUIActionMap) Log.DLazy(() => $"{GetColoredName(sender)}: Disable UI Action Map Event Raised", this); }
        private void OnTitleMenu(IEventSender sender) { if (logTitleMenu) Log.DLazy(() => $"{GetColoredName(sender)}: Title Menu Event Raised", this); }
        private void OnHudMenu(IEventSender sender, HudEnum hud) { if (logHudMenu) Log.DLazy(() => $"{GetColoredName(sender)}: HUD Menu Event Raised for HUD: {hud}", this); }
        private void OnPauseMenu(IEventSender sender) { if (logPauseMenu) Log.DLazy(() => $"{GetColoredName(sender)}: Pause Menu Event Raised", this); }
        private void OnDeathMenu(IEventSender sender) { if (logDeathMenu) Log.DLazy(() => $"{GetColoredName(sender)}: Death Menu Event Raised", this); }
        private void OnWinMenu(IEventSender sender) { if (logWinMenu) Log.DLazy(() => $"{GetColoredName(sender)}: Win Menu Event Raised", this); }
        private void OnEnterLoadingScreen(IEventSender sender) { if (logEnterLoadingScreen) Log.DLazy(() => $"{GetColoredName(sender)}: Enter Loading Screen Event Raised", this); }
        private void OnExitLoadingScreen(IEventSender sender) { if (logExitLoadingScreen) Log.DLazy(() => $"{GetColoredName(sender)}: Exit Loading Screen Event Raised", this); }
        private void OnDialogMenu(IEventSender sender, DialogData dialogData) { if (logDialogMenu) Log.DLazy(() => $"{GetColoredName(sender)}: Dialog Menu Event Raised", this); }
        private void OnWarningEvent(IEventSender sender, string message, Color color) { if (logWarningEvent) Log.DLazy(() => $"{GetColoredName(sender)}: Warning Event Raised with Message: {message}, Color: {color}", this); }
        private void OnCanInteractStart(IEventSender sender, bool canInteract, string description) { if (logCanInteractStart) Log.DLazy(() => $"{GetColoredName(sender)}: Can Interact Start Event Raised with CanInteract: {canInteract}, Description: {description}", this); }
        private void OnCanInteractStop(IEventSender sender) { if (logCanInteractStop) Log.DLazy(() => $"{GetColoredName(sender)}: Can Interact Stop Event Raised", this); }
        private void OnInventoryChanged(IEventSender sender) { if (logInventoryChanged) Log.DLazy(() => $"{GetColoredName(sender)}: Inventory Changed Event Raised", this); }
        private void OnQuestsUpdated(IEventSender sender, int index, int code) { if (logQuestsUpdated) Log.DLazy(() => $"{GetColoredName(sender)}: Quests Updated Event Raised. Quest's index: {index}, Quest's code: {code}", this); }
        #endregion

        #region Echolocation & Enemy AI
        [Header("Echolocation & Enemy AI")]
        [SerializeField] private SOSoundEmissionDataEvent newAudioSphereEvent;
        [SerializeField] private bool logNewAudioSphere = true;

        void RegisterEcholocationAndEnemyAI()
        {
            if (newAudioSphereEvent != null) newAudioSphereEvent.OnEventRaised += OnNewAudioSphere;
        }

        void UnregisterEcholocationAndEnemyAI()
        {
            if (newAudioSphereEvent != null) newAudioSphereEvent.OnEventRaised -= OnNewAudioSphere;
        }

        private void OnNewAudioSphere(IEventSender sender, SoundEmissionData soundEmissionData) { if (logNewAudioSphere) Log.DLazy(() => $"{GetColoredName(sender)}: New Audio Sphere Event Raised at Position: {soundEmissionData.Position}, Radius: {soundEmissionData.Radius}, Intensity: {soundEmissionData.Intensity}", this); }
        #endregion

        #region Echolocation
        [Header("Echolocation")]
        [SerializeField] private SOEventVoid materialToggleEvent;
        [SerializeField] private bool logMaterialToggle = true;

        void RegisterEcholocation()
        {
            if (materialToggleEvent != null) materialToggleEvent.OnEventRaised += OnMaterialToggle;
        }

        void UnregisterEcholocation()
        {
            if (materialToggleEvent != null) materialToggleEvent.OnEventRaised -= OnMaterialToggle;
        }

        private void OnMaterialToggle(IEventSender sender) { if (logMaterialToggle) Log.DLazy(() => $"{GetColoredName(sender)}: Material Toggle Event Raised", this); }
        #endregion

        #region Misc
        [Header("Misc")]
        [SerializeField] private SOIntStringEvent feedbackSubmittedEvent;
        [SerializeField] private bool logFeedbackSubmitted = true;

        void RegisterMisc()
        {
            if (feedbackSubmittedEvent != null) feedbackSubmittedEvent.OnEventRaised += OnFeedbackSubmitted;
        }

        void UnregisterMisc()
        {
            if (feedbackSubmittedEvent != null) feedbackSubmittedEvent.OnEventRaised -= OnFeedbackSubmitted;
        }

        private void OnFeedbackSubmitted(IEventSender sender, int rating, string feedback) { if (logFeedbackSubmitted) Log.DLazy(() => $"{GetColoredName(sender)}: Feedback Submitted Event Raised with Rating: {rating}, Feedback: {feedback}", this); }
        #endregion

        #region To be classified
        [Header("To be classified")]
        //PlayerInput to every InteractableManager for Area Interactables 
        [SerializeField] private SOEventVoid interactEvent;
        [SerializeField] private bool logInteractEvent = true;
        //IDK from who to PlayerInput
        [SerializeField] private SOEventVoid wearEcholocatorEvent;
        [SerializeField] private bool logWearEcholocator = true;
        //Area Interactables to Player 
        [SerializeField] private SOAreaInteractableEvent enterInteractionAreaEvent;
        [SerializeField] private bool logEnterInteractionArea = true;
        [SerializeField] private SOAreaInteractableEvent exitInteractionAreaEvent;
        [SerializeField] private bool logExitInteractionArea = true;
        // Pickable to PlayerInventory
        [SerializeField] private SOPickableDataGameObjectEvent itemPickedEvent;
        [SerializeField] private bool logItemPicked = true;
        // PlayerInventory to Pickable
        [SerializeField] private SOBoolEvent canBePickedEvent;
        [SerializeField] private bool logCanBePicked = true;
        // PlayerController to PlayerInventory
        [SerializeField] private SOIntEvent dropItemEvent;
        [SerializeField] private bool logDropItem = true;
        //RadialMenu to PlayerController
        [SerializeField] private SOEquipItemEvent itemEquippedEvent;
        [SerializeField] private bool logItemEquipped = true;
        //From Player Controller to Enemy to update the position of a continuous sound
        [SerializeField] private SOSoundEmissionDataVector3 playerEmittedSoundEvent;
        [SerializeField] private bool logPlayerEmittedSound = true;

        void RegisterToBeClassified()
        {
            if (interactEvent != null) interactEvent.OnEventRaised += OnInteractEvent;
            if (wearEcholocatorEvent != null) wearEcholocatorEvent.OnEventRaised += OnWearEcholocator;
            if (enterInteractionAreaEvent != null) enterInteractionAreaEvent.OnEventRaised += OnEnterInteractionArea;
            if (exitInteractionAreaEvent != null) exitInteractionAreaEvent.OnEventRaised += OnExitInteractionArea;
            if (itemPickedEvent != null) itemPickedEvent.OnEventRaised += OnItemPicked;
            if (canBePickedEvent != null) canBePickedEvent.OnEventRaised += OnCanBePicked;
            if (dropItemEvent != null) dropItemEvent.OnEventRaised += OnDropItem;
            if (itemEquippedEvent != null) itemEquippedEvent.OnEventRaised += OnItemEquipped;
            if (playerEmittedSoundEvent != null) playerEmittedSoundEvent.OnEventRaised += OnPlayerEmittedSound;
        }

        void UnregisterToBeClassified()
        {
            if (interactEvent != null) interactEvent.OnEventRaised -= OnInteractEvent;
            if (wearEcholocatorEvent != null) wearEcholocatorEvent.OnEventRaised -= OnWearEcholocator;
            if (enterInteractionAreaEvent != null) enterInteractionAreaEvent.OnEventRaised -= OnEnterInteractionArea;
            if (exitInteractionAreaEvent != null) exitInteractionAreaEvent.OnEventRaised -= OnExitInteractionArea;
            if (itemPickedEvent != null) itemPickedEvent.OnEventRaised -= OnItemPicked;
            if (canBePickedEvent != null) canBePickedEvent.OnEventRaised -= OnCanBePicked;
            if (dropItemEvent != null) dropItemEvent.OnEventRaised -= OnDropItem;
            if (itemEquippedEvent != null) itemEquippedEvent.OnEventRaised -= OnItemEquipped;
            if (playerEmittedSoundEvent != null) playerEmittedSoundEvent.OnEventRaised -= OnPlayerEmittedSound;
        }

        private void OnInteractEvent(IEventSender sender) { if (logInteractEvent) Log.DLazy(() => $"{GetColoredName(sender)}: Interact Event Raised", this); }
        private void OnWearEcholocator(IEventSender sender) { if (logWearEcholocator) Log.DLazy(() => $"{GetColoredName(sender)}: Wear Echolocator Event Raised", this); }
        private void OnEnterInteractionArea(IEventSender sender, Interactable interactable) { if (logEnterInteractionArea) Log.DLazy(() => $"{GetColoredName(sender)}: Enter Interaction Area Event Raised for Interactable: {interactable.gameObject.name}", this); }
        private void OnExitInteractionArea(IEventSender sender, Interactable interactable) { if (logExitInteractionArea) Log.DLazy(() => $"{GetColoredName(sender)}: Exit Interaction Area Event Raised for Interactable: {interactable.gameObject.name}", this); }
        private void OnItemPicked(IEventSender sender, PickableData data, GameObject prefab) { if (logItemPicked) Log.DLazy(() => $"{GetColoredName(sender)}: Item Picked Event Raised for Pickable Data: {data.Name}", this); }
        private void OnCanBePicked(IEventSender sender, bool canBePicked) { if (logCanBePicked) Log.DLazy(() => $"{GetColoredName(sender)}: Can Be Picked Event Raised, CanBePicked: {canBePicked}", this); }
        private void OnDropItem(IEventSender sender, int itemIndex) { if (logDropItem) Log.DLazy(() => $"{GetColoredName(sender)}: Drop Item Event Raised for Item Index: {itemIndex}", this); }
        private void OnItemEquipped(IEventSender sender, int itemIndex, SOPickable data, GameObject prefab) { if (logItemEquipped) Log.DLazy(() => $"{GetColoredName(sender)}: Item Equipped Event Raised for Item Index: {itemIndex}, Data: {data.Name}", this); }
        private void OnPlayerEmittedSound(IEventSender sender, Vector3 position, SoundEmissionData soundData) { if (logPlayerEmittedSound) Log.DLazy(() => $"{GetColoredName(sender)}: Player Emitted Sound Event Raised at Position: {position}", this); }
        #endregion

        #region Test events here
        [Header("Test events here")]
        [SerializeField] private SOEventVoid voidEventExample;
        [SerializeField] private bool logVoidEventExample = true;

        void RegisterTestEvents()
        {
            if (voidEventExample != null) voidEventExample.OnEventRaised += OnVoidEventExample;
        }

        void UnregisterTestEvents()
        {
            if (voidEventExample != null) voidEventExample.OnEventRaised -= OnVoidEventExample;
        }

        private void OnVoidEventExample(IEventSender sender) { if (logVoidEventExample) Log.DLazy(() => $"{GetColoredName(sender)}: Void Event Example Raised", this); }
        #endregion

        void OnEnable()
        {
            RegisterGM();
            RegisterPlayer();
            RegisterSceneLoader();
            RegisterInteractables();
            RegisterUI();
            RegisterEcholocationAndEnemyAI();
            RegisterEcholocation();
            RegisterMisc();
            RegisterToBeClassified();
            RegisterTestEvents();
        }

        private string GetColoredName(IEventSender sender)
        {
            string color = SenderColorCache.GetColor(sender);
            return $"<color={color}>[{sender.SenderName}]</color>";
        }
        void OnDisable()
        {
            UnregisterGM();
            UnregisterPlayer();
            UnregisterSceneLoader();
            UnregisterInteractables();
            UnregisterUI();
            UnregisterEcholocationAndEnemyAI();
            UnregisterEcholocation();
            UnregisterMisc();
            UnregisterToBeClassified();
            UnregisterTestEvents();
        }

        [SerializeField] private bool activateDeactivateAll = true;
        void OnValidate()
        {
            if (activateDeactivateAll)
            {
                activateDeactivateAll = false;
                bool newValue = !logSwitchGameState;
                //GM
                logSwitchGameState = newValue;
                logSwitchToTitleState = newValue;
                logSwitchLevel = newValue;
                logSwitchToPlayingState = newValue;
                logSwitchToPauseState = newValue;
                logSwitchToWinState = newValue;
                logSwitchToDeathState = newValue;
                logSwitchToNarrationState = newValue;
                logSwitchToHudState = newValue;
                logEnterLoading = newValue;
                logExitLoading = newValue;
                //Player
                logEnablePlayerActionMap = newValue;
                logDisablePlayerActionMap = newValue;
                logUpdatePlayerAttractionTarget = newValue;
                //SceneLoader
                logLoadLevel = newValue;
                logReloadLevel = newValue;
                logSetPlayerOnSpawn = newValue;
                logUnloadCurrentLevel = newValue;
                //Interactables
                logInteractionOutcome = newValue;
                //UI
                logEnableUIActionMap = newValue;
                logDisableUIActionMap = newValue;
                logTitleMenu = newValue;
                logHudMenu = newValue;
                logPauseMenu = newValue;
                logDeathMenu = newValue;
                logWinMenu = newValue;
                logEnterLoadingScreen = newValue;
                logExitLoadingScreen = newValue;
                logDialogMenu = newValue;
                logWarningEvent = newValue;
                logCanInteractStart = newValue;
                logCanInteractStop = newValue;
                logInventoryChanged = newValue;
                //Echolocation & Enemy AI
                logNewAudioSphere = newValue;
                //Echolocation
                logMaterialToggle = newValue;
                //Misc
                logFeedbackSubmitted = newValue;
                //To be classified
                logInteractEvent = newValue;
                logWearEcholocator = newValue;
                logEnterInteractionArea = newValue;
                logExitInteractionArea = newValue;
                logItemPicked = newValue;
                logCanBePicked = newValue;
                logDropItem = newValue;
                logItemEquipped = newValue;
                logPlayerEmittedSound = newValue;
                //Test events here
                logVoidEventExample = newValue;
            }
        }

#endif
    }
}
