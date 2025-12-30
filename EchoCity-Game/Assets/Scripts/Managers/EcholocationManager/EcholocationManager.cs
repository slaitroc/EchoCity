using System.Collections.Generic;
using UnityEngine;

namespace EchoCity
{
    public enum VisualizationMode
    {
        SolidColor = 0,
        GridLines = 1,
        GridPoints = 2
    }

    public class EcholocationManager : MonoBehaviour
    {
        private const int MAX_AUDIO_SPHERES = 64;

        [SerializeField] private bool useEcholocationMaterial = true;
        [SerializeField] private Material echolocationMaterial;

        [SerializeField] private VisualizationMode visualizationMode = VisualizationMode.GridLines;

        [Range(0.01f, 1.0f)][SerializeField] private float fadeInDuration = 0.1f;

        [Range(0.1f, 3.0f)][SerializeField] private float fadeOutDuration = 0.5f;

        [Header("Grid Settings")]
        [Range(0.1f, 5.0f)]
        [SerializeField]
        private float gridCellSize = 1.0f;

        [Range(0.01f, 0.2f)][SerializeField] private float gridWidth = 0.05f;

        [Range(0.01f, 0.3f)][SerializeField] private float gridPointSize = 0.08f;

        [SerializeField] private List<AudioSphere> activeSpheres = new();
        private readonly float[] sphereFrequencies = new float[MAX_AUDIO_SPHERES];
        private readonly float[] sphereIntensities = new float[MAX_AUDIO_SPHERES];

        private readonly Vector4[] spherePositions = new Vector4[MAX_AUDIO_SPHERES];
        private readonly float[] sphereRadii = new float[MAX_AUDIO_SPHERES];
        private int gridSizeID;
        private int lineWidthID;
        private int pointSizeID;
        private int sphereCountID;
        private int sphereFrequenciesID;
        private int sphereIntensitiesID;

        private int spherePositionsID;
        private int sphereRadiiID;
        private int visualizationModeID;

        private void Awake()
        {
            spherePositionsID = Shader.PropertyToID("_AudioSpherePositions");
            sphereRadiiID = Shader.PropertyToID("_AudioSphereRadii");
            sphereFrequenciesID = Shader.PropertyToID("_AudioSphereFrequencies");
            sphereIntensitiesID = Shader.PropertyToID("_AudioSphereIntensities");
            sphereCountID = Shader.PropertyToID("_AudioSphereCount");
            gridSizeID = Shader.PropertyToID("_GridSize");
            lineWidthID = Shader.PropertyToID("_LineWidth");
            pointSizeID = Shader.PropertyToID("_PointSize");
            visualizationModeID = Shader.PropertyToID("_VisualizationMode");
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                if (useEcholocationMaterial && echolocationMaterial != null)
                {
                    MaterialSwitcher.ApplyOverrideMaterial(echolocationMaterial);
                    Log.DLazy(() => "Echolocation Material Applied", this);
                }
                else
                {
                    MaterialSwitcher.RestoreOriginalMaterials();
                    Log.DLazy(() => "Restored Original Materials", this);
                }
            }
        }

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
            Shader.SetGlobalFloat(gridSizeID, gridCellSize);
            Shader.SetGlobalFloat(lineWidthID, gridWidth);
            Shader.SetGlobalInt(visualizationModeID, (int)visualizationMode);
            Shader.SetGlobalFloat(pointSizeID, gridPointSize);
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

        public void AddAudioSphereHandler(SoundEmissionData data)
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

        public void MaterialSwitcherHandler()
        {
            useEcholocationMaterial = !useEcholocationMaterial;

            if (useEcholocationMaterial && echolocationMaterial != null)
            {
                MaterialSwitcher.ApplyOverrideMaterial(echolocationMaterial);
                Log.DLazy(() => "Switched to Echolocation Material", this);
            }
            else
            {
                MaterialSwitcher.RestoreOriginalMaterials();
                Log.DLazy(() => "Restored Original Materials", this);
            }
        }
    }
}
