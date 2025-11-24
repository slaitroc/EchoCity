using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class InputManagerTest : MonoBehaviour
{
    #region Constants
    private string _LOG_TAG = "INPUT MANAGER TEST";
    private string _LOG_COLOR = "#7039e8ff";
    #endregion

    #region Serialized Fields
    [Header("Invoking Events")]
    [SerializeField] private SOEventVoid voidEvent;
    [SerializeField] private SOStringEvent stringEvent;
    [SerializeField] private SOIntEvent intEvent;
    [SerializeField] private SOSoundEmissionDataEvent newAudioSphereEvent;
    [SerializeField] private SOEventVoid materialToggleEvent;
    [SerializeField] private SOEventVoid areaInteractionEvent;


    [Header("Echolocation Settings")]
    [SerializeField] private float radius;
    [SerializeField] private Frequency frequency;
    [SerializeField] private List<GameObject> emittersPositionsList;
    [SerializeField] private List<AudioClip> audioClipsList;
    [SerializeField] private float intensity;
    [SerializeField] private bool enableMinimumDuration = true;
    [SerializeField] private float minimumDuration;
    [SerializeField] private SoundEmissionData fallbackSoundEmission;

    [Header("Interaction Range Colliders")]
    [SerializeField] private bool inInteractionRange = false;
    [SerializeField] private Interactable inRangeInteractable;
    #endregion

    #region Private Fields
    private int triggeredIntEvents = 0;
    private int triggeredVoidEvents = 0;
    private int triggeredStringEvents = 0;
    private int triggeredEchoEvents = 0;
    #endregion

    private void OnEnable()
    {
        fallbackSoundEmission = new SoundEmissionData(Vector3.zero, radius, intensity, 2f, frequency);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            triggeredVoidEvents++;
            voidEvent?.RaiseEvent();
            Log.D($"Void event triggered {triggeredVoidEvents} times by pressing 'V'", _LOG_COLOR, _LOG_TAG);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            triggeredStringEvents++;
            stringEvent?.RaiseEvent($"'T' key pressed! Event count: {triggeredStringEvents}");
        }

        if (Input.GetKeyDown(KeyCode.I))
            for (var i = 0; i < 100; i++)
            {
                triggeredIntEvents++;
                intEvent?.RaiseEvent(triggeredIntEvents);
            }

        if (Input.GetKeyDown(KeyCode.E))
        {
            triggeredEchoEvents++;
            var data = NewSoundEmissionData();
            var actual = data.HasValue ? data.Value : fallbackSoundEmission;

            newAudioSphereEvent?.RaiseEvent(actual);
            Log.D($"Echo event triggered {triggeredEchoEvents} times by pressing 'E'", _LOG_COLOR, _LOG_TAG);
        }

        if (Input.GetKeyDown(KeyCode.X))
            for (var i = 0; i < 100; i++)
            {
                triggeredEchoEvents++;
                var data = NewSoundEmissionData();
                var actual = data.HasValue ? data.Value : fallbackSoundEmission;
                newAudioSphereEvent?.RaiseEvent(actual);

                Log.D($"Echo event triggered {triggeredEchoEvents} times by pressing 'X'", _LOG_COLOR, _LOG_TAG);
            }

        if (Input.GetKeyDown(KeyCode.R))
        {
            materialToggleEvent?.RaiseEvent();
        }

        if (Input.GetKeyDown(KeyCode.F) && inInteractionRange)
        {
            areaInteractionEvent?.RaiseEvent();
            Log.D($"Interacted with {inRangeInteractable.gameObject.name}'s Interaction Area by pressing 'F'", _LOG_COLOR, _LOG_TAG);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            Physics.Raycast(ray, out RaycastHit hitInfo, 10f, ~(1 << 6), QueryTriggerInteraction.Collide);
            Log.D($"Raycast hit: {(hitInfo.collider == null ? "none" : hitInfo.collider.gameObject.name)}", _LOG_COLOR, _LOG_TAG);
            var interactable = hitInfo.collider?.GetComponent<Interactable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }

    private SoundEmissionData? NewSoundEmissionData()
    {
        if (emittersPositionsList.Count > 0)
        {
            var go = emittersPositionsList[Random.Range(0, emittersPositionsList.Count)];
            var audioClip = audioClipsList[Random.Range(0, audioClipsList.Count)];
            var audioSource = go.GetComponent<AudioSource>();
            audioSource.spatialBlend = 1f; // 3D sound
            audioSource.PlayOneShot(audioClip);
            // AudioSource.PlayClipAtPoint(audioClip, go.transform.position);
            if (enableMinimumDuration)
                return new SoundEmissionData(go.transform.position, radius, intensity, minimumDuration,
                    frequency);
            return new SoundEmissionData(go.transform.position, radius, intensity, audioClip.length, frequency);
        }

        return null;
    }

    //DANGER distinct InteractableArea's colliders MUST NOT intersect otherwise this implementation WILL NOT WORK as expected!
    public void EnterInteractionRangeHandler(Interactable interactable)
    {
        inInteractionRange = true;
        inRangeInteractable = interactable;
    }
    public void ExitInteractionRangeHandler(Interactable interactable)
    {
        inInteractionRange = false;
        inRangeInteractable = null;
    }
}
