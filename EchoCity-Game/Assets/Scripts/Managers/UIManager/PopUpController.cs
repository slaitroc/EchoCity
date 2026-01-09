using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;


namespace EchoCity
{
    public class PopUpController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private UIDocument hudDocument;
        [SerializeField] private UIManager uIManager;

        [Header("Hiding Delay")]
        [SerializeField] private float hidingDelay = 3f;

        #region Private Fields
        private VisualElement _root;
        private VisualElement _popUpPanel;
        private VisualElement _popUpBox;
        private Label _popUpText;
        #endregion

        private void OnEnable()
        {
            if (hudDocument == null) return;
            _root = hudDocument.rootVisualElement;
            _popUpPanel = _root.Q<VisualElement>("PopUpPanel");
            _popUpBox = _root.Q<VisualElement>("PopUpBox");
            _popUpText = _root.Q<Label>("PopUpText");
        }

        public void SpawnPopUp(string message, Color color)
        {
            _popUpText.text = message;
            _popUpText.style.color = color;

            _popUpBox.style.borderBottomColor = color;
            _popUpBox.style.borderTopColor = color;
            _popUpBox.style.borderLeftColor = color;
            _popUpBox.style.borderRightColor = color;

            _popUpPanel.AddToClassList("show");

            StartCoroutine(HidePopUpAfterDelay(hidingDelay));
        }

        IEnumerator HidePopUpAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            _popUpPanel.RemoveFromClassList("show");
        }
    }
}