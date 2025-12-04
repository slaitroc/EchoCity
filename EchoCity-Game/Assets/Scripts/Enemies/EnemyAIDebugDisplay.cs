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
    
    private EnemyAI _enemyAI;
    
    void Start()
    {
        _enemyAI = GetComponent<EnemyAI>();
    }
    
    void OnGUI()
    {
        if (!showDebug || !enabled)
        {
            return;
        }
        
        if (_enemyAI == null)
        {
            GUI.Box(new Rect(20, 20, 400, 150), "ERROR: EnemyAI not found!");
            return;
        }
        
        // Calcola valori
        float distance = 0f;
        bool playerFound = false;
        
        if (_enemyAI.player != null)
        {
            distance = Vector3.Distance(transform.position, _enemyAI.player.position);
            playerFound = true;
        }
        
        float attraction = _enemyAI.GetAttraction();
        
        // Disegna display principale
        float x = 20f;
        float y = 20f;
        float width = 500f;
        float height = 200f;
        
        // Background nero semi-trasparente
        Texture2D bg = new Texture2D(1, 1);
        bg.SetPixel(0, 0, new Color(0, 0, 0, 0.8f));
        bg.Apply();
        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.normal.background = bg;
        
        GUI.Box(new Rect(x, y, width, height), "", boxStyle);
        
        // Testo
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontSize = 24;
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.normal.textColor = Color.white;
        
        GUIStyle valueStyle = new GUIStyle(labelStyle);
        valueStyle.fontSize = 32;
        
        float lineY = y + 20f;
        
        // Header
        GUI.Label(new Rect(x + 10f, lineY, width - 20f, 40f), "ENEMY DEBUG", labelStyle);
        lineY += 50f;
        
        // Distance
        labelStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(x + 10f, lineY, 200f, 40f), "DISTANCE:", labelStyle);
        valueStyle.normal.textColor = Color.yellow;
        string distStr = playerFound ? distance.ToString("F2") + " m" : "NO PLAYER";
        GUI.Label(new Rect(x + 220f, lineY, 250f, 40f), distStr, valueStyle);
        lineY += 50f;
        
        // Attraction
        labelStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(x + 10f, lineY, 200f, 40f), "ATTRACTION:", labelStyle);
        valueStyle.normal.textColor = Color.cyan;
        GUI.Label(new Rect(x + 220f, lineY, 250f, 40f), attraction.ToString("F3"), valueStyle);
    }
}
