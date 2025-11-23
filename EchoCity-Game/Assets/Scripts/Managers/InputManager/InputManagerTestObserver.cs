using UnityEngine;

[RequireComponent(typeof(InputManagerTest))]
public class InputManagerTestObserver : MonoBehaviour
{
    [Header("Observed Events")]
    [SerializeField] private SOAreaInteractableEvent enterInteractableAreaEvent;
    [SerializeField] private SOAreaInteractableEvent exitInteractableAreaEvent;


    [Header("GO with Handlers")]
    [SerializeField] private InputManagerTest inputManagerTest;

    void Awake()
    {
        TryGetComponent(out inputManagerTest);
    }

    void OnEnable()
    {
        if (enterInteractableAreaEvent)
        {
            enterInteractableAreaEvent.OnEventRaised -= inputManagerTest.EnterInteractionRangeHandler;
            enterInteractableAreaEvent.OnEventRaised += inputManagerTest.EnterInteractionRangeHandler;
        }
        if (exitInteractableAreaEvent)
        {
            exitInteractableAreaEvent.OnEventRaised -= inputManagerTest.ExitInteractionRangeHandler;
            exitInteractableAreaEvent.OnEventRaised += inputManagerTest.ExitInteractionRangeHandler;
        }
    }

    void OnDisable()
    {
        if (enterInteractableAreaEvent) enterInteractableAreaEvent.OnEventRaised -= inputManagerTest.EnterInteractionRangeHandler;
        if (exitInteractableAreaEvent) exitInteractableAreaEvent.OnEventRaised -= inputManagerTest.ExitInteractionRangeHandler;
    }

}
