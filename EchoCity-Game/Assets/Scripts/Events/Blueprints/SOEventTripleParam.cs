
using UnityEngine;
using System;

namespace EchoCity
{
    public class SOEventTripleParam<T1, T2, T3> : ScriptableObject
    {
        public event Action<T1, T2, T3> OnEventRaised;
        public void RaiseEvent(T1 value1, T2 value2, T3 value3) => OnEventRaised?.Invoke(value1, value2, value3);
    }
}