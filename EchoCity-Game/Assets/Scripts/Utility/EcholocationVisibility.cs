using UnityEngine;

namespace EchoCity
{
    public static class EcholocationVisibility
    {
        public static bool IsRevealedByAudio(RaycastHit hit)
        {
            var collider = hit.collider;
            if (collider == null) return false;

            var renderer = collider.GetComponent<Renderer>()
                           ?? collider.GetComponentInChildren<Renderer>()
                           ?? collider.GetComponentInParent<Renderer>();

            if (renderer == null || renderer.sharedMaterial == null)
                return true;

            var shader = renderer.sharedMaterial.shader;
            if (shader == null)
                return true;

            if (Shader.GetGlobalFloat("_Echolocation") < 0.5f)
                return true;

            int objectFrequency = GetObjectFrequency(renderer);
            return CheckAudioSphereProximity(hit.point, objectFrequency);
        }

        private static int GetObjectFrequency(Renderer renderer)
        {
            if (renderer == null || renderer.sharedMaterial == null)
                return 0;

            var shader = renderer.sharedMaterial.shader;
            int idx = shader.FindPropertyIndex("_ObjectFrequency");
            if (idx < 0)
                return 0;

            var propType = shader.GetPropertyType(idx);
            if (propType != UnityEngine.Rendering.ShaderPropertyType.Float &&
                propType != UnityEngine.Rendering.ShaderPropertyType.Range)
                return 0;

            var mpb = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(mpb);
            bool hasBlock = !mpb.isEmpty;

            float f = hasBlock ? mpb.GetFloat("_ObjectFrequency") : renderer.sharedMaterial.GetFloat("_ObjectFrequency");
            return Mathf.RoundToInt(f);
        }

        private static bool CheckAudioSphereProximity(Vector3 position, int objectFrequency)
        {
            var positions = Shader.GetGlobalVectorArray("_AudioSpherePositions");
            var radii = Shader.GetGlobalFloatArray("_AudioSphereRadii");
            var frequencies = Shader.GetGlobalFloatArray("_AudioSphereFrequencies");
            int count = Shader.GetGlobalInt("_AudioSphereCount");

            if (positions == null || radii == null || frequencies == null || count <= 0)
                return false;

            int maxCount = Mathf.Min(count, positions.Length);
            for (int i = 0; i < maxCount; i++)
            {
                if (i >= radii.Length || i >= frequencies.Length)
                    break;

                float radius = radii[i];
                if (radius <= 0f)
                    continue;

                float sphereFreq = frequencies[i];
                if (sphereFreq < objectFrequency)
                    continue;

                Vector3 pos = positions[i];
                float distance = Vector3.Distance(position, pos);
                if (distance <= radius)
                    return true;
            }

            return false;
        }
    }
}
