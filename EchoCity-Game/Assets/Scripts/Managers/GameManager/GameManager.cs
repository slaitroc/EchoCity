using UnityEngine;

namespace EchoCity
{
    public class GameManager : MonoBehaviour
    {
#pragma warning disable CS0414
        private string _LOG_TAG = "GAME MANAGER";
        private string _LOG_COLOR = "#00ff00ff";
#pragma warning restore CS0414

        [Header("Invoking Events")]
        [SerializeField] private SOEventDoubleParam<GameStatesEnum, GameStatesEnum> switchGameStateEvent;
        public SOEventVoid EnablePlayerInputEvent;
        public SOEventVoid DisablePlayerInputEvent;
        public SOEventVoid EnableUIInputEvent;
        public SOEventVoid DisableUIInputEvent;

        public SOEventVoid TitleMenuEvent;
        public SOEventVoid PauseMenuEvent;
        public SOHudEnumEvent HudMenuEvent;
        public SODialogDataEvent DialogDataEvent;
        public SOEventVoid DeathMenuEvent;
        public SOEventVoid WinMenuEvent;
        public SOEventVoid EnterLoadingScreenEvent;
        public SOEventVoid ExitLoadingScreenEvent;
        public SOEventVoid SetPlayerOnSpawnEvent;


        public SOSceneEnumEvent LoadLevelEvent;
        public SOEventVoid UnloadCurrentLevelEvent;
        public SOEventVoid ReloadLevelEvent;

        //[SerializeField] private SOEventVoid reloadLevelEvent;

        [Header("Observed Events")]
        [SerializeField] private SOEventVoid switchToTitleStateEvent;
        [SerializeField] private SOSceneEnumEvent switchToInitLevelStateEvent;
        [SerializeField] private SOEventVoid switchToPlayingStateEvent;
        [SerializeField] private SOEventVoid switchToPauseStateEvent;
        [SerializeField] private SOEventVoid switchToWinStateEvent;
        [SerializeField] private SOEventVoid switchToDeathStateEvent;
        [SerializeField] private SODialogDataEvent switchToNarrationStateEvent;
        [SerializeField] private SOHudEnumEvent switchToHudStateEvent;
        [SerializeField] private SOEventVoid enterLoadingEvent;
        [SerializeField] private SOEventVoid exitLoadingEvent;



        [Header("FSM")]
        public GameStatesEnum CurrentState;
        private GameStatesFSM _fsm;


        void Start()
        {
            _fsm = new GameStatesFSM(this);
            _fsm.Initialize();
        }

        void OnEnable()
        {
            if (switchToTitleStateEvent) switchToTitleStateEvent.OnEventRaised += SwitchToTitleStateHandler;
            if (switchToInitLevelStateEvent) switchToInitLevelStateEvent.OnEventRaised += SwitchToInitLevelStateHandler;
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
            if (switchToInitLevelStateEvent) switchToInitLevelStateEvent.OnEventRaised -= SwitchToInitLevelStateHandler;
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
        public void RaiseSwitchStateEvent(GameStatesEnum from, GameStatesEnum to) => switchGameStateEvent.RaiseEvent(from, to);

        public void SwitchToTitleStateHandler() => _fsm.CurrentState.SwitchToTitleHandler();
        public void SwitchToInitLevelStateHandler(SceneEnum scene) => _fsm.CurrentState.SwitchToInitLevelHandler(scene);
        public void SwitchToPlayingStateHandler() => _fsm.CurrentState.SwitchToPlayingHandler();
        public void SwitchToPauseStateHandler() => _fsm.CurrentState.SwitchToPauseHandler();
        public void SwitchToDeathStateHandler() => _fsm.CurrentState.SwitchToDeathHandler();
        public void SwitchToWinStateHandler() => _fsm.CurrentState.SwitchToWinHandler();
        public void SwitchToNarrationStateHandler(DialogData data) => _fsm.CurrentState.SwitchToNarrationHandler(data);
        public void SwitchToHudStateHandler(HudEnum hud) => _fsm.CurrentState.SwitchToHudHandler(hud);

        public void LoadingHandler() => _fsm.EnterLoading();
        public void DoneLoadingHandler() => _fsm.ExitLoading();
    }
}
