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
        [Header("GM to any")]
        [SerializeField] private SOEventDoubleParam<GameStatesEnum, GameStatesEnum> switchGameStateEvent;

        [Header("GM to Player")]
        [SerializeField] private SOEventVoid enablePlayerActionMapEvent;
        [SerializeField] private SOEventVoid disablePlayerActionMapEvent;

        [Header("GM to UI")]
        [SerializeField] private SOEventVoid enableUIActionMapEvent;
        [SerializeField] private SOEventVoid disableUIActionMapEvent;
        [SerializeField] private SOEventVoid titleMenuEvent;
        [SerializeField] private SOHudEnumEvent hudMenuEvent;
        [SerializeField] private SOEventVoid pauseMenuEvent;
        [SerializeField] private SODialogDataEvent dialogMenuEvent;
        [SerializeField] private SOEventVoid deathMenuEvent;
        [SerializeField] private SOEventVoid enterLoadingScreenEvent;
        [SerializeField] private SOEventVoid exitLoadingScreenEvent;


        [Header("Player & UI to GM")]
        [SerializeField] private SOEventVoid switchToTitleStateEvent;
        [SerializeField] private SOSceneEnumEvent switchLevelEvent;
        [SerializeField] private SOEventVoid switchToPlayingStateEvent;
        [SerializeField] private SOEventVoid switchToPauseStateEvent;
        [SerializeField] private SOEventVoid switchToWinStateEvent;
        [SerializeField] private SOEventVoid switchToDeathStateEvent;
        [SerializeField] private SODialogDataEvent switchToNarrationStateEvent;
        [SerializeField] private SOHudEnumEvent switchToHudStateEvent;

        [Header("SL to GM")]
        [SerializeField] private SOEventVoid enterLoadingEvent;
        [SerializeField] private SOEventVoid exitLoadingEvent;

        [SerializeField] private SOEventVoid interactEvent;
        [SerializeField] private SOStringColorEvent spawnWarningEvent;

        [Header("Enemies to Player")]
        [SerializeField] private SOIAttractionEvent updatePlayerAttractionTargetEvent;

        [Header("Any to EM & Enemies")]
        [SerializeField] private SOSoundEmissionDataEvent newAudioSphereEvent;

        [Header("Interactables")]
        [SerializeField] private SOAreaInteractableEvent enterInteractionAreaEvent;
        [SerializeField] private SOAreaInteractableEvent exitInteractionAreaEvent;
        [SerializeField] private SOPickableDataGameObjectEvent pickedPickableEvent;

        [Header("Misc")]
        [SerializeField] private SOIntStringEvent feedbackSubmittedEvent;

        [Header("Test events here")]
        [SerializeField] private SOIntEvent intEventExample;
        [SerializeField] private SOStringEvent stringEventExample;
        [SerializeField] private SOEventVoid voidEventExample;

        void OnEnable()
        {
            if (switchGameStateEvent != null) switchGameStateEvent.OnEventRaised += OnSwitchGameState;

            if (enablePlayerActionMapEvent != null) enablePlayerActionMapEvent.OnEventRaised += OnEnablePlayerActionMap;
            if (disablePlayerActionMapEvent != null) disablePlayerActionMapEvent.OnEventRaised += OnDisablePlayerActionMap;

            if (enableUIActionMapEvent != null) enableUIActionMapEvent.OnEventRaised += OnEnableUIActionMap;
            if (disableUIActionMapEvent != null) disableUIActionMapEvent.OnEventRaised += OnDisableUIActionMap;

            if (titleMenuEvent != null) titleMenuEvent.OnEventRaised += OnTitleMenu;
            if (hudMenuEvent != null) hudMenuEvent.OnEventRaised += OnHudMenu;
            if (pauseMenuEvent != null) pauseMenuEvent.OnEventRaised += OnPauseMenu;
            if (dialogMenuEvent != null) dialogMenuEvent.OnEventRaised += OnDialogMenu;
            if (deathMenuEvent != null) deathMenuEvent.OnEventRaised += OnDeathMenu;
            if (enterLoadingScreenEvent != null) enterLoadingScreenEvent.OnEventRaised += OnEnterLoadingScreen;
            if (exitLoadingScreenEvent != null) exitLoadingScreenEvent.OnEventRaised += OnExitLoadingScreen;

            // if (updatePlayerAttractionTargetEvent != null) updatePlayerAttractionTargetEvent.OnEventRaised += OnUpdatePlayerAttractionTarget;

            if (newAudioSphereEvent != null) newAudioSphereEvent.OnEventRaised += OnNewAudioSphere;

            if (switchToHudStateEvent != null) switchToHudStateEvent.OnEventRaised += OnSwitchToHudState;
            if (interactEvent != null) interactEvent.OnEventRaised += OnInteractEvent;
            if (intEventExample != null) intEventExample.OnEventRaised += OnIntEventExample;
            if (stringEventExample != null) stringEventExample.OnEventRaised += OnStringEventExample;
            if (voidEventExample != null) voidEventExample.OnEventRaised += OnVoidEventExample;
            if (spawnWarningEvent != null) spawnWarningEvent.OnEventRaised += OnSpawnWarningEvent;

            if (enterInteractionAreaEvent != null) enterInteractionAreaEvent.OnEventRaised += OnEnterInteractionRangeEvent;
            if (exitInteractionAreaEvent != null) exitInteractionAreaEvent.OnEventRaised += OnExitInteractionRangeEvent;
            if (pickedPickableEvent != null) pickedPickableEvent.OnEventRaised += OnPickedPickableEvent;
            if (switchToPauseStateEvent != null) switchToPauseStateEvent.OnEventRaised += OnPauseEvent;

            if (feedbackSubmittedEvent != null) feedbackSubmittedEvent.OnEventRaised += OnFeedbackSubmittedEvent;
        }


        void OnDisable()
        {
            if (switchGameStateEvent != null) switchGameStateEvent.OnEventRaised -= OnSwitchGameState;
            if (switchToPauseStateEvent != null) switchToPauseStateEvent.OnEventRaised -= OnPauseEvent;
            if (newAudioSphereEvent != null) newAudioSphereEvent.OnEventRaised -= OnNewAudioSphere;
            if (interactEvent != null) interactEvent.OnEventRaised -= OnInteractEvent;
            if (intEventExample != null) intEventExample.OnEventRaised -= OnIntEventExample;
            if (stringEventExample != null) stringEventExample.OnEventRaised -= OnStringEventExample;
            if (voidEventExample != null) voidEventExample.OnEventRaised -= OnVoidEventExample;
            if (enablePlayerActionMapEvent != null) enablePlayerActionMapEvent.OnEventRaised -= OnEnablePlayerActionMap;
            if (disablePlayerActionMapEvent != null) disablePlayerActionMapEvent.OnEventRaised -= OnDisablePlayerActionMap;
            if (enableUIActionMapEvent != null) enableUIActionMapEvent.OnEventRaised -= OnEnableUIActionMap;
            if (disableUIActionMapEvent != null) disableUIActionMapEvent.OnEventRaised -= OnDisableUIActionMap;
            if (switchToHudStateEvent != null) switchToHudStateEvent.OnEventRaised -= OnSwitchToHudState;
            if (spawnWarningEvent != null) spawnWarningEvent.OnEventRaised -= OnSpawnWarningEvent;

            if (enterInteractionAreaEvent != null) enterInteractionAreaEvent.OnEventRaised -= OnEnterInteractionRangeEvent;
            if (exitInteractionAreaEvent != null) exitInteractionAreaEvent.OnEventRaised -= OnExitInteractionRangeEvent;
            if (pickedPickableEvent != null) pickedPickableEvent.OnEventRaised -= OnPickedPickableEvent;
            if (titleMenuEvent != null) titleMenuEvent.OnEventRaised -= OnTitleMenu;
            if (hudMenuEvent != null) hudMenuEvent.OnEventRaised -= OnHudMenu;
            if (pauseMenuEvent != null) pauseMenuEvent.OnEventRaised -= OnPauseMenu;
            if (dialogMenuEvent != null) dialogMenuEvent.OnEventRaised -= OnDialogMenu;
            if (deathMenuEvent != null) deathMenuEvent.OnEventRaised -= OnDeathMenu;
            if (enterLoadingScreenEvent != null) enterLoadingScreenEvent.OnEventRaised -= OnEnterLoadingScreen;
            if (exitLoadingScreenEvent != null) exitLoadingScreenEvent.OnEventRaised -= OnExitLoadingScreen;

            if (feedbackSubmittedEvent != null) feedbackSubmittedEvent.OnEventRaised -= OnFeedbackSubmittedEvent;
        }

        private string GetColoredName(IEventSender sender)
        {
            string color = SenderColorCache.GetColor(sender);
            return $"<color={color}>[{sender.SenderName}]</color>";
        }

        #region Game Manager Events
        private void OnSwitchGameState(IEventSender sender, GameStatesEnum from, GameStatesEnum to) => Log.DLazy(() => $"{GetColoredName(sender)}: Switch Game State Event Raised from {from} to {to}", this);
        //ui
        private void OnPauseEvent(IEventSender sender) => Log.DLazy(() => $"{GetColoredName(sender)}: Pause Event Raised", this);
        #endregion

        #region Enemies
        private void OnPlayerHit(IEventSender sender, EnemyAI enemyAI) => Log.DLazy(() => $"{GetColoredName(sender)}: Player Hit Event Raised by Enemy: {enemyAI.gameObject.name}", this);
        #endregion

        #region Echolocation 
        private void OnNewAudioSphere(IEventSender sender, SoundEmissionData soundEmissionData) => Log.DLazy(() => $"{GetColoredName(sender)}: New Audio Sphere Event Raised at Position: {soundEmissionData.Position}, Radius: {soundEmissionData.Radius}, Intensity: {soundEmissionData.Intensity}", this);
        #endregion

        #region Input 
        private void OnInteractEvent(IEventSender sender) => Log.DLazy(() => $"{GetColoredName(sender)}: Interact Event Raised", this);
        private void OnIntEventExample(IEventSender sender, int value) => Log.DLazy(() => $"{GetColoredName(sender)}: Int Event Raised with Value: {value}", this);
        private void OnStringEventExample(IEventSender sender, string value) => Log.DLazy(() => $"{GetColoredName(sender)}: String Event Raised with Value: {value}", this);
        private void OnVoidEventExample(IEventSender sender) => Log.DLazy(() => $"{GetColoredName(sender)}: Void Event Raised", this);
        private void OnEnablePlayerActionMap(IEventSender sender) => Log.DLazy(() => $"{GetColoredName(sender)}: Enable Player Action Map Event Raised", this);
        private void OnDisablePlayerActionMap(IEventSender sender) => Log.DLazy(() => $"{GetColoredName(sender)}: Disable Player Action Map Event Raised", this);
        private void OnEnableUIActionMap(IEventSender sender) => Log.DLazy(() => $"{GetColoredName(sender)}: Enable UI Action Map Event Raised", this);
        private void OnDisableUIActionMap(IEventSender sender) => Log.DLazy(() => $"{GetColoredName(sender)}: Disable UI Action Map Event Raised", this);
        private void OnSwitchToHudState(IEventSender sender, HudEnum hud) => Log.DLazy(() => $"{GetColoredName(sender)}: Switch to HUD Menu Event Raised for HUD: {hud}", this);
        private void OnSpawnWarningEvent(IEventSender sender, string message, Color color) => Log.DLazy(() => $"{GetColoredName(sender)}: Spawn Warning Event Raised with Message: {message}, Color: {color}", this);
        #endregion

        #region Interactable
        private void OnEnterInteractionRangeEvent(IEventSender sender, Interactable interactable) => Log.DLazy(() => $"{GetColoredName(sender)}: Enter Interaction Range Event Raised for Interactable: {interactable.gameObject.name}", this);
        private void OnExitInteractionRangeEvent(IEventSender sender, Interactable interactable) => Log.DLazy(() => $"{GetColoredName(sender)}: Exit Interaction Range Event Raised for Interactable: {interactable.gameObject.name}", this);
        private void OnPickedPickableEvent(IEventSender sender, PickableData data, GameObject prefab) => Log.DLazy(() => $"{GetColoredName(sender)}: Picked Pickable Event Raised for Pickable Data: {data.Name}", this);
        #endregion

        #region UI
        private void OnTitleMenu(IEventSender sender) => Log.DLazy(() => $"{GetColoredName(sender)}: Title Menu Event Raised", this);
        private void OnHudMenu(IEventSender sender, HudEnum hud) => Log.DLazy(() => $"{GetColoredName(sender)}: HUD Menu Event Raised for HUD: {hud}", this);
        private void OnPauseMenu(IEventSender sender) => Log.DLazy(() => $"{GetColoredName(sender)}: Pause Menu Event Raised", this);
        private void OnDialogMenu(IEventSender sender, DialogData dialogData) => Log.DLazy(() => $"{GetColoredName(sender)}: Dialog Menu Event Raised", this);
        private void OnDeathMenu(IEventSender sender) => Log.DLazy(() => $"{GetColoredName(sender)}: Death Menu Event Raised", this);
        private void OnEnterLoadingScreen(IEventSender sender) => Log.DLazy(() => $"{GetColoredName(sender)}: Enter Loading Screen Event Raised", this);
        private void OnExitLoadingScreen(IEventSender sender) => Log.DLazy(() => $"{GetColoredName(sender)}: Exit Loading Screen Event Raised", this);

        #endregion

        #region Misc
        private void OnFeedbackSubmittedEvent(IEventSender sender, int rating, string feedback) => Log.DLazy(() => $"{GetColoredName(sender)}: Feedback Submitted Event Raised with Rating: {rating}, Feedback: {feedback}", this);
        #endregion
#endif
    }
}