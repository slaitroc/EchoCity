using System;
using TMPro;
using UnityEditor.Search;
using UnityEngine;

namespace EchoCity
{
    public class GameManager : MonoBehaviour, IFSMOwner, IGMContext
    {
        #region fields and properties
        [Header("Invoking Events")]
        // INPUT
        [SerializeField] private SOPlayerInputEvent playerInputEvent;

        // UI MANAGER EVENTS
        [SerializeField] private SOShowUIEvent showUIEvent;

        // TRANSITIONS
        [SerializeField] private SOGameManagerStateTransitionEvent gameStateTransitionEvent;

        // SCENE MANAGEMENT
        [SerializeField] private SOSetPlayerOnSpawnEvent setPlayerOnSpawnEvent;
        [SerializeField] private SOLevelActionEvent loadSceneEvent;

        string IEventSender.SenderName => gameObject.name;
        int IEventSender.SenderID => GetInstanceID();
        bool IEventSender.IsManager => true;
        EventSenderCategoriesEnum[] IEventSender.SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.GameManager };

        [Header("Observed Events")]
        [SerializeField] private SOSwitchLevelEvent switchLevelEvent;
        [SerializeField] private SOSwitchToGameStateEvent switchToGameStateEvent;

        [Header("FSM")]
        [SerializeField] private GameStatesEnum currentState;
        private GameManagerFSM _fsm;

        // IFSMOwner
        public Transform Transform => this.transform;
        public GameObject GameObject => this.gameObject;

        // GM Context
        IFSMOwner IGMContext.Owner => this;
        GameStatesEnum IGMContext.CurrentStateEnum { get => currentState; set => currentState = value; }
        SOPlayerInputEvent IGMContext.PlayerInputEvent => playerInputEvent;
        SOShowUIEvent IGMContext.ShowUIEvent => showUIEvent;
        SOSetPlayerOnSpawnEvent IGMContext.SetPlayerOnSpawnEvent => setPlayerOnSpawnEvent;
        SOLevelActionEvent IGMContext.LevelActionEvent => loadSceneEvent;
        SOGameManagerStateTransitionEvent IGMContext.GameStateTransitionEvent => gameStateTransitionEvent;
        #endregion

        void Start()
        {
            _fsm = new GameManagerFSM(this);
            _fsm.Initialize();
        }

        void OnEnable()
        {
            if (switchToGameStateEvent != null) switchToGameStateEvent.OnEventRaised += SwitchToGameStateHandler;
            if (switchLevelEvent != null) switchLevelEvent.OnEventRaised += InitLevelHandler;
        }


        void OnDisable()
        {
            if (switchToGameStateEvent != null) switchToGameStateEvent.OnEventRaised -= SwitchToGameStateHandler;
            if (switchLevelEvent != null) switchLevelEvent.OnEventRaised -= InitLevelHandler;
        }

        void Update() => _fsm.Update();

        private void SwitchToGameStateHandler(IEventSender sender, GameStatesEnum @enum, EventParams @params)
        {
            switch (@enum)
            {
                case GameStatesEnum.None:
                    Log.ELazy(() => "SwitchToGameStateHandler - Cannot switch to None state.", this);
                    break;
                case GameStatesEnum.Title:
                    _fsm.CurrentState.SwitchToTitleHandler(_fsm.PreviousState.GetEnum());
                    break;
                case GameStatesEnum.Playing:
                    _fsm.CurrentState.SwitchToPlayingHandler(_fsm.PreviousState.GetEnum());
                    break;
                case GameStatesEnum.Pause:
                    _fsm.CurrentState.SwitchToPauseHandler(_fsm.PreviousState.GetEnum());
                    break;
                case GameStatesEnum.Death:
                    _fsm.CurrentState.SwitchToDeathHandler(_fsm.PreviousState.GetEnum());
                    break;
                case GameStatesEnum.Win:
                    _fsm.CurrentState.SwitchToWinHandler(_fsm.PreviousState.GetEnum());
                    break;
                case GameStatesEnum.Narration:
                    if (@params is ToNarrationParams narrationParams)
                        _fsm.CurrentState.SwitchToNarrationHandler(_fsm.PreviousState.GetEnum(), narrationParams);
                    else
                        Log.ELazy(() => "GameManager: SwitchToGameStateHandler - Missing DialogData for Narration state.", this);
                    break;
                case GameStatesEnum.Hud:
                    if (@params is ToHUDStateParams hudParams)
                        _fsm.CurrentState.SwitchToHudHandler(_fsm.PreviousState.GetEnum(), hudParams.HudState);
                    else
                        Log.ELazy(() => "GameManager: SwitchToGameStateHandler - Missing HudParams for Hud state.", this);
                    break;
                case GameStatesEnum.Loading:
                    if (@params is ToLoadingStateParams loadingParams)
                        if (loadingParams.IsLoading)
                            _fsm.EnterLoading();
                        else
                            _fsm.ExitLoading();
                    break;
                default:
                    Log.ELazy(() => "GameManager: SwitchToGameStateHandler - Unhandled GameStatesEnum " + @enum, this);
                    break;
            }
        }

        public void InitLevelHandler(IEventSender sender, SceneEnum scene, EventParams @params) => _fsm.CurrentState.InitLevelHandler(scene);
    }
}
