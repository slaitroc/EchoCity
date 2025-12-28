# Eventi e Parametri del Sistema Enemy

Questo documento elenca tutti gli eventi ScriptableObject utilizzati nel sistema Enemy e i parametri che vengono passati.

---

## 📤 EVENTI EMESSI (Invoking Events)

### 1. **SOEnemyNoiseUIEvent** 
**Tipo**: `SOEnemyNoiseUIEvent`  
**Emitte da**: `EnemyAI.NotifyUI()` (chiamato ogni frame in `EnemyAI.Update()`)  
**Destinazione**: UI System  
**Scopo**: Notificare la UI con il valore corrente di attraction per mostrare la barra di attenzione del nemico.

**Parametri passati** (`EnemyNoiseData`):
```csharp
public struct EnemyNoiseData
{
    public EnemyAI enemy;              // Riferimento al nemico che emette l'evento
    public float currentNoiseLevel;    // Valore corrente di attraction (0+)
    public float uiThreshold;          // Soglia per mostrare UI (attualmente 0.5, ma non usato)
    public float chaseThreshold;       // Soglia per iniziare chase (1.0 = NoiseThreshold)
    public bool shouldShowUI;         // Calcolato automaticamente: currentNoiseLevel >= uiThreshold && currentNoiseLevel < chaseThreshold
    public bool isChasing;             // True se il nemico sta attualmente inseguendo (Chase o Attack state)
                                       // Utile per mostrare UI anche quando attraction è bassa ma il nemico sta inseguendo per prossimità (8m)
}
```

**Esempio di utilizzo**:
```csharp
// In EnemyAI.NotifyUI()
bool isChasing = CurrentState == EnemyStatesEnum.Chase || CurrentState == EnemyStatesEnum.Attack;
var noiseData = new EnemyNoiseData(this, _attraction, 0f, enemyData.NoiseThreshold, isChasing);
noiseUIEvent.RaiseEvent(noiseData);
```

---

### 2. **SOEnemyInvestigationEvent**
**Tipo**: `SOEnemyInvestigationEvent`  
**Emitte da**: `EnemyAI.PlayInvestigationPhrase()` (chiamato da `StandAndExaminateState.Enter()`)  
**Destinazione**: Sistema di gestione eventi (opzionale)  
**Scopo**: Notificare quando il nemico arriva al punto del suono e riproduce una frase di investigazione.

**Parametri passati** (`EnemyInvestigationData`):
```csharp
public struct EnemyInvestigationData
{
    public EnemyAI enemy;      // Riferimento al nemico che sta investigando
    public AudioClip audioClip; // AudioClip della frase random riprodotta
}
```

**Esempio di utilizzo**:
```csharp
// In EnemyAI.PlayInvestigationPhrase()
var investigationData = new EnemyInvestigationData(this, selectedClip);
investigationEvent.RaiseEvent(investigationData);
```

---

### 3. **SOEnemyIAEvent** (Player Hit Event)
**Tipo**: `SOEnemyIAEvent`  
**Emitte da**: `EnemyAI.OnPlayerHit()` (chiamato quando il nemico colpisce il player)  
**Destinazione**: Sistema di gestione danni/eventi  
**Scopo**: Notificare quando il nemico colpisce il player durante un attacco.

**Parametri passati**:
- `EnemyAI enemy` - Riferimento al nemico che ha colpito il player

**Esempio di utilizzo**:
```csharp
// In EnemyAI.OnPlayerHit()
if (_fsm.CurrentState.OnPlayerHit())
    playerHitEvent?.RaiseEvent(this);
```

---

### 4. **SONewAudioSphereEvent** (Echolocation Sound)
**Tipo**: `SONewAudioSphereEvent`  
**Emitte da**: `StandAndExaminateState.EmitInvestigationSound()`  
**Destinazione**: Echolocation System  
**Scopo**: Emettere un suono per il sistema di echolocation quando il nemico è in `StandAndExaminateState`.

**Parametri passati** (`SoundEmissionData`):
```csharp
public struct SoundEmissionData
{
    public Vector3 position;   // Posizione del suono (posizione del nemico)
    public float radius;      // Raggio del suono (InvestigationSoundRadius da SOEnemyData)
    public float intensity;    // Intensità del suono (InvestigationSoundIntensity da SOEnemyData)
    public float duration;    // Durata del suono (InvestigationSoundDuration da SOEnemyData)
    public float frequency;   // Frequenza del suono (InvestigationSoundFrequency da SOEnemyData)
                              // 0 = Low, 1 = Mid, 2 = High
}
```

**Esempio di utilizzo**:
```csharp
// In StandAndExaminateState.EmitInvestigationSound()
SoundEmissionData soundData = new SoundEmissionData(
    _enemyAI.transform.position,                    // pos
    _enemyData.InvestigationSoundRadius,             // rad
    _enemyData.InvestigationSoundIntensity,          // intens
    _enemyData.InvestigationSoundDuration,           // dur
    _enemyData.InvestigationSoundFrequency           // objFreq
);
_enemyAI.enemySoundEmissionEvent.RaiseEvent(soundData);
```

---

## 📥 EVENTI OSSERVATI (Observed Events)

### 1. **SOPlayerActionEvent**
**Tipo**: `SOPlayerActionEvent`  
**Ascoltato da**: `EnemyAI.OnPlayerAction()` (iscritto in `OnEnable()`, disiscritto in `OnDisable()`)  
**Emitte da**: `InputManager` (o sistema equivalente)  
**Scopo**: Ricevere notifiche quando il player compie un'azione che genera rumore (es. colpire un oggetto con un item).

**Parametri ricevuti** (`PlayerActionData`):
```csharp
public struct PlayerActionData
{
    public Vector3 position;   // Posizione dove è avvenuta l'azione (di solito posizione player o punto di interazione)
    public float intensity;   // Intensità del suono generato (dal oggetto/item usato)
    public float duration;    // Durata del suono generato (dal oggetto/item usato)
    public float frequency;   // Frequenza del suono generato (dal oggetto/item usato)
                              // Low = 0, Mid = 1, High = 2
    public float radius;      // Raggio del suono generato (dal oggetto/item usato)
}
```

**Esempio di utilizzo**:
```csharp
// In InputManager (o sistema equivalente)
PlayerActionData actionData = new PlayerActionData(
    actionPosition,  // Vector3
    intensity,        // float (da oggetto/item)
    duration,         // float (da oggetto/item)
    frequency,        // float (da oggetto/item: 0=Low, 1=Mid, 2=High)
    radius            // float (da oggetto/item)
);
playerActionEvent?.RaiseEvent(actionData);

// In EnemyAI.OnPlayerAction()
void OnPlayerAction(PlayerActionData actionData)
{
    _activePlayerAction = actionData;
    _actionTimeRemaining = actionData.duration;
    _hasActiveAction = true;
    _actionPosition = actionData.position;
    _lastChaseActionPosition = actionData.position;
    _hasLastChaseActionPosition = true;
}
```

---

## 📊 RIEPILOGO EVENTI

| Evento | Tipo | Direzione | Quando viene emesso/ricevuto | Parametri |
|--------|------|-----------|------------------------------|-----------|
| **SOEnemyNoiseUIEvent** | Emesso | EnemyAI → UI | Ogni frame in `EnemyAI.Update()` | `EnemyNoiseData` (enemy, currentNoiseLevel, uiThreshold, chaseThreshold, isChasing) |
| **SOEnemyInvestigationEvent** | Emesso | EnemyAI → Sistema Eventi | Quando nemico riproduce frase investigazione | `EnemyInvestigationData` (enemy, audioClip) |
| **SOEnemyIAEvent** | Emesso | EnemyAI → Sistema Danni | Quando nemico colpisce player | `EnemyAI` (riferimento) |
| **SONewAudioSphereEvent** | Emesso | StandAndExaminateState → Echolocation | Quando nemico emette suono echolocation | `SoundEmissionData` (position, radius, intensity, duration, frequency) |
| **SOPlayerActionEvent** | Ricevuto | InputManager → EnemyAI | Quando player compie azione che genera rumore | `PlayerActionData` (position, intensity, duration, frequency, radius) |

---

## 🔧 CONFIGURAZIONE

### Setup Eventi in Unity Editor

1. **SOEnemyNoiseUIEvent**:
   - `Create → ECHO CITY → ENEMY → EnemyNoiseUIEventSO`
   - Assegnare a `EnemyAI` → "Noise UI Event" (opzionale, solo se serve UI)

2. **SOEnemyInvestigationEvent**:
   - `Create → ECHO CITY → ENEMY → EnemyInvestigationEventSO`
   - Assegnare a `EnemyAI` → "Investigation Event" (opzionale)

3. **SOEnemyIAEvent**:
   - `Create → ECHO CITY → ENEMY → EnemyIAEventSO`
   - Assegnare a `EnemyAI` → "Player Hit Event"

4. **SONewAudioSphereEvent**:
   - `Create → ECHO CITY → NewAudioSphereEventSO`
   - Assegnare a `EnemyAI` → "Enemy Sound Emission Event"

5. **SOPlayerActionEvent**:
   - `Create → ECHO CITY → Events → PlayerActionEventSO`
   - Assegnare a `EnemyAI` → "Player Action Event" (Observed Events)
   - Assegnare a `InputManager` → "Player Action Event" (per emettere eventi)

---

## 💡 ESEMPIO DI IMPLEMENTAZIONE UI

Ecco un esempio pratico di come implementare un listener UI per mostrare la barra di attenzione dei nemici:

```csharp
using UnityEngine;
using System.Collections.Generic;

public class EnemyNoiseUIListener : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private SOEnemyNoiseUIEvent noiseUIEvent;
    
    [Header("UI References")]
    [SerializeField] private Transform uiContainer; // Container per le UI dei nemici
    [SerializeField] private GameObject enemyUIPrefab; // Prefab per l'UI di un singolo nemico
    
    // Dizionario per tracciare le UI di ogni nemico
    private Dictionary<EnemyAI, EnemyUIElement> enemyUIElements = new Dictionary<EnemyAI, EnemyUIElement>();
    
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
    
    void OnNoiseUpdate(EnemyNoiseData noiseData)
    {
        EnemyAI enemy = noiseData.enemy;
        
        // Se non esiste ancora un'UI per questo nemico, creala
        if (!enemyUIElements.ContainsKey(enemy))
        {
            GameObject uiElement = Instantiate(enemyUIPrefab, uiContainer);
            EnemyUIElement element = uiElement.GetComponent<EnemyUIElement>();
            enemyUIElements[enemy] = element;
        }
        
        EnemyUIElement uiElement = enemyUIElements[enemy];
        
        // Mostra/nascondi UI in base ai dati
        bool shouldShow = noiseData.isChasing || noiseData.currentNoiseLevel >= noiseData.chaseThreshold || noiseData.shouldShowUI;
        uiElement.gameObject.SetActive(shouldShow);
        
        if (shouldShow)
        {
            // Aggiorna la barra di attenzione
            float normalizedValue = noiseData.isChasing ? 1f : (noiseData.currentNoiseLevel / noiseData.chaseThreshold);
            uiElement.UpdateAttentionBar(normalizedValue);
            
            // Aggiorna posizione UI (se necessario, es. world space UI)
            // uiElement.UpdatePosition(enemy.transform.position);
        }
    }
    
    // Metodo per rimuovere UI quando un nemico viene distrutto
    public void OnEnemyDestroyed(EnemyAI enemy)
    {
        if (enemyUIElements.ContainsKey(enemy))
        {
            Destroy(enemyUIElements[enemy].gameObject);
            enemyUIElements.Remove(enemy);
        }
    }
}
```

**Note per l'implementazione**:
- L'evento viene emesso **ogni frame**, quindi gestisci l'aggiornamento UI in modo efficiente
- Usa `isChasing` per mostrare sempre l'UI quando il nemico sta inseguendo (anche con attraction bassa)
- Usa `currentNoiseLevel` e `chaseThreshold` per calcolare il valore normalizzato della barra (0-1)
- Gestisci la rimozione delle UI quando i nemici vengono distrutti

---

## 📝 NOTE IMPORTANTI

1. **SOEnemyNoiseUIEvent**: Viene emesso ogni frame, quindi la UI deve gestire la frequenza di aggiornamento e decidere quando mostrare/nascondere gli elementi.

2. **SOPlayerActionEvent**: Solo un'azione attiva alla volta. Quando viene emessa una nuova azione, sostituisce quella precedente.

3. **SONewAudioSphereEvent**: Viene emesso solo quando il nemico entra in `StandAndExaminateState`, non in altri stati.

4. **Condizione globale di prossimità**: Se il player è entro 8 metri dal nemico, il nemico inizia sempre a inseguire, indipendentemente dal valore di attraction. Questa condizione è implementata in `EnemyFSM.Update()` e si applica a tutti gli stati (eccetto ChaseState e AttackState).

