using UnityEngine;

namespace EchoCity
{
    [RequireComponent(typeof(AudioEmitter))]
    public class GeneratorInteractable : PlainInteractable
    {
        [Header("Quest")]
        [SerializeField] private SOQuest triggeredQuestIfFail;

        private AudioEmitter audioEmitter;

        public override InteractionEnum InteractionCode => InteractionEnum.Generator;

        protected override void Awake()
        {
            base.Awake();
            TryGetComponent(out audioEmitter);
        }

        protected override void ResolveInteraction(bool outcome)
        {
            if (outcome)
            {
                audioEmitter?.EmitSound();
            }
            else
            {
                PuzzleManager?.AddToTagsTriggeredQuests(triggeredQuestIfFail);
            }
        }
    }
}
