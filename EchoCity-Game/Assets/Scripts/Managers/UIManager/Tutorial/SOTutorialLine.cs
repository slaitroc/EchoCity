using UnityEngine;

[CreateAssetMenu(fileName = "TutorialLineSO", menuName = "ECHO CITY/Text Lines/Tutorial Line")]
public class SOTutorialLine : ScriptableObject
{
    [TextArea(3, 10)]
    [SerializeField] private string startText;
    [SerializeField] private Sprite icon;
    [TextArea(3, 10)]
    [SerializeField] private string endText;

    public string StartText => startText;
    public Sprite Icon => icon;
    public string EndText => endText;
}
