using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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
    [SerializeField] private SOEventVoid canInteractStartEvent;
    [SerializeField] private SOEventVoid canInteractStopEvent;


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
    private bool canInteract = false;
    #endregion

    private void OnEnable()
    {
        fallbackSoundEmission = new SoundEmissionData(Vector3.zero, radius, intensity, 2f, frequency);
    }

    private void Update()
    {
        #region raycast always active
        var origin = Camera.main.transform.position;
        var direction = Camera.main.transform.forward;
        Ray ray = new Ray(origin, direction);
        Physics.Raycast(ray, out RaycastHit hitInfo, 10f, 1 << 6, QueryTriggerInteraction.Collide);
        var interactable = hitInfo.collider?.GetComponent<Interactable>();
        if (interactable != null)
        {
            if (!canInteract)
            {
                canInteractStartEvent.RaiseEvent();
                Log.D("Can interact", _LOG_COLOR, _LOG_TAG);
                canInteract = true;
            }
        }
        else if (canInteract)
        {
            canInteractStopEvent.RaiseEvent();
            Log.D("Can no longer interact", _LOG_COLOR, _LOG_TAG);
            canInteract = false;
        }
        #endregion

        if (Input.GetKeyDown(KeyCode.V))
        {
            triggeredVoidEvents++;
            voidEvent?.RaiseEvent();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            triggeredStringEvents++;
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
        }

        if (Input.GetKeyDown(KeyCode.X))
            for (var i = 0; i < 100; i++)
            {
                triggeredEchoEvents++;
                var data = NewSoundEmissionData();
                var actual = data.HasValue ? data.Value : fallbackSoundEmission;
                newAudioSphereEvent?.RaiseEvent(actual);

            }

        if (Input.GetKeyDown(KeyCode.R))
        {
            materialToggleEvent?.RaiseEvent();
        }

        if (Input.GetKeyDown(KeyCode.F) && inInteractionRange)
        {
            areaInteractionEvent?.RaiseEvent();
        }

        if (Input.GetMouseButtonDown(0))
        {
            // var origin = Camera.main.transform.position;
            // var direction = Camera.main.transform.forward;
            // Ray ray = new Ray(origin, direction);
            // Physics.Raycast(ray, out RaycastHit hitInfo, 10f, 1 << 6, QueryTriggerInteraction.Collide);
            Debug.DrawRay(origin, direction * 10f, Color.red, 2f);
            Log.D($"Raycast hit: {(hitInfo.collider == null ? "none" : hitInfo.collider.gameObject.name)}", _LOG_COLOR, _LOG_TAG);
            // var interactable = hitInfo.collider?.GetComponent<Interactable>();
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
