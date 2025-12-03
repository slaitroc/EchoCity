# Setup e Configurazione - Sistema Nemico

## 📋 Panoramica

Il sistema nemico funziona basandosi sulle **azioni del player** che generano rumore. Quando il player compie un'azione (es. percuote un oggetto con un item), l'InputManager solleva un evento con i dati dell'azione. I nemici ascoltano questo evento e calcolano l'attraction basandosi sui parametri dell'azione (intensità, durata, frequenza) che provengono dall'oggetto/item usato.

---

## 1. Creare gli Eventi ScriptableObject

In Unity Editor:

### 1.1 SOPlayerActionEvent
1. Crea un nuovo ScriptableObject:
   - `Create → ECHO CITY → Events → PlayerActionEventSO`
   - Assegna un nome (es. `PlayerActionEvent`)
2. Questo evento verrà sollevato dall'InputManager quando il player compie un'azione
3. **Importante**: Crea un'unica istanza condivisa tra InputManager e tutti gli EnemyAI

### 1.2 SOEnemyNoiseUIEvent (Opzionale)
1. `Create → ECHO CITY → ENEMY → EnemyNoiseUIEventSO`
2. Assegna a ogni `EnemyAI` nel campo "Noise UI Event" (solo se vuoi la UI)

### 1.3 SOEnemyInvestigationEvent (Opzionale)
1. `Create → ECHO CITY → ENEMY → EnemyInvestigationEventSO`
2. Assegna a ogni `EnemyAI` nel campo "Investigation Event" (solo se vuoi gestire l'evento)

---

## 2. Configurare EnemyAI

Per ogni nemico nella scena:

1. Seleziona il GameObject con `EnemyAI`
2. Verifica che abbia:
   - `NavMeshAgent` (richiesto automaticamente)
   - `Animator` (richiesto automaticamente)
   - `AudioSource` (richiesto automaticamente)
3. Nella sezione "Observed Events":
   - Assegna `SOPlayerActionEvent` nel campo **"Player Action Event"**
     - Questo è l'evento che l'InputManager solleverà quando il player compie azioni
4. Nella sezione "Invoking Events":
   - Assegna `SOEnemyNoiseUIEvent` nel campo "Noise UI Event" (opzionale)
   - Assegna `SOEnemyInvestigationEvent` nel campo "Investigation Event" (opzionale)
5. Nella sezione "References":
   - Assegna `SOEnemyData` (ScriptableObject con parametri del nemico)
   - Assegna `Transform player` (riferimento al player)
   - Assegna `Transform[] waypoints` (waypoints per il patrol)
   - Assegna `AttackRangeDetector` (componente per rilevare attacchi)

---

## 3. Configurare SOEnemyData

Per ogni tipo di nemico:

1. Apri l'asset `SOEnemyData` (es. `BasicEnemy.asset`)
2. Nella sezione **"Noise Detection"**:
   - **NoiseThreshold**: Soglia per iniziare chase (default: 1.0)
     - Quando `attraction >= 1.0`, il nemico inizia a inseguire il PLAYER
   - **NoiseLoseThreshold**: Soglia per continuare chase (default: 0.8)
     - Dopo 3 secondi, se `attraction > 0.8` E `player <= 15m`, continua chase
   - **MinChaseDuration**: Durata minima di inseguimento in secondi (default: 3)
     - Durante questo tempo, il nemico insegue sempre, ignorando attraction
   - **NoiseDecayRate**: Velocità decadimento attraction quando no azioni (default: 1/sec)
   - **MaxNoisePerceptionDistance**: Distanza massima di percezione (default: 50)
     - Azioni oltre questa distanza non contribuiscono all'attraction
3. Nella sezione **"Noise Calculation (Attraction Formula)"**:
   - **NoiseIntensityFactor**: Fattore moltiplicatore per intensità (default: 1.0)
   - **NoiseRangeFactor**: Fattore moltiplicatore per distanza (default: 1.0)
   - **NoiseDistanceDecay**: Esponente per falloff distanza (default: 1.0)
     - Valori più alti = decadimento più rapido con la distanza
4. Nella sezione **"Investigation"**:
   - **InvestigationPhrases**: Array di `AudioClip` da riprodurre quando arriva al punto dell'azione e non trova nulla
     - Se vuoto o null, nessuna frase viene riprodotta
     - Viene selezionato un clip random dall'array

---

## 4. Implementare InputManager

L'InputManager deve sollevare `SOPlayerActionEvent` quando il player compie un'azione.

### 4.1 Struttura Base

```csharp
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private SOPlayerActionEvent playerActionEvent;
    
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    
    void Start()
    {
        // Verifica che l'evento sia assegnato
        if (playerActionEvent == null)
        {
            Debug.LogError("InputManager: SOPlayerActionEvent non assegnato!");
        }
    }
    
    // Esempio: quando il player percuote un oggetto con un item
    public void OnPlayerHitObject(ItemData itemUsed, Vector3 hitPosition)
    {
        // Crea PlayerActionData con i parametri dall'item
        PlayerActionData actionData = new PlayerActionData(
            position: hitPosition,              // Dove è avvenuta l'azione
            intensity: itemUsed.intensity,      // Dall'item usato
            duration: itemUsed.duration,        // Dall'item usato
            frequency: itemUsed.frequency,      // Dall'item usato (0=Low, 1=Mid, 2=High)
            radius: itemUsed.radius             // Dall'item usato
        );
        
        // Solleva l'evento - tutti gli EnemyAI lo riceveranno
        playerActionEvent?.RaiseEvent(actionData);
    }
    
    // Esempio: quando il player usa un oggetto
    public void OnPlayerUseItem(ItemData itemUsed)
    {
        // Usa la posizione del player come punto di origine
        Vector3 actionPosition = playerTransform.position;
        
        PlayerActionData actionData = new PlayerActionData(
            position: actionPosition,
            intensity: itemUsed.intensity,
            duration: itemUsed.duration,
            frequency: itemUsed.frequency,
            radius: itemUsed.radius
        );
        
        playerActionEvent?.RaiseEvent(actionData);
    }
}
```

### 4.2 Struttura ItemData (Esempio)

Gli oggetti/items devono avere questi parametri:

```csharp
[CreateAssetMenu(fileName = "ItemData", menuName = "ECHO CITY/Items/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Sound Properties")]
    [Tooltip("Intensity of sound generated when this item is used")]
    [Min(0f)]
    public float intensity = 0.5f;
    
    [Tooltip("Duration of sound generated when this item is used (seconds)")]
    [Min(0.1f)]
    public float duration = 1.0f;
    
    [Tooltip("Frequency: 0=Low, 1=Mid, 2=High")]
    [Range(0f, 2f)]
    public float frequency = 0f;
    
    [Tooltip("Radius of sound generated when this item is used")]
    [Min(0f)]
    public float radius = 5f;
    
    // Altri parametri dell'item...
}
```

### 4.3 Setup InputManager in Unity

1. Crea un GameObject vuoto chiamato "InputManager"
2. Aggiungi il componente `InputManager` (script)
3. Assegna:
   - `SOPlayerActionEvent` (l'istanza creata al punto 1.1)
   - `Transform player` (riferimento al player)
4. Collega le chiamate ai metodi `OnPlayerHitObject()` o `OnPlayerUseItem()` quando il player compie azioni

---

## 5. Implementare la UI (Opzionale)

La UI deve ascoltare `SOEnemyNoiseUIEvent` per mostrare la barra di attraction:

```csharp
using UnityEngine;
using UnityEngine.UI;

public class NoiseUIManager : MonoBehaviour
{
    [SerializeField] private SOEnemyNoiseUIEvent noiseUIEvent;
    [SerializeField] private GameObject noiseBarUI;
    [SerializeField] private Image noiseBarFill;
    [SerializeField] private float uiThreshold = 0.5f; // Soglia per mostrare UI
    
    void OnEnable()
    {
        if (noiseUIEvent != null)
        noiseUIEvent.OnEventRaised += OnNoiseUpdate;
    }
    
    void OnDisable()
    {
        if (noiseUIEvent != null)
        noiseUIEvent.OnEventRaised -= OnNoiseUpdate;
    }
    
    void OnNoiseUpdate(EnemyNoiseData data)
    {
        // data.currentNoiseLevel contiene l'attraction corrente
        // Tu decidi quando mostrare/nascondere la UI
        if (data.currentNoiseLevel >= uiThreshold)
        {
            noiseBarUI.SetActive(true);
            // Normalizza tra uiThreshold e NoiseThreshold (1.0)
            float normalized = Mathf.Clamp01((data.currentNoiseLevel - uiThreshold) / (data.chaseThreshold - uiThreshold));
            noiseBarFill.fillAmount = normalized;
        }
        else
        {
            noiseBarUI.SetActive(false);
        }
    }
}
```

**Nota**: Il sistema passa solo il valore di `attraction` alla UI. La logica di quando mostrare/nascondere la barra è gestita dallo sviluppatore UI.

---

## 6. Implementare il Listener per Eventi di Investigazione (Opzionale)

Se vuoi gestire quando il nemico arriva al punto dell'azione e non trova nulla:

```csharp
using UnityEngine;

public class InvestigationEventListener : MonoBehaviour
{
    [SerializeField] private SOEnemyInvestigationEvent investigationEvent;
    
    void OnEnable()
    {
        if (investigationEvent != null)
        investigationEvent.OnEventRaised += OnInvestigation;
    }
    
    void OnDisable()
    {
        if (investigationEvent != null)
        investigationEvent.OnEventRaised -= OnInvestigation;
    }
    
    void OnInvestigation(EnemyInvestigationData data)
    {
        // data.enemy: Il nemico che ha investigato
        // data.audioClip: Il clip audio della frase riprodotta
        // Nota: L'audio viene già riprodotto automaticamente da EnemyAI
        // Questo evento è utile se vuoi fare altre azioni (es. log, UI, effetti)
        Debug.Log($"{data.enemy.name} ha investigato e non ha trovato nulla. Frase: {data.audioClip.name}");
    }
}
```

---

## 7. Flusso Completo

```
1. Player compie azione (es. percuote oggetto con item)
   ↓
2. InputManager.OnPlayerHitObject(itemData, hitPosition)
   ↓
3. InputManager crea PlayerActionData:
   - position: hitPosition
   - intensity: itemData.intensity
   - duration: itemData.duration
   - frequency: itemData.frequency
   - radius: itemData.radius
   ↓
4. InputManager solleva: playerActionEvent.RaiseEvent(actionData)
   ↓
5. Tutti gli EnemyAI ricevono: OnPlayerAction(actionData)
   ↓
6. EnemyAI salva _activePlayerAction e calcola attraction ogni frame
   ↓
7. Stati nemico reagiscono a attraction:
   - attraction >= 1.0 → CHASE (PLAYER)
   - attraction < 0.8 → CHASE_SOUND → STAND_EXAMINATE
```

---

## 8. Checklist Setup

- [ ] Creato `SOPlayerActionEvent` ScriptableObject
- [ ] Creato `SOEnemyNoiseUIEvent` (se serve UI)
- [ ] Creato `SOEnemyInvestigationEvent` (se serve listener)
- [ ] Configurato tutti gli `EnemyAI` con:
  - [ ] `SOPlayerActionEvent` assegnato
  - [ ] `SOEnemyData` assegnato
  - [ ] `Transform player` assegnato
  - [ ] `Transform[] waypoints` assegnati
  - [ ] `AttackRangeDetector` assegnato
- [ ] Configurato `SOEnemyData` con soglie e parametri
- [ ] Implementato `InputManager` che solleva `SOPlayerActionEvent`
- [ ] Creati `ItemData` ScriptableObjects con parametri (intensity, duration, frequency, radius)
- [ ] Collegate le azioni del player all'InputManager
- [ ] (Opzionale) Implementata UI per mostrare attraction
- [ ] (Opzionale) Implementato listener per eventi investigazione

---

## 9. Note Importanti

1. **Un'azione alla volta**: Se il player compie una nuova azione mentre una è ancora attiva, la nuova sostituisce la precedente
2. **Posizione azione**: Il nemico insegue la **posizione dell'azione** (non del player) quando passa a `ChaseSoundState`
3. **Attraction centralizzata**: Tutto il calcolo è in `EnemyAI`, gli stati ricevono solo il valore pronto
4. **Parametri dall'item**: I valori di `intensity`, `duration`, `frequency`, `radius` devono venire dall'oggetto/item usato dal player
5. **Evento condiviso**: Un solo `SOPlayerActionEvent` condiviso tra InputManager e tutti gli EnemyAI

---

**Ultimo aggiornamento**: Sistema basato su azioni del player (PlayerActionData)
