using System;
using System.Diagnostics;
using UnityEngine;
public class EventObserverExample : MonoBehaviour
{

    [Header("Observed Events")]
    [SerializeField] private SOStringEvent stringEvent;
    [SerializeField] private SOEventVoid voidEvent;
    [SerializeField] private SOIntEvent intEvent;

    [Header("GO with Handlers")]
    [SerializeField] private GameManager_EventTest gameManager;

    void OnEnable()
    {
        if (stringEvent)
        {
            stringEvent.OnEventRaised -= gameManager.WriteMessage;
            stringEvent.OnEventRaised += gameManager.WriteMessage;
        }
        if (voidEvent)
        {
            voidEvent.OnEventRaised -= gameManager.WriteMessage;
            voidEvent.OnEventRaised += gameManager.WriteMessage;
            voidEvent.OnEventRaised -= gameManager.PauseGameHandler;
            voidEvent.OnEventRaised += gameManager.PauseGameHandler;
        }

        if (intEvent)
        {
            intEvent.OnEventRaised -= gameManager.WriteIntMessage;
            intEvent.OnEventRaised += gameManager.WriteIntMessage;
        }

    }

    void OnDisable()
    {

        if (stringEvent) stringEvent.OnEventRaised -= gameManager.WriteMessage;
        if (voidEvent)
        {
            voidEvent.OnEventRaised -= gameManager.WriteMessage;
            voidEvent.OnEventRaised -= gameManager.PauseGameHandler;
        }
        if (intEvent) intEvent.OnEventRaised -= gameManager.WriteIntMessage;

    }
}