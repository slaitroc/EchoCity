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


        void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();
            var root = uiDocument.rootVisualElement;

            _crosshair = root.Q<VisualElement>("crosshair");
            _interactionPanel = root.Q<VisualElement>("interaction-panel");
            _interactionText = root.Q<TextElement>("interaction-text");
            _interactionKey = root.Q<VisualElement>("interaction-key");

            SetVisible(true);
        }

        private void OnDisable() => SetVisible(false);


        private void SetVisible(bool visible)
        {
            if (_crosshair == null) return;
            _crosshair.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            _interactionPanel.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void IsInteractable(bool showPanel, bool isInteractable = false, string interactionText = "Interact")
        {
            if (_crosshair == null) return;

            if (showPanel)
            {
                _interactionPanel.AddToClassList("visible");
                _interactionText.text = interactionText;

                if (isInteractable)
                {
                    _crosshair.AddToClassList("interact");
                    _crosshair.RemoveFromClassList("no-interact");
                    _interactionKey.style.display = DisplayStyle.Flex;
                }
                else
                {
                    _crosshair.RemoveFromClassList("interact");
                    _crosshair.AddToClassList("no-interact");
                    _interactionKey.style.display = DisplayStyle.None;
                }
            }
            else
            {
                _crosshair.RemoveFromClassList("interact");
                _crosshair.RemoveFromClassList("no-interact");
                _interactionPanel.RemoveFromClassList("visible");
            }
        }

    }
}