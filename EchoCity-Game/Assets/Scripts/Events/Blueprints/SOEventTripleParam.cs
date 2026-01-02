
using UnityEngine;
using System;

namespace EchoCity
{
    public class SOEventTripleParam<T1, T2, T3> : SOEventBase
    {
        public event Action<IEventSender, T1, T2, T3> OnEventRaised;
        public virtual void RaiseEvent(IEventSender sender, T1 value1, T2 value2, T3 value3) => OnEventRaised?.Invoke(sender, value1, value2, value3);
    }
}