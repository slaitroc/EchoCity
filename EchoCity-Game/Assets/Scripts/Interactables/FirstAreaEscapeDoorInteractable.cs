using EchoCity;
using UnityEngine;
using static EchoCity.EchoCitySound;

public class FirstAreaEscapeDoorInteractable : DoorInteractable
{
    [SerializeField] private SOSoundSource lockedSound;

    public override InteractionEnum InteractionCode => InteractionEnum.FirstAreaEscapeDoor;

    protected override void ResolveInteraction(bool outcome)
    {
        base.ResolveInteraction(outcome);
        if (!outcome)
            if (lockedSound != null)
                PlayAtPosition(transform.position, lockedSound, _audioContext, MixerGroupEnum.SFX);
    }

    protected override void OnFailInteractionLine(bool outcome)
    {
        if (!outcome)
        {
            if (PuzzleManager.CheckTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.PryTool_Picked, false) }))
            {
                base.OnFailInteractionLine(outcome);
            }
        }
    }

}