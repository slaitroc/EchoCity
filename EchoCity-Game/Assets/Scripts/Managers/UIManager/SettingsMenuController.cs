using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

namespace EchoCity
{
    public class SettingsMenuController : MonoBehaviour
    {
        [Header("UI ")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private UIDocument settingsDocument;


        [Header("Music Sliders")]
        [SerializeField] private AudioMixer masterMixer;
        [SerializeField] private string[] _mixerGroupsVolumes = { "Master", "Soundtrack", "SFX" };

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

            _masterSlider.lowValue = 0f;
            _masterSlider.highValue = 10f;
            _musicSlider.lowValue = 0f;
            _musicSlider.highValue = 10f;
            _sfxSlider.lowValue = 0f;
            _sfxSlider.highValue = 10f;
            yield return null;
            SetupVolumeSliders();

            _masterSlider.RegisterValueChangedCallback(OnMasterVolumeChanged);
            _musicSlider.RegisterValueChangedCallback(OnMusicVolumeChanged);
            _sfxSlider.RegisterValueChangedCallback(OnSfxVolumeChanged);
            if (_backButton != null) _backButton.clicked += BackClickHandler;
        }

        private void BackClickHandler()
        {
            uiManager.CloseSettingsMenu();
        }

        private void SetupVolumeSliders()
        {
            float currentDB;
            float linearValue;

            // MASTER
            if (masterMixer.GetFloat(_mixerGroupsVolumes[0], out currentDB))
            {
                linearValue = Mathf.Pow(10f, currentDB / 20f);
                // Multiply by 10 to scale the 0-1 range to the 0-10 slider range
                _masterSlider.value = linearValue * 10f;
            }

            // MUSIC
            if (masterMixer.GetFloat(_mixerGroupsVolumes[1], out currentDB))
            {
                linearValue = Mathf.Pow(10f, currentDB / 20f);
                _musicSlider.value = linearValue * 10f;
            }

            // SFX
            if (masterMixer.GetFloat(_mixerGroupsVolumes[2], out currentDB))
            {
                linearValue = Mathf.Pow(10f, currentDB / 20f);
                _sfxSlider.value = linearValue * 10f;
            }
        }

        private void OnMasterVolumeChanged(ChangeEvent<float> evt)
        {
            // The received value (evt.newValue) is between 0 and 10
            float rawValue = evt.newValue;

            // Normalize: scale 0-10 to 0-1.
            float normalizedValue = rawValue / 10f;

            // Convert to Decibels (dB) using the normalized value.
            // Using 0.0001f prevents issues with log(0).
            float dBValue = Mathf.Log10(Mathf.Clamp(normalizedValue, 0.0001f, 1f)) * 20f;

            masterMixer.SetFloat(_mixerGroupsVolumes[0], dBValue);
        }

        private void OnMusicVolumeChanged(ChangeEvent<float> evt)
        {
            float rawValue = evt.newValue;
            float normalizedValue = rawValue / 10f;

            float dBValue = Mathf.Log10(Mathf.Clamp(normalizedValue, 0.0001f, 1f)) * 20f;
            masterMixer.SetFloat(_mixerGroupsVolumes[1], dBValue);
        }

        private void OnSfxVolumeChanged(ChangeEvent<float> evt)
        {
            float rawValue = evt.newValue;
            float normalizedValue = rawValue / 10f;

            float dBValue = Mathf.Log10(Mathf.Clamp(normalizedValue, 0.0001f, 1f)) * 20f;
            masterMixer.SetFloat(_mixerGroupsVolumes[2], dBValue);
        }

        private void OnDisable()
        {
            _masterSlider.UnregisterValueChangedCallback(OnMasterVolumeChanged);
            _musicSlider.UnregisterValueChangedCallback(OnMusicVolumeChanged);
            _sfxSlider.UnregisterValueChangedCallback(OnSfxVolumeChanged);
            if (_backButton != null) _backButton.clicked -= BackClickHandler;
        }
    }
}
