using EchoCity;
using UnityEngine;
using static EchoCity.EchoCitySound;

public class ExitLabAreaDoorInteractable : DoorInteractable
{
    [SerializeField] private SOSoundSource lockedSound;
    [SerializeField] private SOQuest triggeredQuestIfFail;

    public override InteractionEnum InteractionCode => InteractionEnum.LaboratoryAreaDoor;

    protected override void ResolveInteraction(bool outcome)
    {
        base.ResolveInteraction(outcome);
        if (!outcome)
            if (lockedSound != null)
                PlayAtPosition(transform.position, lockedSound, _audioContext, MixerGroupEnum.SFX);
    }
}