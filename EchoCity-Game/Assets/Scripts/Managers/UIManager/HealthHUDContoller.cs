using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoCity
{
    public class HealthHUDController : MonoBehaviour
    {
        #region Serialized Fields

        [Header("UI")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private UIDocument hudDocument;

        [Header("Player Health")]
        [SerializeField] private PlayerController playerController;

        [Header("Behaviour")]
        [Tooltip("Time delay before hiding the panel after last update.")]
        [SerializeField] private float hideDelay = 2.0f;

        [Header("Bar Settings")]
        [Tooltip("Minimal normalized fill (0–1)")]
        [Range(0f, 1f)]
        [SerializeField] private float minFillNormalized = 0.05f;
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float highHealthThreshold = 50f;
        [SerializeField] private float mediumHealthThreshold = 20f;

        #endregion

        #region Private Fields

        private VisualElement _root;
        private VisualElement _panel;
        private VisualElement _barFill;
        private bool _isVisible;
        private float normalizedHealth;
        private float displayedHealth;
        private Coroutine _hideRoutine;

        #endregion



        private void OnEnable()
        {
            if (playerController == null)
            {
                enabled = false;
                return;
            }

            if (hudDocument == null)
            {
                enabled = false;
                return;
            }

            _root = hudDocument.rootVisualElement;

            _panel = _root.Q<VisualElement>("HealthPanel");
            _barFill = _root.Q<VisualElement>("HealthBarFill");

            if (_panel == null || _barFill == null)
            {
                enabled = false;
                return;
            }

            _isVisible = false;
            HidePanel();
        }

        void Update()
        {
            if (playerController == null || _barFill == null)
                return;

            UpdateBar(playerController.CurrentHealth);
        }


        private void ShowPanel()
        {
            if (_hideRoutine != null)
            {
                StopCoroutine(_hideRoutine);
                _hideRoutine = null;
            }

            if (_isVisible) return;

            _isVisible = true;
            _panel.AddToClassList("visible");
        }
        IEnumerator HideAfterDelayCoroutine()
        {
            yield return new WaitForSeconds(hideDelay);
            HidePanel();

            _hideRoutine = null;
        }

        private void HidePanel()
        {
            if (!_isVisible) return;

            _isVisible = false;
            _panel.RemoveFromClassList("visible");
        }

        private void UpdateBar(float value)
        {
            normalizedHealth = value / maxHealth;
            displayedHealth = Mathf.Lerp(minFillNormalized, 1f, normalizedHealth);

            _barFill.style.width = new Length(displayedHealth * 100f, LengthUnit.Percent);

            if (value < 100f)
            {
                ShowPanel();

                if (_hideRoutine != null)
                {
                    StopCoroutine(_hideRoutine);
                    _hideRoutine = null;
                }

                if (value > highHealthThreshold)
                    _barFill.style.backgroundColor = Color.green;
                else if (value > mediumHealthThreshold)
                    _barFill.style.backgroundColor = Color.yellow;
                else
                    _barFill.style.backgroundColor = Color.red;
            }
            else
            {
                if (_isVisible && _hideRoutine == null)
                    _hideRoutine = StartCoroutine(HideAfterDelayCoroutine());
            }
        }
    }
}
