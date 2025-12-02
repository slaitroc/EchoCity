using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using EchoCity;
using System.Collections;


public class RadialMenuController : MonoBehaviour
{
    [SerializeField] private UIDocument hudDocument;
    [SerializeField] private PlayerInventory playerInventory;
    [Tooltip("Optional: offset in degrees so that the first item is at the top (-90) instead of on the right (0).")]
    [SerializeField] private float startAngleDegrees = -90f;

    private VisualElement _root;
    private VisualElement _radialRoot;
    private VisualElement _radialCenter;
    private VisualElement _infoPanel;
    private Label _infoText;

    private Camera _camera;
    private bool _isOpen;

    #region Keep track of runtime inventory items
    private class RuntimeItem
    {
        public InventoryItem InventoryItem;
        public VisualElement Element;
    }

    private readonly List<RuntimeItem> _runtimeItems = new();
    #endregion


    IEnumerator InitCallbacksNextFrame()
    {
        _radialRoot = _root.Q<VisualElement>("RadialMenuRoot");
        _radialCenter = _root.Q<VisualElement>("RadialCenter");
        _infoPanel = _root.Q<VisualElement>("RadialInfoPanel");
        _infoText = _root.Q<Label>("RadialInfoText");

        _radialRoot.style.display = DisplayStyle.None;
        _infoPanel.style.display = DisplayStyle.None;

        yield return null;
        _radialRoot.RegisterCallback<PointerMoveEvent>(evt =>
        {
            if (!_isOpen) return;

            VisualElement picked = _root.panel.Pick(evt.position);
            UpdateHover(picked);
        });

        _radialCenter.RegisterCallback<GeometryChangedEvent>(evt => LayoutItems());

        MethodsUI.ShowCursor();
        RebuildFromInventory();
    }


    public void OnEnable()
    {
        _root = hudDocument.rootVisualElement;
        StartCoroutine(InitCallbacksNextFrame());

        _isOpen = true;
        _radialRoot.AddToClassList("active");
        _radialRoot.style.display = DisplayStyle.Flex;
    }

    public void OnDisable()
    {
        _isOpen = false;
        _radialRoot.RemoveFromClassList("active");
        _radialRoot.style.display = DisplayStyle.None;

        ClearHover();
        _infoPanel.style.display = DisplayStyle.None;
        MethodsUI.HideCursor();
    }

    private void UpdateHover(VisualElement picked)
    {
        ClearHover();
        _infoPanel.style.display = DisplayStyle.None;

        if (picked == null) return;

        VisualElement item = picked;
        while (item != null && !item.ClassListContains("radial-item"))
            item = item.parent;

        if (item == null) return;

        item.AddToClassList("hovered");
        ShowItemDescription(item);
    }

    private void ClearHover()
    {
        List<VisualElement> items = _radialRoot
            .Query<VisualElement>(className: "radial-item")
            .ToList();

        foreach (VisualElement item in items)
            item.RemoveFromClassList("hovered");
    }

    private void ShowItemDescription(VisualElement item)
    {
        if (_infoPanel == null || _infoText == null || _radialCenter == null)
            return;

        InventoryItem invItem = item.userData as InventoryItem;
        if (invItem != null && invItem.Data != null)
        {
            _infoText.text = invItem.Data.PickableDescription;
        }
        else
        {
            _infoText.text = "Unknown item.";
        }

        // Fixed tooltip size must match USS
        float tooltipWidth = 220f;
        float tooltipHeight = 80f; // approximate height with padding

        Rect itemRect = item.worldBound;
        Rect centerRect = _radialCenter.worldBound;

        Vector2 itemCenter = itemRect.center;
        Vector2 center = centerRect.center;

        Vector2 dir = (itemCenter - center);
        if (dir.sqrMagnitude < 0.0001f)
            dir = Vector2.up;

        dir.Normalize();

        float itemRadius = Mathf.Max(itemRect.width, itemRect.height) * 0.5f;
        float padding = 10f;
        float offsetDistance = itemRadius + tooltipHeight * 0.5f + padding;

        Vector2 anchor = itemCenter + dir * offsetDistance;

        float left = anchor.x - tooltipWidth * 0.5f;
        float top = anchor.y - tooltipHeight * 0.5f;

        _infoPanel.style.left = left;
        _infoPanel.style.top = top;
        _infoPanel.style.display = DisplayStyle.Flex;
    }


    public void RebuildFromInventory()
    {
        ClearRadialItems();

        if (playerInventory == null) return;

        foreach (InventoryItem invItem in playerInventory.Items)
        {
            CreateRadialItem(invItem);
        }
    }

    private void ClearRadialItems()
    {
        // Remove existing runtime elements from UI
        foreach (RuntimeItem runtimeItem in _runtimeItems)
        {
            runtimeItem.Element.RemoveFromHierarchy();
        }

        _runtimeItems.Clear();

        // If you still have static radial-item elements in UXML and want to remove them too:
        var staticItems = _radialCenter
            .Query<VisualElement>(className: "radial-item")
            .ToList();

        foreach (var element in staticItems)
        {
            element.RemoveFromHierarchy();
        }
    }

    private void CreateRadialItem(InventoryItem invItem)
    {
        SOPickableData data = invItem.Data;
        if (data == null) return;

        VisualElement itemElement = new VisualElement();
        itemElement.AddToClassList("radial-item");

        // Store the InventoryItem (or directly the SOPickableData) in userData
        itemElement.userData = invItem;

        // Icon
        if (data.PickableIcon != null)
        {
            VisualElement iconElement = new VisualElement();
            iconElement.AddToClassList("radial-item-icon");
            iconElement.style.backgroundImage = new StyleBackground(data.PickableIcon);
            itemElement.Add(iconElement);
        }

        // Label with the item name
        Label label = new Label(data.PickableName);
        label.AddToClassList("radial-item-label");
        itemElement.Add(label);

        _radialCenter.Add(itemElement);

        _runtimeItems.Add(new RuntimeItem
        {
            InventoryItem = invItem,
            Element = itemElement
        });
    }

    private void SelectUnderPointer(Vector2 mouseScreenPos)
    {
        Vector2 panelPos = ScreenToPanelPosition(mouseScreenPos);
        VisualElement picked = _root.panel.Pick(panelPos);
        if (picked == null) return;

        VisualElement item = picked;
        while (item != null && !item.ClassListContains("radial-item"))
            item = item.parent;

        if (item != null)
        {
            InventoryItem invItem = item.userData as InventoryItem;

            if (invItem != null && invItem.Data != null)
            {
                Debug.Log($"Radial menu selected: {invItem.Data.PickableName}");
                // TODO: raise event to gameplay (e.g. use/equip this InventoryItem)
            }
        }
    }

    private Vector2 ScreenToPanelPosition(Vector2 screenPos)
    {
        if (_camera == null)
            _camera = Camera.main;

        Vector2 panelPos = screenPos;
        panelPos.y = _camera.pixelHeight - panelPos.y;
        return panelPos;
    }


    private void LayoutItems()
    {
        List<VisualElement> items = _radialCenter
            .Query<VisualElement>(className: "radial-item")
            .ToList();

        int count = items.Count;
        if (count == 0)
            return;

        float centerWidth = _radialCenter.resolvedStyle.width;
        float centerHeight = _radialCenter.resolvedStyle.height;

        float cx = centerWidth * 0.5f;
        float cy = centerHeight * 0.5f;

        // Use the smallest dimension as circle base
        float radiusBase = Mathf.Min(centerWidth, centerHeight) * 0.5f;

        // Use item size to keep them inside the circle
        float itemWidth = items[0].resolvedStyle.width;
        float itemHeight = items[0].resolvedStyle.height;
        float itemRadius = Mathf.Max(itemWidth, itemHeight) * 0.5f;

        float radius = radiusBase - itemRadius;

        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            VisualElement item = items[i];

            float angleDeg = startAngleDegrees + angleStep * i;
            float angleRad = angleDeg * Mathf.Deg2Rad;

            float x = cx + Mathf.Cos(angleRad) * radius;
            float y = cy - Mathf.Sin(angleRad) * radius; // minus because UI y grows downwards

            // Position top-left of the item so it is centered on (x, y)
            item.style.left = x - itemWidth * 0.5f;
            item.style.top = y - itemHeight * 0.5f;
        }
    }
}
