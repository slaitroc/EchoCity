using UnityEngine;

namespace EchoCity
{
    [System.Serializable]
    public struct DialogLines
    {
        [SerializeField] private string speakerName;
        [TextArea(3, 10)]
        [SerializeField] private string dialogText;
        [SerializeField] private AudioClip audioClip;

        public readonly string SpeakerName => speakerName;
        public readonly string DialogText => dialogText;
        public readonly AudioClip AudioClip => audioClip;

        public DialogLines(string speakerName, string dialogText, AudioClip audioClip)
        {
            this.speakerName = speakerName;
            this.dialogText = dialogText;
            this.audioClip = audioClip;
        }
    }
    [CreateAssetMenu(fileName = "DialogContainerSO", menuName = "ECHO CITY/Dialogs/Dialog Container SO")]
    public class SODialogContainer : ScriptableObject
    {
        [SerializeField] private DialogLines[] dialogLines;

        public DialogLines[] DialogLines => dialogLines;
    }
}