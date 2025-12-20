using UnityEngine;

namespace EchoCity
{
    [System.Serializable]
    public enum SceneEnum
    {
        None = 0,
        Persistent = 1,
        Level1 = 2,
        Level2 = 3,
        Playground = 100
    }

    [CreateAssetMenu(fileName = "SceneEnumEventSO", menuName = "ECHO CITY/Events/SceneEnumEventSO")]
    public class SOSceneEnumEvent : SOEvent<SceneEnum> { }

}
