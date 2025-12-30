using UnityEngine;
using System;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "VoidEventSO", menuName = "ECHO CITY/Events/VoidEventSO")]
    public class SOEventVoid : ScriptableObject
    {
        public event Action<IEventSender> OnEventRaised;
        public void RaiseEvent(IEventSender sender) => OnEventRaised?.Invoke(sender);
    }
}