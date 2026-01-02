using System;
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
        [SerializeField] private SOShowUIEvent showUIEvent;

        // SCENE MANAGEMENT
        [SerializeField] private SOEventVoid setPlayerOnSpawnEvent;
        [SerializeField] private SOSceneEnumEvent loadLevelEvent;
        [SerializeField] private SOEventVoid unloadCurrentLevelEvent;
        [SerializeField] private SOEventVoid reloadLevelEvent;

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
        SOEventVoid IGMContext.EnablePlayerInputEvent => enablePlayerInputEvent;
        SOEventVoid IGMContext.DisablePlayerInputEvent => disablePlayerInputEvent;
        SOEventVoid IGMContext.EnableUIInputEvent => enableUIInputEvent;
        SOEventVoid IGMContext.DisableUIInputEvent => disableUIInputEvent;
        SOShowUIEvent IGMContext.ShowUIEvent => showUIEvent;
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
            if (switchToGameStateEvent != null) switchToGameStateEvent.OnEventRaised += SwitchToGameStateHandler;
            if (switchLevelEvent != null) switchLevelEvent.OnEventRaised += InitLevelHandler;
        }


        void OnDisable()
        {
            if (switchToGameStateEvent != null) switchToGameStateEvent.OnEventRaised -= SwitchToGameStateHandler;
            if (switchLevelEvent != null) switchLevelEvent.OnEventRaised -= InitLevelHandler;
        }

        private void SwitchToGameStateHandler(IEventSender sender, GameStatesEnum @enum, EventParams @params)
        {
            switch (@enum)
            {
                case GameStatesEnum.None:
                    Debug.LogError("GameManager: SwitchToGameStateHandler - Cannot switch to None state.");
                    break;
                case GameStatesEnum.Title:
                    _fsm.CurrentState.SwitchToTitleHandler();
                    break;
                case GameStatesEnum.Playing:
                    _fsm.CurrentState.SwitchToPlayingHandler();
                    break;
                case GameStatesEnum.Pause:
                    _fsm.CurrentState.SwitchToPauseHandler();
                    break;
                case GameStatesEnum.Death:
                    _fsm.CurrentState.SwitchToDeathHandler();
                    break;
                case GameStatesEnum.Win:
                    _fsm.CurrentState.SwitchToWinHandler();
                    break;
                case GameStatesEnum.Narration:
                    if (@params is ToDialogueStateParams dialogParams)
                        _fsm.CurrentState.SwitchToNarrationHandler(dialogParams.DialogData);
                    else
                        Debug.LogError("GameManager: SwitchToGameStateHandler - Missing DialogParams for Narration state.");
                    break;
                case GameStatesEnum.Hud:
                    if (@params is ToHUDStateParams hudParams)
                        _fsm.CurrentState.SwitchToHudHandler(hudParams.HudState);
                    else
                        Debug.LogError("GameManager: SwitchToGameStateHandler - Missing HudParams for Hud state.");
                    break;
                case GameStatesEnum.Loading:
                    if (@params is ToLoadingStateParams loadingParams)
                        if (loadingParams.IsLoading)
                            _fsm.EnterLoading();
                        else
                            _fsm.ExitLoading();
                    break;
                default:
                    Debug.LogError("GameManager: SwitchToGameStateHandler - Unhandled GameStatesEnum " + @enum);
                    break;
            }
        }

        void Update() => _fsm.Update();
        public void InitLevelHandler(IEventSender sender, SceneEnum scene, EventParams @params) => _fsm.CurrentState.InitLevelHandler(scene);
    }
}
