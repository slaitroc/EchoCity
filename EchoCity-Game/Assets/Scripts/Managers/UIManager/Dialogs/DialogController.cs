using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

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
        uiManager.EnableUIActionMap();
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

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         AdvanceDialog();
    //     }
    // }

    private void AdvanceDialog()
    {
        Log.D("Advance Dialog", "green", _LOG_TAG);
        _currentLineIndex++;
        
        if (_currentLineIndex < _currentDialogData.DialogLines.Length) ShowCurrentLine();
        else CloseDialog();
    }

    private void CloseDialog()
    {
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        _root.style.display = DisplayStyle.None;
        uiManager.DisableUIActionMap();
        uiManager.EnablePlayerActionMap();
        _continueButton.clicked -= AdvanceDialog;
    }
}
