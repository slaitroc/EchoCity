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
        private int _cachedIndex;
        private bool _isClosed;
        // private bool _isReady;
        // private SceneEnum _destinationScene;
        private bool _isFading;
        private int _queuedNext;
        #endregion

        private void OnEnable()
        {
            if (narrationDocument == null) return;

            _root = narrationDocument.rootVisualElement;

            _index = 0;
            _isClosed = false;
            // _isReady = false;

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
        }

        private void Update()
        {
            MethodsUI.SetCursorState(true);
        }

        private void OnDisable()
        {
            if (_continueButton != null) _continueButton.clicked -= ForceNextDialog;
            if (_skipButton != null) _skipButton.clicked -= Finish;

            MethodsUI.SetCursorState(false);
        }

        public void StartNarration(NarrationParams narrationParams)
        {
            // _destinationScene = narrationParams.DestinationScene;

            if (narrationParams == null || narrationParams.NarrationContainer.DialogLines == null || narrationParams.NarrationContainer.DialogLines.Length == 0)
            {
                Finish();
                return;
            }

            _lines = narrationParams.NarrationContainer.DialogLines;
            if (!narrationParams.UseCached)
            {
                _index = 0;
                _cachedIndex = 0;
            }
            else
            {
                _index = _cachedIndex;
            }

            _isClosed = false;
            _isFading = false;

            Show();
            RenderCurrentLine();
            _narrationText.RemoveFromClassList("visible");
            _narrationText.schedule.Execute(() => _narrationText.AddToClassList("visible")).ExecuteLater(1);
        }

        private void RenderCurrentLine()
        {
            if (_isClosed || _lines == null || _lines.Length == 0) return;

            var line = _lines[_index];
            _narrationText.text = line.DialogText;

        }

        private void ForceNextDialog()
        {
            uiManager.PlayNextNarrationLine(_index + 1);
            NextDialog(true);
        }

        public void NextDialog(bool instant = false)
        {
            if (_isClosed || _lines == null || _lines.Length == 0) return;

            if (_index >= _lines.Length - 1)
            {
                Finish();
                return;
            }

            _index++;
            _cachedIndex = _index;
            RenderCurrentLine();

            if (instant)
            {
                _narrationText.AddToClassList("visible");
                _isFading = false;
                return;
            }

            _narrationText.RemoveFromClassList("visible");
            _narrationText.schedule.Execute(() =>
            {
                _narrationText.AddToClassList("visible");
                _isFading = false;
            }).ExecuteLater(1);
        }

        public void StartFadeOut()
        {
            if (_isClosed || _lines == null || _lines.Length == 0) return;

            if (_isFading) return;

            _isFading = true;

            _narrationText.RemoveFromClassList("visible");
        }

        private void Finish()
        {
            if (_isClosed) return;

            _isClosed = true;
            Hide();

            uiManager.StopNarration();
            uiManager.SwitchToInitLevel(SceneEnum.None); //No need to specify scene here (already in narration state)
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
