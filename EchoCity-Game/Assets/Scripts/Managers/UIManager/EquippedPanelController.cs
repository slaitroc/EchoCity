using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoCity
{
    public class EquippedPanelController : MonoBehaviour
    {
        #region Serialized Fields
        [Header("UI")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private UIDocument hudDocument;

        [Header("Default State")]
        [SerializeField] private string noItemText = "No Item Equipped";
        #endregion

        #region Private Fields
        private VisualElement _root;
        private VisualElement _equippedPanel;
        private VisualElement _equippedIcon;
        private Label _equippedName;
        #endregion

        private void OnEnable()
        {
            if (hudDocument == null) return;
            _root = hudDocument.rootVisualElement;

            StartCoroutine(InitCallbacksNextFrame());
        }

        IEnumerator InitCallbacksNextFrame()
        {
            _equippedPanel = _root.Q<VisualElement>("EquippedPanel");
            _equippedIcon = _root.Q<VisualElement>("EquippedIcon");
            _equippedName = _root.Q<Label>("EquippedName");

            ClearEquipped();

            yield return null;
        }

        public void SetEquippedItem(Sprite icon, string itemName)
        {
            if (_equippedPanel == null || _equippedIcon == null || _equippedName == null)
                return;

            _equippedName.text = string.IsNullOrWhiteSpace(itemName) ? noItemText : itemName;

            if (icon != null)
            {
                _equippedIcon.style.backgroundImage = new StyleBackground(icon);
            }
            else
            {
                _equippedIcon.style.backgroundImage = StyleKeyword.None;
            }
        }

        public void ClearEquipped()
        {
            if (_equippedPanel == null || _equippedIcon == null || _equippedName == null)
                return;

            _equippedName.text = noItemText;
            _equippedIcon.style.backgroundImage = StyleKeyword.None;
        }
    }
}
