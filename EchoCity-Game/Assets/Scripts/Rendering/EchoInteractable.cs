using UnityEngine;

public class EchoInteractable : MonoBehaviour
{
    public TriggerType triggerType = TriggerType.OnClick;

    private AudioEcholocator echolocator;

    public enum TriggerType
    {
        OnCollision,  // Triggered by collision
        OnClick,      // Triggered by mouse click
    }

    void Start()
    {
        echolocator = GetComponent<AudioEcholocator>();
        if (echolocator == null)
        {
            Debug.LogWarning($"EchoInteractable '{gameObject.name}' requires an AudioEcholocator component!");
        }

        if (triggerType == TriggerType.OnClick || triggerType == TriggerType.OnCollision)
        {
            Collider col = GetComponent<Collider>();
            if (col == null)
            {
                Debug.LogWarning($"EchoInteractable '{gameObject.name}' requires a Collider!");
            }

            if (triggerType == TriggerType.OnCollision)
            {
                col.isTrigger = true;
            }
        }
    }

    void Update()
    {
        if (triggerType == TriggerType.OnClick)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject)
                {
                    if (echolocator != null)
                    {
                        echolocator.EmitEcho();
                    }
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggerType == TriggerType.OnCollision)
        {
            if (echolocator != null)
            {
                echolocator.EmitEcho();
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (triggerType == TriggerType.OnCollision)
        {
            if (echolocator != null)
            {
                echolocator.EmitEcho();
            }
        }
    }
}
