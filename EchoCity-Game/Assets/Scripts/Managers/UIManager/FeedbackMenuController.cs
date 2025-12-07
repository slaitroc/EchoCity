using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoCity
{
    [RequireComponent(typeof(UIDocument))]
    public class FeedbackMenuController : MonoBehaviour
    {
        private const string _LOG_TAG = "UI-FeedbackMenu";
        private const string _LOG_COLOR = "#ff0000ff";

        [Header("UI")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private UIDocument feedbackDocument;

        [Header("Invoking events")]
        [SerializeField] private SOIntStringEvent feedbackSubmittedEvent;

        #region Private Fields
        private VisualElement _root;
        private VisualElement _feedbackPanel;
        private VisualElement _thankYouPanel;

        private VisualElement _ratingStarsContainer;
        private Button[] _starButtons;
        private int _currentRating;

        private TextField _feedbackTextField;
        private Button _submitButton;
        private Button _thankYouContinueButton;

        private bool _showCursor;
        #endregion

        private void OnEnable()
        {
            if (feedbackDocument == null) return;
            _root = feedbackDocument.rootVisualElement;
            StartCoroutine(InitCallbacksNextFrame());
            _showCursor = true;
        }

        IEnumerator InitCallbacksNextFrame()
        {
            _feedbackPanel = _root.Q<VisualElement>("FeedbackPanel");
            _thankYouPanel = _root.Q<VisualElement>("ThankYouPanel");

            _ratingStarsContainer = _root.Q<VisualElement>("RatingStars");
            _feedbackTextField = _root.Q<TextField>("FeedbackText");
            _submitButton = _root.Q<Button>("SubmitFeedbackButton");
            _thankYouContinueButton = _root.Q<Button>("ThankYouContinueButton");

            _starButtons = new Button[5];
            _starButtons[0] = _root.Q<Button>("Star1");
            _starButtons[1] = _root.Q<Button>("Star2");
            _starButtons[2] = _root.Q<Button>("Star3");
            _starButtons[3] = _root.Q<Button>("Star4");
            _starButtons[4] = _root.Q<Button>("Star5");

            yield return null;

            for (int i = 0; i < _starButtons.Length; i++)
            {
                int rating = i + 1;
                if (_starButtons[i] != null)
                    _starButtons[i].RegisterCallback<ClickEvent>(_ => SetRating(rating));
            }

            if (_submitButton != null)
                _submitButton.clicked += SubmitFeedbackClickHandler;

            if (_thankYouContinueButton != null)
                _thankYouContinueButton.clicked += ThankYouContinueClickHandler;

            if (_feedbackPanel != null)
                _feedbackPanel.style.display = DisplayStyle.Flex;

            if (_thankYouPanel != null)
                _thankYouPanel.style.display = DisplayStyle.None;

            SetRating(0);
        }

        private void Update()
        {
            MethodsUI.SetCursorState(_showCursor);
        }

        private void SetRating(int rating)
        {
            _currentRating = rating;

            for (int i = 0; i < _starButtons.Length; i++)
            {
                if (_starButtons[i] == null) continue;

                bool active = i < rating;
                _starButtons[i].text = active ? "★" : "☆";

                if (active)
                    _starButtons[i].style.color = new StyleColor(new Color(1f, 0.2f, 0.2f, 1f));
                else
                    _starButtons[i].style.color = new StyleColor(new Color(1f, 1f, 1f, 1f));
            }
        }

        private void SubmitFeedbackClickHandler()
        {
            string feedbackText = _feedbackTextField != null ? _feedbackTextField.value : string.Empty;
            feedbackSubmittedEvent?.RaiseEvent((_currentRating, feedbackText));

            if (_feedbackPanel != null)
                _feedbackPanel.style.display = DisplayStyle.None;

            if (_thankYouPanel != null)
                _thankYouPanel.style.display = DisplayStyle.Flex;
        }

        private void ThankYouContinueClickHandler()
        {
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            if (_starButtons != null)
            {
                for (int i = 0; i < _starButtons.Length; i++)
                {
                    if (_starButtons[i] == null) continue;
                    _starButtons[i].UnregisterCallback<ClickEvent>(_ => SetRating(i + 1));
                }
            }

            if (_submitButton != null)
                _submitButton.clicked -= SubmitFeedbackClickHandler;

            if (_thankYouContinueButton != null)
                _thankYouContinueButton.clicked -= ThankYouContinueClickHandler;
        }
    }
}
