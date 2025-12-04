using UnityEngine;
using System;

[CreateAssetMenu(fileName = "VoidEventSO", menuName = "ECHO CITY/Events/VoidEventSO")]
public class SOEventVoid : ScriptableObject
{
    public event Action OnEventRaised;
    public void RaiseEvent() => OnEventRaised?.Invoke();
}