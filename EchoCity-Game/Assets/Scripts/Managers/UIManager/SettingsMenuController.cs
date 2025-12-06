using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoCity
{
    public class SettingsMenuController : MonoBehaviour
    {
        [Header("UI ")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private UIDocument settingsDocument;

        [Header("Invoked Events")]
        [SerializeField] private SOEventVoid closeSettingsMenuEvent;

        [Header("Music Sliders")]
        [SerializeField] private float masterVolume = 0.5f;
        [SerializeField] private float musicVolume = 0.5f;
        [SerializeField] private float sfxVolume = 0.5f;

        #region Private Fields
        private VisualElement _root;
        private Slider _masterSlider;
        private Slider _musicSlider;
        private Slider _sfxSlider;
        private Button _backButton;
        #endregion

        private void OnEnable()
        {
            if (settingsDocument == null) return;
            _root = settingsDocument.rootVisualElement;

            StartCoroutine(InitCallbacksNextFrame());
        }
        IEnumerator InitCallbacksNextFrame()
        {
            _masterSlider = _root.Q<Slider>("MasterSlider");
            _musicSlider = _root.Q<Slider>("MusicSlider");
            _sfxSlider = _root.Q<Slider>("SfxSlider");
            _backButton = _root.Q<Button>("BackButton");

            _masterSlider.value = masterVolume;
            _musicSlider.value = musicVolume;
            _sfxSlider.value = sfxVolume;

            yield return null;

            _masterSlider.RegisterValueChangedCallback(evt => masterVolume = evt.newValue);
            _musicSlider.RegisterValueChangedCallback(evt => musicVolume = evt.newValue);
            _sfxSlider.RegisterValueChangedCallback(evt => sfxVolume = evt.newValue);

            if (_backButton != null) _backButton.clicked += BackClickHandler;
        }

        private void BackClickHandler()
        {
            uiManager.CloseSettingsMenuHandler();
            closeSettingsMenuEvent?.RaiseEvent();
        }

        private void OnDisable()
        {
            _masterSlider.UnregisterValueChangedCallback(evt => masterVolume = evt.newValue);
            _musicSlider.UnregisterValueChangedCallback(evt => musicVolume = evt.newValue);
            _sfxSlider.UnregisterValueChangedCallback(evt => sfxVolume = evt.newValue);
            if (_backButton != null) _backButton.clicked -= BackClickHandler;
        }
    }
}
