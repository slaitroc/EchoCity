using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class RadialMenu : MonoBehaviour
{
    [SerializeField] private UIDocument hudDocument;

    [Tooltip("Optional: offset in degrees so that the first item is at the top (-90) instead of on the right (0).")]
    [SerializeField] private float startAngleDegrees = -90f;

    private VisualElement _root;
    private VisualElement _radialCenter;

    private void Awake()
    {
        _root = hudDocument.rootVisualElement;
        _radialCenter = _root.Q<VisualElement>("RadialCenter");

        // Re-layout when geometry is ready or changes
        _radialCenter.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        LayoutItems();
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
