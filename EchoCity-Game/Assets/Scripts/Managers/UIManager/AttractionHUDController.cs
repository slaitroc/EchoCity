using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        [Header("Bar Settings")]
        [Tooltip("Minimal normalized fill (0–1)")]
        [Range(0f, 1f)]
        [SerializeField] private float minFillNormalized = 0.05f;
        [SerializeField] private float maxAttraction = 2f;
        [SerializeField] private float highAttractionThreshold = 1f;
        [SerializeField] private float mediumAttractionThreshold = 0.5f;

        #endregion

        #region Private Fields

        private VisualElement _root;
        private VisualElement _panel;
        private VisualElement _barFill;
        private bool _isVisible;
        private float normalizedAttraction;
        private float displayedAttraction;
        private Coroutine _hideRoutine;

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

            _panel = _root.Q<VisualElement>("EnemyAttractionPanel");
            _barFill = _root.Q<VisualElement>("EnemyAttractionBarFill");

            _isVisible = false;
        }

        void Update()
        {
            UpdateBar(_playerA.CurrentAttraction);
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
            normalizedAttraction = value / maxAttraction;
            displayedAttraction = Mathf.Lerp(minFillNormalized, 1f, normalizedAttraction);

            _barFill.style.width = new Length(displayedAttraction * 100f, LengthUnit.Percent);

            if (value > 0f)
            {
                ShowPanel();

                if (_hideRoutine != null)
                {
                    StopCoroutine(_hideRoutine);
                    _hideRoutine = null;
                }

                if (value > highAttractionThreshold)
                    _barFill.style.backgroundColor = Color.red;
                else if (value > mediumAttractionThreshold)
                    _barFill.style.backgroundColor = Color.yellow;
                else
                    _barFill.style.backgroundColor = Color.green;
            }
            else
            {
                if (_isVisible && _hideRoutine == null)
                    _hideRoutine = StartCoroutine(HideAfterDelayCoroutine());
            }
        }
    }
}
