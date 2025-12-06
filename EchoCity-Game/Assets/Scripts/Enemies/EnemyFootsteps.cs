using EchoCity;
using UnityEngine;

public class EnemyFootsteps : MonoBehaviour
{
    [System.Serializable]
    public class GroundFootstep
    {
        public LayerMask layer;
        public SOSoundSource SoundSource;
    }

    [Header("Invoking Events")]
    public SOSoundEmissionDataEvent newAudioSphereEvent;

    [Header("Footstep Settings")]
    public GroundFootstep[] groundTypes;
    public float rayDistance = 1.5f;

    private void PlayFootstepSound()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance))
        {
            int hitLayer = hit.collider.gameObject.layer;

            for (int i = 0; i < groundTypes.Length; i++)
            {
                if ((groundTypes[i].layer.value & (1 << hitLayer)) != 0)
                {
                    SOSoundSource soundSource = groundTypes[i].SoundSource;
                    ECSound.PlayRandomAtPosition(soundSource, hit.point, newAudioSphereEvent, "SFX");
                    return;
                }
            }
        }
    }
}
