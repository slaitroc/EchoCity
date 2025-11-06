using System.Diagnostics;
using UnityEngine;
public class EventObserverExample : MonoBehaviour
{

    [Header("Observed Events")]
    [SerializeField] private StringEventSO stringEvent;
    [SerializeField] private VoidEventSO voidEvent;
    [SerializeField] private IntEventSO intEvent;

    [Header("GO with Handlers")]
    [SerializeField] private GameManager_EventTest gameManager;

    void OnEnable()
    {
        stringEvent.OnEventRaised -= gameManager.WriteMessage;
        stringEvent.OnEventRaised += gameManager.WriteMessage;

        voidEvent.OnEventRaised -= gameManager.WriteMessage;
        voidEvent.OnEventRaised += gameManager.WriteMessage;

        intEvent.OnEventRaised -= gameManager.WriteIntMessage;
        intEvent.OnEventRaised += gameManager.WriteIntMessage;

    }

    void OnDisable()
    {
        stringEvent.OnEventRaised -= gameManager.WriteMessage;
        voidEvent.OnEventRaised -= gameManager.WriteMessage;
        intEvent.OnEventRaised -= gameManager.WriteIntMessage;
    }
}