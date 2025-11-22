using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EventSubjectExample : MonoBehaviour
{
    [Header("Invoking Events")] [SerializeField]
    private SOEventVoid voidEvent;

    [SerializeField] private SOStringEvent stringEvent;
    [SerializeField] private SOIntEvent intEvent;
    [SerializeField] private SONewAudioSphereEvent newAudioSphereEvent;

    [Header("Script Variables")]
    [Tooltip("0/false = Original Materials, 1/true = Echolocation Material")]
    [SerializeField]
    private bool useEcholocationMaterial = true;

    [SerializeField] private Material echolocationMaterial;
    [SerializeField] private int triggeredIntEvents;
    [SerializeField] private int triggeredVoidEvents;
    [SerializeField] private int triggeredStringEvents;
    [SerializeField] private int triggeredEchoEvents;
    [SerializeField] private List<GameObject> emittersPositionsList;
    [SerializeField] private List<AudioClip> audioClipsList;
    [SerializeField] private float radius;
    [SerializeField] private Frequency frequency;
    [SerializeField] private float intensity;
    [SerializeField] private bool enableMinimumDuration = true;
    [SerializeField] private float minimumDuration;
    private readonly string _LOG_COLOR = "#7039e8ff";

    private readonly string _LOG_TAG = "EVENT SUBJECT";
    [SerializeField] private SoundEmissionData fallbackSoundEmission;

    private void Start()
    {
        if (useEcholocationMaterial && echolocationMaterial != null)
            MaterialSwitcher.ApplyOverrideMaterial(echolocationMaterial);
    }

    private void Update()
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
            Log.D($"Echo event triggered {triggeredEchoEvents} times by pressing 'E'", $"{_LOG_COLOR}", $"{_LOG_TAG}");
        }

        if (Input.GetKeyDown(KeyCode.X))
            for (var i = 0; i < 100; i++)
            {
                triggeredEchoEvents++;
                var data = NewSoundEmissionData();
                var actual = data.HasValue ? data.Value : fallbackSoundEmission;
                newAudioSphereEvent?.RaiseEvent(actual);

                Log.D($"Echo event triggered {triggeredEchoEvents} times by pressing 'X'", $"{_LOG_COLOR}",
                    $"{_LOG_TAG}");
            }

        if (Input.GetKeyDown(KeyCode.R))
        {
            useEcholocationMaterial = !useEcholocationMaterial;

            if (useEcholocationMaterial && echolocationMaterial != null)
            {
                MaterialSwitcher.ApplyOverrideMaterial(echolocationMaterial);
                Log.D("Switched to Echolocation Material", $"{_LOG_COLOR}", $"{_LOG_TAG}");
            }
            else
            {
                MaterialSwitcher.RestoreOriginalMaterials();
                Log.D("Restored Original Materials", $"{_LOG_COLOR}", $"{_LOG_TAG}");
            }
        }
    }


    private void OnEnable()
    {
        fallbackSoundEmission = new SoundEmissionData(Vector3.zero, radius, intensity, 2f, (float)frequency);
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
                    (float)frequency);
            return new SoundEmissionData(go.transform.position, radius, intensity, audioClip.length, (float)frequency);
        }

        return null;
    }
}