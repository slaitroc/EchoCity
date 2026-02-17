using UnityEngine;

namespace EchoCity
{
    [System.Serializable]
    public enum SceneEnum
    {
        //NOTE: numbers must match scene loader's scenes array indices
        None = 0,
        Persistent,
        Playground,
        Narration_Initial,
        Tutorial,
        Narration_AfterTutorial,
        FirstLevel,
        Narration_AfterFirstLevel,
        SecondLevel,
        MAX
    }

    public enum LevelActionCodeEnum
    {
        LoadActiveLevel,
        LoadLevel,
        UnloadLevel,
        ReloadLevel,
    }

    [CreateAssetMenu(fileName = "LevelActionEvent", menuName = "ECHO CITY/Events/Level Action")]
    public class SOLevelActionEvent : SOEventDoubleParam<LevelActionCodeEnum, SceneEnum>
    {
        public override void RaiseEvent(IEventSender sender, LevelActionCodeEnum code, SceneEnum scene = SceneEnum.None)
        {
            base.RaiseEvent(sender, code, scene);
        }
    }

}
