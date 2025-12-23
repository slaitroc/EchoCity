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


        public EnemyFSM(IEnemyContext context)
        {
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
        }
    }
}