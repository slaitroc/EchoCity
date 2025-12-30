using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

namespace EchoCity
{
    public class AudioPooler : MonoBehaviour
    {
        [SerializeField] private GameObject inactiveObjsParent;
        [SerializeField] private GameObject activeObjsParent;
        [SerializeField] private int initCapacity = 5;
        [SerializeField] private int maxCapacity = 15;

        private static IObjectPool<GameObject> _audioPool;
        private static GameObject _inactiveObjsParentStatic;
        private static GameObject _activeObjsParentStatic;

        void Awake()
        {
            _inactiveObjsParentStatic = inactiveObjsParent;
            _activeObjsParentStatic = activeObjsParent;
            _audioPool = new ObjectPool<GameObject>(
                createFunc: CreateAudioObject,
                actionOnGet: OnGetAudioObject,
                actionOnRelease: OnReleaseAudioObject,
                actionOnDestroy: OnDestroyAudioObject,
                collectionCheck: false,
                defaultCapacity: initCapacity,
                maxSize: maxCapacity
            );
        }

        private GameObject CreateAudioObject()
        {
            GameObject audioObj = new GameObject("PooledAudioSource");
            audioObj.transform.SetParent(_inactiveObjsParentStatic.transform);

            AudioSource audioSource = audioObj.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = EchoCitySound.GetMixerGroup(EchoCitySound.MixerGroupEnum.Master);
            audioSource.spatialBlend = 1.0f; // Default to 3D sound

            audioObj.AddComponent<PooledObject>().SetPool(_audioPool);
            audioObj.SetActive(false);
            return audioObj;
        }

        private void OnGetAudioObject(GameObject audioObj)
        {
            //only universal resets here
        }

        private void OnReleaseAudioObject(GameObject audioObj)
        {
            audioObj.transform.SetParent(_inactiveObjsParentStatic.transform);
            audioObj.SetActive(false);
        }

        private void OnDestroyAudioObject(GameObject audioObj)
        {
            Destroy(audioObj);
        }

        public static void PlayPooledAudio(Vector3 pos, AudioClip clip, float volume, AudioMixerGroup mixerGroup, bool is3D = true)
        {
            if (clip == null)
                return;
            GameObject pooled = _audioPool.Get();
            pooled.transform.position = pos;
            pooled.transform.SetParent(_activeObjsParentStatic.transform);

            AudioSource audioSource = pooled.GetComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.spatialBlend = is3D ? 1.0f : 0.0f; // 3D or 2D sound based on is3D parameter
            audioSource.outputAudioMixerGroup = mixerGroup;

            pooled.SetActive(true);
            audioSource.Play();

            if (pooled.TryGetComponent<PooledObject>(out var pooledObjComponent))
                pooledObjComponent.ReturnToPool(clip.length);
        }
    }
}