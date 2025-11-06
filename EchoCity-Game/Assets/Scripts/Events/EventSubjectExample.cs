using UnityEngine;

public class EventSubjectExample : MonoBehaviour
{
    [SerializeField] private VoidEventSO voidEvent;
    [SerializeField] private StringEventSO stringEvent;
    [SerializeField] private IntEventSO intEvent;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            voidEvent.RaiseEvent();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            stringEvent.RaiseEvent("'R' key pressed!");
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            intEvent.RaiseEvent(42);
        }
    }

}