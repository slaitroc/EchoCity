using EchoCity;
using UnityEngine;
using static EchoCity.EchoCitySound;

public class LightOutTutorialTrigger : TutorialTrigger
{
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private Transform[] _audioPositions;
    [SerializeField] private GameObject[] _lightsToTurnOff;
    [SerializeField] private SOEventVoid wearEcholocatorEvent;

    protected override void ResolveInteraction(bool outcome)
    {
        foreach (var light in _lightsToTurnOff)
        {
            if (light != null)
            {
                light.SetActive(false);
            }
        }
        wearEcholocatorEvent?.RaiseEvent();
        PlayAtPosition(audioClips[0], _audioPositions[0].position, 1f, MixerGroupEnum.SFX);
        PlayAtPosition(audioClips[1], _audioPositions[1].position, 1f, MixerGroupEnum.SFX);
        base.ResolveInteraction(outcome);
    }
}
