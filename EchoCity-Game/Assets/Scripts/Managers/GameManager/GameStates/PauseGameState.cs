using System;
using UnityEngine;
using UnityEngine.InputSystem;


namespace EchoCity
{
    public class PauseGameState : GameState
    {
        [Header("UI Action Map")]
        private const string UI_ACTION_MAP = "UI";

        public PauseGameState(GameManager gameManager, GameStatesFSM fsm) : base(gameManager, fsm) { }
        public override void Enter()
        {
            Log.D("Game Paused");
            Time.timeScale = 0;
        }

        public override void Update()
        {
        }

        public override void Exit()
        {
            Time.timeScale = 1;
        }



        public override GameStatesEnum GetEnum()
        {
            return GameStatesEnum.Pause;
        }

        public override bool PauseGameHandler()
        {
            _fsm.SwitchState(_fsm.PreviousState);
            return true;
        }

        public override bool DeathHandler()
        {
            _fsm.SwitchState(_fsm.DeathState);
            return true;
        }
    }
}