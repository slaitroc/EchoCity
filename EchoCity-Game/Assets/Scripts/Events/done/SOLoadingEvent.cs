using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "LoadingEventSO", menuName = "ECHO CITY/Events/Loading")]
    public class SOLoadingEvent : SOSigleParamEvent<bool>
    {
        public override void RaiseEvent(IEventSender sender, bool loading)
        {
            base.RaiseEvent(sender, loading);
        }
    }
}