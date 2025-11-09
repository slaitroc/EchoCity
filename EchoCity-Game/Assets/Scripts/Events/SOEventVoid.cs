using UnityEngine;
using System;

[CreateAssetMenu(fileName = "VoidEventSO", menuName = "ECHO CITY/VoidEventSO")]
public class SOEventVoid : ScriptableObject
{
    public event Action OnEventRaised;
    public void RaiseEvent() => OnEventRaised?.Invoke();
}