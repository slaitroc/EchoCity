using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EchoCity
{
    public enum VisualizationMode
    {
        SolidColor = 0,
        GridLines = 1,
        GridPoints = 2
    }

    public class EcholocationManager : MonoBehaviour, IEventSender
    {
        [Header("Invoking Events")]
        [SerializeField] private SOEchoMaterialUpdated echoMaterialUpdatedEvent;

        string IEventSender.SenderName => gameObject.name;
        int IEventSender.SenderID => GetInstanceID();
        bool IEventSender.IsManager => true;
        EventSenderCategoriesEnum[] IEventSender.SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.Echolocation };

        [Header("Observed Events")]
        [SerializeField] private SOSoundEmittedEvent soundEmittedEvent;
        [SerializeField] private SOSetMaterialEvent setMaterialEvent;


        [Header("Echolocation Settings")]
        [Range(0.01f, 1.0f)][SerializeField] private float fadeInDuration = 0.1f;

        [Range(0.1f, 3.0f)][SerializeField] private float fadeOutDuration = 0.5f;

        private const int MAX_AUDIO_SPHERES = 64;
        private bool _isEcholocationActive;
        public bool IsEcholocationActive => _isEcholocationActive;
        private const string GlobalToggleProp = "_Echolocation";
        private const string KeywordOn = "ECHOLOCATION_ON";
        private MaterialPropertyBlock _mpb;

        [SerializeField] private List<AudioSphere> activeSpheres = new();
        private readonly float[] sphereFrequencies = new float[MAX_AUDIO_SPHERES];
        private readonly float[] sphereIntensities = new float[MAX_AUDIO_SPHERES];

        private readonly Vector4[] spherePositions = new Vector4[MAX_AUDIO_SPHERES];
        private readonly float[] sphereRadii = new float[MAX_AUDIO_SPHERES];
        private int sphereCountID;
        private int sphereFrequenciesID;
        private int sphereIntensitiesID;

        private int spherePositionsID;
        private int sphereRadiiID;

        private void Awake()
        {
            spherePositionsID = Shader.PropertyToID("_AudioSpherePositions");
            sphereRadiiID = Shader.PropertyToID("_AudioSphereRadii");
            sphereFrequenciesID = Shader.PropertyToID("_AudioSphereFrequencies");
            sphereIntensitiesID = Shader.PropertyToID("_AudioSphereIntensities");
            sphereCountID = Shader.PropertyToID("_AudioSphereCount");

            _mpb = new MaterialPropertyBlock();
            _isEcholocationActive = false;
            Shader.SetGlobalInt(GlobalToggleProp, 0);
            Shader.DisableKeyword(KeywordOn);
        }

        void OnEnable()
        {
            if (soundEmittedEvent) soundEmittedEvent.OnEventRaised += AddAudioSphereHandler;
            if (setMaterialEvent) setMaterialEvent.OnEventRaised += SetMaterialHandler;

        }

        void OnDisable()
        {
            if (soundEmittedEvent) soundEmittedEvent.OnEventRaised -= AddAudioSphereHandler;
            if (setMaterialEvent) setMaterialEvent.OnEventRaised -= SetMaterialHandler;
        }

        // private void OnValidate()
        // {
        //     if (Application.isPlaying)
        //     {
        //         if (_isEcholocationActive) // TODO: ????
        //         {
        //             SetEcholocationActive(true);
        //             Log.DLazy(() => "Echolocation Material Applied", this);
        //         }
        //         else
        //         {
        //             SetEcholocationActive(false);
        //             Log.DLazy(() => "Restored Original Materials", this);
        //         }
        //     }
        // }

        private void Update()
        {
            // Update all active spheres
            for (var i = activeSpheres.Count - 1; i >= 0; i--)
            {
                var sphere = activeSpheres[i];
                sphere.TimeRemaining -= Time.deltaTime;

                // Update based on fade in/out
                sphere.CurrentIntensity = sphere.GetCurrentIntensity();

                if (sphere.IsExpired) activeSpheres.RemoveAt(i);
                else activeSpheres[i] = sphere;
            }

            // Clear arrays
            for (var i = 0; i < spherePositions.Length; i++)
            {
                spherePositions[i] = Vector4.zero;
                sphereRadii[i] = 0f;
                sphereFrequencies[i] = 0f;
                sphereIntensities[i] = 0f;
            }

            // Fill arrays with active spheres
            var count = Mathf.Min(activeSpheres.Count, spherePositions.Length);
            for (var i = 0; i < count; i++)
            {
                var pos = activeSpheres[i].Position;
                spherePositions[i] = new Vector4(pos.x, pos.y, pos.z, 1f);
                sphereRadii[i] = activeSpheres[i].Radius;
                sphereFrequencies[i] = (float)activeSpheres[i].Frequency;
                sphereIntensities[i] = activeSpheres[i].CurrentIntensity;
            }

            Shader.SetGlobalVectorArray(spherePositionsID, spherePositions);
            Shader.SetGlobalFloatArray(sphereRadiiID, sphereRadii);
            Shader.SetGlobalFloatArray(sphereFrequenciesID, sphereFrequencies);
            Shader.SetGlobalFloatArray(sphereIntensitiesID, sphereIntensities);
            Shader.SetGlobalInt(sphereCountID, count);
        }


        public void AddAudioSphere(SoundEmissionData data, float fadeIn, float fadeOut)
        {
            var newSphere = new AudioSphere(
                data.Position,
                data.Radius,
                data.SoundClass.Frequency,
                data.Intensity,
                data.Duration,
                fadeIn + data.Duration + fadeOut,
                fadeIn,
                fadeOut
            );
            activeSpheres.Add(newSphere);
        }

        public void AddAudioSphereHandler(IEventSender sender, SoundEmissionData data)
        {
            var newSphere = new AudioSphere(
                data.Position,
                data.Radius,
                data.SoundClass.Frequency,
                data.Intensity,
                data.Duration,
                fadeInDuration + data.Duration + fadeOutDuration,
                fadeInDuration,
                fadeOutDuration
            );
            activeSpheres.Add(newSphere);
        }


        public void SetMaterialHandler(IEventSender sender, EchoMaterialCodeEnum code)
        {
            switch (code)
            {
                case EchoMaterialCodeEnum.Active:
                    SetEcholocationActive(true);
                    Log.DLazy(() => "Switched to Echolocation Shader", this);
                    echoMaterialUpdatedEvent?.RaiseEvent(this, true);
                    break;
                case EchoMaterialCodeEnum.Inactive:
                    SetEcholocationActive(false);
                    Log.DLazy(() => "Restored Original Shader", this);
                    echoMaterialUpdatedEvent?.RaiseEvent(this, false);
                    break;
                case EchoMaterialCodeEnum.ReApply:
                    SetEcholocationActive(false);
                    SetEcholocationActive(true);
                    Log.DLazy(() => "Reapplied Echolocation Shader", this);
                    break;
                case EchoMaterialCodeEnum.Toggle:
                    if (_isEcholocationActive)
                    {
                        SetEcholocationActive(false);
                        Log.DLazy(() => "Toggle: Set URP Lit Shader", this);
                        echoMaterialUpdatedEvent?.RaiseEvent(this, false);
                    }
                    else
                    {
                        SetEcholocationActive(true);
                        Log.DLazy(() => "Toggle: Set AudioShader", this);
                        echoMaterialUpdatedEvent?.RaiseEvent(this, true);
                    }
                    break;
                default:
                    Log.ELazy(() => "SetMaterialHandler received wrong code", this);
                    break;
            }
        }

        public void SetEcholocationActive(bool active)
        {
            if (_isEcholocationActive == active) return;
            _isEcholocationActive = active;

            Shader.SetGlobalInt(GlobalToggleProp, active ? 1 : 0);
            if (active)
                Shader.EnableKeyword(KeywordOn);
            else
                Shader.DisableKeyword(KeywordOn);

            foreach (var r in FindObjectsByType<Renderer>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            ))
            {
                r.GetPropertyBlock(_mpb);

                _mpb.SetFloat(GlobalToggleProp, active ? 1f : 0f);
                r.SetPropertyBlock(_mpb);
            }
        }

    }
}
