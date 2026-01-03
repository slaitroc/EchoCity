
using System;

namespace EchoCity
{
    public class SOSigleParamEvent<T> : SOEventBase
    {
        public event Action<IEventSender, T> OnEventRaised;
        public virtual void RaiseEvent(IEventSender sender, T value) => OnEventRaised?.Invoke(sender, value);
    }
}