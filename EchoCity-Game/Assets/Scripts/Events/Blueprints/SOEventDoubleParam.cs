
using UnityEngine;
using System;

namespace EchoCity
{
    public class SOEventDoubleParam<T1, T2> : ScriptableObject
    {
        public event Action<T1, T2> OnEventRaised;
        public void RaiseEvent(T1 value1, T2 value2) => OnEventRaised?.Invoke(value1, value2);
    }
}