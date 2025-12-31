
using UnityEngine;
using System;

namespace EchoCity
{
    public enum EventSenderCategoriesEnum
    {
        None,
        GameManager,
        SceneLoader,
        Interactable,
        Echolocation,
        Puzzle,
        Player,
        Enemy,
        UI,
        Emitter,
        Utility,
        Tutorial
    }
    public class SOEvent<T> : SOEventBase
    {
        public event Action<IEventSender, T> OnEventRaised;
        public void RaiseEvent(IEventSender sender, T value) => OnEventRaised?.Invoke(sender, value);
    }
}