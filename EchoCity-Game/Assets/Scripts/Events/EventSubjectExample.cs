using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class EventSubjectExample : MonoBehaviour
{

    private string _LOG_TAG = "EVENT SUBJECT";
    private string _LOG_COLOR = "#7039e8ff";

    [Header("Invoking Events")]
    [SerializeField] private SOEventVoid voidEvent;
    [SerializeField] private SOStringEvent stringEvent;
    [SerializeField] private SOIntEvent intEvent;
    [SerializeField] private SONewAudioSphereEvent newAudioSphereEvent;

    [Header("Script Variables")]
    [Tooltip("0/false = PCF Renderer, 1/true = Audio Visual")]
    [SerializeField] private bool activeRenderer = true; // 0/false = PCF Renderer, 1/true = Audio Visual
    [SerializeField] private int triggeredIntEvents = 0;
    [SerializeField] private int triggeredVoidEvents = 0;
    [SerializeField] private int triggeredStringEvents = 0;
    [SerializeField] private int triggeredEchoEvents = 0;
    [SerializeField] private List<GameObject> emittersPositionsList;
    [SerializeField] private List<AudioClip> audioClipsList;
    [SerializeField] private float radius;
    [SerializeField] private float intensity;
    [SerializeField] private bool enableMinimumDuration = true;
    [SerializeField] private float minimumDuration;
    [SerializeField] private SoundEmissionData fallbackSoundEmission;


    void OnEnable()
    {
        fallbackSoundEmission = new SoundEmissionData(Vector3.zero, radius, intensity, 2f);
    }

    void Start()
    {
        Camera.main.GetUniversalAdditionalCameraData().SetRenderer(Convert.ToInt32(activeRenderer));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            triggeredVoidEvents++;
            voidEvent?.RaiseEvent();
            Log.D($"Void event triggered {triggeredVoidEvents} times by pressing 'V'", $"{_LOG_COLOR}", $"{_LOG_TAG}");
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            triggeredStringEvents++;
            stringEvent?.RaiseEvent($"'T' key pressed! Event count: {triggeredStringEvents}");
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            for (int i = 0; i < 100; i++)
            {
                triggeredIntEvents++;
                intEvent?.RaiseEvent(triggeredIntEvents);
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            triggeredEchoEvents++;
            SoundEmissionData? data = NewSoundEmissionData();
            SoundEmissionData actual = (data.HasValue) ? data.Value : fallbackSoundEmission;

            newAudioSphereEvent?.RaiseEvent(actual);
            Log.D($"Echo event triggered {triggeredEchoEvents} times by pressing 'E'", $"{_LOG_COLOR}", $"{_LOG_TAG}");
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            for (int i = 0; i < 100; i++)
            {
                triggeredEchoEvents++;
                SoundEmissionData? data = NewSoundEmissionData();
                SoundEmissionData actual = (data.HasValue) ? data.Value : fallbackSoundEmission;
                newAudioSphereEvent?.RaiseEvent(actual);

                Log.D($"Echo event triggered {triggeredEchoEvents} times by pressing 'X'", $"{_LOG_COLOR}", $"{_LOG_TAG}");
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            activeRenderer = !activeRenderer;
            Camera.main.GetUniversalAdditionalCameraData().SetRenderer(Convert.ToInt32(!activeRenderer));
        }
    }

    private SoundEmissionData? NewSoundEmissionData()
    {
        if (emittersPositionsList.Count > 0)
        {
            GameObject go = emittersPositionsList[UnityEngine.Random.Range(0, emittersPositionsList.Count)];
            AudioClip audioClip = audioClipsList[UnityEngine.Random.Range(0, audioClipsList.Count)];
            AudioSource audioSource = go.GetComponent<AudioSource>();
            audioSource.spatialBlend = 1f; // 3D sound
            audioSource.PlayOneShot(audioClip);
            // AudioSource.PlayClipAtPoint(audioClip, go.transform.position);
            if (enableMinimumDuration) return new SoundEmissionData(go.transform.position, radius, intensity, minimumDuration);
            return new SoundEmissionData(go.transform.position, radius, intensity, audioClip.length);
        }
        else return null;
    }
}