using UnityEngine;

namespace EchoCity
{
    [RequireComponent(typeof(AudioEmitter))]
    public class GeneratorInteractable : PlainInteractable
    {
        [Header("Invoking Events")]
        [SerializeField] private SOShowUIEvent showUIEvent;

        [Header("Messages")]
        [SerializeField] private SODialogContainer dialogContainerSuccess;
        [SerializeField] private SODialogContainer dialogContainerFail;

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
                showUIEvent?.RaiseEvent(this, ShowableUIEnum.Warning, new WarningParams("Generator started!", new Color(1f, 0.5f, 0f, 1f)));
                showUIEvent?.RaiseEvent(this, ShowableUIEnum.Dialog, new DialogParams(new DialogData(dialogContainerSuccess)));
                audioEmitter?.EmitSound();
            }
            else
            {
                showUIEvent?.RaiseEvent(this, ShowableUIEnum.Warning, new WarningParams("Can not start generator without cable!", new Color(1f, 0.5f, 0f, 1f)));
                showUIEvent?.RaiseEvent(this, ShowableUIEnum.Dialog, new DialogParams(new DialogData(dialogContainerFail)));
            }
        }
    }
}
