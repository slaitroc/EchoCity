using Unity.VisualScripting;
using UnityEngine;

namespace EchoCity
{
    [System.Serializable]
    public enum ShowableUIEnum
    {
        TitleMenu,
        LoadingScreen,
        HUD,
        PauseMenu,
        DeathMenu,
        WinMenu,
        Dialog,
        Warning
    }

    [System.Serializable]
    public enum HudEnum
    {
        None = 0,
        Inventory = 1
    }

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
        [SerializeField] private QuestController questController;

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
        // [SerializeField] private SOEventVoid switchToTitleStateEvent;
        // [SerializeField] private SOEventVoid switchToPlayStateEvent;
        [SerializeField] private SOSwitchLevelEvent switchLevelEvent;
        [SerializeField] private SOSwitchToGameStateEvent switchToGameStateEvent;

        public string SenderName => gameObject.name;
        public int SenderID => GetInstanceID();
        public bool IsManager => true;
        public EventSenderCategoriesEnum[] SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.UI };

        [Header("Observed Events From GM")]
        // [SerializeField] private SOEventVoid titleMenuEvent;
        // [SerializeField] private SOHudEnumEvent hudMenuEvent;
        // [SerializeField] private SOEventVoid pauseMenuEvent;
        // [SerializeField] private SOShowDialogEvent dialogMenuEvent;
        // [SerializeField] private SOEventVoid deathMenuEvent;
        // [SerializeField] private SOEventVoid enterLoadingScreenEvent;
        // [SerializeField] private SOEventVoid exitLoadingScreenEvent;
        // [SerializeField] private SOEventVoid winMenuEvent;
        [SerializeField] private SOShowUIEvent showUIEvent;


        [Header("Observed Events From Others")]
        // [SerializeField] private SOShowInteractionEvent canInteractStartEvent;
        // [SerializeField] private SOEventVoid canInteractStopEvent;
        // [SerializeField] private SOStringColorEvent spawnWarningEvent;
        [SerializeField] private SOEquippedItemChanged equippedItemChanged;
        [SerializeField] private SOInventoryChangedEvent inventoryChangedEvent;
        // [SerializeField] private SOIntIntEvent questsUpdatedEvent;

        [Header("External References")]
        [SerializeField] private PlayerController playerController;

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


            if (playerController == null)
            {
                Log.ELazy(() => "PlayerController reference is missing in UIManager!", this);
            }

        }

        private void OnEnable()
        {
            // if (titleMenuEvent) titleMenuEvent.OnEventRaised += OpenTitleMenuHandler;
            // if (hudMenuEvent) hudMenuEvent.OnEventRaised += OpenHUDMenuHandler;
            // if (pauseMenuEvent) pauseMenuEvent.OnEventRaised += OpenPauseMenuHandler;
            // if (dialogMenuEvent) dialogMenuEvent.OnEventRaised += OpenDialogMenuHandler;
            // if (deathMenuEvent) deathMenuEvent.OnEventRaised += OpenDeathMenuHandler;
            // if (enterLoadingScreenEvent) enterLoadingScreenEvent.OnEventRaised += OpenLoadingScreenHandler;
            // if (exitLoadingScreenEvent) exitLoadingScreenEvent.OnEventRaised += CloseLoadingScreenHandler;
            // if (winMenuEvent) winMenuEvent.OnEventRaised += OpenWinMenuHandler;
            if (showUIEvent) showUIEvent.OnEventRaised += ShowUIHandler;

            // if (canInteractStartEvent) canInteractStartEvent.OnEventRaised += CrosshairInteractableStartHandler;
            // if (canInteractStopEvent) canInteractStopEvent.OnEventRaised += CrosshairInteractableStopHandler;
            // if (spawnWarningEvent) spawnWarningEvent.OnEventRaised += SpawnWarningHandler;
            if (equippedItemChanged) equippedItemChanged.OnEventRaised += ItemEquippedHandler;
            if (inventoryChangedEvent) inventoryChangedEvent.OnEventRaised += DropItemEventHandler;
            // if (questsUpdatedEvent) questsUpdatedEvent.OnEventRaised += QuestsUpdatedEventHandler;
        }

        #region Public Methods - State Switching
        public void SwitchToPlayState()
        {
            HideAllElements();
            _hud.SetActive(true);
            switchToGameStateEvent?.RaiseEvent(this, GameStatesEnum.Playing, null);
        }

        public void SwitchToTitleState()
        {
            HideAllElements();
            _titleMenu.SetActive(true);
            switchToGameStateEvent?.RaiseEvent(this, GameStatesEnum.Title, null);
        }

        public void SwitchToInitLevel(SceneEnum scene)
        {
            HideAllElements();
            _hud.SetActive(true);
            switchLevelEvent?.RaiseEvent(this, scene, null);
        }


        public void OpenSettingsMenu() => _settingsMenu.SetActive(true);
        public void CloseSettingsMenu() => _settingsMenu.SetActive(false);
        public void OpenFeedbackMenu() => _feedbackMenu.SetActive(true);
        public void CloseFeedbackMenu() => _feedbackMenu.SetActive(false);
        public void EquipItem(int index, SOPickable pickableData, GameObject obj) => playerController.EquipItem(index, pickableData, obj);
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

        private void ShowUIHandler(IEventSender sender, ShowableUIEnum uiElement, EventParams eventParams)
        {
            switch (uiElement)
            {
                case ShowableUIEnum.TitleMenu:
                    HideAllElements();
                    _titleMenu.SetActive(true);
                    break;
                case ShowableUIEnum.HUD:
                    var hudParams = eventParams as HudParams; // currently not used
                    radialMenuController.enabled = !radialMenuController.enabled;
                    crosshairController.enabled = !crosshairController.enabled;
                    break;
                case ShowableUIEnum.PauseMenu:
                    HideAllElements();
                    _pauseMenu.SetActive(true);
                    break;
                case ShowableUIEnum.Dialog:
                    HideAllElements();
                    _dialog.SetActive(true);
                    break;
                case ShowableUIEnum.DeathMenu:
                    HideAllElements();
                    _deathMenu.SetActive(true);
                    break;
                case ShowableUIEnum.LoadingScreen:
                    var loadingParams = eventParams as LoadingParams;
                    _loadingScreen.SetActive(loadingParams != null && loadingParams.IsLoading);
                    break;
                case ShowableUIEnum.WinMenu:
                    HideAllElements();
                    _winMenu.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        private void HideAllElements()
        {
            _titleMenu.SetActive(false);
            _hud.SetActive(false);
            _pauseMenu.SetActive(false);
            _dialog.SetActive(false);
            _deathMenu.SetActive(false);
            _loadingScreen.SetActive(false);
            _winMenu.SetActive(false);
        }

        private void OpenLoadingScreenHandler(IEventSender sender) => _loadingScreen.SetActive(true);
        private void CloseLoadingScreenHandler(IEventSender sender) => _loadingScreen.SetActive(false);

        private void CrosshairInteractableStartHandler(IEventSender sender, bool isInteractable, string text) => crosshairController.IsInteractable(true, isInteractable, text);
        private void CrosshairInteractableStopHandler(IEventSender sender) => crosshairController.IsInteractable(false);
        private void SpawnWarningHandler(IEventSender sender, string warningText, Color color) => warningController.SpawnWarning(warningText, color);
        private void ItemEquippedHandler(IEventSender sender) => equippedPanelController.SetEquippedItem(playerController.equippedItem.Data.Icon, playerController.equippedItem.Data.Name);
        private void DropItemEventHandler(IEventSender sender, PickablesEnum pickable, PickableTypeEnum pickableType, InventoryCodesEnum code)
        {
            if (code == InventoryCodesEnum.ItemDropped && playerController.equippedItem != null && playerController.equippedItem.Data.PickableEnum == pickable)
                equippedPanelController.ClearEquipped();
        }
        private void QuestsUpdatedEventHandler(IEventSender sender, int questID, int progression) => questController.UpdateQuest((QuestsEnum)questID, progression);


        #endregion
        private void OnDisable()
        {
            // if (titleMenuEvent) titleMenuEvent.OnEventRaised -= OpenTitleMenuHandler;
            // if (hudMenuEvent) hudMenuEvent.OnEventRaised -= OpenHUDMenuHandler;
            // if (pauseMenuEvent) pauseMenuEvent.OnEventRaised -= OpenPauseMenuHandler;
            // if (dialogMenuEvent) dialogMenuEvent.OnEventRaised -= OpenDialogMenuHandler;
            // if (deathMenuEvent) deathMenuEvent.OnEventRaised -= OpenDeathMenuHandler;
            // if (enterLoadingScreenEvent) enterLoadingScreenEvent.OnEventRaised -= OpenLoadingScreenHandler;
            // if (exitLoadingScreenEvent) exitLoadingScreenEvent.OnEventRaised -= CloseLoadingScreenHandler;
            // if (winMenuEvent) winMenuEvent.OnEventRaised -= OpenWinMenuHandler;
            // if (canInteractStartEvent) canInteractStartEvent.OnEventRaised -= CrosshairInteractableStartHandler;
            // if (canInteractStopEvent) canInteractStopEvent.OnEventRaised -= CrosshairInteractableStopHandler;
            // if (spawnWarningEvent) spawnWarningEvent.OnEventRaised -= SpawnWarningHandler;
            if (equippedItemChanged) equippedItemChanged.OnEventRaised -= ItemEquippedHandler;
            if (inventoryChangedEvent) inventoryChangedEvent.OnEventRaised -= DropItemEventHandler;
            // if (questsUpdatedEvent) questsUpdatedEvent.OnEventRaised -= QuestsUpdatedEventHandler;
        }

    }

}