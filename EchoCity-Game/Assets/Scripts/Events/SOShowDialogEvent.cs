using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "DialogDataEventSO", menuName = "ECHO CITY/Events/DialogDataEventSO")]
    public class SOShowDialogEvent : SOSigleParamEvent<DialogData>
    {
        public override void RaiseEvent(IEventSender sender, DialogData dialogData)
        {
            base.RaiseEvent(sender, dialogData);
        }
    }
}
