using UnityEngine;

namespace EchoCity
{
    public enum SceneLoaderTriggerEnum
    {
        InitLevel
    }
    [CreateAssetMenu(fileName = "SceneLoaderTriggerEvent", menuName = "ECHO CITY/Events/Scene Loader Trigger")]
    public class SOSceneLoaderTriggerEvent : SOEventSingleParam<SceneLoaderTriggerEnum>
    {
        public override void RaiseEvent(IEventSender sender, SceneLoaderTriggerEnum triggerCode)
        {
            base.RaiseEvent(sender, triggerCode);
        }
    }
}