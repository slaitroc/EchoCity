using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;




public class WarningController : MonoBehaviour
{

    [Header("UI")]
    [SerializeField] private UIDocument hudDocument;

    [Header("Hiding Delay")]
    [SerializeField] private float hidingDelay = 3f;

    #region Private Fields
    private VisualElement _root;
    private VisualElement _warningPanel;
    private VisualElement _warningBox;
    private Label _warningText;
    #endregion


    private void OnEnable()
    {
        if (hudDocument == null) return;
        _root = hudDocument.rootVisualElement;
        _warningPanel = _root.Q<VisualElement>("WarningPanel");
        _warningBox = _root.Q<VisualElement>("WarningBox");
        _warningText = _root.Q<Label>("WarningText");    
    }

    public void SpawnWarning(string message, Color color)
    {
        _warningText.text = message;
        _warningText.style.color = color;

        _warningBox.style.borderBottomColor = color;
        _warningBox.style.borderTopColor = color;
        _warningBox.style.borderLeftColor = color;
        _warningBox.style.borderRightColor = color;

        _warningPanel.style.display = DisplayStyle.Flex;
        _warningPanel.RemoveFromClassList("hide");
        
        StartCoroutine(HideWarningAfterDelay(hidingDelay));
    }

    IEnumerator HideWarningAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _warningPanel.AddToClassList("hide");
    }
}
