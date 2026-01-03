using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "QuestUpdatedEvent", menuName = "ECHO CITY/Events/Quest Updated")]
    public class SOQuestUpdatedEvent : SOEventDoubleParam<int, int>
    {
        public override void RaiseEvent(IEventSender sender, int questIndex, int code)
        {
            base.RaiseEvent(sender, questIndex, code);
        }
    }
}