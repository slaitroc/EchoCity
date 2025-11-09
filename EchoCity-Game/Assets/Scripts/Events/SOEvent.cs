
using UnityEngine;
using System;

public class SOEvent<T> : ScriptableObject
{
    public event Action<T> OnEventRaised;
    public void RaiseEvent(T value) => OnEventRaised?.Invoke(value);
}