using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoCity
{
    public class TutorialPanelController : MonoBehaviour
    {
        #region Serialized Fields
        [Header("UI")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private UIDocument tutorialDocument;

        [Header("Tutorial Lines")]
        [SerializeField] private SOTutorialLine[] tutorialLines;
        #endregion

        #region Private Fields
        private VisualElement _root;
        private VisualElement _tutorialPanel;
        private VisualElement _tutorialLinesContainer;

        #endregion

        private void OnEnable()
        {
            if (tutorialDocument == null) return;
            _root = tutorialDocument.rootVisualElement;

            StartCoroutine(InitCallbacksNextFrame());
        }

        IEnumerator InitCallbacksNextFrame()
        {
            _tutorialPanel = _root.Q<VisualElement>("TutorialPanel");
            _tutorialLinesContainer = _root.Q<VisualElement>("TutorialLines");

            yield return null;
        }

        public void BuildFromLines(SOTutorialLine[] lines)
        {
            if (_tutorialLinesContainer == null || _tutorialPanel == null)
                return;

            _tutorialLinesContainer.Clear();


            foreach (var lineData in lines)
            {
                var line = new VisualElement();
                line.AddToClassList("tutorial-line");

                if (!string.IsNullOrWhiteSpace(lineData.StartText))
                {
                    var pre = new Label(lineData.StartText);
                    pre.AddToClassList("tutorial-text");
                    line.Add(pre);
                }

                if (lineData.Icon != null)
                {
                    var icon = new VisualElement();
                    icon.AddToClassList("tutorial-key-icon");
                    icon.style.backgroundImage = new StyleBackground(lineData.Icon);
                    line.Add(icon);
                }

                if (!string.IsNullOrWhiteSpace(lineData.EndText))
                {
                    var post = new Label(lineData.EndText);
                    post.AddToClassList("tutorial-text");
                    line.Add(post);
                }

                _tutorialLinesContainer.Add(line);
            }

        }
        public void ShowHideLines(bool showTutorial = true)
        {
            if (showTutorial)
            {
                _tutorialLinesContainer.AddToClassList("expanded");
                BuildFromLines(tutorialLines);
            }
            else
                HideLines();
        }

        private void HideLines()
        {
            _tutorialLinesContainer.Clear();
            _tutorialLinesContainer.RemoveFromClassList("expanded");
        }

    }
}