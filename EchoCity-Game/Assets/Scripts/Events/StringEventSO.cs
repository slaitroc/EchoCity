using System;
using UnityEngine;

[CreateAssetMenu(fileName = "StringEventSO", menuName = "ECHO CITY/StringEventSO")]
public class StringEventSO : ScriptableObject
{
    public event Action<string> OnEventRaised;
    public void RaiseEvent(string message) => OnEventRaised?.Invoke(message);
}