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
        [SerializeField] private SOSwitchToGameStateEvent switchToGameStateEvent;
        [SerializeField] private SOSwitchLevelEvent switchLevelEvent;

        public string SenderName => gameObject.name;
        public int SenderID => GetInstanceID();
        public bool IsManager => true;
        public EventSenderCategoriesEnum[] SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.UI };

        [Header("Observed Events From GM")]
        [SerializeField] private SOShowUIEvent showUIEvent;


        [Header("Observed Events From Others")]
        [SerializeField] private SOShowInteractionEvent showInteractionEvent;
        [SerializeField] private SOEquippedItemChanged equippedItemChanged;
        [SerializeField] private SOInventoryChangedEvent inventoryChangedEvent;
        [SerializeField] private SOQuestUpdatedEvent questUpdatedEvent;

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
        #region Public Properties
        public bool IsPauseMenuActive => _pauseMenu.activeSelf;
        #endregion

        private void Awake()
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
            if (showUIEvent) showUIEvent.OnEventRaised += ShowUIHandler;

            if (showInteractionEvent) showInteractionEvent.OnEventRaised += ShowInteractionHandler;
            if (equippedItemChanged) equippedItemChanged.OnEventRaised += EquippedItemHandler;
            if (inventoryChangedEvent) inventoryChangedEvent.OnEventRaised += InventoryChangedHandler;
            if (questUpdatedEvent) questUpdatedEvent.OnEventRaised += QuestUpdatedEventHandler;
        }

        #region Public Methods
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
                case ShowableUIEnum.Warning:
                    var warningParams = eventParams as WarningParams;
                    warningController.SpawnWarning(warningParams.Message, warningParams.Color);
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


        private void ShowInteractionHandler(IEventSender sender, bool isInteractable, bool showDescription, string text) => crosshairController.IsInteractable(showDescription, isInteractable, text);
        private void EquippedItemHandler(IEventSender sender) => equippedPanelController.SetEquippedItem(playerController.equippedItem.Data.Icon, playerController.equippedItem.Data.Name);
        private void InventoryChangedHandler(IEventSender sender, PickablesEnum pickable, PickableTypeEnum pickableType, InventoryCodesEnum code)
        {
            if (code == InventoryCodesEnum.ItemDropped) equippedPanelController.ClearEquipped();
        }
        private void QuestUpdatedEventHandler(IEventSender sender, int questID, int progression) => questController.UpdateQuest((QuestsEnum)questID, progression);

        #endregion

        private void OnDisable()
        {
            if (showUIEvent) showUIEvent.OnEventRaised -= ShowUIHandler;

            if (showInteractionEvent) showInteractionEvent.OnEventRaised -= ShowInteractionHandler;
            if (equippedItemChanged) equippedItemChanged.OnEventRaised -= EquippedItemHandler;
            if (inventoryChangedEvent) inventoryChangedEvent.OnEventRaised -= InventoryChangedHandler;
            if (questUpdatedEvent) questUpdatedEvent.OnEventRaised -= QuestUpdatedEventHandler;
        }

    }

}