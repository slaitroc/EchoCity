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
        if (outcome)
        {
            base.ResolveInteraction(outcome);
        }
        else
        {
            if (lockedSound != null)
                PlayAtPosition(transform.position, lockedSound, _audioContext, MixerGroupEnum.SFX);
            PuzzleManager.AddToTagsTriggeredQuests(triggeredQuestIfFail);
            PuzzleManager.CheckQuests();
        }
    }

}