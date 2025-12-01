# Schema del Sistema Nemico - EchoCity

## 📊 Architettura Generale

```
┌─────────────────────────────────────────────────────────────────┐
│                        SISTEMA SUONO                            │
│  AudioEmitter → SOSoundEmissionDataEvent → EnemyAI             │
│  (1 suono alla volta, durata fissa)                             │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                         ENEMY AI                                │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  Update() - Ogni Frame:                                  │   │
│  │  1. UpdateActiveSound() - Gestisce durata suono          │   │
│  │  2. CalculateAttraction() - Calcola attraction            │   │
│  │     • Se suono attivo: accumula (formula Attraction.cs)  │   │
│  │     • Se no suono: applica decay                         │   │
│  │     • Clamp a 0 (mai negativo)                           │   │
│  │  3. UpdateNoiseUI() - Notifica UI se necessario          │   │
│  │  4. _fsm.Update(attraction) - Passa valore agli stati   │   │
│  └──────────────────────────────────────────────────────────┘   │
│                                                                  │
│  Output: attraction (valore pronto per gli stati)               │
│  Output: soundPosition (posizione del suono)                     │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                         ENEMY FSM                               │
│  Riceve: attraction (float)                                    │
│  Passa a: CurrentState.Update(attraction)                        │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
                    ┌─────────┴─────────┐
                    │   STATI NEMICO     │
                    └─────────┬─────────┘
```

---

## 🔄 Diagramma degli Stati (State Machine)

```
                    ┌──────────────┐
                    │    PATROL     │ ◄──┐
                    │               │    │
                    │ • Waypoints   │    │
                    │ • Attende     │    │
                    └───────┬───────┘    │
                            │            │
        attraction >= 1.0 (Threshold)    │
                            │            │
                            ▼            │
                    ┌──────────────┐    │
                    │    CHASE     │    │
                    │               │    │
                    │ • Insegue     │    │
                    │   PLAYER      │    │
                    │ • Min 3 sec   │    │
                    │ • Continua se │    │
                    │   > 0.8 E    │    │
                    │   player ≤15m│    │
                    └───────┬───────┘    │
                            │            │
        distToPlayer <= AttackRange      │
                            │            │
                            ▼            │
                    ┌──────────────┐    │
                    │    ATTACK    │    │
                    │               │    │
                    │ • Attacca     │    │
                    │ • Cooldown    │    │
                    └───────┬───────┘    │
                            │            │
        Dopo cooldown:      │            │
        • attraction >= 1.0 → CHASE     │
        • attraction < 0.8 → CHASE_SOUND│
        • altrimenti → CHASE             │
                            │            │
                            ▼            │
        ┌───────────────────────────────┐│
        │  CHASE (dopo 3 sec):          ││
        │  • attraction >= 1.0 → continua│
        │  • attraction > 0.8 E          ││
        │    player ≤15m → continua      ││
        │  • attraction < 0.8 O          ││
        │    player >15m → CHASE_SOUND   ││
        └───────────────────────────────┘│
                            │            │
                            ▼            │
                    ┌──────────────┐    │
                    │ CHASE_SOUND  │    │
                    │               │    │
                    │ • Va verso    │    │
                    │   posizione   │    │
                    │   ultimo suono│    │
                    └───────┬───────┘    │
                            │            │
        attraction >= 1.0 → CHASE       │
        Arrivato a suono (≤2m)          │
                            │            │
                            ▼            │
                    ┌──────────────┐    │
                    │STAND_EXAMINATE│   │
                    │               │    │
                    │ • Fermo       │    │
                    │ • Guarda      │    │
                    │   attorno     │    │
                    │ • Emette      │    │
                    │   frasi       │    │
                    └───────┬───────┘    │
                            │            │
        attraction >= 1.0 → CHASE       │
        Player <= 3m → CHASE           │
        Dopo 3 secondi → PATROL         │
                            │            │
                            └────────────┘
```

---

## 📋 Dettaglio Stati e Transizioni

### 🚶 PATROL STATE
**Input**: `attraction` (float)

**Comportamento**:
- Si muove tra waypoints
- Attende ai waypoints (WaypointPauseDuration)
- Velocità: PatrolSpeed

**Transizioni**:
```
attraction >= NoiseThreshold
    ↓
CHASE
```

**Note**: Nessun calcolo suono, solo reazione a `attraction`

---

### 🏃 CHASE STATE
**Input**: `attraction` (float)

**Comportamento**:
- **Insegue il PLAYER** (non il suono!)
- Velocità: ChaseSpeed
- Durante `MinChaseDuration` (3 sec): sempre insegue PLAYER (ignora attraction)
- Dopo `MinChaseDuration`: controlla attraction E distanza player

**Transizioni**:
```
PRIORITÀ 1: Se attraction >= 1.0
    → continua CHASE (PLAYER)

Durante MinChaseDuration (3 sec):
    distToPlayer <= AttackRange → ATTACK
    → sempre CHASE (PLAYER)

Dopo MinChaseDuration:
    PRIORITÀ 1: attraction >= 1.0 → continua CHASE (PLAYER)
    PRIORITÀ 2: attraction > 0.8 E player <= 15m → continua CHASE (PLAYER)
    PRIORITÀ 3: attraction < 0.8 O player > 15m → CHASE_SOUND
    distToPlayer <= AttackRange → ATTACK
```

**Note**: 
- **NON SMETTE MAI** di fare chasing finché `attraction > 0.8` E `player in range 15m`
- Insegue sempre PLAYER, mai la posizione del suono
- Se attraction > 1.0, sempre CHASE (in qualsiasi momento)

---

### ⚔️ ATTACK STATE
**Input**: `attraction` (float)

**Comportamento**:
- Fermo, animazione attacco
- Cooldown dopo attacco
- Ruota verso player durante cooldown

**Transizioni**:
```
Dopo cooldown:
    attraction >= NoiseThreshold → CHASE
    attraction < NoiseLoseThreshold → CHASE_SOUND
    altrimenti → CHASE
```

---

### 🎯 CHASE_SOUND STATE
**Input**: `attraction` (float)

**Comportamento**:
- Si muove verso ultima posizione nota del suono (quello che ha fatto scattare la chase)
- Velocità: ChaseSpeed
- Arrivo: distanza ≤ 2m

**Transizioni**:
```
PRIORITÀ 1: attraction >= 1.0 → CHASE (PLAYER)
Arrivato a suono (≤2m) → STAND_EXAMINATE
No sound position → PATROL
```

**Note**: 
- Usa `_enemyAI.GetSoundPosition()` salvata (ultimo suono sopra soglia)
- **Se attraction > 1.0 in QUALUNQUE MOMENTO**, interrompe e torna a CHASE del PLAYER

---

### 👀 STAND_AND_EXAMINATE STATE
**Input**: `attraction` (float)

**Comportamento**:
- Fermo alla posizione del suono
- Guarda attorno (ruota verso player)
- **Emette frase investigazione** (PlayInvestigationPhrase)
- Durata: 3 secondi
- Solo se player NON è nelle vicinanze (attraction < 0.8)

**Transizioni**:
```
PRIORITÀ 1: attraction >= 1.0 → CHASE (PLAYER)
PRIORITÀ 2: Player <= 3m → CHASE (PLAYER)
Dopo 3 secondi → PATROL (resetta attraction)
```

**Note**: 
- Quando finisce, chiama `_enemyAI.ResetAttraction()`
- **Se attraction > 1.0 in QUALUNQUE MOMENTO**, interrompe e torna a CHASE del PLAYER
- Se player è vicino (≤3m), interrompe e torna a CHASE

---

## 🧮 Calcolo Attraction (EnemyAI.CalculateAttraction)

### Formula (da Attraction.cs):
```
Se suono attivo E distanza <= MaxNoisePerceptionDistance:
    distance = Vector3.Distance(enemy.position, sound.position)
    
    frequencyMultiplier = 
        frequency <= 0 → 1.0x (Low)
        frequency <= 1 → 1.5x (Mid)
        frequency > 1  → 2.0x (High)
    
    distanceMultiplier = 1 / Pow((distance + 0.01) * NoiseRangeFactor, NoiseDistanceDecay)
    
    contribution = intensity 
                   * NoiseIntensityFactor 
                   * frequencyMultiplier 
                   * distanceMultiplier 
                   * Time.deltaTime
    
    attraction += contribution

Altrimenti (no suono o troppo lontano):
    attraction -= NoiseDecayRate * Time.deltaTime

Sempre:
    attraction = Max(0, attraction)  // Clamp a 0
```

### Parametri (SOEnemyData):
- `NoiseIntensityFactor`: moltiplicatore intensità
- `NoiseRangeFactor`: moltiplicatore range
- `NoiseDistanceDecay`: esponente decadimento distanza
- `NoiseDecayRate`: velocità decay quando no suono
- `MaxNoisePerceptionDistance`: distanza max percezione

---

## 🎚️ Soglie (Thresholds)

### Valori in SOEnemyData:
```
NoiseUIThreshold < NoiseLoseThreshold < NoiseThreshold
```

**Valori Default**:
- `NoiseUIThreshold = 0.5` (mostra UI bar)
- `NoiseLoseThreshold = 0.8` (soglia per continuare chase)
- `NoiseThreshold = 1.0` (soglia per iniziare chase)

### Comportamento per Attraction:
```
attraction = 0
    → PATROL

0 < attraction < NoiseUIThreshold
    → PATROL (UI nascosta)

NoiseUIThreshold <= attraction < NoiseThreshold
    → PATROL (UI visibile)

attraction >= 1.0 (NoiseThreshold)
    → CHASE (PLAYER)

Durante CHASE (dopo 3 sec):
    attraction >= 1.0
        → continua CHASE (PLAYER) [PRIORITÀ 1]
    
    attraction > 0.8 E player <= 15m
        → continua CHASE (PLAYER) [PRIORITÀ 2]
    
    attraction < 0.8 O player > 15m
        → CHASE_SOUND [PRIORITÀ 3]

In QUALUNQUE STATO:
    attraction >= 1.0
        → CHASE (PLAYER) [PRIORITÀ ASSOLUTA]
```

### Regola Speciale:
**NON SMETTE MAI** di fare chasing finché:
- `attraction > 0.8` **E**
- `player in range 15m` (ChaseRange)

---

## 🔄 Flusso Completo Esempio

### Scenario: Player emette suono

```
1. AudioEmitter emette suono
   ↓
2. SOSoundEmissionDataEvent.RaiseEvent(soundData)
   ↓
3. EnemyAI.OnSoundEmitted(soundData)
   • Sostituisce _activeSound (solo 1 suono alla volta)
   • Salva _soundPosition (per ChaseSoundState)
   • Imposta _soundTimeRemaining = duration
   ↓
4. Ogni frame: EnemyAI.Update()
   • UpdateActiveSound() - decrementa _soundTimeRemaining
   • CalculateAttraction() - accumula attraction
   • UpdateNoiseUI() - mostra UI se necessario
   • _fsm.Update(attraction)
   ↓
5. PatrolEnemyState.Update(attraction)
   • Se attraction >= 1.0 → CHASE (PLAYER)
   ↓
6. ChaseEnemyState.Update(attraction)
   • Insegue PLAYER (non suono!)
   • Durante 3 sec: sempre CHASE (PLAYER)
   • Dopo 3 sec:
     - Se attraction >= 1.0 → continua CHASE (PLAYER)
     - Se attraction > 0.8 E player <= 15m → continua CHASE (PLAYER)
     - Se attraction < 0.8 O player > 15m → CHASE_SOUND
   ↓
7. ChaseSoundState.Update(attraction)
   • Va verso posizione ultimo suono
   • Se attraction >= 1.0 → CHASE (PLAYER) [interrompe]
   • Se arrivato (≤2m) → STAND_EXAMINATE
   ↓
8. StandAndExaminateState.Update(attraction)
   • Fermo, guarda attorno
   • Emette frase
   • Se attraction >= 1.0 → CHASE (PLAYER) [interrompe]
   • Se player <= 3m → CHASE (PLAYER) [interrompe]
   • Dopo 3s → PATROL (resetta attraction)
```

---

## 🎯 Punti Chiave

1. **Un solo suono attivo**: ogni nuovo suono sostituisce il precedente
2. **CHASE insegue PLAYER**: durante CHASE, il nemico insegue sempre il PLAYER (non il suono)
3. **Attraction centralizzata**: tutto il calcolo è in EnemyAI, stati ricevono valore pronto
4. **Stati reattivi**: gli stati reagiscono solo a `attraction`, non calcolano nulla
5. **MinChaseDuration**: garantisce chase minimo di 3 secondi (sempre PLAYER)
6. **Investigation flow**: CHASE → CHASE_SOUND → STAND_EXAMINATE → PATROL
7. **Priorità assoluta**: se `attraction >= 1.0` in QUALUNQUE momento, torna a CHASE (PLAYER)
8. **Range continuo**: non smette mai di fare chasing finché `attraction > 0.8` E `player <= 15m`
9. **Soglie**: 
   - `1.0` = inizia chase
   - `0.8` = continua chase (dopo 3 sec)
   - `0.5` = mostra UI

---

## 📊 Flusso Dati

```
┌─────────────┐
│ Sound Event │
└──────┬──────┘
       │
       ▼
┌─────────────────┐
│ EnemyAI         │
│ • _activeSound  │
│ • _attraction   │
│ • _soundPosition│
└──────┬──────────┘
       │
       │ CalculateAttraction()
       │
       ▼
┌─────────────────┐
│ FSM.Update()    │
│ (attraction)    │
└──────┬──────────┘
       │
       ▼
┌─────────────────┐
│ CurrentState    │
│ .Update(        │
│   attraction)   │
└─────────────────┘
```

---

## 🔧 Metodi Pubblici EnemyAI (per Stati)

```csharp
float GetAttraction()              // Valore attraction corrente
Vector3 GetSoundPosition()         // Posizione suono attivo
bool HasActiveSound()              // C'è un suono attivo?
void ResetAttraction()             // Resetta attraction a 0
void PlayInvestigationPhrase()     // Emette frase investigazione
```

---

## ⚙️ Configurazione (SOEnemyData)

### Ranges:
- `ChaseRange`: (non più usato direttamente)
- `AttackRange`: distanza per attaccare
- `LoseRange`: (non più usato direttamente)

### Noise Detection:
- `NoiseUIThreshold`: soglia per mostrare UI
- `NoiseThreshold`: soglia per iniziare chase
- `NoiseLoseThreshold`: soglia per perdere chase
- `MinChaseDuration`: durata minima chase (secondi)

### Noise Calculation:
- `MaxNoisePerceptionDistance`: distanza max percezione
- `NoiseIntensityFactor`: moltiplicatore intensità
- `NoiseRangeFactor`: moltiplicatore range
- `NoiseDistanceDecay`: esponente decadimento
- `NoiseDecayRate`: velocità decay

### Investigation:
- `InvestigationPhrases[]`: array AudioClip per frasi

---

**Schema creato il**: 2024
**Versione sistema**: Centralizzato con un solo suono attivo

