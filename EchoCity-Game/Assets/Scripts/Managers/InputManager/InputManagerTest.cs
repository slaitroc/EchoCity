using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PLACEHOLDER InputManager per testare il sistema nemico.
/// Basato sul branch Develop, adattato per usare PlayerActionData e SOPlayerActionEvent.
/// 
/// Controlli:
/// - 'E': Emette un'azione con parametri base
/// - '1': Emette azione Low Frequency (intensity bassa)
/// - '2': Emette azione Mid Frequency (intensity media)
/// - '3': Emette azione High Frequency (intensity alta)
/// - 'X': Emette 100 azioni rapidamente (stress test)
/// </summary>
public class InputManagerTest : MonoBehaviour
{
    #region Constants
    private string _LOG_TAG = "INPUT MANAGER TEST";
    private string _LOG_COLOR = "#7039e8ff";
    #endregion

    #region Serialized Fields
    [Header("Invoking Events")]
    [Tooltip("Event sollevato quando il player compie un'azione (per sistema nemico)")]
    [SerializeField] private SOPlayerActionEvent playerActionEvent;

    [Header("Player Action Test Settings")]
    [Tooltip("Posizione dove avviene l'azione (se null, usa posizione player)")]
    [SerializeField] private Transform actionPositionSource;
    
    [Tooltip("Lista di posizioni alternative per test (se vuota, usa actionPositionSource o player)")]
    [SerializeField] private List<Transform> testPositionsList;

    [Header("Action Parameters (Placeholder - normalmente da oggetti/items)")]
    [Tooltip("Intensity base per azioni di test")]
    [Min(0f)]
    [SerializeField] private float baseIntensity = 0.5f;
    
    [Tooltip("Duration base per azioni di test (secondi)")]
    [Min(0.1f)]
    [SerializeField] private float baseDuration = 1.0f;
    
    [Tooltip("Radius base per azioni di test")]
    [Min(0f)]
    [SerializeField] private float baseRadius = 5f;

    [Header("Frequency Presets (per test rapido)")]
    [Tooltip("Intensity per azioni Low Frequency")]
    [SerializeField] private float lowFreqIntensity = 0.3f;
    
    [Tooltip("Intensity per azioni Mid Frequency")]
    [SerializeField] private float midFreqIntensity = 0.6f;
    
    [Tooltip("Intensity per azioni High Frequency")]
    [SerializeField] private float highFreqIntensity = 1.0f;

    [Header("Debug")]
    [SerializeField] private bool logActions = true;
    #endregion

    #region Private Fields
    private int triggeredActionEvents = 0;
    private Transform playerTransform;
    #endregion

    void Awake()
    {
        // Trova player se non assegnato
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        
        // Se actionPositionSource non è assegnato, usa player
        if (actionPositionSource == null && playerTransform != null)
        {
            actionPositionSource = playerTransform;
        }
    }

    void Update()
    {
        // 'E': Azione base
        if (Input.GetKeyDown(KeyCode.E))
        {
            TriggerPlayerAction(baseIntensity, baseDuration, 1f, baseRadius, "Base Action");
        }

        // '1': Low Frequency (0 = Low)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TriggerPlayerAction(lowFreqIntensity, baseDuration, 0f, baseRadius, "Low Frequency Action");
        }

        // '2': Mid Frequency (1 = Mid)
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TriggerPlayerAction(midFreqIntensity, baseDuration, 1f, baseRadius, "Mid Frequency Action");
        }

        // '3': High Frequency (2 = High)
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TriggerPlayerAction(highFreqIntensity, baseDuration, 2f, baseRadius, "High Frequency Action");
        }

        // 'X': Stress test - 100 azioni rapide
        if (Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(StressTestActions());
        }
    }

    /// <summary>
    /// Triggera un'azione del player con i parametri specificati
    /// </summary>
    private void TriggerPlayerAction(float intensity, float duration, float frequency, float radius, string actionName)
    {
        if (playerActionEvent == null)
        {
            Log.W("InputManagerTest: SOPlayerActionEvent non assegnato! Assegnare l'evento nell'Inspector.", _LOG_COLOR, _LOG_TAG);
            return;
        }

        // Determina posizione azione
        Vector3 actionPosition = GetActionPosition();

        // Crea PlayerActionData
        // Parametri: (Vector3 pos, float intens, float dur, float freq, float rad)
        PlayerActionData actionData = new PlayerActionData(
            actionPosition,    // pos
            intensity,          // intens
            duration,           // dur
            frequency,          // freq (0=Low, 1=Mid, 2=High)
            radius              // rad
        );

        // Solleva evento - tutti gli EnemyAI lo riceveranno
        playerActionEvent.RaiseEvent(actionData);
        triggeredActionEvents++;

        if (logActions)
        {
            string freqName = frequency <= 0f ? "Low" : (frequency <= 1f ? "Mid" : "High");
            Log.D($"{actionName} triggered #{triggeredActionEvents} | Pos: {actionPosition} | Intensity: {intensity:F2} | Duration: {duration:F2}s | Frequency: {freqName} | Radius: {radius:F2}m", 
                _LOG_COLOR, _LOG_TAG);
        }
    }

    /// <summary>
    /// Ottiene la posizione dove avviene l'azione
    /// </summary>
    private Vector3 GetActionPosition()
    {
        // Priorità 1: Posizione da testPositionsList (random)
        if (testPositionsList != null && testPositionsList.Count > 0)
        {
            Transform randomPos = testPositionsList[Random.Range(0, testPositionsList.Count)];
            if (randomPos != null)
            {
                return randomPos.position;
            }
        }

        // Priorità 2: actionPositionSource
        if (actionPositionSource != null)
        {
            return actionPositionSource.position;
        }

        // Priorità 3: Player position
        if (playerTransform != null)
        {
            return playerTransform.position;
        }

        // Fallback: origine
        return Vector3.zero;
    }

    /// <summary>
    /// Stress test: emette 100 azioni rapidamente
    /// </summary>
    private IEnumerator StressTestActions()
    {
        Log.D("Starting stress test: 100 actions...", _LOG_COLOR, _LOG_TAG);
        
        for (int i = 0; i < 100; i++)
        {
            // Alterna tra frequenze
            float freq = (i % 3); // 0, 1, 2
            float intensity = freq == 0 ? lowFreqIntensity : (freq == 1 ? midFreqIntensity : highFreqIntensity);
            
            TriggerPlayerAction(intensity, baseDuration, freq, baseRadius, $"Stress Test Action {i + 1}");
            
            // Piccolo delay per non sovraccaricare
            yield return new WaitForSeconds(0.01f);
        }
        
        Log.D($"Stress test completed: {triggeredActionEvents} total actions triggered", _LOG_COLOR, _LOG_TAG);
    }

    // #region Interaction Handlers (Placeholder - decommentare se aggiungi sistema Interactable)
    // /// <summary>
    // /// Handler per quando il player entra nel range di un Interactable
    // /// </summary>
    // public void EnterInteractionRangeHandler(Interactable interactable)
    // {
    //     if (logActions)
    //     {
    //         Log.D($"Entered interaction range with: {interactable.gameObject.name}", _LOG_COLOR, _LOG_TAG);
    //     }
    // }
    //
    // /// <summary>
    // /// Handler per quando il player esce dal range di un Interactable
    // /// </summary>
    // public void ExitInteractionRangeHandler(Interactable interactable)
    // {
    //     if (logActions)
    //     {
    //         Log.D($"Exited interaction range with: {interactable.gameObject.name}", _LOG_COLOR, _LOG_TAG);
    //     }
    // }
    // #endregion

    void OnDrawGizmosSelected()
    {
        // Disegna posizioni test
        if (testPositionsList != null)
        {
            Gizmos.color = Color.cyan;
            foreach (var pos in testPositionsList)
            {
                if (pos != null)
                {
                    Gizmos.DrawWireSphere(pos.position, 0.5f);
                }
            }
        }

        // Disegna actionPositionSource
        if (actionPositionSource != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(actionPositionSource.position, 0.3f);
        }
    }
}

