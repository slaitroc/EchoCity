using UnityEngine;

namespace EchoCity
{
    public class UIManager : MonoBehaviour
    {
#pragma warning disable CS0414
        private const string _LOG_COLOR = "cyan";
        private const string _LOG_TAG_FULL = "UI MANAGER";
#pragma warning restore CS0414
        [Header("UI Controllers")]

        [Header("Title Menu")]
        [SerializeField] private TitleMenuController titleMenuController;

        [Header("HUD")]
        [SerializeField] private CrosshairController crosshairController;
        [SerializeField] private RadialMenuController radialMenuController;
        [SerializeField] private WarningController warningController;
        [SerializeField] private EquippedPanelController equippedPanelController;

        [Header("Pause Menu")]
        [SerializeField] private PauseMenuController pauseMenuController;

        [Header("Settings Menu")]
        [SerializeField] private SettingsMenuController settingsMenuController;

        [Header("Dialogs")]
        [SerializeField] private DialogController dialogController;

        [Header("Death Menu")]
        [SerializeField] private DeathMenuController deathMenuController;

        [Header("Loading Screen")]
        [SerializeField] private LoadingScreenController loadingScreenController;
        [SerializeField] private FeedbackMenuController feedbackMenuController;

        [Header("Win Menu")]
        [SerializeField] private WinMenuController winMenuController;

        [Header("Events")]
        [Header("Invoking Events for GM")]
        [SerializeField] private SOEventVoid switchToTitleStateEvent;
        [SerializeField] private SOEventVoid switchToPlayStateEvent;
        [SerializeField] private SOSceneEnumEvent switchToInitLevelEvent;


        [Header("Observed Events From GM")]
        [SerializeField] private SOEventVoid titleMenuEvent;
        [SerializeField] private SOHudEnumEvent hudMenuEvent;
        [SerializeField] private SOEventVoid pauseMenuEvent;
        [SerializeField] private SODialogDataEvent dialogMenuEvent;
        [SerializeField] private SOEventVoid deathMenuEvent;
        [SerializeField] private SOEventVoid enterLoadingScreenEvent;
        [SerializeField] private SOEventVoid exitLoadingScreenEvent;
        [SerializeField] private SOEventVoid winMenuEvent;

        [Header("Observed Events From Others")]
        [SerializeField] private SOEventVoid canInteractStartEvent;
        [SerializeField] private SOEventVoid canInteractStopEvent;
        [SerializeField] private SOStringColorEvent spawnWarningEvent;
        [SerializeField] private SOIntegerPickableDataGameObjectEvent itemEquippedEvent;
        [SerializeField] private SOIntEvent dropItemEvent;

        [Header("External References")]
        [SerializeField] private PlayerInventory _playerInventory;

        #region Private Fields
        private GameObject _titleMenu;
        private GameObject _hud;
        private GameObject _pauseMenu;
        private GameObject _settingsMenu;
        private GameObject _dialog;
        private GameObject _deathMenu;
        private GameObject _loadingScreen;
        private GameObject _feedbackMenu;
        private GameObject _winMenu;
        #endregion



        void Awake()
        {
            _titleMenu = titleMenuController.gameObject;
            _hud = crosshairController.gameObject;
            _pauseMenu = pauseMenuController.gameObject;
            _settingsMenu = settingsMenuController.gameObject;
            _dialog = dialogController.gameObject;
            _deathMenu = deathMenuController.gameObject;
            _loadingScreen = loadingScreenController.gameObject;
            _feedbackMenu = feedbackMenuController.gameObject;
            _winMenu = winMenuController.gameObject;

            if (_playerInventory == null)
            {
                Log.E("PlayerInventory reference is missing in UIManager!", _LOG_COLOR, _LOG_TAG_FULL);
            }
        }

        private void OnEnable()
        {
            if (titleMenuEvent) titleMenuEvent.OnEventRaised += OpenTitleMenuHandler;
            if (hudMenuEvent) hudMenuEvent.OnEventRaised += OpenHUDMenuHandler;
            if (pauseMenuEvent) pauseMenuEvent.OnEventRaised += OpenPauseMenuHandler;
            if (dialogMenuEvent) dialogMenuEvent.OnEventRaised += OpenDialogMenuHandler;
            if (deathMenuEvent) deathMenuEvent.OnEventRaised += OpenDeathMenuHandler;
            if (enterLoadingScreenEvent) enterLoadingScreenEvent.OnEventRaised += OpenLoadingScreenHandler;
            if (exitLoadingScreenEvent) exitLoadingScreenEvent.OnEventRaised += CloseLoadingScreenHandler;
            if (winMenuEvent) winMenuEvent.OnEventRaised += OpenWinMenuHandler;

            if (canInteractStartEvent) canInteractStartEvent.OnEventRaised += CrosshairInteractableHandler;
            if (canInteractStopEvent) canInteractStopEvent.OnEventRaised += CrosshairInteractableHandler;
            if (spawnWarningEvent) spawnWarningEvent.OnEventRaised += SpawnWarningHandler;
            if (itemEquippedEvent) itemEquippedEvent.OnEventRaised += ItemEquippedHandler;
            if (dropItemEvent) dropItemEvent.OnEventRaised += DropItemEventHandler;
        }

        #region Public Methods - State Switching
        public void SwitchToPlayState()
        {
            _titleMenu.SetActive(false);
            _pauseMenu.SetActive(false);
            _dialog.SetActive(false);
            _deathMenu.SetActive(false);
            _winMenu.SetActive(false);

            _hud.SetActive(true);
            switchToPlayStateEvent?.RaiseEvent();
        }

        public void SwitchToTitleState()
        {
            _hud.SetActive(false);
            _pauseMenu.SetActive(false);
            _dialog.SetActive(false);
            _deathMenu.SetActive(false);
            _winMenu.SetActive(false);

            _titleMenu.SetActive(true);
            switchToTitleStateEvent?.RaiseEvent();
        }

        public void SwitchToInitLevel(SceneEnum scene)
        {
            _titleMenu.SetActive(false);
            _pauseMenu.SetActive(false);
            _dialog.SetActive(false);
            _deathMenu.SetActive(false);
            _winMenu.SetActive(false);

            _hud.SetActive(true);
            switchToInitLevelEvent?.RaiseEvent(scene);
        }


        public void OpenSettingsMenu() => _settingsMenu.SetActive(true);
        public void CloseSettingsMenu() => _settingsMenu.SetActive(false);
        public void OpenFeedbackMenu() => _feedbackMenu.SetActive(true);
        public void CloseFeedbackMenu() => _feedbackMenu.SetActive(false);
        #endregion


        #region Private Event Handlers
        private void OpenTitleMenuHandler()
        {
            _hud.SetActive(false);
            _pauseMenu.SetActive(false);
            _dialog.SetActive(false);
            _deathMenu.SetActive(false);
            _winMenu.SetActive(false);

            _titleMenu.SetActive(true);
        }

        private void OpenHUDMenuHandler(HudEnum hud)
        {
            radialMenuController.enabled = !radialMenuController.enabled;
        }

        private void OpenPauseMenuHandler()
        {
            _hud.SetActive(false);
            _titleMenu.SetActive(false);
            _dialog.SetActive(false);
            _deathMenu.SetActive(false);
            _winMenu.SetActive(false);

            _pauseMenu.SetActive(true);
        }

        private void OpenDialogMenuHandler(DialogData dialogData)
        {
            _hud.SetActive(false);
            _pauseMenu.SetActive(false);
            _titleMenu.SetActive(false);
            _deathMenu.SetActive(false);
            _winMenu.SetActive(false);

            _dialog.SetActive(true);
            dialogController.SpawnDialogHandler(dialogData);
        }

        private void OpenDeathMenuHandler()
        {
            _hud.SetActive(false);
            _pauseMenu.SetActive(false);
            _dialog.SetActive(false);
            _titleMenu.SetActive(false);
            _winMenu.SetActive(false);

            _deathMenu.SetActive(true);
        }

        private void OpenWinMenuHandler()
        {
            _hud.SetActive(false);
            _pauseMenu.SetActive(false);
            _dialog.SetActive(false);
            _titleMenu.SetActive(false);
            _deathMenu.SetActive(false);

            _winMenu.SetActive(true);
        }

        private void OpenLoadingScreenHandler() => _loadingScreen.SetActive(true);
        private void CloseLoadingScreenHandler() => _loadingScreen.SetActive(false);

        private void CrosshairInteractableHandler() => crosshairController.IsInteractable(!crosshairController.isInteractable);
        private void SpawnWarningHandler(string warningText, Color color) => warningController.SpawnWarning(warningText, color);

        private void ItemEquippedHandler(int index, PickableData pickableData, GameObject obj) => equippedPanelController.SetEquippedItem(pickableData.Icon, pickableData.Name);
        private void DropItemEventHandler(int index) => equippedPanelController.ClearEquipped();


        #endregion
        private void OnDisable()
        {
            if (titleMenuEvent) titleMenuEvent.OnEventRaised -= OpenTitleMenuHandler;
            if (hudMenuEvent) hudMenuEvent.OnEventRaised -= OpenHUDMenuHandler;
            if (pauseMenuEvent) pauseMenuEvent.OnEventRaised -= OpenPauseMenuHandler;
            if (dialogMenuEvent) dialogMenuEvent.OnEventRaised -= OpenDialogMenuHandler;
            if (deathMenuEvent) deathMenuEvent.OnEventRaised -= OpenDeathMenuHandler;
        }

    }

}