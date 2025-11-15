using UnityEngine;

public class EcholocationManagerObserver : MonoBehaviour
{

    [Header("Observed Events")]
    [SerializeField] private SONewAudioSphereEvent newAudioSphereEvent;
    [Header("GO with Handlers")]
    [SerializeField] private EcholocationManager echolocationManager;

    void OnEnable()
    {
        if (newAudioSphereEvent)
        {
            newAudioSphereEvent.OnEventRaised -= echolocationManager.AddAudioSphereHandler;
            newAudioSphereEvent.OnEventRaised += echolocationManager.AddAudioSphereHandler;
        }
    }

    void OnDisable()
    {
        if (newAudioSphereEvent)
        {
            newAudioSphereEvent.OnEventRaised -= echolocationManager.AddAudioSphereHandler;
        }
    }





}