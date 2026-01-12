using UnityEngine;

public class ObjectFrequencySetter : MonoBehaviour
{
    private static readonly int ObjectFrequency = Shader.PropertyToID("_ObjectFrequency");
    [SerializeField] public int objectFrequency;

    void Start()
    {
        var objectRenderer = GetComponent<Renderer>();
        var mp = new MaterialPropertyBlock();
        if (objectRenderer == null) return;
        objectRenderer.GetPropertyBlock(mp);

        mp.SetFloat(ObjectFrequency, objectFrequency);

        objectRenderer.SetPropertyBlock(mp);
    }
}
