using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoCity
{
    public class NarrationController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private UIDocument narrationDocument;
        [SerializeField] private UIManager uiManager;

        #region UI Elements
        private VisualElement _root;
        private Label _narrationText;
        private Button _continueButton;
        private Button _skipButton;
        #endregion

        #region Narration Data
        private DialogLine[] _lines;
        private int _index;
        private bool _isClosed;
        private bool _isReady;
        private SceneEnum _destinationScene;
        #endregion

        private void OnEnable()
        {
            if (narrationDocument == null) return;

            _root = narrationDocument.rootVisualElement;

            _index = 0;
            _isClosed = false;
            _isReady = false;

            StartCoroutine(InitCallbacksNextFrame());
        }

        private IEnumerator InitCallbacksNextFrame()
        {
            _narrationText = _root.Q<Label>("NarrationText");
            _continueButton = _root.Q<Button>("ContinueButton");
            _skipButton = _root.Q<Button>("SkipButton");

            yield return null;

            if (_continueButton != null) _continueButton.clicked += ForceNextDialog;
            if (_skipButton != null) _skipButton.clicked += Finish;

            _isReady = true;

            Hide();
        }

        private void OnDisable()
        {
            if (_continueButton != null) _continueButton.clicked -= ForceNextDialog;
            if (_skipButton != null) _skipButton.clicked -= Finish;
        }

        public void StartNarration(NarrationParams narrationParams)
        {
            _destinationScene = narrationParams.DestinationScene;
            if (!_isReady) return;

            if (narrationParams == null || narrationParams.NarrationContainer.DialogLines == null || narrationParams.NarrationContainer.DialogLines.Length == 0)
            {
                Finish();
                return;
            }

            _lines = narrationParams.NarrationContainer.DialogLines;
            _index = 0;
            _isClosed = false;

            Show();
            RenderCurrentLine();
        }

        private void RenderCurrentLine()
        {
            if (_isClosed || _lines == null || _lines.Length == 0) return;

            _index = Mathf.Clamp(_index, 0, _lines.Length - 1);

            var line = _lines[_index];
            if (_narrationText != null)
                _narrationText.text = line.DialogText;
        }

        private void ForceNextDialog()
        {
            uiManager.PlayNextNarrationLine(_index++);
            NextDialog();
        }

        public void NextDialog()
        {
            if (_isClosed || _lines == null || _lines.Length == 0) return;

            if (_index >= _lines.Length - 1)
            {
                Finish();
                return;
            }

            _index++;
            RenderCurrentLine();
        }


        private void Finish()
        {
            if (_isClosed) return;

            _isClosed = true;
            Hide();

            uiManager.SwitchToInitLevel(_destinationScene);
        }

        private void Show()
        {
            if (_root != null) _root.style.display = DisplayStyle.Flex;
        }

        private void Hide()
        {
            if (_root != null) _root.style.display = DisplayStyle.None;
        }
    }
}
