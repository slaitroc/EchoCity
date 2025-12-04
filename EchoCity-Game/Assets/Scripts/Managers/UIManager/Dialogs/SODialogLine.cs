using UnityEngine;

[CreateAssetMenu(fileName = "DialogLineSO", menuName = "ECHO CITY/Dialogs/Dialog Line SO")]
public class SODialogLine : ScriptableObject
{
    [SerializeField] private string speakerName;
    [TextArea(3, 10)]
    [SerializeField] private string dialogText;

    public string SpeakerName => speakerName;
    public string DialogText => dialogText;
}
