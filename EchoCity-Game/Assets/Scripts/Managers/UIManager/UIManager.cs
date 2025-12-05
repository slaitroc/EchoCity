using UnityEngine;
using EchoCity;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
#pragma warning disable CS0414
    private const string _LOG_COLOR = "cyan";
    private const string _LOG_TAG_FULL = "UI MANAGER";
#pragma warning restore CS0414

    [SerializeField] private UIInput _uiInput;
    [SerializeField] private PauseMenuController _pauseMenuController;

    [Header("HUD")]
    [SerializeField] private CrosshairController _crosshairController;
    [SerializeField] private RadialMenuController _radialMenuController;
    [SerializeField] private WarningController warningController;
    [SerializeField] private DialogController dialogController;


    [Header("Invoking Events")]
    [SerializeField] private SOEventVoid enablePlayerActionMapEvent;
    // [SerializeField] private SOEventVoid disablePlayerActionMapEvent;

    private GameObject _pauseMenu;
    private GameObject HUD;

    [SerializeField] private PlayerInventory _playerInventory;

    void Awake()
    {
        _pauseMenu = _pauseMenuController.gameObject;
        HUD = _crosshairController.gameObject;

        if (_playerInventory == null)
        {
            Log.E("PlayerInventory reference is missing in UIManager!", _LOG_COLOR, _LOG_TAG_FULL);
        }
    }

    public void PauseMenuHandler() => _pauseMenu.SetActive(!_pauseMenu.activeSelf);
    public void HUDInteractableHandler() => _crosshairController.IsInteractable(!_crosshairController.isInteractable);
    public void OpenRadialMenuHandler() => _radialMenuController.enabled = true;
    public void CloseRadialMenuHandler() => _radialMenuController.enabled = false;

    public void RemoveInventoryItemHandler(InventoryItem item)
    {
        Log.D("Remove Inventory Item Handler", "green", "UI MANAGER");
    }

    public void RebuildRadialMenuHandler()
    {
        _radialMenuController.RebuildFromInventory();
        Log.D("Rebuild Radial Menu Handler", "green", "UI MANAGER");
    }

    public void SpawnWarningHandler(string warningText, Color color)
    {
        Log.D("Spawn Warning Handler", "green", "UI MANAGER");
        warningController.SpawnWarning(warningText, color);
    }
    
    public void SpawnDialogHandler(DialogData dialogData)
    {
        dialogController.gameObject.SetActive(true);
        dialogController.SpawnDialogHandler(dialogData);
    }



    public void EnablePlayerActionMap() => enablePlayerActionMapEvent?.RaiseEvent();
    public void EnableUIActionMap() => _uiInput.EnableUIActionMap();
    public void DisableUIActionMap() => _uiInput.DisableUIActionMap();
}
