using System.Collections.Generic;
using UnityEngine;

public enum VisualizationMode
{
    SolidColor = 0,
    GridLines = 1,
    GridPoints = 2
}

public class AudioVisibilityManager : MonoBehaviour
{
    public VisualizationMode visualizationMode = VisualizationMode.GridLines;

    [Range(0.01f, 1.0f)]
    public float fadeInDuration = 0.1f;

    [Range(0.1f, 3.0f)]
    public float fadeOutDuration = 0.5f;

    [Header("Grid Settings")]
    [Range(0.1f, 5.0f)]
    public float gridCellSize = 1.0f;

    [Range(0.01f, 0.2f)]
    public float gridWidth = 0.05f;

    [Range(0.01f, 0.3f)]
    public float gridPointSize = 0.08f;

    private List<AudioSphere> activeSpheres = new List<AudioSphere>();
    private static AudioVisibilityManager instance;

    private int spherePositionsID;
    private int sphereRadiiID;
    private int sphereIntensitiesID;
    private int sphereCountID;
    private int gridSizeID;
    private int lineWidthID;
    private int pointSizeID;
    private int visualizationModeID;

    private Vector4[] spherePositions = new Vector4[16];
    private float[] sphereRadii = new float[16];
    private float[] sphereIntensities = new float[16];

    public static AudioVisibilityManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<AudioVisibilityManager>();
                if (instance == null)
                {
                    GameObject manager = new GameObject("AudioVisibilityManager");
                    instance = manager.AddComponent<AudioVisibilityManager>();
                }
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            spherePositionsID = Shader.PropertyToID("_AudioSpherePositions");
            sphereRadiiID = Shader.PropertyToID("_AudioSphereRadii");
            sphereIntensitiesID = Shader.PropertyToID("_AudioSphereIntensities");
            sphereCountID = Shader.PropertyToID("_AudioSphereCount");
            gridSizeID = Shader.PropertyToID("_GridSize");
            lineWidthID = Shader.PropertyToID("_LineWidth");
            pointSizeID = Shader.PropertyToID("_PointSize");
            visualizationModeID = Shader.PropertyToID("_VisualizationMode");
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Update all active spheres
        for (int i = activeSpheres.Count - 1; i >= 0; i--)
        {
            AudioSphere sphere = activeSpheres[i];
            sphere.timeRemaining -= Time.deltaTime;

            // Update based on fade in/out
            sphere.currentIntensity = sphere.GetCurrentIntensity();

            if (sphere.IsExpired)
            {
                activeSpheres.RemoveAt(i);
            }
            else
            {
                activeSpheres[i] = sphere;
            }
        }

        // Clear arrays
        for (int i = 0; i < spherePositions.Length; i++)
        {
            spherePositions[i] = Vector4.zero;
            sphereRadii[i] = 0f;
            sphereIntensities[i] = 0f;
        }

        // Fill arrays with active spheres
        int count = Mathf.Min(activeSpheres.Count, spherePositions.Length);
        for (int i = 0; i < count; i++)
        {
            spherePositions[i] = new Vector4(activeSpheres[i].position.x, activeSpheres[i].position.y, activeSpheres[i].position.z, 1f);
            sphereRadii[i] = activeSpheres[i].radius;
            sphereIntensities[i] = activeSpheres[i].currentIntensity;
        }

        Shader.SetGlobalVectorArray(spherePositionsID, spherePositions);
        Shader.SetGlobalFloatArray(sphereRadiiID, sphereRadii);
        Shader.SetGlobalFloatArray(sphereIntensitiesID, sphereIntensities);
        Shader.SetGlobalInt(sphereCountID, count);
        Shader.SetGlobalFloat(gridSizeID, gridCellSize);
        Shader.SetGlobalFloat(lineWidthID, gridWidth);
        Shader.SetGlobalInt(visualizationModeID, (int)visualizationMode);
        Shader.SetGlobalFloat(pointSizeID, gridPointSize);
    }

    public void AddAudioSphere(Vector3 position, float radius, float intensity, float audioClipDuration)
    {
        AudioSphere newSphere = new AudioSphere(
            position,
            radius,
            intensity,
            audioClipDuration,
            this.fadeInDuration + audioClipDuration + this.fadeOutDuration,
            this.fadeInDuration,
            this.fadeOutDuration
        );
        activeSpheres.Add(newSphere);
    }

    public void AddAudioSphere(Vector3 position, float radius, float intensity, float audioClipDuration, float fadeIn, float fadeOut)
    {
        AudioSphere newSphere = new AudioSphere(
            position,
            radius,
            intensity,
            audioClipDuration,
            fadeIn + audioClipDuration + fadeOut,
            fadeIn,
            fadeOut
        );
        activeSpheres.Add(newSphere);
    }

}
