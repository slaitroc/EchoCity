using UnityEngine;

public class ObjectFrequencySetter : MonoBehaviour
{
    private static readonly int ObjectFrequency = Shader.PropertyToID("_ObjectFrequency");
    [SerializeField] public int objectFrequency;

    void Start()
    {
        var objectRenderer = GetComponent<Renderer>();
        var mp = new MaterialPropertyBlock();
        objectRenderer.GetPropertyBlock(mp);

        mp.SetFloat(ObjectFrequency, objectFrequency);

        objectRenderer.SetPropertyBlock(mp);
    }
}
