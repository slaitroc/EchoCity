using UnityEngine;
using static EchoCity.EchoCitySound;

namespace EchoCity
{
    public class CardReaderInteractable : PlainInteractable
    {
        [Header("Invoking Events")]
        [SerializeField] protected SOSetMaterialEvent materialSetEvent;
        [SerializeField] private SOShowUIEvent showUIEvent;
        [Header("Sound")]
        [SerializeField] private SOSoundSource useSoundSource;

        [Header("Prefabs")]
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private Transform cardSpawnPoint;
        [Header("Messages")]
        [SerializeField] private SODialogContainer dialogContainerSuccess;
        [SerializeField] private SODialogContainer dialogContainerFail;

        protected override void ResolveInteraction(bool outcome)
        {
            if (outcome)
            {
                showUIEvent?.RaiseEvent(this, ShowableUIEnum.Warning, new WarningParams("CardReader Used!", new Color(1f, 0.5f, 0f, 1f)));
                PlayAtPosition(transform.position, useSoundSource, _audioContext, MixerGroupEnum.SFX);
                Instantiate(cardPrefab, cardSpawnPoint.position, cardSpawnPoint.rotation);
                materialSetEvent?.RaiseEvent(this, false);
                materialSetEvent?.RaiseEvent(this, true);
                showUIEvent?.RaiseEvent(this, ShowableUIEnum.Dialog, new DialogParams(new DialogData(dialogContainerSuccess)));
            }
            else
            {
                showUIEvent?.RaiseEvent(this, ShowableUIEnum.Warning, new WarningParams("CardReader can not be used!", new Color(1f, 0.5f, 0f, 1f)));
                showUIEvent?.RaiseEvent(this, ShowableUIEnum.Dialog, new DialogParams(new DialogData(dialogContainerFail)));
            }
        }
    }
}
