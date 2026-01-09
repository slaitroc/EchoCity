using UnityEngine;
using static EchoCity.EchoCitySound;

namespace EchoCity
{
    public class CardReaderInteractable : PlainInteractable
    {
        [Header("Invoking Events")]
        [SerializeField] protected SOSetMaterialEvent setMaterialEvent;
        [SerializeField] private SOShowUIEvent showUIEvent;
        [Header("Sound")]
        [SerializeField] private SOSoundSource useSoundSource;

        [Header("Prefabs")]
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private Transform cardSpawnPoint;
        [Header("Messages")]
        [SerializeField] private SODialogContainer dialogContainerSuccess;
        [SerializeField] private SODialogContainer dialogContainerFail;

        [Header("Quest")]
        [SerializeField] private SOQuest triggeredQuestIfFail;

        protected override void ResolveInteraction(bool outcome)
        {
            if (outcome)
            {
                showUIEvent?.RaiseEvent(this, ShowableUIEnum.PopUpMessage, new PopUpMessageParams("CardReader Used!", new Color(1f, 0.5f, 0f, 1f)));
                PlayAtPosition(transform.position, useSoundSource, _audioContext, MixerGroupEnum.SFX);
                Instantiate(cardPrefab, cardSpawnPoint.position, cardSpawnPoint.rotation);
                setMaterialEvent?.RaiseEvent(this, EchoMaterialCodeEnum.ReApply);
            }
        }
    }
}
