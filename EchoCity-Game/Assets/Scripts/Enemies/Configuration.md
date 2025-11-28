
---

## Setup e Configurazione

### 1. Creare gli Eventi ScriptableObject

In Unity Editor:
1. **SOSoundEmissionDataEvent**: 
   - `Create → ECHO CITY → Events → SoundEmissionDataEventSO`
   - Assegnare a tutti gli `AudioEmitter` e componenti che emettono suoni

2. **SOEnemyNoiseUIEvent**:
   - `Create → ECHO CITY → ENEMY → EnemyNoiseUIEventSO`
   - Assegnare a ogni `EnemyAI` nel campo "Noise UI Event"

### 2. Configurare EnemyAI

Per ogni nemico nella scena:
1. Seleziona il GameObject con `EnemyAI`
2. Verifica che abbia `NavMeshAgent`, `Animator` e `AudioSource` (richiesti automaticamente)
3. Assegna `SOSoundEmissionDataEvent` nel campo "New Audio Sphere Event"
4. Assegna `SOEnemyNoiseUIEvent` nel campo "Noise UI Event" (opzionale, solo se si vuole la UI)
5. Assegna `SOEnemyInvestigationEvent` nel campo "Investigation Event" (opzionale, solo se si vuole gestire l'evento)

### 3. Configurare SOEnemyData

Per ogni tipo di nemico:
1. Apri l'asset `SOEnemyData` (es. `BasicEnemy.asset`)
2. Nella sezione "Noise Detection":
   - **NoiseUIThreshold**: Quando mostrare la UI (es. 5)
   - **NoiseThreshold**: Quando iniziare a inseguire (es. 10)
   - **NoiseLoseThreshold**: Quando smettere di inseguire (es. 3)
   - **MinChaseDuration**: Durata minima di inseguimento in secondi (es. 3)
   - **NoiseDecayRate**: Velocità decadimento (es. 1/sec)
   - **MaxNoisePerceptionDistance**: Distanza max (es. 50)
   - **NoiseIntensityFactor**: Fattore intensità (es. 1.0)
   - **NoiseRangeFactor**: Fattore distanza (es. 1.0)
   - **NoiseDistanceDecay**: Esponente falloff (es. 1.0)
3. Nella sezione "Investigation":
   - **InvestigationPhrases**: Array di `AudioClip` da riprodurre quando arriva al punto del rumore e non trova nulla
     - Se vuoto o null, nessuna frase viene riprodotta
     - Viene selezionato un clip random dall'array

### 4. Implementare la UI

La UI deve ascoltare `SOEnemyNoiseUIEvent`:

```csharp
public class NoiseUIManager : MonoBehaviour {
    [SerializeField] private SOEnemyNoiseUIEvent noiseUIEvent;
    [SerializeField] private GameObject noiseBarUI;
    [SerializeField] private Image noiseBarFill;
    
    void OnEnable() {
        noiseUIEvent.OnEventRaised += OnNoiseUpdate;
    }
    
    void OnDisable() {
        noiseUIEvent.OnEventRaised -= OnNoiseUpdate;
    }
    
    void OnNoiseUpdate(EnemyNoiseData data) {
        if (data.shouldShowUI) {
            noiseBarUI.SetActive(true);
            noiseBarFill.fillAmount = data.GetNormalizedNoise();
        } else {
            noiseBarUI.SetActive(false);
        }
    }
}
```

### 5. Implementare il Listener per Eventi di Investigazione (Opzionale)

Se vuoi gestire quando il nemico arriva al punto e non trova nulla:

```csharp
public class InvestigationEventListener : MonoBehaviour {
    [SerializeField] private SOEnemyInvestigationEvent investigationEvent;
    
    void OnEnable() {
        investigationEvent.OnEventRaised += OnInvestigation;
    }
    
    void OnDisable() {
        investigationEvent.OnEventRaised -= OnInvestigation;
    }
    
    void OnInvestigation(EnemyInvestigationData data) {
        // data.enemy: Il nemico che ha investigato
        // data.audioClip: Il clip audio della frase riprodotta
        // Nota: L'audio viene già riprodotto automaticamente da EnemyAI
        // Questo evento è utile se vuoi fare altre azioni (es. log, UI, effetti)
        Debug.Log($"{data.enemy.name} ha investigato e non ha trovato nulla. Frase: {data.audioClip.name}");
    }
}
```

---