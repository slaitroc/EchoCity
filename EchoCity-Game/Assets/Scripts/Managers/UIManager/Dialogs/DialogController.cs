using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoCity
{
    public class DialogController : MonoBehaviour
    {
        #region Constants
        private const string _LOG_TAG = "UI-Dialogs";
        private const string _LOG_COLOR = "#f0e40fff";
        #endregion

        #region Serialized Fields
        [Header("UI")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private UIDocument dialogDocument;
        #endregion

        #region Private Fields
        private VisualElement _root;
        private Label _speakerLabel;
        private Label _dialogueLabel;
        private Button _continueButton;
        private DialogData _currentDialogData;
        private int _currentLineIndex = 0;
        #endregion

        private void OnEnable()
        {
            if (dialogDocument == null) return;
            _root = dialogDocument.rootVisualElement;

            StartCoroutine(InitCallbacksNextFrame());
        }

        IEnumerator InitCallbacksNextFrame()
        {
            _speakerLabel = _root.Q<Label>("SpeakerLabel");
            _dialogueLabel = _root.Q<Label>("DialogueLabel");
            _continueButton = _root.Q<Button>("ContinueButton");

            yield return null;

            _continueButton.clicked += AdvanceDialog;
            _continueButton.Focus();
        }

        public void SpawnDialogHandler(DialogData dialogData)
        {
            _currentDialogData = dialogData;
            _currentLineIndex = 0;
            ShowCurrentLine();
            _root.style.display = DisplayStyle.Flex;
        }

        private void ShowCurrentLine()
        {
            if (_currentLineIndex < _currentDialogData.DialogLines.Length)
            {
                DialogLines currentLine = _currentDialogData.DialogLines[_currentLineIndex];
                _speakerLabel.text = currentLine.SpeakerName;
                _dialogueLabel.text = currentLine.DialogText;
            }
        }

        private void AdvanceDialog()
        {
            Log.D("Advance Dialog", "green", _LOG_TAG);
            _currentLineIndex++;

            if (_currentLineIndex < _currentDialogData.DialogLines.Length) ShowCurrentLine();
            else CloseDialog();
        }

        private void CloseDialog()
        {
            uiManager.SwitchToPlayState();
        }

        void OnDisable()
        {
            _root.style.display = DisplayStyle.None;
            _continueButton.clicked -= AdvanceDialog;
        }
    }

}