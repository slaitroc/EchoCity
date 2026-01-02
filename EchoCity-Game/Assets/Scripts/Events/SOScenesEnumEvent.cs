using UnityEngine;

namespace EchoCity
{
    [System.Serializable]
    public enum SceneEnum
    {
        //NOTE: numbers must match scene loader's scenes array indices
        None = 0,
        Persistent = 1,
        Playground = 2,
        Level1 = 3,
        Level2 = 4,
    }

    [CreateAssetMenu(fileName = "SceneEnumEventSO", menuName = "ECHO CITY/Events/SceneEnumEventSO")]
    public class SOSceneEnumEvent : SOSigleParamEvent<SceneEnum> { }

}
