using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoCity
{
    public class CrosshairController : MonoBehaviour
    {

        [Header("Controllers")]

        [Header("UI Elements")]
        private VisualElement _crosshair;
        private VisualElement _interactionPanel;
        private TextElement _interactionText;
        private VisualElement _interactionKey;


        public bool isInteractable = false;

        void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();
            var root = uiDocument.rootVisualElement;

            _crosshair = root.Q<VisualElement>("crosshair");
            _interactionPanel = root.Q<VisualElement>("interaction-panel");
            _interactionText = root.Q<TextElement>("interaction-text");
            _interactionKey = root.Q<VisualElement>("interaction-key");

        }

        public void SetVisible(bool visible)
        {
            if (_crosshair == null) return;
            _crosshair.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void IsInteractable(bool isOnInteractable)
        {
            if (_crosshair == null) return;

            if (isOnInteractable)
            {
                _crosshair.AddToClassList("interact");
                _interactionPanel.AddToClassList("visible");
                _interactionText.text = "Interact";
            }
            else
            {
                _crosshair.RemoveFromClassList("interact");
                _interactionPanel.RemoveFromClassList("visible");
            }

            isInteractable = isOnInteractable;

        }
    }
}