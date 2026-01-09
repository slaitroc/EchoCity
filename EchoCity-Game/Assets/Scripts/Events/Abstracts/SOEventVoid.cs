using UnityEngine;
using System;

namespace EchoCity
{
    public abstract class SOEventVoid : SOEventBase
    {
        public event Action<IEventSender> OnEventRaised;
        public void RaiseEvent(IEventSender sender) => OnEventRaised?.Invoke(sender);
    }
}