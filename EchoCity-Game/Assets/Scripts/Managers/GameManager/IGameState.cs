public interface IGameState
{
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
    public abstract GameStatesEnum GetEnum();
    public string Kind() => GetEnum().ToString();
}