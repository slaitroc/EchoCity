using UnityEngine;

[CreateAssetMenu(fileName = "DialogContainerSO", menuName = "ECHO CITY/Dialogs/Dialog Container SO")]
public class SODialogContainer : ScriptableObject
{
    [SerializeField] private SODialogLine[] dialogLines;
    public SODialogLine[] DialogLines => dialogLines;
}