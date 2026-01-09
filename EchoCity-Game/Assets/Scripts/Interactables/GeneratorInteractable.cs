using UnityEngine;

namespace EchoCity
{
    [RequireComponent(typeof(AudioEmitter))]
    public class GeneratorInteractable : PlainInteractable
    {
        [Header("Invoking Events")]
        [SerializeField] private SOShowUIEvent showUIEvent;
        [SerializeField] protected SOInteractionEvent interactionEvent;

        [Header("Messages")]
        [SerializeField] private SODialogContainer dialogContainerSuccess;
        [SerializeField] private SODialogContainer dialogContainerFail;

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
            //TODO add quest
        }
    }
}
