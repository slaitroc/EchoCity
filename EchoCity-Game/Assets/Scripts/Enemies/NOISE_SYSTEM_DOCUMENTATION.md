# Sistema di Rumore/Fastidio per Nemici - Documentazione

## 📋 Indice
1. [Riepilogo File Creati e Modificati](#riepilogo-file-creati-e-modificati)
2. [Input e Output del Sistema](#input-e-output-del-sistema)
3. [Panoramica](#panoramica)
4. [Architettura del Sistema](#architettura-del-sistema)
5. [Come Funziona](#come-funziona)
6. [Componenti Principali](#componenti-principali)
7. [Differenze rispetto alla Versione Precedente](#differenze-rispetto-alla-versione-precedente)
8. [Setup e Configurazione](#setup-e-configurazione)
9. [Parametri di Tuning](#parametri-di-tuning)

---

## Riepilogo File Creati e Modificati

### 📁 File Creati (5)

1. **`/Assets/Scripts/Enemies/NOISE_SYSTEM_DOCUMENTATION.md`**
   - Documentazione completa del sistema di rumore

2. **`/Assets/Scripts/Enemies/EnemyNoiseData.cs`**
   - Struttura dati per eventi UI del rumore

3. **`/Assets/Scripts/Enemies/EnemyInvestigationData.cs`**
   - Struttura dati per eventi di investigazione (con `AudioClip`)

4. **`/Assets/Scripts/Events/SOEnemyNoiseUIEvent.cs`**
   - ScriptableObject event per notifiche UI del rumore

5. **`/Assets/Scripts/Events/SOEnemyInvestigationEvent.cs`**
   - ScriptableObject event per notifiche di investigazione

---

### ✏️ File Modificati (4)

1. **`/Assets/Scripts/Enemies/EnemyAI.cs`**
   - Aggiunto sistema di accumulo rumore continuo
   - Aggiunto decadimento del rumore
   - Aggiunto tracking posizione ultimo rumore
   - Aggiunto `AudioSource` (RequireComponent)
   - Aggiunto metodo `PlayInvestigationPhrase()`
   - Aggiunti metodi pubblici per gestione rumore e posizione
   - Aggiunta logica UI notifications
   - Aggiunta lista `_activeSounds` per suoni duraturi

2. **`/Assets/Scripts/Enemies/SOEnemyData.cs`**
   - Aggiunti parametri rumore: `NoiseUIThreshold`, `NoiseThreshold`, `NoiseLoseThreshold`
   - Aggiunto `MinChaseDuration`
   - Aggiunti parametri calcolo: `NoiseIntensityFactor`, `NoiseRangeFactor`, `NoiseDistanceDecay`
   - Aggiunto `NoiseDecayRate`
   - Aggiunto `MaxNoisePerceptionDistance`
   - Aggiunto `InvestigationPhrases` (AudioClip[])

3. **`/Assets/Scripts/Enemies/EnemyStates/PatrolEnemyState.cs`**
   - Modificato trigger chase: da distanza a rumore
   - Aggiunto salvataggio posizione player quando inizia chase

4. **`/Assets/Scripts/Enemies/EnemyStates/ChaseEnemyState.cs`**
   - Aggiunto timer minimo di inseguimento (`MinChaseDuration`)
   - Aggiunta logica investigazione quando rumore scende
   - Aggiunta verifica player presente/assente al punto rumore
   - Aggiunta chiamata a `PlayInvestigationPhrase()`
   - Aggiunta logica "rumore basso E distanza grande" per perdere chase
   - Aggiunta gestione rumore che risale durante investigazione
   - Mantenuto `ChaseSpeed` durante investigazione

---

### 📊 Statistiche

- **File creati:** 5
- **File modificati:** 4
- **Totale file coinvolti:** 9

Tutti i file sono nel sistema di nemici (`/Enemies/`) o nel sistema eventi (`/Events/`), mantenendo la struttura compartimentata del progetto.

---

## Input e Output del Sistema

### Input Necessari

Il sistema richiede i seguenti input per funzionare:

#### 1. Eventi ScriptableObject
- **`SOSoundEmissionDataEvent`**: Evento che viene sollevato quando viene emesso un suono
  - Deve essere assegnato nel campo "New Audio Sphere Event" di ogni `EnemyAI`
  - Deve essere sollevato da `AudioEmitter` o altri componenti che emettono suoni

#### 2. Dati di Emissione Suono (`SoundEmissionData`)
Ogni volta che viene emesso un suono, deve contenere:
- **`position`** (Vector3): Posizione dell'emissione del suono
- **`radius`** (float): Raggio del suono (usato per echolocation, default: 10m)
- **`intensity`** (float): Intensità del suono (0.0 - 1.0)
- **`duration`** (float): Durata del suono in secondi
- **`frequency`** (float): Frequenza del suono (0.0 = Low, 1.0 = Mid, 2.0 = High)

#### 3. Configurazione Nemico (`SOEnemyData`)
Ogni nemico deve avere un `SOEnemyData` configurato con:
- **`NoiseUIThreshold`** (float): Soglia per mostrare la UI (default: 5)
- **`NoiseThreshold`** (float): Soglia per iniziare il chasing (default: 10)
- **`NoiseLoseThreshold`** (float): Soglia per smettere di inseguire (default: 3)
- **`MinChaseDuration`** (float): Durata minima di inseguimento in secondi (default: 3) - **NUOVO**
- **`NoiseDecayRate`** (float): Velocità di decadimento del rumore/sec (default: 1.0)
- **`MaxNoisePerceptionDistance`** (float): Distanza massima di percezione (default: 50)
- **`NoiseIntensityFactor`** (float): Fattore moltiplicativo intensità (default: 1.0)
- **`NoiseRangeFactor`** (float): Fattore moltiplicativo distanza (default: 1.0)
- **`NoiseDistanceDecay`** (float): Esponente per falloff distanza (default: 1.0)

#### 4. Componenti Unity
- **`EnemyAI`**: Componente principale su ogni nemico
  - Richiede `NavMeshAgent`, `Animator` e `AudioSource`
  - Deve avere riferimento a `SOEnemyData`
  - Deve avere riferimento a `player` (Transform)
  - Deve avere array di `waypoints` (Transform[])

#### 5. Eventi (Opzionali)
- **`SOEnemyNoiseUIEvent`**: Evento per notificare la UI (opzionale, solo se si vuole la UI)
  - Deve essere assegnato nel campo "Noise UI Event" di ogni `EnemyAI`
- **`SOEnemyInvestigationEvent`**: Evento per notificare quando il nemico arriva al punto del rumore e non trova nulla (opzionale)
  - Deve essere assegnato nel campo "Investigation Event" di ogni `EnemyAI`

#### 6. Frasi Audio di Investigazione
- **`InvestigationPhrases`** (AudioClip[]): Array di clip audio che il nemico riproduce quando arriva al punto del rumore e non trova nulla
  - Configurato in `SOEnemyData` nella sezione "Investigation"
  - Se vuoto o null, nessuna frase viene riprodotta

---

### Output del Sistema

Il sistema produce i seguenti output:

#### 1. Livello di Fastidio (`_currentNoiseLevel`)
- **Tipo**: `float`
- **Range**: 0.0 - ∞ (teoricamente, ma praticamente limitato dal decadimento)
- **Accesso**: Tramite `EnemyAI.GetNoiseLevel()`
- **Uso**: Controllato da `PatrolEnemyState` per decidere se iniziare a inseguire

#### 2. Transizione di Stato
- **Da `PatrolEnemyState` a `ChaseEnemyState`**:
  - Quando `_currentNoiseLevel >= NoiseThreshold`
  - Il nemico inizia automaticamente a inseguire il player

#### 3. Notifiche UI (`EnemyNoiseData` via `SOEnemyNoiseUIEvent`)
Quando il fastidio è tra `NoiseUIThreshold` e `NoiseThreshold`, viene sollevato l'evento con:
- **`enemy`** (EnemyAI): Riferimento al nemico che sta causando il fastidio
- **`currentNoiseLevel`** (float): Valore corrente del fastidio
- **`uiThreshold`** (float): Soglia per mostrare UI
- **`chaseThreshold`** (float): Soglia per iniziare chasing
- **`shouldShowUI`** (bool): Flag se la UI deve essere mostrata
- **`GetNormalizedNoise()`** (float): Valore normalizzato 0-1 per la barra UI

#### 4. Transizioni e Comportamento di Investigazione
- **Quando entra in Chase**: 
  - Salva la posizione del player come "ultima posizione del rumore"
  - Inizia un timer minimo di `MinChaseDuration` secondi (default: 3)
  - Durante questo periodo, insegue sempre il player indipendentemente dal rumore
  - Mantiene la velocità di `ChaseSpeed` durante tutto l'inseguimento
  
- **Durante l'inseguimento (dopo MinChaseDuration)**:
  - Se il rumore >= `NoiseThreshold` → continua a inseguire il player
  - Se il rumore < `NoiseLoseThreshold` → va a investigare l'ultima posizione del rumore (mantiene `ChaseSpeed`)
  - Durante l'investigazione, se il rumore risale >= `NoiseThreshold` → torna immediatamente a inseguire il player
  - Se arriva alla posizione del rumore (raggio 2m):
    - Se il player NON è lì E il rumore è ancora basso → riproduce una frase audio random, solleva evento, e torna a Patrol
    - Se il player è lì O il rumore è risalito → continua a inseguire il player
  
- **Perdere il chase**:
  - Il nemico smette di inseguire solo se ENTRAMBE le condizioni sono vere:
    1. Rumore < `NoiseLoseThreshold`
    2. Distanza > `LoseRange`
  - Questo avviene solo dopo `MinChaseDuration` e solo se non sta investigando
  
- **Bilanciamento gameplay**:
  - Il player non può fare rumore e scappare senza conseguenze
  - Il nemico va comunque a controllare dove ha sentito il suono
  - Crea tensione: anche se il player si ferma, il nemico arriva comunque nella zona
  - Il nemico mantiene la velocità di inseguimento anche durante l'investigazione
  
- **Memoria del rumore**: Il sistema traccia la posizione del suono più forte tra quelli attivi, aggiornandola continuamente

#### 5. Eventi di Investigazione (`EnemyInvestigationData` via `SOEnemyInvestigationEvent`)
Quando il nemico arriva al punto del rumore e non trova nulla, viene sollevato l'evento con:
- **`enemy`** (EnemyAI): Riferimento al nemico che ha investigato
- **`audioClip`** (AudioClip): Clip audio della frase riprodotta (random dall'array `InvestigationPhrases`)

---

## Panoramica

Il sistema di rumore/fastidio permette ai nemici di reagire ai suoni emessi dal player e dall'ambiente. Ogni nemico calcola **autonomamente** il proprio livello di fastidio basandosi su:
- **Intensità** del suono
- **Frequenza** del suono (Low/Mid/High)
- **Distanza** dal suono

Quando il fastidio supera una certa soglia, il nemico inizia a inseguire il player. Una soglia intermedia attiva la UI per avvisare il player del pericolo imminente.

**Caratteristiche principali:**
- ✅ Accumulo continuo nel tempo (supporta suoni duraturi, non solo impulsi)
- ✅ Formula di distanza basata su potenza inversa (da Attraction.cs)
- ✅ Decadimento automatico del rumore nel tempo
- ✅ Sistema completamente decentralizzato (ogni nemico calcola autonomamente)
- ✅ Integrazione con UI per feedback visivo al player

---

## Architettura del Sistema

### Flusso Principale

```
INPUT: SoundEmissionData (evento SOSoundEmissionDataEvent)
    ↓
EnemyAI.OnSoundEmitted() 
    ↓
Aggiunge suono alla lista _activeSounds (con durata)
    ↓
Update() ogni frame:
    1. Rimuove suoni scaduti dalla lista
    2. Per ogni suono attivo:
       - Calcola distanza corrente
       - Accumula: intensity × freqMult × distMult × Time.deltaTime
    3. Aggiunge accumulo a _currentNoiseLevel
    4. Applica decadimento: _currentNoiseLevel -= NoiseDecayRate × Time.deltaTime
    5. Controlla soglie e notifica UI se necessario
    ↓
PatrolEnemyState.Update():
    - Se _currentNoiseLevel >= NoiseThreshold → OUTPUT: SwitchState(ChaseState)
    - Altrimenti continua Patrol
    ↓
OUTPUT: Transizione a ChaseEnemyState
OUTPUT: ResetNoiseLevel() (fastidio = 0)
OUTPUT: Evento UI (se implementato)
```

### Componenti Chiave

1. **EnemyAI**: Gestisce il calcolo e l'accumulo del rumore
2. **SOEnemyData**: Contiene le soglie e parametri configurabili
3. **EnemyNoiseData**: Struttura dati per comunicare con la UI
4. **SOEnemyNoiseUIEvent**: Evento per notificare la UI
5. **PatrolEnemyState**: Controlla il fastidio per decidere se inseguire

---

## Come Funziona

Il sistema funziona in modo **decentralizzato**: ogni nemico (`EnemyAI`) calcola autonomamente il proprio livello di fastidio ascoltando gli eventi di emissione suono. Non c'è un manager centrale che coordina tutti i nemici.

### Ciclo di Vita Completo

1. **Emissione Suono** → Evento `SOSoundEmissionDataEvent` sollevato
2. **Registrazione** → Ogni `EnemyAI` ascolta l'evento e aggiunge il suono alla sua lista attiva, salva la posizione come "ultima posizione del rumore"
3. **Accumulo Continuo** → Ogni frame, ogni nemico calcola il contributo di ogni suono attivo
4. **Decadimento** → Il rumore totale decade nel tempo
5. **Controllo Soglie** → Se supera `NoiseThreshold`, inizia a inseguire
6. **Inseguimento Minimo** → Per i primi `MinChaseDuration` secondi, insegue sempre il player
7. **Dopo MinChaseDuration**:
   - Se rumore alto → continua a inseguire
   - Se rumore basso → va a investigare la posizione del rumore
   - Se durante investigazione il rumore risale → torna a inseguire
8. **Arrivo al Punto** → Se player non c'è e rumore basso → riproduce frase audio, solleva evento, torna a Patrol
9. **Perdere Chase** → Solo se rumore basso E distanza > `LoseRange` (dopo MinChaseDuration)

### 1. Emissione di Suono

Quando viene emesso un suono (da `AudioEmitter` o altri componenti), viene sollevato l'evento `SOSoundEmissionDataEvent` con i dati:
```csharp
SoundEmissionData {
    position: Vector3,      // Posizione dell'emissione
    radius: float,         // Raggio del suono
    intensity: float,       // Intensità (0-1)
    duration: float,       // Durata
    frequency: float       // Frequenza (0=Low, 1=Mid, 2=High)
}
```

### 2. Registrazione e Accumulo Continuo di Rumore

Quando un nemico riceve l'evento `SOSoundEmissionDataEvent`:

1. **Aggiunge il suono alla lista attiva**: Il suono viene memorizzato con la sua durata rimanente
2. **Non calcola immediatamente**: A differenza del sistema precedente, il contributo NON viene calcolato all'emissione
3. **Accumulo continuo**: Ogni frame, per ogni suono attivo nella lista:
   - Calcola la distanza corrente (il nemico potrebbe essersi mosso)
   - Calcola il contributo usando la formula
   - Accumula il contributo al livello di rumore totale

**Vantaggio**: Questo permette di gestire suoni duraturi (es. un ronzio continuo) che accumulano rumore per tutta la loro durata, non solo all'inizio.

#### Formula Principale (per frame)

**Formula completa implementata:**

```
noiseAccumulation += intensity × frequencyMultiplier(frequency) × distanceMultiplier(distance) × Time.deltaTime
```

Dove `distanceMultiplier` usa la **formula di Attraction.cs**:

```
distanceMultiplier = intensityFactor / ((distance + 0.01) × rangeFactor)^decay
```

**Formula matematica completa:**

```
noiseAccumulation_per_frame = intensity × freqMult(freq) × (intensityFactor / ((d + 0.01) × rangeFactor)^decay) × Δt
```

**Accumulo continuo:** Il rumore viene accumulato ogni frame per ogni suono attivo, permettendo di gestire suoni duraturi.

**Parametri:**
- `intensity`: Intensità del suono (0.0 - 1.0) - da `SoundEmissionData`
- `frequency`: Frequenza del suono (0.0 = Low, 1.0 = Mid, 2.0 = High) - da `SoundEmissionData`
- `distance`: Distanza tra la posizione del suono e la posizione del nemico - calcolata ogni frame
- `intensityFactor`: Da `SOEnemyData.NoiseIntensityFactor` (default: 1.0)
- `rangeFactor`: Da `SOEnemyData.NoiseRangeFactor` (default: 1.0)
- `decay`: Da `SOEnemyData.NoiseDistanceDecay` (default: 1.0)
- `Δt`: `Time.deltaTime` - Tempo trascorso dall'ultimo frame

#### Moltiplicatore di Frequenza

```csharp
frequencyMultiplier(frequency) = {
    1.0  se frequency ≤ 0.5    // Low frequency
    1.5  se 0.5 < frequency ≤ 1.5  // Mid frequency
    2.0  se frequency > 1.5   // High frequency
}
```

**Formula matematica:**
```
freqMult = {
    1.0  if frequency ≤ 0.5
    1.5  if 0.5 < frequency ≤ 1.5
    2.0  if frequency > 1.5
}
```

#### Moltiplicatore di Distanza

**Formula esatta (da Attraction.cs):**

```csharp
distanceMultiplier = intensityFactor / Mathf.Pow((distance + 0.01f) * rangeFactor, decay)
```

**Formula matematica:**

```
distMult(d) = intensityFactor / ((d + 0.01) × rangeFactor)^decay
```

**Parametri:**
- `d` = distance (distanza corrente tra suono e nemico)
- `intensityFactor` = `SOEnemyData.NoiseIntensityFactor` (default: 1.0)
- `rangeFactor` = `SOEnemyData.NoiseRangeFactor` (default: 1.0)
- `decay` = `SOEnemyData.NoiseDistanceDecay` (default: 1.0)

**Limiti:**
- Se `distance > MaxNoisePerceptionDistance`: `distMult = 0` (suono troppo lontano)
- Il `+ 0.01` previene divisione per zero quando `distance = 0`

**Comportamento:**
- Con `decay = 1.0`: Falloff lineare inverso (1/distance) - comportamento standard
- Con `decay = 2.0`: Falloff quadratico inverso (1/distance²) - più realistico, suoni più localizzati
- Con `decay < 1.0`: Falloff più lento, suoni si sentono da più lontano
- Con `decay > 2.0`: Falloff molto rapido, suoni molto localizzati

**Nota:** Questa è l'**unica formula** usata per il calcolo della distanza. Non ci sono formule alternative o zone diverse.

#### Esempio di Calcolo Completo (per frame)

**Scenario:**
- Intensità: 0.8
- Frequenza: 2.0 (High)
- Distanza: 12 metri
- MaxNoisePerceptionDistance: 50 metri
- Parametri: intensityFactor=1.0, rangeFactor=1.0, decay=1.0
- Time.deltaTime: 0.016s (60 FPS)

**Calcolo per frame:**
1. `frequencyMultiplier(2.0)` = 2.0 (High frequency)
2. `distanceMultiplier(12)`:
   - `distMult` = 1.0 / Pow((12 + 0.01) × 1.0, 1.0)
   - `distMult` = 1.0 / 12.01 = **0.0833**
3. `noiseAccumulation` = 0.8 × 2.0 × 0.0833 × 0.016 = **0.00213** per frame

**Nota:** Con un suono che dura 2 secondi (120 frame), l'accumulo totale sarebbe: 0.00213 × 120 = **0.256**

### 3. Gestione Suoni Attivi e Decadimento

Ogni frame, in `EnemyAI.Update()`, viene eseguito questo processo:

```csharp
// STEP 1: Rimuovi suoni scaduti
foreach (activeSound in _activeSounds) {
    activeSound.timeRemaining -= Time.deltaTime
    if (activeSound.timeRemaining <= 0) {
        remove from list  // Il suono è finito, non contribuisce più
    }
}

// STEP 2: Accumula rumore da tutti i suoni attivi
float noiseAccumulation = 0f;
foreach (activeSound in _activeSounds) {
    // Calcola distanza corrente (il nemico potrebbe essersi mosso)
    float distance = Vector3.Distance(activeSound.position, transform.position);
    
    // Salta se troppo lontano
    if (distance > MaxNoisePerceptionDistance) continue;
    
    // Calcola contributo per questo frame
    float freqMult = GetFrequencyMultiplier(activeSound.frequency);
    float distMult = GetDistanceMultiplier(distance, activeSound.radius);
    noiseAccumulation += activeSound.intensity × freqMult × distMult × Time.deltaTime;
}

// STEP 3: Aggiungi accumulo al livello totale
_currentNoiseLevel += noiseAccumulation;

// STEP 4: Applica decadimento
_currentNoiseLevel -= NoiseDecayRate × Time.deltaTime;
_currentNoiseLevel = Max(0, _currentNoiseLevel);  // Non può andare sotto 0

// STEP 5: Controlla soglie e notifica UI
UpdateNoiseUI();
```

**Punti chiave:**
- I suoni duraturi continuano ad accumulare rumore per tutta la loro durata
- La distanza viene ricalcolata ogni frame (il nemico potrebbe muoversi)
- Il decadimento applica una costante riduzione del rumore nel tempo
- Se il nemico si allontana da un suono, il contributo diminuisce automaticamente

### 4. Controllo Soglie e Transizioni

#### In PatrolEnemyState:
```csharp
if (currentNoiseLevel >= NoiseThreshold) {
    // Inizia a inseguire
    SwitchState(ChaseState)
}
// Nota: Non c'è più fallback sulla distanza - solo il rumore può attivare l'inseguimento
```

#### In ChaseEnemyState:
```csharp
// Quando entra in Chase
Enter() {
    // Salva la posizione del player come "ultima posizione del rumore"
    SetLastNoisePosition(player.position)
    
    // Inizia timer minimo di inseguimento
    chaseStartTime = Time.time
    minChaseDurationElapsed = false
    
    // Mantiene ChaseSpeed durante tutto l'inseguimento
    agent.speed = ChaseSpeed
}

// Durante l'inseguimento
Update() {
    float currentNoiseLevel = GetNoiseLevel()
    bool isInvestigating = false
    
    // PRIMI 3 SECONDI: Insegue sempre il player, indipendentemente dal rumore
    if (!minChaseDurationElapsed) {
        if (Time.time - chaseStartTime >= MinChaseDuration) {
            minChaseDurationElapsed = true
        }
        SetDestination(player.position)  // Continua a inseguire player
    }
    // DOPO 3 SECONDI: Controlla rumore
    else {
        if (currentNoiseLevel >= NoiseThreshold) {
            // Rumore alto (o risalito) - continua a inseguire player
            SetDestination(player.position)
        }
        else if (currentNoiseLevel < NoiseLoseThreshold) {
            // Rumore sceso - va a investigare l'ultima posizione del rumore
            if (HasLastNoisePosition()) {
                Vector3 noisePos = GetLastNoisePosition()
                float distToNoise = Distance(transform.position, noisePos)
                
                // Se arriva al punto del rumore (raggio 2m)
                if (distToNoise < 2f) {
                    // Verifica: player non c'è E rumore ancora basso
                    float distToPlayerAtNoise = Distance(noisePos, player.position)
                    bool playerNotThere = distToPlayerAtNoise > AttackRange
                    bool noiseStillLow = currentNoiseLevel < NoiseLoseThreshold
                    
                    if (playerNotThere && noiseStillLow) {
                        // Non trova nulla - riproduce frase audio, solleva evento, torna a Patrol
                        PlayInvestigationPhrase()  // Riproduce AudioClip random
                        ClearLastNoisePosition()
                        SwitchState(PatrolState)
                        return
                    } else {
                        // Player c'è o rumore risalito - continua a inseguire
                        SetDestination(player.position)
                        return
                    }
                }
                
                // Altrimenti, continua a investigare (mantiene ChaseSpeed)
                SetDestination(noisePos)
                isInvestigating = true
            } else {
                // Nessuna posizione da investigare, torna a Patrol
                SwitchState(PatrolState)
                return
            }
        }
        else {
            // Rumore tra NoiseLoseThreshold e NoiseThreshold - continua a inseguire
            SetDestination(player.position)
        }
    }
    
    // Perdere chase: ENTRAMBE le condizioni devono essere vere
    // 1. Rumore < NoiseLoseThreshold
    // 2. Distanza > LoseRange
    // Solo dopo MinChaseDuration e solo se non sta investigando
    if (minChaseDurationElapsed && 
        currentNoiseLevel < NoiseLoseThreshold && 
        distToPlayer > LoseRange && 
        !isInvestigating) {
        SwitchState(PatrolState)
        return
    }
    
    // Controllo attacco (solo se non sta investigando)
    if (!isInvestigating && distToPlayer <= AttackRange) {
        SwitchState(AttackState)
    }
}
```

**Comportamento:**
1. **Primi 3 secondi**: Insegue sempre il player (garantisce che l'inseguimento inizi anche se il nemico è lontano)
2. **Dopo 3 secondi**: 
   - Se rumore >= `NoiseThreshold` → continua a inseguire
   - Se rumore < `NoiseLoseThreshold` → va a investigare la posizione del rumore (mantiene `ChaseSpeed`)
   - Se durante investigazione il rumore risale >= `NoiseThreshold` → torna immediatamente a inseguire
3. **Arrivo al punto**:
   - Se player NON c'è E rumore basso → riproduce frase audio random, solleva evento, torna a Patrol
   - Se player c'è O rumore risalito → continua a inseguire
4. **Perdere chase**: Solo se rumore basso E distanza > `LoseRange` (dopo MinChaseDuration, non durante investigazione)
5. **Bilanciamento**: Il player non può fare rumore e scappare - il nemico va comunque a controllare dove ha sentito il suono, mantenendo la velocità di inseguimento

### 5. Notifica UI

La UI viene notificata quando:
- Il fastidio **supera** `NoiseUIThreshold` (mostra barra)
- Il fastidio **scende sotto** `NoiseUIThreshold` (nasconde barra)
- Il fastidio è tra `NoiseUIThreshold` e `NoiseThreshold` (aggiorna valore)

```csharp
EnemyNoiseData {
    enemy: EnemyAI,              // Quale nemico
    currentNoiseLevel: float,    // Valore corrente
    uiThreshold: float,          // Soglia UI
    chaseThreshold: float,        // Soglia chasing
    shouldShowUI: bool,           // Mostrare UI?
    GetNormalizedNoise(): float   // 0-1 per barra
}
```

---

## Componenti Principali

### EnemyAI

**Responsabilità:**
- Ascolta `SOSoundEmissionDataEvent`
- Calcola e accumula il fastidio
- Gestisce il decadimento nel tempo
- Notifica la UI quando necessario
- Gestisce la riproduzione audio delle frasi di investigazione
- Traccia la posizione dell'ultimo rumore percepito

**Componenti richiesti:**
- `NavMeshAgent`: Per il movimento
- `Animator`: Per le animazioni
- `AudioSource`: Per riprodurre le frasi audio di investigazione

**Metodi pubblici:**
- `GetNoiseLevel()`: Restituisce il fastidio corrente
- `SetNoiseLevel(float level)`: Imposta il livello di rumore (usato quando entra in chase)
- `ResetNoiseLevel()`: Resetta il fastidio e la lista di suoni attivi
- `OnPlayerHit()`: Gestisce il colpo al player (delega allo stato corrente)
- `PlayInvestigationPhrase()`: Riproduce una frase audio random e solleva l'evento di investigazione
- `GetLastNoisePosition()`: Restituisce l'ultima posizione del rumore percepito
- `HasLastNoisePosition()`: Verifica se c'è una posizione del rumore salvata
- `SetLastNoisePosition(Vector3 position)`: Salva la posizione del rumore
- `ClearLastNoisePosition()`: Cancella la posizione del rumore salvata

**Campi serializzati:**
- `newAudioSphereEvent`: Evento da ascoltare (SOSoundEmissionDataEvent)
- `noiseUIEvent`: Evento per notificare la UI (SOEnemyNoiseUIEvent)
- `investigationEvent`: Evento per notificare quando arriva al punto e non trova nulla (SOEnemyInvestigationEvent)

### SOEnemyData

**Parametri di Rumore:**
```csharp
NoiseUIThreshold = 5f              // Soglia per mostrare UI
NoiseThreshold = 10f               // Soglia per iniziare chasing
NoiseLoseThreshold = 3f            // Soglia per smettere di inseguire (deve essere < NoiseThreshold)
MinChaseDuration = 3f             // Durata minima di inseguimento in secondi
NoiseDecayRate = 1f                // Decadimento per secondo
MaxNoisePerceptionDistance = 50f   // Distanza max percezione

// Parametri formula Attraction.cs
NoiseIntensityFactor = 1f          // Fattore moltiplicativo intensità
NoiseRangeFactor = 1f              // Fattore moltiplicativo distanza
NoiseDistanceDecay = 1f            // Esponente falloff distanza
```

**Parametri di Investigazione:**
```csharp
InvestigationPhrases = AudioClip[]  // Array di clip audio da riprodurre quando arriva al punto e non trova nulla
```

**Ordine delle soglie:** `NoiseLoseThreshold < NoiseUIThreshold < NoiseThreshold`

### EnemyNoiseData

Struttura dati per la comunicazione con la UI:
```csharp
public struct EnemyNoiseData {
    public EnemyAI enemy;
    public float currentNoiseLevel;
    public float uiThreshold;
    public float chaseThreshold;
    public bool shouldShowUI;
    
    // Ritorna valore normalizzato 0-1 per la barra UI
    public float GetNormalizedNoise() {
        // Normalizza tra uiThreshold e chaseThreshold
    }
}
```

### PatrolEnemyState

**Logica di Transizione:**
1. Controlla fastidio → Se >= `NoiseThreshold` → Chase
2. Altrimenti continua Patrol

**Nota:** Non c'è più fallback sulla distanza. I nemici iniziano a inseguire **solo** quando il fastidio supera la soglia.

### ChaseEnemyState

**All'ingresso:**
- Salva la posizione del player come "ultima posizione del rumore" (per investigazione futura)
- Inizia timer minimo di `MinChaseDuration` secondi (default: 3)
- Imposta velocità a `ChaseSpeed` (mantenuta durante tutto l'inseguimento, incluso investigazione)
- Se il livello di rumore è sotto `NoiseThreshold`, lo imposta almeno a `NoiseThreshold` per garantire l'inizio dell'inseguimento

**Durante l'inseguimento - Primi 3 secondi:**
- Insegue sempre il player, indipendentemente dal livello di rumore
- Garantisce che l'inseguimento inizi anche se il nemico è lontano (40+ metri)
- Mantiene `ChaseSpeed` durante tutto il periodo

**Durante l'inseguimento - Dopo 3 secondi:**
- Se rumore >= `NoiseThreshold` → continua a inseguire il player
- Se rumore < `NoiseLoseThreshold` → va a investigare l'ultima posizione del rumore (mantiene `ChaseSpeed`)
- Se durante l'investigazione il rumore risale >= `NoiseThreshold` → torna immediatamente a inseguire il player
- Se arriva alla posizione del rumore (raggio 2m):
  - Se player NON c'è E rumore ancora basso → riproduce frase audio random, solleva evento, torna a Patrol
  - Se player c'è O rumore risalito → continua a inseguire il player
- Perdere chase: Solo se rumore < `NoiseLoseThreshold` E distanza > `LoseRange` (non durante investigazione)

**Bilanciamento gameplay:**
- Il player non può fare rumore e scappare senza conseguenze
- Anche se il player si ferma, il nemico arriva comunque nella zona dove ha sentito il rumore
- Il nemico mantiene la velocità di inseguimento anche durante l'investigazione
- Crea tensione: il player deve gestire sia la fuga che la riduzione del rumore
- Crea un trade-off strategico tra usare suoni (utili per echolocation) e attirare nemici
- Il nemico "ricorda" dove ha sentito il rumore e va a controllare, anche se il player si è allontanato

---

## Differenze rispetto alla Versione Precedente

### ❌ PRIMA (Sistema Vecchio)

1. **Nessun sistema di rumore**
   - I nemici iniziavano a inseguire solo in base alla distanza
   - `ChaseRange` era l'unico parametro

2. **Struttura EnemyState**
   - Naming senza underscore: `enemyAI`, `fsm`, `agent`, `animator`, `enemyData`
   - Nessun metodo `OnPlayerHit()`
   - `currentState` minuscolo e privato

3. **Eventi**
   - Usava `SONewAudioSphereEvent` (nome diverso)
   - Nessun sistema di notifica UI

4. **ChaseEnemyState**
   - Usava `ChaseRange` per tornare a Patrol (inconsistente)

### ✅ ADESSO (Sistema Nuovo)

1. **Sistema di rumore completo**
   - Ogni nemico calcola autonomamente il fastidio
   - Reazione basata su intensità, frequenza e distanza
   - Decadimento nel tempo

2. **Struttura EnemyState migliorata**
   - Naming con underscore: `_enemyAI`, `_fsm`, `_agent`, `_animator`, `_enemyData`
   - Metodo astratto `OnPlayerHit()` implementato in tutti gli stati
   - `CurrentState` maiuscolo e pubblico

3. **Eventi aggiornati**
   - Usa `SOSoundEmissionDataEvent` (nome standardizzato)
   - Sistema completo di notifica UI con `SOEnemyNoiseUIEvent`

4. **ChaseEnemyState corretto**
   - Usa `LoseRange` per tornare a Patrol (più logico: serve più distanza per "perdere" il nemico)

5. **Sistema UI integrato**
   - `EnemyNoiseData` per passare dati alla UI
   - Notifiche automatiche quando mostrare/nascondere
   - Valore normalizzato per la barra

6. **Sistema di investigazione completo**
   - Timer minimo di inseguimento (`MinChaseDuration`) per garantire che l'inseguimento inizi anche da lontano
   - Memoria della posizione del rumore per investigazione
   - Comportamento di investigazione quando il rumore scende
   - Riproduzione audio di frasi quando arriva al punto e non trova nulla
   - Evento `SOEnemyInvestigationEvent` per notificare l'investigazione
   - Mantenimento di `ChaseSpeed` durante l'investigazione
   - Logica "sottosoglia E abbastanza lontano" per perdere il chase (entrambe le condizioni)

7. **AudioSource integrato**
   - `EnemyAI` richiede `AudioSource` per riprodurre le frasi di investigazione
   - Metodo `PlayInvestigationPhrase()` per gestire la riproduzione audio
   - Array `InvestigationPhrases` (AudioClip[]) in `SOEnemyData`

### 📊 Confronto Visivo

| Aspetto | Prima | Adesso |
|---------|-------|--------|
| **Trigger Chasing** | Solo distanza | Rumore OPPURE distanza |
| **Calcolo Rumore** | ❌ Non esisteva | ✅ Per ogni nemico |
| **UI Warning** | ❌ Non esisteva | ✅ Barra quando fastidio alto |
| **Frequenza Suono** | ❌ Ignorata | ✅ Influisce sul rumore |
| **Decadimento** | ❌ Non esisteva | ✅ Decade nel tempo |
| **LoseRange** | ❌ Non usato | ✅ Usato correttamente |
| **Timer Minimo Chase** | ❌ Non esisteva | ✅ MinChaseDuration (3s) |
| **Investigazione** | ❌ Non esisteva | ✅ Va al punto del rumore |
| **Frasi Audio** | ❌ Non esisteva | ✅ AudioClip quando non trova nulla |
| **Rumore che Risale** | ❌ Non gestito | ✅ Torna a inseguire durante investigazione |
| **Perdere Chase** | ❌ Solo distanza | ✅ Rumore E distanza (entrambe) |


## Parametri di Tuning

### NoiseUIThreshold (default: 5)
- **Basso** (2-3): UI appare presto, più avvertimenti
- **Alto** (7-8): UI appare tardi, meno avvertimenti
- **Consiglio**: 40-50% di `NoiseThreshold`

### NoiseThreshold (default: 10)
- **Basso** (5-7): Nemici più aggressivi, reagiscono a suoni deboli
- **Alto** (15-20): Nemici più pazienti, servono suoni forti
- **Consiglio**: Basato su intensità media dei suoni nel gioco

### NoiseLoseThreshold (default: 3)
- **Basso** (1-2): Nemici smettono di inseguire facilmente, più perdonanti
- **Alto** (5-7): Nemici persistenti, continuano a inseguire anche con poco rumore
- **Consiglio**: 30-50% di `NoiseThreshold` per un buon bilanciamento
- **Importante**: Deve essere < `NoiseUIThreshold` per evitare loop di transizioni

### MinChaseDuration (default: 3 secondi)
- **Basso** (1-2): Inseguimento minimo breve, nemici più reattivi
- **Alto** (5-7): Inseguimento minimo lungo, nemici più persistenti
- **Consiglio**: 3 secondi è un buon bilanciamento - garantisce che l'inseguimento inizi anche da lontano
- **Effetto**: Durante questo periodo, il nemico insegue sempre il player, anche se il rumore scende

### NoiseDecayRate (default: 1/sec)
- **Basso** (0.5): Rumore persiste a lungo, nemici "ricordano" più a lungo
- **Alto** (2-3): Rumore decade velocemente, nemici si calmano presto
- **Consiglio**: 1-2 per un equilibrio naturale

### MaxNoisePerceptionDistance (default: 50)
- **Basso** (20-30): Nemici sentono solo suoni vicini
- **Alto** (70-100): Nemici sentono suoni lontani
- **Consiglio**: Basato sulla dimensione media delle aree di gioco

### NoiseIntensityFactor (default: 1.0)
- **Basso** (0.5-0.7): Suoni contribuiscono meno al rumore
- **Alto** (1.5-2.0): Suoni contribuiscono di più
- **Consiglio**: Usa per bilanciare globalmente l'accumulo

### NoiseRangeFactor (default: 1.0)
- **Basso** (0.5-0.7): Distanza ha meno effetto (suoni più "uniformi")
- **Alto** (1.5-2.0): Distanza ha più effetto (suoni più "localizzati")
- **Consiglio**: 1.0 per comportamento standard

### NoiseDistanceDecay (default: 1.0)
- **Basso** (0.5-0.8): Falloff più lento, suoni si sentono da più lontano
- **Alto** (1.5-2.5): Falloff più rapido, suoni più localizzati
- **Consiglio**: 1.0 = lineare inverso, 2.0 = quadratico inverso (più realistico)

### Formula di Contributo

Vedi la sezione [Calcolo del Contributo di Rumore](#2-accumulo-continuo-di-rumore) per la formula completa e dettagliata.

**Formula base (per frame):**
```
accumulo_per_frame = intensity × frequencyMultiplier(frequency) × distanceMultiplier(distance) × Time.deltaTime
```

Dove `distanceMultiplier` usa la formula di Attraction.cs:
```
distanceMultiplier = intensityFactor / ((distance + 0.01) × rangeFactor)^decay
```

**Esempi pratici (accumulo per frame a 60 FPS, deltaTime = 0.016s):**

1. **Suono forte, High frequency, vicino:**
   - Intensity: 1.0
   - Frequency: 2.0 (High) → multiplier: 2.0
   - Distance: 5m → multiplier: 1.0 / (5.01 × 1.0)^1.0 = 0.2
   - **Per frame:** `1.0 × 2.0 × 0.2 × 0.016 = 0.0064`
   - **Per secondo (60 frame):** `0.0064 × 60 = 0.384`

2. **Suono debole, Low frequency, lontano:**
   - Intensity: 0.3
   - Frequency: 0.0 (Low) → multiplier: 1.0
   - Distance: 20m → multiplier: 1.0 / (20.01 × 1.0)^1.0 = 0.05
   - **Per frame:** `0.3 × 1.0 × 0.05 × 0.016 = 0.00024`
   - **Per secondo:** `0.00024 × 60 = 0.0144`

3. **Suono medio, Mid frequency, medio:**
   - Intensity: 0.6
   - Frequency: 1.0 (Mid) → multiplier: 1.5
   - Distance: 10m → multiplier: 1.0 / (10.01 × 1.0)^1.0 = 0.1
   - **Per frame:** `0.6 × 1.5 × 0.1 × 0.016 = 0.00144`
   - **Per secondo:** `0.00144 × 60 = 0.0864`

---

## Note Importanti

1. **Ogni nemico calcola autonomamente**: Non c'è un manager globale, ogni `EnemyAI` è indipendente
2. **Timer minimo di inseguimento**: I primi `MinChaseDuration` secondi, il nemico insegue sempre il player, garantendo che l'inseguimento inizi anche se il nemico è lontano
3. **Investigazione intelligente**: Quando il rumore scende, il nemico va a investigare la posizione dove ha sentito il rumore, non torna subito a Patrol. Questo crea tensione e bilanciamento gameplay
4. **Perdere il chase**: Il nemico smette di inseguire solo se ENTRAMBE le condizioni sono vere: rumore < `NoiseLoseThreshold` E distanza > `LoseRange` (dopo MinChaseDuration)
5. **Rumore che risale**: Se durante l'investigazione il rumore risale sopra `NoiseThreshold`, il nemico torna immediatamente a inseguire il player
6. **Velocità durante investigazione**: Il nemico mantiene `ChaseSpeed` anche quando va a investigare, creando tensione continua
7. **Frasi audio**: Le frasi vengono riprodotte automaticamente quando il nemico arriva al punto e non trova nulla. L'evento viene sollevato per permettere altre azioni (log, UI, effetti)
8. **UI per nemico**: Se ci sono più nemici, la UI dovrebbe gestire quale mostrare (es. il più vicino o quello con fastidio più alto)
9. **LoseRange vs ChaseRange**: 
   - `ChaseRange`: Distanza per iniziare a inseguire (ora usato solo come riferimento, il trigger è il rumore)
   - `LoseRange`: Distanza per smettere di inseguire (deve essere >= ChaseRange)
10. **Frequenza**: Il campo `frequency` in `SoundEmissionData` è un float (0=Low, 1=Mid, 2=High)
11. **AudioSource richiesto**: `EnemyAI` richiede automaticamente `AudioSource` per riprodurre le frasi di investigazione



## Esempi di Configurazione

### Nemico Aggressivo
```
NoiseUIThreshold: 3
NoiseThreshold: 6
NoiseLoseThreshold: 1.5
MinChaseDuration: 2.0
NoiseDecayRate: 0.5
MaxNoisePerceptionDistance: 60
InvestigationPhrases: [AudioClip1, AudioClip2, AudioClip3]
```

### Nemico Paziente
```
NoiseUIThreshold: 7
NoiseThreshold: 15
NoiseLoseThreshold: 5.0
MinChaseDuration: 4.0
NoiseDecayRate: 2.0
MaxNoisePerceptionDistance: 30
InvestigationPhrases: [AudioClip1, AudioClip2]
```

### Nemico Bilanciato (default)
```
NoiseUIThreshold: 5
NoiseThreshold: 10
NoiseLoseThreshold: 3.0
MinChaseDuration: 3.0
NoiseDecayRate: 1.0
MaxNoisePerceptionDistance: 50
InvestigationPhrases: [AudioClip1, AudioClip2, AudioClip3]
```


---

*Documentazione aggiornata dopo l'adattamento al nuovo branch con naming conventions, struttura migliorata, sistema di investigazione completo e frasi audio.*

