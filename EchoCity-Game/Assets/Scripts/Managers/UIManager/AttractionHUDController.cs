using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoCity
{
    public class AttractionHUDController : MonoBehaviour
    {
        #region Serialized Fields

        [Header("UI")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private UIDocument hudDocument;

        [Header("Player Attraction")]
        [SerializeField] private PlayerController playerController;
        private IAttraction _playerA;

        [Header("Behaviour")]
        [Tooltip("Time delay before hiding the panel after last update.")]
        [SerializeField] private float hideDelay = 5.0f;

        [Tooltip("Minimal normalized fill (0–1)")]
        [Range(0f, 1f)]
        [SerializeField] private float minFillNormalized = 0.05f;
        [SerializeField] private float _currentAttraction;
        [SerializeField] private bool _shouldShow;

        #endregion

        #region Private Fields

        private VisualElement _root;
        private VisualElement _panel;
        private VisualElement _barFill;
        private Label _label;
        private bool _isVisible;


        private readonly Dictionary<EnemyAI, float> _noiseValues = new();

        #endregion


        private void OnValidate()
        {
            if (playerController != null)
                _playerA = playerController;
            Debug.Assert(_playerA != null, "PlayerController does not implement IAttraction", this);
        }



        private void OnEnable()
        {
            if (hudDocument == null)
                return;

            _root = hudDocument.rootVisualElement;

            _panel = _root.Q<VisualElement>("EnemyNoisePanel");
            _barFill = _root.Q<VisualElement>("EnemyNoiseBarFill");
            // _label = _root.Q<Label>("EnemyNoiseLabel");
        }

        void Update()
        {
            UpdateBar(_playerA.CurrentAttraction);
        }

        private void OnDisable() { }

        private void OnAttractionUpdate(IAttraction data)
        { }

        private void ShowPanel()
        {
            if (_isVisible)
                return;

            _isVisible = true;
            _panel.AddToClassList("visible");
        }

        private IEnumerator HideAfterDelay()
        {
            yield return new WaitForSecondsRealtime(hideDelay);

            HidePanel();
        }

        private void HidePanel()
        {
            _isVisible = false;
            _panel.RemoveFromClassList("visible");

            _barFill.style.width = new Length(0f, LengthUnit.Percent);
        }

        private void UpdateBar(float value)
        {
            float maxAttraction = 2f;
            float normalized = value / maxAttraction;
            float displayed = Mathf.Lerp(minFillNormalized, 1f, normalized);

            _barFill.style.width = new Length(displayed * 100f, LengthUnit.Percent);
            // _label.text = GetAwarenessLabel(clamped, isChasing);
        }

        private string GetAwarenessLabel(float normalized, bool isChasing)
        {
            if (isChasing)
                return "Enemy is chasing you";

            if (normalized >= 0.95f)
                return "Detection imminent";
            if (normalized >= 0.60f)
                return "Enemy highly suspicious";
            if (normalized >= 0.30f)
                return "Enemy sensed something";

            return "Low awareness";
        }
    }
}
