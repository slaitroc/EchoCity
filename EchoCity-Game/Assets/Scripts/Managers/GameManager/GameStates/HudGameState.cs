using UnityEngine;

namespace EchoCity
{
    public class HudGameState : GameState
    {
        private HudEnum _currentHud;
        public HudGameState(IGMContext context, GameManagerFSM fsm) : base(context, fsm) { }
        public override GameStatesEnum GetEnum() => GameStatesEnum.Hud;
        public override void Enter() => _context.PlayerInputEvent.RaiseEvent(_context, InputEnum.UI, true);
        public void EnterHud(HudEnum hud)
        {
            Enter();
            switch (hud)
            {
                case HudEnum.Inventory:
                    _context.ShowUIEvent.RaiseEvent(_context, ShowableUIEnum.HUD, new HudParams(HudEnum.Inventory));
                    _currentHud = HudEnum.Inventory;
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
                    _context.ShowUIEvent.RaiseEvent(_context, ShowableUIEnum.HUD, new HudParams(HudEnum.Inventory));
                    break;
                //TODO other HUDs
                default:
                    break;
            }
            _currentHud = HudEnum.None;
        }
        public override void SwitchToPlayingHandler(GameStatesEnum previousState) => _fsm.SwitchState(_fsm.PlayingState);
        public override void SwitchToPauseHandler(GameStatesEnum previousState) => _fsm.SwitchState(_fsm.PauseState);
        public override void SwitchToDeathHandler(GameStatesEnum previousState) => _fsm.SwitchState(_fsm.DeathState);
        public override void SwitchToNarrationHandler(GameStatesEnum previousState, ToNarrationParams @params) => _fsm.SwitchToNarration(@params);
    }
}