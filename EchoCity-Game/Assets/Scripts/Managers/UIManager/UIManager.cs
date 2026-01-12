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
        Narration,
        PopUpMessage,
        Subtitles,
    }

    [System.Serializable]
    public enum HudEnum
    {
        None = 0,
        Inventory = 1,
        Tutorial = 2
    }

    public class UIManager : MonoBehaviour, IEventSender
    {
        [Header("UI Controllers")]

        [Header("Title Menu")]
        [SerializeField] private TitleMenuController titleMenuController;

        [Header("HUD")]
        [SerializeField] private CrosshairController crosshairController;
        [SerializeField] private RadialMenuController radialMenuController;
        [SerializeField] private PopUpController popUpController;
        [SerializeField] private EquippedPanelController equippedPanelController;
        [SerializeField] private TutorialPanelController tutorialPanelController;
        [SerializeField] private QuestController questController;

        [Header("Pause Menu")]
        [SerializeField] private PauseMenuController pauseMenuController;

        [Header("Settings Menu")]
        [SerializeField] private SettingsMenuController settingsMenuController;

        [Header("Dialogs")]
        [SerializeField] private DialogController dialogController;
        [Header("Subtitle")]
        [SerializeField] private SubtitlesController subtitlesController;
        [SerializeField] private float _subtitlesBottomGapPx = 10f;

        [Header("Death Menu")]
        [SerializeField] private DeathMenuController deathMenuController;

        [Header("Loading Screen")]
        [SerializeField] private LoadingScreenController loadingScreenController;
        [SerializeField] private FeedbackMenuController feedbackMenuController;

        [Header("Win Menu")]
        [SerializeField] private WinMenuController winMenuController;

        [Header("Narration")]
        [SerializeField] private NarrationController narrationController;

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
        [SerializeField] private SOEquippedItemChangedEvent equippedItemChanged;
        [SerializeField] private SOInventoryChangedEvent inventoryChangedEvent;
        [SerializeField] private SOQuestUpdatedEvent questUpdatedEvent;
        [SerializeField] private SOTimerEvent timerEvent;

        [Header("External References")]
        [SerializeField] private PlayerController playerController;

        #region Private Fields
        private GameObject _titleMenu;
        private GameObject _hud;
        private GameObject _pauseMenu;
        private GameObject _settingsMenu;
        private GameObject _dialog;
        private GameObject _subtitles;
        private GameObject _deathMenu;
        private GameObject _loadingScreen;
        private GameObject _feedbackMenu;
        private GameObject _winMenu;
        private GameObject _narration;

        private bool _showTutorial = true;

        #endregion
        #region Public Properties
        public bool IsPauseMenuActive => _pauseMenu.activeSelf;
        #endregion

        #region Test and Debug 

        #endregion

        private void Awake()
        {
            _titleMenu = titleMenuController.gameObject;
            _hud = crosshairController.gameObject;
            _pauseMenu = pauseMenuController.gameObject;
            _settingsMenu = settingsMenuController.gameObject;
            _dialog = dialogController.gameObject;
            _subtitles = subtitlesController.gameObject;
            _deathMenu = deathMenuController.gameObject;
            _loadingScreen = loadingScreenController.gameObject;
            _feedbackMenu = feedbackMenuController.gameObject;
            _winMenu = winMenuController.gameObject;
            _narration = narrationController.gameObject;


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
            if (timerEvent) timerEvent.OnEventRaised += TimerEventHandler;
        }

        #region Public Methods
        public void SwitchToPlayState()
        {
            HideAllElements();
            _hud.SetActive(true);
            _subtitles.SetActive(true);
            tutorialPanelController.ShowHideLines(_showTutorial);
            EquippedItemHandler(this, PickablesEnum.None);
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
            _subtitles.SetActive(true);
            tutorialPanelController.ShowHideLines(_showTutorial);
            EquippedItemHandler(this, PickablesEnum.None);
            switchLevelEvent?.RaiseEvent(this, scene, null);
        }


        public void OpenSettingsMenu() => _settingsMenu.SetActive(true);
        public void CloseSettingsMenu() => _settingsMenu.SetActive(false);
        public void OpenFeedbackMenu() => _feedbackMenu.SetActive(true);
        public void CloseFeedbackMenu() => _feedbackMenu.SetActive(false);
        public void EquipItem(int index, SOPickable pickableData, GameObject obj) => playerController.EquipItem(index, pickableData, obj);
        public void PlayNextNarrationLine(int index) => EchoCitySound.PlayNarrationLine(index);
        public void StopNarration() => EchoCitySound.StopNarration();
        public void ShowPlaygroundButton()
        {
            if (_titleMenu.activeSelf) titleMenuController.ShowPlaygroundButton();
            if (_pauseMenu.activeSelf) pauseMenuController.ShowPlaygroundButton();
            if (_deathMenu.activeSelf) deathMenuController.ShowPlaygroundButton();
        }
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
                    var hudParams = eventParams as HudParams;
                    switch (hudParams.HudState)
                    {
                        case HudEnum.Inventory:
                            radialMenuController.enabled = !radialMenuController.enabled;
                            crosshairController.enabled = !crosshairController.enabled;
                            tutorialPanelController.enabled = !tutorialPanelController.enabled;
                            break;
                        case HudEnum.Tutorial:
                            _showTutorial = !_showTutorial;
                            tutorialPanelController.ShowHideLines(_showTutorial);
                            break;
                        default:
                            Log.DLazy(() => $"HUD State {hudParams.HudState} not handled in UIManager!", this);
                            break;
                    }
                    break;
                case ShowableUIEnum.PauseMenu:
                    HideAllElements(narration: true);
                    _pauseMenu.SetActive(true);
                    break;
                case ShowableUIEnum.Narration:
                    var narrationParams = eventParams as NarrationParams;
                    HideAllElements();
                    _narration.SetActive(true);
                    narrationController.StartNarration(narrationParams);
                    if (!narrationParams.UseCached) EchoCitySound.PlayNarration(narrationParams.NarrationContainer, timerEvent, sender, eventTime: 1f);
                    break;
                case ShowableUIEnum.Subtitles:
                    var subtitleParams = eventParams as SubtitleParams;
                    subtitlesController.ShowSubtitle(subtitleParams.Subtitle, subtitleParams.Duration, subtitleParams.SpeakerName);
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
                case ShowableUIEnum.PopUpMessage:
                    var popUpParams = eventParams as PopUpMessageParams;
                    popUpController.SpawnPopUp(popUpParams.Message, popUpParams.Color);
                    break;
                default:
                    break;
            }
        }

        private void HideAllElements(bool title = false, bool hud = false, bool pause = false, bool dialog = false,
            bool subtitles = false, bool death = false, bool loading = false, bool win = false, bool narration = false)
        {
            if (!title) _titleMenu.SetActive(false);
            if (!hud) _hud.SetActive(false);
            if (!pause) _pauseMenu.SetActive(false);
            if (!dialog) _dialog.SetActive(false);
            if (!subtitles) _subtitles.SetActive(false);
            if (!death) _deathMenu.SetActive(false);
            if (!loading) _loadingScreen.SetActive(false);
            if (!win) _winMenu.SetActive(false);
            if (!narration) _narration.SetActive(false);
        }


        private void ShowInteractionHandler(IEventSender sender, bool isInteractable, bool showDescription, string text)
        {
            crosshairController.IsInteractable(showDescription, isInteractable, text);
            if (showDescription)
                subtitlesController.ApplyOffset(crosshairController.InteractionPanelHeight + _subtitlesBottomGapPx);
            else
                subtitlesController.ApplyOffset(0f);
        }
        private void EquippedItemHandler(IEventSender sender, PickablesEnum newEquippedItem) => equippedPanelController.SetEquippedItem(playerController.equippedItem.Data.Icon, playerController.equippedItem.Data.Name);
        private void InventoryChangedHandler(IEventSender sender, PickablesEnum pickable, PickableTypeEnum pickableType, InventoryCodesEnum code)
        {
            if (code == InventoryCodesEnum.ItemDropped) equippedPanelController.ClearEquipped();
        }
        private void QuestUpdatedEventHandler(IEventSender sender, int questID, int progression) => questController.UpdateQuest((QuestsEnum)questID, progression);
        private void TimerEventHandler(IEventSender sender, TimerEventEnum timerEventEnum)
        {
            switch (timerEventEnum)
            {
                case TimerEventEnum.NarrationLineHalfway:
                    narrationController.StartFadeOut();
                    break;

                case TimerEventEnum.NarrationLineEnded:
                    narrationController.NextDialog();
                    break;

                default:
                    break;
            }
        }
        #endregion

        private void OnDisable()
        {
            if (showUIEvent) showUIEvent.OnEventRaised -= ShowUIHandler;

            if (showInteractionEvent) showInteractionEvent.OnEventRaised -= ShowInteractionHandler;
            if (equippedItemChanged) equippedItemChanged.OnEventRaised -= EquippedItemHandler;
            if (inventoryChangedEvent) inventoryChangedEvent.OnEventRaised -= InventoryChangedHandler;
            if (questUpdatedEvent) questUpdatedEvent.OnEventRaised -= QuestUpdatedEventHandler;
            if (timerEvent) timerEvent.OnEventRaised -= TimerEventHandler;
        }

    }

}