using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoCity
{
    public class EnemyNoiseHUDController : MonoBehaviour
    {
        #region Serialized Fields

        [Header("UI")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private UIDocument hudDocument;

        [Header("Events")]
        // [SerializeField] private SOEnemyNoiseUIEvent enemyNoiseUIEvent;

        [Header("Behaviour")]
        [Tooltip("Time delay before hiding the panel after last update.")]
        [SerializeField] private float hideDelay = 5.0f;

        [Tooltip("Minimal normalized fill (0–1)")]
        [Range(0f, 1f)]
        [SerializeField] private float minFillNormalized = 0.05f;
        // [SerializeField] private EnemyNoiseData _currentData;
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


        private void OnEnable()
        {
            if (hudDocument == null)
                return;

            _root = hudDocument.rootVisualElement;

            _panel = _root.Q<VisualElement>("EnemyNoisePanel");
            _barFill = _root.Q<VisualElement>("EnemyNoiseBarFill");
            // _label = _root.Q<Label>("EnemyNoiseLabel");

            // if (enemyNoiseUIEvent != null)
            // enemyNoiseUIEvent.OnEventRaised += OnEnemyNoiseUpdate;
        }

        private void OnDisable()
        {
            // if (enemyNoiseUIEvent != null)
            // enemyNoiseUIEvent.OnEventRaised -= OnEnemyNoiseUpdate;
        }

        // private void OnEnemyNoiseUpdate(EnemyNoiseData data)
        // {
        //     if (_panel == null)
        //         return;


        //     if (!_noiseValues.ContainsKey(data.enemy))
        //         _noiseValues.Add(data.enemy, 0f);

        //     _noiseValues[data.enemy] = data.currentNoiseLevel;

        //     _shouldShow =
        //         data.isChasing ||
        //         data.shouldShowUI ||
        //         data.currentNoiseLevel >= data.chaseThreshold;

        //     if (!_shouldShow)
        //         return;

        //     // ShowPanel();
        //     // StartCoroutine(HideAfterDelay());

        //     float normalized = data.isChasing
        //         ? 1f
        //         : (data.chaseThreshold > 0f
        //                 ? Mathf.Clamp01(data.currentNoiseLevel / data.chaseThreshold)
        //                 : 0f);

        //     UpdateBar(normalized, data.isChasing);

        //     // Inspector debug
        //     _currentData = data;
        // }



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

        private void UpdateBar(float normalizedValue, bool isChasing)
        {
            float clamped = Mathf.Clamp01(normalizedValue);
            float displayed = Mathf.Lerp(minFillNormalized, 1f, clamped);

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
