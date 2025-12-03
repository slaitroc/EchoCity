using UnityEngine;

/// <summary>
/// Debug display component to show enemy distance and attraction values frame by frame.
/// Attach this to the Enemy GameObject to see real-time debug information.
/// </summary>
public class EnemyAIDebugDisplay : MonoBehaviour
{
    [Header("Debug Settings")]
    [Tooltip("Enable/disable the debug display")]
    public bool showDebug = true;
    
    [Tooltip("Position offset for the debug text (in screen space)")]
    public Vector2 screenOffset = new Vector2(10, 10);
    
    [Tooltip("Font size for debug text")]
    public int fontSize = 14;
    
    private EnemyAI _enemyAI;
    private Camera _mainCamera;
    
    void Start()
    {
        _enemyAI = GetComponent<EnemyAI>();
        if (_enemyAI == null)
        {
            Debug.LogWarning("EnemyAIDebugDisplay: EnemyAI component not found!");
            enabled = false;
            return;
        }
        
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            _mainCamera = FindObjectOfType<Camera>();
        }
    }
    
    void OnGUI()
    {
        if (!showDebug || _enemyAI == null) return;
        
        // Get current values
        float attraction = _enemyAI.GetAttraction();
        float distance = 0f;
        
        if (_enemyAI.player != null)
        {
            distance = Vector3.Distance(transform.position, _enemyAI.player.position);
        }
        
        // Get current state
        string currentState = _enemyAI.CurrentState.ToString();
        
        // Create debug text
        string debugText = $"=== ENEMY DEBUG ===\n";
        debugText += $"State: {currentState}\n";
        debugText += $"Distance: {distance:F2}m\n";
        debugText += $"Attraction: {attraction:F3}\n";
        debugText += $"HasConfirmedPlayer: {_enemyAI.HasConfirmedPlayer}\n";
        debugText += $"IsNoiseChaseActive: {_enemyAI.IsNoiseChaseActive}";
        
        // Set up GUI style
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = fontSize;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.UpperLeft;
        style.padding = new RectOffset(10, 10, 10, 10);
        
        // Background box
        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.normal.background = MakeTex(2, 2, new Color(0, 0, 0, 0.7f));
        
        // Calculate position
        Vector2 position = screenOffset;
        
        // Draw background box
        Vector2 textSize = style.CalcSize(new GUIContent(debugText));
        Rect boxRect = new Rect(position.x - 5, position.y - 5, textSize.x + 10, textSize.y + 10);
        GUI.Box(boxRect, "", boxStyle);
        
        // Draw text
        GUI.Label(new Rect(position.x, position.y, textSize.x, textSize.y), debugText, style);
    }
    
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;
        
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        
        return result;
    }
}

