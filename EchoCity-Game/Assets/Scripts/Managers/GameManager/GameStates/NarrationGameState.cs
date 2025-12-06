using UnityEngine;
using System;

namespace EchoCity
{
    public class NarrationGameState : GameState
    {
        public NarrationGameState(GameManager gameManager, GameStatesFSM fsm) : base(gameManager, fsm)
        {
        }

        public override void Enter()
        {
        }

        public override void Update()
        {
        }

        public override void Exit()
        {
        }

        public override GameStatesEnum GetEnum()
        {
            return GameStatesEnum.Narration;
        }

        public override bool PauseGameHandler()
        {
            _fsm.SwitchState(_fsm.PauseState);
            return true;
        }

        public override bool DeathHandler()
        {
            _fsm.SwitchState(_fsm.DeathState);
            return true;
        }
    }
}
