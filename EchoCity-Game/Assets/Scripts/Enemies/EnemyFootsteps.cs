using EchoCity;
using UnityEngine;
using static EchoCity.EchoCitySound;

public class EnemyFootsteps : MonoBehaviour, IEventSender
{
    [System.Serializable]
    public class GroundFootstep
    {
        public LayerMask layer;
        public SOSoundSource SoundSource;
    }

    [Header("Invoking Events")]
    public SOSoundEmittedEvent soundEmittedEvent;
    string IEventSender.SenderName => gameObject.name;
    int IEventSender.SenderID => GetInstanceID();
    bool IEventSender.IsManager => false;
    EventSenderCategoriesEnum[] IEventSender.SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.Enemy };
    private AudioContext _audioContext;

    [Header("Footstep Settings")]
    public GroundFootstep[] groundTypes;
    public float rayDistance = 1.5f;


    private void Start()
    {
        _audioContext = new AudioContext(this, soundEmittedEvent);
    }

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
                    PlayRandomAtPosition(transform.position, soundSource, _audioContext, MixerGroupEnum.SFX);
                    return;
                }
            }
        }
    }
}
