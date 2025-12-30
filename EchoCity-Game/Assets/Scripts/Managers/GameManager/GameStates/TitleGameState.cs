using TMPro.EditorUtilities;
using UnityEngine;

namespace EchoCity
{
    public class TitleGameState : GameState
    {
        public TitleGameState(IGMContext context, GameManagerFSM fsm) : base(context, fsm) { }
        public override void Enter()
        {//TODO audio
            Time.timeScale = 0;
            _context.DisablePlayerInputEvent.RaiseEvent(_context);
            _context.EnableUIInputEvent.RaiseEvent(_context);
            _context.TitleMenuEvent.RaiseEvent(_context);
        }
        public override void Update() { }
        public override void Exit() => _context.SetPlayerOnSpawnEvent.RaiseEvent(_context);
        public override GameStatesEnum GetEnum() => GameStatesEnum.Title;
        public override void InitLevelHandler(SceneEnum scene) => _context.LoadLevelEvent.RaiseEvent(_context, scene);
        public override void EnterLoading() { }
        public override void ExitLoading()
        {
            _context.DisablePlayerInputEvent.RaiseEvent(_context);
            _context.EnableUIInputEvent.RaiseEvent(_context);
            _fsm.SwitchState(_fsm.PlayingState);
        }
    }
}