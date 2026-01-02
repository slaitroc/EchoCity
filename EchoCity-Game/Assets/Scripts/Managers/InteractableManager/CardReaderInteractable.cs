using UnityEngine;
using static EchoCity.EchoCitySound;

namespace EchoCity
{
    public class CardReaderInteractable : PlainInteractable
    {
        [Header("Invoking Events")]
        [SerializeField] protected SOEventVoid materialToggleEvent;
        [Header("Sound")]
        [SerializeField] SOSoundSource useSoundSource;

        [Header("Prefabs")]
        [SerializeField] GameObject cardPrefab;
        [SerializeField] Transform cardSpawnPoint;

        [Header("Messages")]
        [SerializeField] SODialogContainer dialogContainerSuccess;
        [SerializeField] SODialogContainer dialogContainerFail;
        [SerializeField] SODialogDataEvent dialogDataEvent;
        [SerializeField] SOStringColorEvent spawnMessageEvent;

        protected override void ResolveInteraction(bool outcome)
        {
            if (outcome)
            {
                spawnMessageEvent?.RaiseEvent(this, "CardReader Used!", new Color(1f, 0.5f, 0f, 1f));
                PlayAtPosition(transform.position, useSoundSource, _audioContext, MixerGroupEnum.SFX);
                Instantiate(cardPrefab, cardSpawnPoint.position, cardSpawnPoint.rotation);
                materialToggleEvent?.RaiseEvent(this);
                materialToggleEvent?.RaiseEvent(this);
                dialogDataEvent?.RaiseEvent(this, new DialogData(dialogContainerSuccess));
            }
            else
            {
                spawnMessageEvent?.RaiseEvent(this, "CardReader can not be used!", new Color(1f, 0.5f, 0f, 1f));
                dialogDataEvent?.RaiseEvent(this, new DialogData(dialogContainerFail));
            }
        }
    }
}
