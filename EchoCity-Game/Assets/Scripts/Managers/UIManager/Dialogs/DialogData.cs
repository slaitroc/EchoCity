using UnityEngine;

public struct DialogLines
{
    private string _speakerName;
    private string _dialogText;

    public string SpeakerName => _speakerName;
    public string DialogText => _dialogText;

    public DialogLines(string speakerName, string dialogText)
    {
        _speakerName = speakerName;
        _dialogText = dialogText;
    }
}

public struct DialogData
{
    private DialogLines[] _dialogLines;
    public DialogLines[] DialogLines => _dialogLines;
    public DialogData(SODialogLine[] dialogLines)
    {
        _dialogLines = new DialogLines[dialogLines.Length];
        for (int i = 0; i < _dialogLines.Length; i++)
        {
            _dialogLines[i] = new DialogLines(dialogLines[i].SpeakerName, dialogLines[i].DialogText);
        }
    }

    public DialogData(SODialogContainer dialogContainer)
    {
        SODialogLine[] dialogLines = dialogContainer.DialogLines;
        _dialogLines = new DialogLines[dialogLines.Length];
        for (int i = 0; i < _dialogLines.Length; i++)
        {
            _dialogLines[i] = new DialogLines(dialogLines[i].SpeakerName, dialogLines[i].DialogText);
        }
    }
}

