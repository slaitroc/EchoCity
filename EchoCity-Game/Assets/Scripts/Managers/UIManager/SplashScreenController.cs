using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoCity
{

    public class SplashScreenController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private UIDocument splashScreenDocument;
        [SerializeField] UIManager uiManager;
        [Header("Splash Screen Settings")]
        [SerializeField] private float polimiScreenDuration = 3f;
        [SerializeField] private float teamScreenDuration = 3f;
        [Tooltip("Must match the duration of the fade out animation in the splash screen's USS")]
        [SerializeField] private float timeBetweenScreens = 2f;



        #region private fields
        private VisualElement _root;
        private VisualElement _polimiBackground;
        private VisualElement _teamBackground;
        #endregion


        private void OnEnable()
        {
            _root = splashScreenDocument.rootVisualElement;
            _polimiBackground = _root.Q<VisualElement>("SplashScreenPolimiBackground");
            _teamBackground = _root.Q<VisualElement>("SplashScreenTeamBackground");

            StartCoroutine(PlaySplashScreenSequence());
        }



        IEnumerator PlaySplashScreenSequence()
        {
            yield return null;

            // Show Polimi background
            _polimiBackground.AddToClassList("show");
            _teamBackground.RemoveFromClassList("show");
            yield return new WaitForSecondsRealtime(polimiScreenDuration);

            _polimiBackground.RemoveFromClassList("show");
            yield return new WaitForSecondsRealtime(timeBetweenScreens);

            // Show Team background
            _teamBackground.AddToClassList("show");
            yield return new WaitForSecondsRealtime(teamScreenDuration);

            // Hide splash screen and show main menu
            _teamBackground.RemoveFromClassList("show");
            yield return new WaitForSecondsRealtime(timeBetweenScreens);

            _polimiBackground.style.display = DisplayStyle.None;
            _teamBackground.style.display = DisplayStyle.None;
            uiManager.ShowTitleMenu();
        }

    }
}
