using EchoCity;
using UnityEngine;
using static EchoCity.EchoCitySound;

public class SoundToggleTrigger : Trigger
{
    [SerializeField] private int _audioCount = 2;
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private Transform[] _audioPositions;
    [SerializeField] private GameObject[] _toggleObjects;
    [SerializeField] private SOSetMaterialEvent setMaterialEvent;

    protected override void ResolveInteraction(bool outcome)
    {
        base.ResolveInteraction(outcome);
        setMaterialEvent?.RaiseEvent(this, EchoMaterialCodeEnum.Active);
        PlayAudiosAtPositions();

        if (_toggleObjects == null) return;
        foreach (var obj in _toggleObjects)
            if (obj != null)
                obj.SetActive(false);
    }

    private void PlayAudiosAtPositions()
    {
        for (int i = 0; i < _audioCount; i++)
        {
            if (audioClips[i] != null && _audioPositions[i] != null)
            {
                PlayAtPosition(audioClips[i], _audioPositions[i].position, 1f, MixerGroupEnum.SFX);
            }
        }
    }

    private void OnValidate()
    {
        if (_audioCount < 1) _audioCount = 1;
        if (audioClips.Length != _audioCount)
        {
            AudioClip[] newArray = new AudioClip[_audioCount];
            for (int i = 0; i < Mathf.Min(audioClips.Length, _audioCount); i++)
            {
                newArray[i] = audioClips[i];
            }
            audioClips = newArray;
        }
        if (_audioPositions.Length != _audioCount)
        {
            Transform[] newArray = new Transform[_audioCount];
            for (int i = 0; i < Mathf.Min(_audioPositions.Length, _audioCount); i++)
            {
                newArray[i] = _audioPositions[i];
            }
            _audioPositions = newArray;
        }

    }
}
