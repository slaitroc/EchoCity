using UnityEngine;
using System;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "VoidEventSO", menuName = "ECHO CITY/Events/VoidEventSO")]
    public class SOEventVoid : ScriptableObject
    {
        public event Action OnEventRaised;
        public void RaiseEvent() => OnEventRaised?.Invoke();
    }
}