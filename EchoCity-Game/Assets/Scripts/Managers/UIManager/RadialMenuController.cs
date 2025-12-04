using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using EchoCity;
using System.Collections;
using Unity.VisualScripting;


public class RadialMenuController : MonoBehaviour
{   
    [Header("Needed scripts")]
    [SerializeField] private UIDocument hudDocument;
    [SerializeField] private PlayerInventory playerInventory;

    [Header("Invoking Events")]
    [SerializeField] private SOIntegerPickableDataGameObjectEvent equipItemEvent;


    #region Private Fields UI
    private VisualElement _root;
    private VisualElement _crosshair;
    private VisualElement _radialRoot;
    private VisualElement _radialCenter;
    private VisualElement _infoPanel;
    
    private Label _infoTitle;
    private Label _infoText;
    #endregion
    #region Private Fields Inventory
    private InventoryItem[] _inventoryItems;
    private GameObject[] _itemsPrefabs;
    private Button[] _radialItems;
    private int _selectedIndex;
    private InventoryItem _selectedItem;
    private GameObject _selectedPrefab;
    #endregion
    #region Private Fields Other
    private Camera _camera;
    private bool _isOpen;

    #endregion

    private void Awake()
    {
        if (playerInventory == null)
        {
            Log.E("PlayerInventory reference is missing in RadialMenuController!", "#ff0000", "RADIAL MENU");
        }
    }

    IEnumerator InitCallbacksNextFrame()
    {
        _crosshair = _root.Q<VisualElement>("crosshair");
        _radialRoot = _root.Q<VisualElement>("RadialMenuRoot");
        _radialCenter = _root.Q<VisualElement>("RadialCenter");
        _infoPanel = _root.Q<VisualElement>("RadialInfoPanel");
        _infoTitle = _root.Q<Label>("RadialInfoTitle");
        _infoText = _root.Q<Label>("RadialInfoText");

        for (int i=0; i<_inventoryItems.Length; i++)
        {
            int itemIdx = i; // Capture index for the lambda

            _radialItems[i] = _radialCenter.Q<Button>($"Item{i}");
            if (_radialItems[i] == null)
            {
                Log.E($"Radial item UI element Item{i} not found in RadialMenuController", "#ff0000", "RADIAL MENU");
            }
            _radialItems[i].RegisterCallback<MouseEnterEvent>(evt =>
            {
                var hoveredItem = evt.currentTarget as Button;
                OnHover(hoveredItem, itemIdx);
            });
            _radialItems[i].RegisterCallback<MouseLeaveEvent>(evt =>
            {
                ClearHover();
            });
        }

        _radialRoot.style.display = DisplayStyle.None;
        _infoPanel.style.display = DisplayStyle.None;
        RebuildFromInventory(); // moved here before the yield because otherwise the menu would open with all the items for a frame

        yield return null;
    }


    public void OnEnable()
    {
        _root = hudDocument.rootVisualElement;
        _inventoryItems = playerInventory.Items.ToArray();
        _itemsPrefabs = playerInventory.Prefabs.ToArray();
        _radialItems = new Button[_inventoryItems.Length];
        StartCoroutine(InitCallbacksNextFrame());

        _isOpen = true;
        _selectedIndex = -1;
        _selectedItem = null;
        _selectedPrefab = null;
        _radialRoot.AddToClassList("active");
        _radialRoot.style.display = DisplayStyle.Flex;
        _infoPanel.style.display = DisplayStyle.Flex;
        _crosshair.style.display = DisplayStyle.None;
    }

    public void OnDisable()
    {
        if (_selectedIndex != -1) equipItemEvent.RaiseEvent(_selectedIndex, _selectedItem.Data, _selectedPrefab);
        
        _isOpen = false;
        _radialRoot.RemoveFromClassList("active");
        _radialRoot.style.display = DisplayStyle.None;
        _infoPanel.style.display = DisplayStyle.None;
        _crosshair.style.display = DisplayStyle.Flex;
        MethodsUI.HideCursor();
    }
    

    void Update()
    {
        MethodsUI.SetCursorState(true);
    }

    private void OnHover(Button item, int idx)
    {
        _selectedIndex = idx;
        _selectedItem = _inventoryItems[idx];
        _selectedPrefab = _itemsPrefabs[idx];
        ShowItemDescription(item);
    }


    private void ClearHover()
    {
        _selectedItem = null;
        _selectedPrefab = null;
        _selectedIndex = -1;
        ShowItemDescription(null, clear: true);
    }

    private void ShowItemDescription(VisualElement item, bool clear = false)
    {
        if (_infoPanel == null || _infoText == null || _radialCenter == null)
            return;
        
        if (item != null)
        {
            InventoryItem invItem = item.userData as InventoryItem;
            _infoTitle.text = invItem.Data.Name;
            _infoText.text = invItem.Data.Description;
        }
        else if (!clear)
        {
            _infoTitle.text = "Unknown Item";
            _infoText.text = "";
        }
        else
        {
            _infoTitle.text = "No Item Selected";
            _infoText.text = "Select an Item and release the button to equip";
        }
    }


    public void RebuildFromInventory()
    {
        if (playerInventory == null) return;

        for (int i = 0; i<_inventoryItems.Length; i++)
        {
            UpdateRadialItem(_inventoryItems[i], i);
        }
    }


    private void UpdateRadialItem(InventoryItem invItem, int idx)
    {
        Button itemButton = _radialItems[idx];

        if (invItem == null)
        {
            itemButton.style.display = DisplayStyle.None;
            return;
        }
        else
        {
            PickableData data = invItem.Data;
            itemButton.userData = invItem;

            if(data.Icon != null){
                itemButton.style.backgroundImage = new StyleBackground(data.Icon); 
                itemButton.Q<Label>().text = "";
            }
                
            itemButton.style.display = DisplayStyle.Flex;
        }
    }

}
