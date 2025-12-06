using UnityEngine;

namespace EchoCity
{
    public class HudGameState : GameState
    {
        private HudEnum _currentHud;
        public HudGameState(GameManager gameManager, GameStatesFSM fsm) : base(gameManager, fsm) { }
        public override GameStatesEnum GetEnum() => GameStatesEnum.Hud;
        public override void Enter() => _gameManager.EnableUIInputEvent.RaiseEvent();
        public void EnterHud(HudEnum hud)
        {
            Enter();
            switch (hud)
            {
                case HudEnum.Inventory:
                    _gameManager.HudMenuEvent.RaiseEvent(HudEnum.Inventory);
                    break;
                //TODO other HUDs
                default:
                    Debug.LogWarning("HudGameState: EnterHud - HUDEnum not handled");
                    break;
            }
        }
        public override void Update() { }
        public override void Exit()
        {
            switch (_currentHud)
            {
                case HudEnum.Inventory:
                    _gameManager.HudMenuEvent.RaiseEvent(HudEnum.None);
                    break;
                //TODO other HUDs
                default:
                    break;
            }
        }
        public override void SwitchToPlayingHandler() => _fsm.SwitchState(_fsm.PlayingState);
        public override void SwitchToPauseHandler() => _fsm.SwitchState(_fsm.PauseState);
        public override void SwitchToDeathHandler() => _fsm.SwitchState(_fsm.DeathState);
        public override void SwitchToNarrationHandler(DialogData data) => _fsm.SwitchToNarration(data);
    }
}