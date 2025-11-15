using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

[RequireComponent(typeof(AudioSource))]
public class AudioEmitter : MonoBehaviour
{
    [Header("Invoking Events")]
    [SerializeField] SONewAudioSphereEvent newAudioSphereEvent;

    [Header("Echo Settings")]
    [Min(0f)]
    [SerializeField] private float echoRadius = 10f;

    [Tooltip("How often to automatically emit sound (0 = no auto-emit)")]
    [Min(0f)]
    [SerializeField] private float autoEmitInterval = 0f;

    [Min(0f)]
    [SerializeField] private float echoIntensity = 0.5f;

    [SerializeField] private bool emitOnStart = false;

    [Header("Audio Settings")]
    public AudioClip audioClip;

    [Tooltip("Overrides audioClip")]
    [SerializeField] private AudioClip[] randomAudioClips;
    private List<AudioClip> _validClips = new();

    [Tooltip("Minimum visibility duration, use if audio is very short or null")]

    private float minimumDuration = 0.1f;

    private AudioSource audioSource;
    private float nextAutoEmit;

    void Awake() => RebuildValidClips();

    void Start()
    {
        TryGetComponent(out audioSource);
        audioSource.playOnAwake = false;

        if (emitOnStart)
        {
            EmitSound();
        }

        if (autoEmitInterval > 0f)
        {
            nextAutoEmit = Time.time + autoEmitInterval;
        }
    }

    void Update()
    {
        if (autoEmitInterval > 0f && Time.time >= nextAutoEmit) //FIX
        {
            EmitSound();
            nextAutoEmit = Time.time + autoEmitInterval;
        }
    }

    public void EmitSound()
    {
        AudioClip clipToPlay = GetAudioClip();
        if (clipToPlay != null) audioSource.PlayOneShot(clipToPlay);
        float audioDuration = clipToPlay != null ? clipToPlay.length : minimumDuration;
        newAudioSphereEvent?.RaiseEvent(new SoundEmissionData(transform.position, echoRadius, echoIntensity, audioDuration));
    }
    private AudioClip GetAudioClip()
    {
        if (_validClips.Count > 0) return _validClips[Random.Range(0, _validClips.Count)];
        return audioClip;
    }

    private void RebuildValidClips()
    {
        _validClips.Clear();
        if (randomAudioClips != null)
            foreach (var sound in randomAudioClips)
            {
                if (sound) _validClips.Add(sound);
            }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, echoRadius);
    }
}
