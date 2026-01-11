using UnityEngine;

namespace EchoCity
{
    public enum InteractionEnum
    {
        None,
        Radio,
        DeskFan,
        Generator,
        CardReader,
        Door,
        ChangeSceneDoor,
        LaboratoryAreaDoor,
        DoorArea,
        Pickable,
        WallLightSwitch,
        TutorialTrigger
    }

    public enum InteractionObjectTypeEnum
    {

    }
    [CreateAssetMenu(fileName = "InteractionEvent", menuName = "ECHO CITY/Events/Interaction")]
    public class SOInteractionEvent : SOEventSingleParam<InteractionEnum>
    {
        public override void RaiseEvent(IEventSender sender, InteractionEnum interaction)
        {
            base.RaiseEvent(sender, interaction);
        }
    }
}