using UnityEngine;

namespace EchoCity
{
    public enum UITriggerEnum
    {
    }

    [CreateAssetMenu(fileName = "UITriggerEvent", menuName = "ECHO CITY/Events/UI Trigger")]
    public class SOUITriggerEvent : SOSingleParamEvent<UITriggerEnum>
    {
        public override void RaiseEvent(IEventSender sender, UITriggerEnum triggerCode)
        {
            base.RaiseEvent(sender, triggerCode);
        }
    }
}