using UnityEngine;

namespace EchoCity
{
    [RequireComponent(typeof(AudioEmitter))]
    public class GeneratorInteractable : PlainInteractable
    {
        [Header("Quest")]
        [SerializeField] private SOQuest triggeredQuestIfFail;

        private AudioEmitter audioEmitter;

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
                puzzleManager?.AddToTagsTriggeredQuests(triggeredQuestIfFail);
            }
        }
    }
}
