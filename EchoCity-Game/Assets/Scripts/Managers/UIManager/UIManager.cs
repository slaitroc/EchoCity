using UnityEngine;
using EchoCity;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
#pragma warning disable CS0414
    private const string _LOG_COLOR = "cyan";
    private const string _LOG_TAG_FULL = "UI MANAGER";
#pragma warning restore CS0414
    [Header("Input")]
    [SerializeField] private UIInput uiInput;

    [Header ("Title Menu")]
    [SerializeField] private TitleMenuController titleMenuController;

    [Header("HUD")]
    [SerializeField] private CrosshairController crosshairController;
    [SerializeField] private RadialMenuController radialMenuController;
    [SerializeField] private WarningController warningController;

    [Header("Pause Menu")]
    [SerializeField] private PauseMenuController pauseMenuController;

    [Header("Dialogs")]
    [SerializeField] private DialogController dialogController;


    [Header("Invoking Events")]
    [SerializeField] private SOEventVoid enablePlayerActionMapEvent;
    // [SerializeField] private SOEventVoid disablePlayerActionMapEvent;

    private GameObject _titleMenu;
    private GameObject _hud;
    private GameObject _pauseMenu;
    private GameObject _dialog;

    [SerializeField] private PlayerInventory _playerInventory;

    void Awake()
    {
        _titleMenu = titleMenuController.gameObject;
        _hud = crosshairController.gameObject;
        _pauseMenu = pauseMenuController.gameObject;
        _dialog = dialogController.gameObject;

        if (_playerInventory == null)
        {
            Log.E("PlayerInventory reference is missing in UIManager!", _LOG_COLOR, _LOG_TAG_FULL);
        }
    }

    public void PauseMenuHandler() => _pauseMenu.SetActive(!_pauseMenu.activeSelf);
    public void HUDInteractableHandler() => crosshairController.IsInteractable(!crosshairController.isInteractable);
    public void OpenRadialMenuHandler() => radialMenuController.enabled = true;
    public void CloseRadialMenuHandler() => radialMenuController.enabled = false;

    public void RemoveInventoryItemHandler(InventoryItem item)
    {
        Log.D("Remove Inventory Item Handler", "green", "UI MANAGER");
    }

    public void RebuildRadialMenuHandler()
    {
        radialMenuController.RebuildFromInventory();
    }

    public void SpawnWarningHandler(string warningText, Color color)
    {
        warningController.SpawnWarning(warningText, color);
    }
    
    public void SpawnDialogHandler(DialogData dialogData)
    {
        dialogController.gameObject.SetActive(true);
        dialogController.SpawnDialogHandler(dialogData);
    }

    // In the following handlers we enable/disable the relevant UI elements
    // The element to be enabled must be enabled after disabling others to ensure proper activation of the UI Action Map
    public void StartGameHandler()
    {
        _titleMenu.SetActive(false);
        _pauseMenu.SetActive(false);
        _dialog.SetActive(false);
        
        _hud.SetActive(true);
    }

    public void QuitToTitleHandler()
    {
        _hud.SetActive(false);
        _pauseMenu.SetActive(false);
        _dialog.SetActive(false);

        _titleMenu.SetActive(true);
    }

    public void RestartGameHandler()
    {
        _titleMenu.SetActive(false);
        _pauseMenu.SetActive(false);
        _dialog.SetActive(false);
        
        _hud.SetActive(true);
    }



    public void EnablePlayerActionMap() => enablePlayerActionMapEvent?.RaiseEvent();
    public void EnableUIActionMap() => uiInput.EnableUIActionMap();
    public void DisableUIActionMap() => uiInput.DisableUIActionMap();
}
