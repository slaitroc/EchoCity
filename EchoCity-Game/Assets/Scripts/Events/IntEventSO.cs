using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "IntEventSO", menuName = "ECHO CITY/IntEventSO")]
public class IntEventSO : ScriptableObject
{
    public event Action<int> OnEventRaised;
    public void RaiseEvent(int value) => OnEventRaised?.Invoke(value);

}
