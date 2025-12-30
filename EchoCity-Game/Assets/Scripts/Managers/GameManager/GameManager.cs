using UnityEditor.Search;
using UnityEngine;

namespace EchoCity
{
    public class GameManager : MonoBehaviour, IFSMOwner, IGMContext
    {
        #region fields and properties
        [Header("Invoking Events")]
        [SerializeField] private SOGameManagerStateTransitionEvent switchGameStateEvent;
        string IEventSender.SenderName => gameObject.name;
        int IEventSender.SenderID => GetInstanceID();
        bool IEventSender.IsManager => true;
        EventSenderCategoriesEnum[] IEventSender.SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.GameManager };

        // INPUT
        [SerializeField] private SOEventVoid enablePlayerInputEvent;
        [SerializeField] private SOEventVoid disablePlayerInputEvent;
        [SerializeField] private SOEventVoid enableUIInputEvent;
        [SerializeField] private SOEventVoid disableUIInputEvent;

        // UI MANAGER EVENTS
        [SerializeField] private SOEventVoid titleMenuEvent;
        [SerializeField] private SOEventVoid pauseMenuEvent;
        [SerializeField] private SOHudEnumEvent hudMenuEvent;
        [SerializeField] private SODialogDataEvent dialogDataEvent;
        [SerializeField] private SOEventVoid deathMenuEvent;
        [SerializeField] private SOEventVoid winMenuEvent;
        [SerializeField] private SOEventVoid enterLoadingScreenEvent;
        [SerializeField] private SOEventVoid exitLoadingScreenEvent;

        // SCENE MANAGEMENT
        [SerializeField] private SOEventVoid setPlayerOnSpawnEvent;
        [SerializeField] private SOSceneEnumEvent loadLevelEvent;
        [SerializeField] private SOEventVoid unloadCurrentLevelEvent;
        [SerializeField] private SOEventVoid reloadLevelEvent;

        [Header("Observed Events")]
        [SerializeField] private SOEventVoid switchToTitleStateEvent;
        [SerializeField] private SOSceneEnumEvent switchLevelEvent;
        [SerializeField] private SOEventVoid switchToPlayingStateEvent;
        [SerializeField] private SOEventVoid switchToPauseStateEvent;
        [SerializeField] private SOEventVoid switchToWinStateEvent;
        [SerializeField] private SOEventVoid switchToDeathStateEvent;
        [SerializeField] private SODialogDataEvent switchToNarrationStateEvent;
        [SerializeField] private SOHudEnumEvent switchToHudStateEvent;
        [SerializeField] private SOEventVoid enterLoadingEvent;
        [SerializeField] private SOEventVoid exitLoadingEvent;

        [Header("FSM")]
        [SerializeField] private GameStatesEnum currentState;
        private GameManagerFSM _fsm;

        // IFSMOwner
        public Transform Transform => this.transform;
        public GameObject GameObject => this.gameObject;

        // GM Context
        IFSMOwner IGMContext.Owner => this;
        GameStatesEnum IGMContext.CurrentStateEnum { get => currentState; set => currentState = value; }
        SOEventVoid IGMContext.EnablePlayerInputEvent => enablePlayerInputEvent;
        SOEventVoid IGMContext.DisablePlayerInputEvent => disablePlayerInputEvent;
        SOEventVoid IGMContext.EnableUIInputEvent => enableUIInputEvent;
        SOEventVoid IGMContext.DisableUIInputEvent => disableUIInputEvent;
        SOEventVoid IGMContext.TitleMenuEvent => titleMenuEvent;
        SOEventVoid IGMContext.PauseMenuEvent => pauseMenuEvent;
        SOHudEnumEvent IGMContext.HudMenuEvent => hudMenuEvent;
        SODialogDataEvent IGMContext.DialogDataEvent => dialogDataEvent;
        SOEventVoid IGMContext.DeathMenuEvent => deathMenuEvent;
        SOEventVoid IGMContext.WinMenuEvent => winMenuEvent;
        SOEventVoid IGMContext.EnterLoadingScreenEvent => enterLoadingScreenEvent;
        SOEventVoid IGMContext.ExitLoadingScreenEvent => exitLoadingScreenEvent;
        SOEventVoid IGMContext.SetPlayerOnSpawnEvent => setPlayerOnSpawnEvent;
        SOSceneEnumEvent IGMContext.LoadLevelEvent => loadLevelEvent;
        SOEventVoid IGMContext.UnloadCurrentLevelEvent => unloadCurrentLevelEvent;
        SOEventVoid IGMContext.ReloadLevelEvent => reloadLevelEvent;
        SOGameManagerStateTransitionEvent IGMContext.SwitchGameStateEvent => switchGameStateEvent;

        #endregion

        void Start()
        {
            _fsm = new GameManagerFSM(this);
            _fsm.Initialize();
        }

        void OnEnable()
        {
            if (switchToTitleStateEvent) switchToTitleStateEvent.OnEventRaised += SwitchToTitleStateHandler;
            if (switchLevelEvent) switchLevelEvent.OnEventRaised += InitLevelHandler;
            if (switchToPlayingStateEvent) switchToPlayingStateEvent.OnEventRaised += SwitchToPlayingStateHandler;
            if (switchToPauseStateEvent) switchToPauseStateEvent.OnEventRaised += SwitchToPauseStateHandler;
            if (switchToDeathStateEvent) switchToDeathStateEvent.OnEventRaised += SwitchToDeathStateHandler;
            if (switchToWinStateEvent) switchToWinStateEvent.OnEventRaised += SwitchToWinStateHandler;
            if (switchToNarrationStateEvent) switchToNarrationStateEvent.OnEventRaised += SwitchToNarrationStateHandler;
            if (switchToHudStateEvent) switchToHudStateEvent.OnEventRaised += SwitchToHudStateHandler;
            if (enterLoadingEvent) enterLoadingEvent.OnEventRaised += LoadingHandler;
            if (exitLoadingEvent) exitLoadingEvent.OnEventRaised += DoneLoadingHandler;
        }

        void OnDisable()
        {
            if (switchToTitleStateEvent) switchToTitleStateEvent.OnEventRaised -= SwitchToTitleStateHandler;
            if (switchLevelEvent) switchLevelEvent.OnEventRaised -= InitLevelHandler;
            if (switchToPlayingStateEvent) switchToPlayingStateEvent.OnEventRaised -= SwitchToPlayingStateHandler;
            if (switchToPauseStateEvent) switchToPauseStateEvent.OnEventRaised -= SwitchToPauseStateHandler;
            if (switchToDeathStateEvent) switchToDeathStateEvent.OnEventRaised -= SwitchToDeathStateHandler;
            if (switchToWinStateEvent) switchToWinStateEvent.OnEventRaised -= SwitchToWinStateHandler;
            if (switchToNarrationStateEvent) switchToNarrationStateEvent.OnEventRaised -= SwitchToNarrationStateHandler;
            if (switchToHudStateEvent) switchToHudStateEvent.OnEventRaised -= SwitchToHudStateHandler;
            if (enterLoadingEvent) enterLoadingEvent.OnEventRaised -= LoadingHandler;
            if (exitLoadingEvent) exitLoadingEvent.OnEventRaised -= DoneLoadingHandler;
        }

        void Update() => _fsm.Update();
        public void RaiseSwitchStateEvent(IEventSender sender, GameStatesEnum from, GameStatesEnum to) => switchGameStateEvent.RaiseEvent(this, from, to);
        public void SwitchToTitleStateHandler(IEventSender sender) => _fsm.CurrentState.SwitchToTitleHandler();
        public void InitLevelHandler(IEventSender sender, SceneEnum scene) => _fsm.CurrentState.InitLevelHandler(scene);
        public void SwitchToPlayingStateHandler(IEventSender sender) => _fsm.CurrentState.SwitchToPlayingHandler();
        public void SwitchToPauseStateHandler(IEventSender sender) => _fsm.CurrentState.SwitchToPauseHandler();
        public void SwitchToDeathStateHandler(IEventSender sender) => _fsm.CurrentState.SwitchToDeathHandler();
        public void SwitchToWinStateHandler(IEventSender sender) => _fsm.CurrentState.SwitchToWinHandler();
        public void SwitchToNarrationStateHandler(IEventSender sender, DialogData data) => _fsm.CurrentState.SwitchToNarrationHandler(data);
        public void SwitchToHudStateHandler(IEventSender sender, HudEnum hud) => _fsm.CurrentState.SwitchToHudHandler(hud);

        public void LoadingHandler(IEventSender sender) => _fsm.EnterLoading();
        public void DoneLoadingHandler(IEventSender sender) => _fsm.ExitLoading();
    }
}
