using System;
using Unity.VisualScripting;

namespace EchoCity
{
    [System.Serializable]
    public class EnemyFSM : IEnemyStatesFSM
    {
        public IEnemyState CurrentState { get; private set; }
        public IEnemyState PreviousState { get; private set; }

        public IEnemyState PatrolState;
        public IEnemyState SoundChaseState;
        public IEnemyState PlayerChaseState;
        public IEnemyState AttackState;

        private IEnemyContext _context;

        public EnemyFSM(IEnemyContext context)
        {
            _context = context;
            PatrolState = new PatrolEnemyState(context, this);
            SoundChaseState = new SoundChaseEnemyState(context, this);
            PlayerChaseState = new PlayerChaseEnemyState(context, this);
            AttackState = new AttackEnemyState(context, this);
        }

        public void Initialize()
        {
            CurrentState = PatrolState;
            CurrentState.Enter();
        }

        public void Update()
        {
            CurrentState.Update();
        }

        public void SwitchState(IEnemyState newState)
        {
            CurrentState.Exit();
            PreviousState = CurrentState;
            CurrentState = newState;
            CurrentState.Enter();
            _context.EnemyStateTransitionEvent.RaiseEvent(_context, PreviousState.GetEnum(), CurrentState.GetEnum(), _context.Owner.Transform);
        }
    }
}