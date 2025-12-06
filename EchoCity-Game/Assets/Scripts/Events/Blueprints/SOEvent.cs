
using UnityEngine;
using System;

namespace EchoCity
{
    public class SOEvent<T> : ScriptableObject
    {
        public event Action<T> OnEventRaised;
        public void RaiseEvent(T value) => OnEventRaised?.Invoke(value);
    }
}