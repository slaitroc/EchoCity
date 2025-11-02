using UnityEngine;

public class AudioEcholocator : MonoBehaviour
{
    [Header("Echo Settings")]
    public float echoRadius = 10f;

    [Tooltip("How often to automatically emit sound (0 = no auto-emit)")]
    public float autoEmitInterval = 0f;

    public float echoIntensity = 0.5f;

    public bool echoOnStart = false;

    [Header("Audio Settings")]
    public AudioClip audioClip;

    [Tooltip("Overrides audioClip")]
    public AudioClip[] randomAudioClips;

    [Tooltip("Minimum visibility duration, use if audio is very short or null")]
    public float minimumDuration = 0.1f;

    private AudioSource audioSource;
    private float nextAutoEmit;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;

        if (echoOnStart)
        {
            EmitEcho();
        }

        if (autoEmitInterval > 0f)
        {
            nextAutoEmit = Time.time + autoEmitInterval;
        }
    }

    void Update()
    {
        if (autoEmitInterval > 0f && Time.time >= nextAutoEmit)
        {
            EmitEcho();
            nextAutoEmit = Time.time + autoEmitInterval;
        }
    }

    public void EmitEcho()
    {
        AudioClip clipToPlay = GetAudioClip();

        if (audioSource != null && clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay);
        }

        float audioDuration = clipToPlay != null ? clipToPlay.length : minimumDuration;
        AudioVisibilityManager.Instance.AddAudioSphere(
            transform.position,
            echoRadius,
            echoIntensity,
            audioDuration
        );
    }

    AudioClip GetAudioClip()
    {
        if (randomAudioClips != null && randomAudioClips.Length > 0)
        {
            var validClips = System.Array.FindAll(randomAudioClips, clip => clip != null);
            if (validClips.Length > 0)
            {
                return validClips[Random.Range(0, validClips.Length)];
            }
        }

        return audioClip;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, echoRadius);
    }
}
