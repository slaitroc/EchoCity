using UnityEngine;

namespace EchoCity
{
    public class UIManager : MonoBehaviour, IEventSender
    {
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

        public string SenderName => gameObject.name;
        public int SenderID => GetInstanceID();
        public bool IsManager => true;
        public EventSenderCategoriesEnum[] SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.UI };

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
        [SerializeField] private SOBoolStringEvent canInteractStartEvent;
        [SerializeField] private SOEventVoid canInteractStopEvent;
        [SerializeField] private SOStringColorEvent spawnWarningEvent;
        [SerializeField] private SOEquipItemEvent itemEquippedEvent;
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
                Log.ELazy(() => "PlayerInventory reference is missing in UIManager!", this);
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

            if (canInteractStartEvent) canInteractStartEvent.OnEventRaised += CrosshairInteractableStartHandler;
            if (canInteractStopEvent) canInteractStopEvent.OnEventRaised += CrosshairInteractableStopHandler;
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
            switchToPlayStateEvent?.RaiseEvent(this);
        }

        public void SwitchToTitleState()
        {
            _hud.SetActive(false);
            _pauseMenu.SetActive(false);
            _dialog.SetActive(false);
            _deathMenu.SetActive(false);
            _winMenu.SetActive(false);

            _titleMenu.SetActive(true);
            switchToTitleStateEvent?.RaiseEvent(this);
        }

        public void SwitchToInitLevel(SceneEnum scene)
        {
            _titleMenu.SetActive(false);
            _pauseMenu.SetActive(false);
            _dialog.SetActive(false);
            _deathMenu.SetActive(false);
            _winMenu.SetActive(false);

            _hud.SetActive(true);
            switchToInitLevelEvent?.RaiseEvent(this, scene);
        }


        public void OpenSettingsMenu() => _settingsMenu.SetActive(true);
        public void CloseSettingsMenu() => _settingsMenu.SetActive(false);
        public void OpenFeedbackMenu() => _feedbackMenu.SetActive(true);
        public void CloseFeedbackMenu() => _feedbackMenu.SetActive(false);
        #endregion


        #region Private Event Handlers
        private void OpenTitleMenuHandler(IEventSender sender)
        {
            _hud.SetActive(false);
            _pauseMenu.SetActive(false);
            _dialog.SetActive(false);
            _deathMenu.SetActive(false);
            _winMenu.SetActive(false);

            _titleMenu.SetActive(true);
        }

        private void OpenHUDMenuHandler(IEventSender sender, HudEnum hud)
        {
            radialMenuController.enabled = !radialMenuController.enabled;
            crosshairController.enabled = !crosshairController.enabled;
        }

        private void OpenPauseMenuHandler(IEventSender sender)
        {
            _hud.SetActive(false);
            _titleMenu.SetActive(false);
            _dialog.SetActive(false);
            _deathMenu.SetActive(false);
            _winMenu.SetActive(false);

            _pauseMenu.SetActive(true);
        }

        private void OpenDialogMenuHandler(IEventSender sender, DialogData dialogData)
        {
            _hud.SetActive(false);
            _pauseMenu.SetActive(false);
            _titleMenu.SetActive(false);
            _deathMenu.SetActive(false);
            _winMenu.SetActive(false);

            _dialog.SetActive(true);
            dialogController.SpawnDialogHandler(dialogData);
        }

        private void OpenDeathMenuHandler(IEventSender sender)
        {
            _hud.SetActive(false);
            _pauseMenu.SetActive(false);
            _dialog.SetActive(false);
            _titleMenu.SetActive(false);
            _winMenu.SetActive(false);

            _deathMenu.SetActive(true);
        }

        private void OpenWinMenuHandler(IEventSender sender)
        {
            _hud.SetActive(false);
            _pauseMenu.SetActive(false);
            _dialog.SetActive(false);
            _titleMenu.SetActive(false);
            _deathMenu.SetActive(false);

            _winMenu.SetActive(true);
        }

        private void OpenLoadingScreenHandler(IEventSender sender) => _loadingScreen.SetActive(true);
        private void CloseLoadingScreenHandler(IEventSender sender) => _loadingScreen.SetActive(false);

        private void CrosshairInteractableStartHandler(IEventSender sender, bool isInteractable, string text) => crosshairController.IsInteractable(true, isInteractable, text);
        private void CrosshairInteractableStopHandler(IEventSender sender) => crosshairController.IsInteractable(false);
        private void SpawnWarningHandler(IEventSender sender, string warningText, Color color) => warningController.SpawnWarning(warningText, color);

        private void ItemEquippedHandler(IEventSender sender, int index, SOPickable pickableData, GameObject obj) => equippedPanelController.SetEquippedItem(pickableData.Icon, pickableData.Name);
        private void DropItemEventHandler(IEventSender sender, int index) => equippedPanelController.ClearEquipped();


        #endregion
        private void OnDisable()
        {
            if (titleMenuEvent) titleMenuEvent.OnEventRaised -= OpenTitleMenuHandler;
            if (hudMenuEvent) hudMenuEvent.OnEventRaised -= OpenHUDMenuHandler;
            if (pauseMenuEvent) pauseMenuEvent.OnEventRaised -= OpenPauseMenuHandler;
            if (dialogMenuEvent) dialogMenuEvent.OnEventRaised -= OpenDialogMenuHandler;
            if (deathMenuEvent) deathMenuEvent.OnEventRaised -= OpenDeathMenuHandler;
            if (enterLoadingScreenEvent) enterLoadingScreenEvent.OnEventRaised -= OpenLoadingScreenHandler;
            if (exitLoadingScreenEvent) exitLoadingScreenEvent.OnEventRaised -= CloseLoadingScreenHandler;
            if (winMenuEvent) winMenuEvent.OnEventRaised -= OpenWinMenuHandler;
            if (canInteractStartEvent) canInteractStartEvent.OnEventRaised -= CrosshairInteractableStartHandler;
            if (canInteractStopEvent) canInteractStopEvent.OnEventRaised -= CrosshairInteractableStopHandler;
            if (spawnWarningEvent) spawnWarningEvent.OnEventRaised -= SpawnWarningHandler;
            if (itemEquippedEvent) itemEquippedEvent.OnEventRaised -= ItemEquippedHandler;
            if (dropItemEvent) dropItemEvent.OnEventRaised -= DropItemEventHandler;
        }

    }

}