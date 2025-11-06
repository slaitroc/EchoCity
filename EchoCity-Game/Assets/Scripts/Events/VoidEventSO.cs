using System;
using UnityEngine;

[CreateAssetMenu(fileName = "VoidEventSO", menuName = "ECHO CITY/VoidEventSO")]
public class VoidEventSO : ScriptableObject
{
    public event Action OnEventRaised;
    public void RaiseEvent() => OnEventRaised?.Invoke();
}