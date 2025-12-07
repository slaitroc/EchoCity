using UnityEngine;

namespace EchoCity
{
    public class EcholocationManagerObserver : MonoBehaviour
    {

        [Header("Observed Events")]
        [SerializeField] private SOSoundEmissionDataEvent newAudioSphereEvent;
        [SerializeField] private SOEventVoid materialToggleEvent;
        [Header("GO with Handlers")]
        [SerializeField] private EcholocationManager echolocationManager;

        void OnEnable()
        {
            if (newAudioSphereEvent)
            {
                newAudioSphereEvent.OnEventRaised -= echolocationManager.AddAudioSphereHandler;
                newAudioSphereEvent.OnEventRaised += echolocationManager.AddAudioSphereHandler;
            }
            if (materialToggleEvent)
            {
                materialToggleEvent.OnEventRaised -= echolocationManager.MaterialSwitcherHandler;
                materialToggleEvent.OnEventRaised += echolocationManager.MaterialSwitcherHandler;

            }
        }

        void OnDisable()
        {
            if (newAudioSphereEvent)
            {
                newAudioSphereEvent.OnEventRaised -= echolocationManager.AddAudioSphereHandler;
            }
            if (materialToggleEvent)
            {
                materialToggleEvent.OnEventRaised -= echolocationManager.MaterialSwitcherHandler;
            }
        }
    }
}
