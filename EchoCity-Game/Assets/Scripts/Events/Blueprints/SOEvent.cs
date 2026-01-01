
using UnityEngine;
using System;

namespace EchoCity
{
    public class SOEvent<T> : SOEventBase
    {
        public event Action<IEventSender, T> OnEventRaised;
        public void RaiseEvent(IEventSender sender, T value) => OnEventRaised?.Invoke(sender, value);
    }
}