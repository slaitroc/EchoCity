using UnityEngine;

public class UIManagerObserver : MonoBehaviour
{
    [Header("Observed Events")]
    [SerializeField] private SOEventVoid pauseEvent;
    
    [Header("GO with Handlers")]
    [SerializeField] private UIManager uiManager;

    
    void OnEnable()
    {
        if (pauseEvent)
        {
            pauseEvent.OnEventRaised -= uiManager.PauseMenuHandler;
            pauseEvent.OnEventRaised += uiManager.PauseMenuHandler;
        }
    }
    
    
    void OnDisable()
    {
        if (pauseEvent) pauseEvent.OnEventRaised -= uiManager.PauseMenuHandler;
    }
}
