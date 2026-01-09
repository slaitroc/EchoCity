using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "SoundEmittedEvent", menuName = "ECHO CITY/Events/Sound Emitted")]
    public class SOSoundEmittedEvent : SOEventSingleParam<SoundEmissionData>
    {
        public override void RaiseEvent(IEventSender sender, SoundEmissionData soundData)
        {
            base.RaiseEvent(sender, soundData);
        }
    }
}