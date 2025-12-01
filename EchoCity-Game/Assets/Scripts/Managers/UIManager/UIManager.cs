using UnityEngine;
using EchoCity;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private HUD hud;

    private RadialMenuController _radialMenuController;
    private PlayerInventory _playerInventory;


    void Awake()
    {
        _radialMenuController = hud.GetComponent<RadialMenuController>();
        _playerInventory = hud.GetComponent<PlayerInventory>();
    }

    public void PauseMenuHandler()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }

    public void HUDInteractableHandler()
    {
        hud.IsInteractable(!hud.isInteractable);
        Log.D("HUD Interactable Handler", "green", "UI MANAGER");
    }

    public void RadialMenuOpenHandler()
    {
        _radialMenuController.OpenMenu();
        Log.D("Radial Menu Open Handler", "green", "UI MANAGER");
    }
    public void RadialMenuCloseHandler()
    {
        _radialMenuController.CloseMenu();
        Log.D("Radial Menu Close Handler", "green", "UI MANAGER");
    }

    public void AddInventoryItemHandler(SOPickableData data)
    {
        _playerInventory.AddItem(data);
        Log.D("Add Inventory Item Handler", "green", "UI MANAGER");
    }
    
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
    
}
