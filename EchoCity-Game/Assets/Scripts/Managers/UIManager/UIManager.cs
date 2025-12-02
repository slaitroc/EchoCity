using UnityEngine;
using EchoCity;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    [SerializeField] private PauseMenuController _pauseMenuController;

    [Header("HUD")]
    [SerializeField] private CrosshairController _crosshairController;
    [SerializeField] private RadialMenuController _radialMenuController;

    [Header("Invoking Events")]
    [SerializeField] private SOEventVoid enablePlayerActionMapEvent;
    [SerializeField] private SOEventVoid disablePlayerActionMapEvent;
    [SerializeField] private SOEventVoid disableUIActionMapEvent;
    [SerializeField] private SOEventVoid enableUIActionMapEvent;

    private GameObject _pauseMenu;
    private GameObject HUD;

    private PlayerInventory _playerInventory;


    void Awake()
    {
        _pauseMenu = _pauseMenuController.gameObject;
        HUD = _crosshairController.gameObject;

        _playerInventory = _crosshairController.GetComponent<PlayerInventory>();
    }

    public void PauseMenuHandler() => _pauseMenu.SetActive(!_pauseMenu.activeSelf);
    public void HUDInteractableHandler() => _crosshairController.IsInteractable(!_crosshairController.isInteractable);
    public void RadialMenuOpenHandler() => _radialMenuController.enabled = true;
    public void RadialMenuCloseHandler() => _radialMenuController.enabled = false;
    public void AddInventoryItemHandler(SOPickableData data) => _playerInventory.AddItem(data);

    public void RemoveInventoryItemHandler(InventoryItem item)
    {
        // _playerInventory.RemoveItem(item);
        Log.D("Remove Inventory Item Handler", "green", "UI MANAGER");
    }

    public void RebuildRadialMenuHandler()
    {
        _radialMenuController.RebuildFromInventory();
        Log.D("Rebuild Radial Menu Handler", "green", "UI MANAGER");
    }


    public void DisablePlayerActionMap() => disablePlayerActionMapEvent?.RaiseEvent();
    public void DisableUIActionMap() => disableUIActionMapEvent?.RaiseEvent();
    public void EnableUIActionMap() => enableUIActionMapEvent?.RaiseEvent();
    public void EnablePlayerActionMap() => enablePlayerActionMapEvent?.RaiseEvent();
}
