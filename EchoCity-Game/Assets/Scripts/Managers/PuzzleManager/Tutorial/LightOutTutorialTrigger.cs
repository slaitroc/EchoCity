using EchoCity;
using UnityEngine;

public class LightOutTutorialTrigger : TutorialTrigger
{
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private Transform[] _audioPositions;
    [SerializeField] private GameObject[] _lightsToTurnOff;
    [SerializeField] private SOEventVoid wearEcholocatorEvent;

    protected override void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        foreach (var light in _lightsToTurnOff)
        {
            if (light != null)
            {
                light.SetActive(false);
            }
        }
        checkTagsEvent?.RaiseEvent(this, checkTags);
        wearEcholocatorEvent?.RaiseEvent(this);
        EchoCitySound.PlayAtPosition(audioClips[0], _audioPositions[0].position, 1f, EchoCitySound.MixerGroupEnum.SFX);
        EchoCitySound.PlayAtPosition(audioClips[1], _audioPositions[1].position, 1f, EchoCitySound.MixerGroupEnum.SFX);
        switchToNarrationStateEvent?.RaiseEvent(this, new DialogData(tutorialDialogContainer));
        gameObject.SetActive(false);
    }
}
