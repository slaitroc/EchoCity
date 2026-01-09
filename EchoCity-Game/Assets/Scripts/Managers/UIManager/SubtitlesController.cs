using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoCity
{
    [RequireComponent(typeof(UIDocument))]
    public class SubtitlesController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private UIDocument subtitlesDocument;

        [Header("Auto Hide")]
        [Tooltip("Extra seconds added after the audio duration before hiding.")]
        [SerializeField] private float hidePaddingSeconds = 0.15f;

        [Header("Offset")]
        [Tooltip("Optional base offset (px) always applied on top of runtime offset.")]
        [SerializeField] private float baseOffsetPx = 0f;

        #region Private Fields
        private VisualElement _root;
        private VisualElement _anchor;
        private VisualElement _panel;
        private Label _speakerLabel;
        private Label _textLabel;

        private Coroutine _hideCoroutine;
        private float _runtimeOffsetPx;
        private bool _isShowingSubtitle;
        #endregion

        #region Public Properties

        #endregion

        private void OnEnable()
        {
            if (subtitlesDocument == null) return;

            _root = subtitlesDocument.rootVisualElement;

            _anchor = _root.Q<VisualElement>("subtitles-anchor");
            _panel = _root.Q<VisualElement>("subtitles-panel");
            _speakerLabel = _root.Q<Label>("subtitles-speaker");
            _textLabel = _root.Q<Label>("subtitles-text");

            if (_panel != null)
            {
                _panel.RemoveFromClassList("visible");
                _panel.AddToClassList("hidden");
            }
        }

        private void OnDisable()
        {
            HideSubtitle();
        }

        #region Public API
        public void ApplyOffset(float offsetPx = 0f)
        {
            if (_anchor == null) return;
            _runtimeOffsetPx = Mathf.Max(0f, offsetPx);

            float totalPx = baseOffsetPx + _runtimeOffsetPx;

            // translate Y negative => moves up
            _anchor.style.translate = new Translate(0f, -totalPx, 0f);
        }

        public void ShowSubtitle(string text, float audioDurationSeconds, string speaker = null)
        {
            if (_panel == null) return;

            // If a previous subtitle was waiting to hide, cancel it
            StopHideCoroutine();

            // Set text
            if (_textLabel != null)
                _textLabel.text = text ?? string.Empty;

            // Speaker optional
            bool hasSpeaker = !string.IsNullOrEmpty(speaker);
            if (_speakerLabel != null)
            {
                _speakerLabel.style.display = hasSpeaker ? DisplayStyle.Flex : DisplayStyle.None;
                if (hasSpeaker) _speakerLabel.text = speaker;
            }

            // Show
            _panel.RemoveFromClassList("hidden");
            _panel.AddToClassList("visible");

            // Schedule hide AFTER audio ends (+ padding)
            if (audioDurationSeconds > 0f)
            {
                float delay = Mathf.Max(0f, audioDurationSeconds + hidePaddingSeconds);
                _hideCoroutine = StartCoroutine(HideAfterDelay(delay));
            }
        }

        #endregion

        #region Internals
        private void HideSubtitle()
        {
            if (_panel == null) return;

            StopHideCoroutine();

            _panel.RemoveFromClassList("visible");
            _panel.AddToClassList("hidden");
        }



        private void StopHideCoroutine()
        {
            if (_hideCoroutine != null)
            {
                StopCoroutine(_hideCoroutine);
                _hideCoroutine = null;
            }
        }

        private IEnumerator HideAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            // Hide with USS transition
            if (_panel != null)
            {
                _panel.RemoveFromClassList("visible");
                _panel.AddToClassList("hidden");
            }

            _hideCoroutine = null;
        }

        #endregion
    }
}