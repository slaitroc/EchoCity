using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoCity
{
    public class LoadingScreenController : MonoBehaviour
    {
        #region Serialized Fields
        [Header("UI")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private UIDocument loadingDocument;

        [Header("Spinner Settings")]
        [SerializeField] private float rotationSpeed = 36f;
        #endregion

        #region Private Fields
        private VisualElement _root;
        private VisualElement _loadingScreen;
        private VisualElement _spinner;

        private float _currentAngle;
        #endregion

        private void OnEnable()
        {
            if (loadingDocument == null) return;
            _root = loadingDocument.rootVisualElement;

            StartCoroutine(InitCallbacksNextFrame());
        }

        IEnumerator InitCallbacksNextFrame()
        {
            _loadingScreen = _root.Q<VisualElement>("LoadingScreen");
            _spinner = _root.Q<VisualElement>("Spinner");

            _currentAngle = 0f;

            yield return null;
        }

        private void Update()
        {
            if (_spinner == null || _loadingScreen == null)
                return;

            _currentAngle -= rotationSpeed * Time.unscaledDeltaTime;
            _spinner.transform.rotation = Quaternion.Euler(0f, 0f, _currentAngle);
        }

    }

}