using UnityEngine;

/// <summary>
/// Observer per InputManagerTest - gestisce la connessione agli eventi.
/// Pattern simile a quello visto nel branch Develop.
/// 
/// NOTA: Se in futuro aggiungi sistema Interactable, puoi decommentare le sezioni relative.
/// </summary>
[RequireComponent(typeof(InputManagerTest))]
public class InputManagerTestObserver : MonoBehaviour
{
    // [Header("Observed Events")]
    // [SerializeField] private SOAreaInteractableEvent enterInteractableAreaEvent;
    // [SerializeField] private SOAreaInteractableEvent exitInteractableAreaEvent;

    [Header("GO with Handlers")]
    [SerializeField] private InputManagerTest inputManagerTest;

    void Awake()
    {
        TryGetComponent(out inputManagerTest);
    }

    void OnEnable()
    {
        // Se in futuro aggiungi sistema Interactable, decommenta:
        // if (enterInteractableAreaEvent != null && inputManagerTest != null)
        // {
        //     enterInteractableAreaEvent.OnEventRaised -= inputManagerTest.EnterInteractionRangeHandler;
        //     enterInteractableAreaEvent.OnEventRaised += inputManagerTest.EnterInteractionRangeHandler;
        // }
        // if (exitInteractableAreaEvent != null && inputManagerTest != null)
        // {
        //     exitInteractableAreaEvent.OnEventRaised -= inputManagerTest.ExitInteractionRangeHandler;
        //     exitInteractableAreaEvent.OnEventRaised += inputManagerTest.ExitInteractionRangeHandler;
        // }
    }

    void OnDisable()
    {
        // Se in futuro aggiungi sistema Interactable, decommenta:
        // if (enterInteractableAreaEvent != null && inputManagerTest != null)
        // {
        //     enterInteractableAreaEvent.OnEventRaised -= inputManagerTest.EnterInteractionRangeHandler;
        // }
        // if (exitInteractableAreaEvent != null && inputManagerTest != null)
        // {
        //     exitInteractableAreaEvent.OnEventRaised -= inputManagerTest.ExitInteractionRangeHandler;
        // }
    }
}

