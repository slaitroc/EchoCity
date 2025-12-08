using UnityEngine;

namespace EchoCity
{
    public class CardReaderInteractable : Interactable
    {
        #region Constants
        protected override string _LOG_TAG => "CARD_READER";
        protected override string _TYPE_LOG_TAG => "GENERAL";
        #endregion

        [Header("Sound")]
        [SerializeField] SOSoundEmissionDataEvent newAudioSphereEvent;
        [SerializeField] SOSoundSource useSoundSource;

        [Header("Prefabs")]
        [SerializeField] GameObject cardPrefab;
        [SerializeField] Transform cardSpawnPoint;

        [Header("Messages")]
        [SerializeField] SODialogContainer dialogContainerSuccess;
        [SerializeField] SODialogContainer dialogContainerFail;
        [SerializeField] SODialogDataEvent dialogDataEvent;
        [SerializeField] SOStringColorEvent spawnMessageEvent;

        public override void InteractionOutcomeHandler(bool outcome)
        {
            if (_waitForInteractionOutcome)
            {
                if (outcome)
                {
                    spawnMessageEvent?.RaiseEvent("CardReader Used!", new Color(1f, 0.5f, 0f, 1f));
                    ECSound.PlayAtPosition(useSoundSource, transform.position, newAudioSphereEvent, "SFX");
                    Instantiate(cardPrefab, cardSpawnPoint.position, cardSpawnPoint.rotation);
                    materialToggleEvent?.RaiseEvent();
                    materialToggleEvent?.RaiseEvent();
                    dialogDataEvent?.RaiseEvent(new DialogData(dialogContainerSuccess));

                }
                else
                {
                    spawnMessageEvent?.RaiseEvent("CardReader can not be used!", new Color(1f, 0.5f, 0f, 1f));
                    dialogDataEvent?.RaiseEvent(new DialogData(dialogContainerFail));
                }
            }
            _waitForInteractionOutcome = false;
        }
    }
}
