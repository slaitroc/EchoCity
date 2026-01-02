using UnityEngine;

namespace EchoCity
{
    public class HudGameState : GameState
    {
        private HudEnum _currentHud;
        public HudGameState(IGMContext context, GameManagerFSM fsm) : base(context, fsm) { }
        public override GameStatesEnum GetEnum() => GameStatesEnum.Hud;
        public override void Enter() => _context.EnableUIInputEvent.RaiseEvent(_context);
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
        public override void SwitchToPlayingHandler() => _fsm.SwitchState(_fsm.PlayingState);
        public override void SwitchToPauseHandler() => _fsm.SwitchState(_fsm.PauseState);
        public override void SwitchToDeathHandler() => _fsm.SwitchState(_fsm.DeathState);
        public override void SwitchToNarrationHandler(DialogData data) => _fsm.SwitchToNarration(data);
    }
}