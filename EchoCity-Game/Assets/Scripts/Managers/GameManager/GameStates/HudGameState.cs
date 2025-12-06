using UnityEngine;

namespace EchoCity
{
    public class HudGameState : GameState
    {
        public HudGameState(GameManager gameManager, GameStatesFSM fsm) : base(gameManager, fsm) { }
        public override GameStatesEnum GetEnum() => GameStatesEnum.Hud;
        public override void Enter() => _gameManager.EnableUIInputEvent.RaiseEvent();
        public void EnterHud(HUDEnum hud)
        {
            Enter();
            switch (hud)
            {
                case HUDEnum.Inventory:
                    _gameManager.HudMenuEvent.RaiseEvent(HUDEnum.Inventory);
                    break;
                //TODO other HUDs
                default:
                    Debug.LogWarning("HudGameState: EnterHud - HUDEnum not handled");
                    break;
            }
        }
        public override void Update() { }
        public override void Exit() { }
        public override void SwitchToPlayingHandler() => _fsm.SwitchState(_fsm.PlayingState);
        public override void SwitchToPauseHandler() => _fsm.SwitchState(_fsm.PauseState);
        public override void SwitchToDeathHandler() => _fsm.SwitchState(_fsm.DeathState);
        public override void SwitchToNarrationHandler(DialogData data) => _fsm.SwitchToNarration(data);
    }
}