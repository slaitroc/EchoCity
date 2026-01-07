using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

namespace EchoCity
{
    public class RadialMenuController : MonoBehaviour, IEventSender
    {
        [Header("UI")]
        [SerializeField] private UIDocument hudDocument;
        [SerializeField] UIManager uiManager;
        [SerializeField] private PlayerInventory playerInventory;
        [Header("Sound Tool Info")]
        [Header("Sound Class Icon")]
        [SerializeField] private Sprite lowFreqIcon;
        [SerializeField] private Sprite midFreqIcon;
        [SerializeField] private Sprite highFreqIcon;
        [SerializeField] private Sprite transientIcon;
        [Header("Tools Tutorial Info")]
        [TextArea(3, 10)]
        [SerializeField] private string tutorialTextLowFreq;
        [TextArea(3, 10)]
        [SerializeField] private string tutorialTextMidFreq;
        [TextArea(3, 10)]
        [SerializeField] private string tutorialTextHighFreq;
        [TextArea(3, 10)]
        [SerializeField] private string tutorialTextTransient;
        [TextArea(3, 10)]
        [SerializeField] private string tutorialTextRange;

        string IEventSender.SenderName => gameObject.name;
        int IEventSender.SenderID => GetInstanceID();
        bool IEventSender.IsManager => false;
        EventSenderCategoriesEnum[] IEventSender.SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.UI };


        #region Private Fields UI
        private VisualElement _root;
        private VisualElement _crosshair;
        private VisualElement _radialRoot;
        private VisualElement _radialCenter;
        private VisualElement _infoPanel;
        private Label _infoTitle;
        private Label _infoText;
        private VisualElement _infoProperties;
        private VisualElement _infoClassIcon;
        private VisualElement _infoTransientIcon;
        private VisualElement _infoRangeFill;
        private VisualElement _toolsTutorialPanel;
        private VisualElement _toolsTutIconLow;
        private VisualElement _toolsTutIconMid;
        private VisualElement _toolsTutIconHigh;
        private VisualElement _toolsTutIconTransient;
        private Label _toolsTutTextLow;
        private Label _toolsTutTextMid;
        private Label _toolsTutTextHigh;
        private Label _toolsTutTextTransient;
        private Label _toolsTutTextRange;

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
#pragma warning disable CS0414
        private Camera _camera;
        private bool _isOpen;
#pragma warning restore CS0414
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
            _infoProperties = _root.Q<VisualElement>("RadialInfoProperties");
            _infoClassIcon = _root.Q<VisualElement>("RadialInfoClassIcon");
            _infoTransientIcon = _root.Q<VisualElement>("RadialInfoTransientIcon");
            _infoRangeFill = _root.Q<VisualElement>("RadialInfoRangeFill");

            _toolsTutorialPanel = _root.Q<VisualElement>("ToolsTutorialPanel");
            _toolsTutIconLow = _root.Q<VisualElement>("ToolsTutIconLow");
            _toolsTutIconMid = _root.Q<VisualElement>("ToolsTutIconMid");
            _toolsTutIconHigh = _root.Q<VisualElement>("ToolsTutIconHigh");
            _toolsTutIconTransient = _root.Q<VisualElement>("ToolsTutIconTransient");
            _toolsTutTextLow = _root.Q<Label>("ToolsTutTextLow");
            _toolsTutTextMid = _root.Q<Label>("ToolsTutTextMid");
            _toolsTutTextHigh = _root.Q<Label>("ToolsTutTextHigh");
            _toolsTutTextTransient = _root.Q<Label>("ToolsTutTextTransient");
            _toolsTutTextRange = _root.Q<Label>("ToolsTutTextRange");



            for (int i = 0; i < _inventoryItems.Length; i++)
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

            if (_infoProperties != null)
                _infoProperties.style.display = DisplayStyle.None; // Hide properties until an item is selected

            if (_toolsTutIconLow != null) _toolsTutIconLow.style.backgroundImage = lowFreqIcon != null ? new StyleBackground(lowFreqIcon) : StyleKeyword.None;
            if (_toolsTutIconMid != null) _toolsTutIconMid.style.backgroundImage = midFreqIcon != null ? new StyleBackground(midFreqIcon) : StyleKeyword.None;
            if (_toolsTutIconHigh != null) _toolsTutIconHigh.style.backgroundImage = highFreqIcon != null ? new StyleBackground(highFreqIcon) : StyleKeyword.None;
            if (_toolsTutIconTransient != null) _toolsTutIconTransient.style.backgroundImage = transientIcon != null ? new StyleBackground(transientIcon) : StyleKeyword.None;

            if (_toolsTutTextLow != null) _toolsTutTextLow.text = tutorialTextLowFreq;
            if (_toolsTutTextMid != null) _toolsTutTextMid.text = tutorialTextMidFreq;
            if (_toolsTutTextHigh != null) _toolsTutTextHigh.text = tutorialTextHighFreq;
            if (_toolsTutTextTransient != null) _toolsTutTextTransient.text = tutorialTextTransient;
            if (_toolsTutTextRange != null) _toolsTutTextRange.text = tutorialTextRange;

        }


        private void OnEnable()
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


        private void Update()
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
                UpdateInfoProperties(invItem.Data);
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
                ClearInfoProperties();
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
                SOPickable data = invItem.Data;
                itemButton.userData = invItem;

                if (data.Icon != null)
                {
                    itemButton.style.backgroundImage = new StyleBackground(data.Icon);
                    itemButton.Q<Label>().text = "";
                }

                itemButton.style.display = DisplayStyle.Flex;
            }
        }

        private void UpdateInfoProperties(SOPickable data)
        {
            if (_infoProperties == null || data.PickableType != PickableTypeEnum.SoundTool) return;

            _infoProperties.style.display = DisplayStyle.Flex;

            SOSoundClass soundClass = data.ToolSound.SoundClass;
            Sprite classIcon;
            float volume = Mathf.Clamp01(data.ToolSound.Radius / 50f);

            switch (soundClass.Frequency)
            {
                case Frequency.Low:
                    classIcon = lowFreqIcon;
                    break;
                case Frequency.Mid:
                    classIcon = midFreqIcon;
                    break;
                case Frequency.High:
                    classIcon = highFreqIcon;
                    break;
                default:
                    classIcon = null;
                    break;
            }

            if (_infoClassIcon != null)
                _infoClassIcon.style.backgroundImage = classIcon != null ? new StyleBackground(classIcon) : StyleKeyword.None;

            if (_infoTransientIcon != null && soundClass.IsTransient)
                _infoTransientIcon.style.backgroundImage = transientIcon != null ? new StyleBackground(transientIcon) : StyleKeyword.None;

            if (_infoRangeFill != null)
            {
                _infoRangeFill.style.width = Length.Percent(volume * 100f);
            }
        }

        private void ClearInfoProperties()
        {
            if (_infoProperties != null)
                _infoProperties.style.display = DisplayStyle.None;

            if (_infoClassIcon != null)
                _infoClassIcon.style.backgroundImage = StyleKeyword.None;

            if (_infoTransientIcon != null)
                _infoTransientIcon.style.backgroundImage = StyleKeyword.None;

            if (_infoRangeFill != null)
            {
                _infoRangeFill.style.width = Length.Percent(0f);
            }
        }

        public void RebuildFromInventory()
        {
            if (playerInventory == null) return;

            for (int i = 0; i < _inventoryItems.Length; i++)
            {
                UpdateRadialItem(_inventoryItems[i], i);
            }
        }


        private void OnDisable()
        {
            if (_selectedIndex != -1) uiManager.EquipItem(_selectedIndex, _selectedItem.Data, _selectedPrefab);

            _isOpen = false;
            _radialRoot.RemoveFromClassList("active");
            _radialRoot.style.display = DisplayStyle.None;
            _infoPanel.style.display = DisplayStyle.None;
            _crosshair.style.display = DisplayStyle.Flex;
            MethodsUI.HideCursor();
        }

    }
}