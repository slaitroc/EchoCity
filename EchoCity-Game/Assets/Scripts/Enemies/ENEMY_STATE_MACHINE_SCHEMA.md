# Schema Completo della FSM del Nemico

## Indice
1. [Parametri e Soglie](#parametri-e-soglie)
2. [Flag di Sistema](#flag-di-sistema)
3. [Stati Disponibili](#stati-disponibili)
4. [Trigger Globali](#trigger-globali)
5. [Transizioni Dettagliate](#transizioni-dettagliate)
6. [Diagramma di Flusso](#diagramma-di-flusso)
7. [Comportamento Post-Attacco](#comportamento-post-attacco)

---

## Parametri e Soglie

### Parametri di Distanza
- **`D_enter`** = 10m: Soglia per chase per distanza (prossimità)
- **`D_exit`** = 15m: Soglia per perdere definitivamente il player
- **`AttackRange`** = 3m: Distanza per attaccare il player

### Parametri di Attraction
- **`NoiseThreshold`** (A_enter) = 1.0: Soglia per iniziare MandatoryChase
- **`NoiseLoseThreshold`** (A_exit) = 0.8: Soglia per continuare chase
- **`NoiseDecayRate`** = 0.5: Velocità di decadimento attraction/sec quando non ci sono suoni

### Variabili
- **`A`** = Attraction (livello di attrazione del nemico verso il player)
- **`d`** = distToPlayer (distanza dal player in metri)

---

## Flag di Sistema

### HasConfirmedPlayer
- **`true`**: Il nemico ha già confermato l'esistenza del player (ha fatto un vero chase)
- **`false`**: Il nemico non ha ancora confermato il player (solo sospetto)
- **Settato a `true` in**: `ChaseEnemyState.Enter()`, `ChaseDistanceState.Enter()`
- **Resettato a `false` in**: `PatrolEnemyState.Enter()`

### IsNoiseChaseActive
- **`true`**: Il nemico sta inseguendo perché ha superato la soglia di rumore o è arrivato da un suono investigato
- **`false`**: Il nemico sta inseguendo solo per distanza (senza threshold superata)
- **Settato a `true` in**: `MandatoryChaseState.Enter()`, `CheckSoundState` (quando passa a chase)
- **Resettato a `false` in**: `PatrolEnemyState.Enter()`, `LostTargetState.Enter()`

---

## Stati Disponibili

### 1. **PatrolEnemyState**
- **Descrizione**: Stato di pattugliamento normale
- **Comportamento**: Si muove tra waypoints, aspetta ai waypoint
- **Animazione**: Locomotion (Speed variabile)
- **Flag**: Resetta `HasConfirmedPlayer = false`, `IsNoiseChaseActive = false`

### 2. **MandatoryChaseState**
- **Descrizione**: Chase obbligatorio di 3 secondi quando A >= 1.0
- **Durata**: 3 secondi fissi
- **Comportamento**: Insegue sempre il player durante i 3 secondi
- **Animazione**: Locomotion (Speed = 1.0)
- **Flag**: Imposta `IsNoiseChaseActive = true`

### 3. **ChaseEnemyState**
- **Descrizione**: Chase normale basato su attraction
- **Comportamento**: Insegue il player finché A >= 0.8 OPPURE d <= 10m
- **Animazione**: Locomotion (Speed = 1.0)
- **Flag**: Imposta `HasConfirmedPlayer = true`

### 4. **ChaseDistanceState**
- **Descrizione**: Chase per prossimità (ignora attraction)
- **Comportamento**: Insegue il player finché d <= 15m
- **Animazione**: Locomotion (Speed = 1.0)
- **Flag**: Imposta `HasConfirmedPlayer = true`

### 5. **AttackEnemyState**
- **Descrizione**: Stato di attacco al player
- **Comportamento**: Esegue animazione di attacco, poi cooldown. Se player è ancora in range, può saltare il cooldown e riprendere subito la chase
- **Animazione**: Attack (isAttacking = true)
- **Logica post-attacco**: 
  - Skip cooldown: Se `HasConfirmedPlayer == true` usa `D_exit` (15m), altrimenti `D_enter` (10m)
  - Dopo cooldown: Dipende da `IsNoiseChaseActive`
- **Exit()**: Solo cleanup, nessuna transizione

### 6. **CheckSoundState**
- **Descrizione**: Nemico si muove verso l'ultima posizione nota del suono
- **Comportamento**: Arriva al punto, si ferma, poi decide il prossimo stato
- **Animazione**: Locomotion (Speed = 1.0 durante movimento, 0.0 quando arriva)
- **Durata fermo**: Nessuna (decide immediatamente quando arriva)

### 7. **StandAndExaminateState**
- **Descrizione**: Stato di SOSPETTO prima di confermare il player
- **Comportamento**: Fermo, guarda intorno, emette frasi di sospetto, emette suono echolocation
- **Animazione**: Anim_StandAndExamine (trigger)
- **Durata**: 7 secondi
- **Condizione**: Solo se `HasConfirmedPlayer == false`

### 8. **LostTargetState**
- **Descrizione**: Stato di PERDITA dopo un vero chase
- **Comportamento**: Fermo, dice "Ti ritroverò", poi torna a patrol
- **Animazione**: Anim_LostTarget (trigger)
- **Durata**: 2 secondi
- **Condizione**: Solo se `HasConfirmedPlayer == true`

### 9. **GettingConfusedState**
- **Descrizione**: Nemico distratto da ConfusingSoundSource
- **Comportamento**: Si muove verso la sorgente confusa, si ferma, animazione confusa
- **Animazione**: Anim_Confused (trigger)
- **Durata confusione**: `ConfusingSoundDuration` (default 6s)
- **Condizione**: Solo da stati non-chase

---

## Trigger Globali

I trigger globali vengono controllati in `EnemyFSM.Update()` PRIMA di chiamare `CurrentState.Update()`.

### Trigger 1: MandatoryChase
- **Condizione**: `A >= NoiseThreshold` (1.0)
- **Da**: QUALSIASI stato (tranne se già in MandatoryChaseState)
- **A**: `MandatoryChaseState`
- **Priorità**: Massima (controllato per primo)

### Trigger 2: ChaseDistance
- **Condizione**: `d <= D_enter` (10m)
- **Da**: Solo stati non-chase:
  - `PatrolEnemyState`
  - `CheckSoundState`
  - `StandAndExaminateState`
  - `LostTargetState`
  - `GettingConfusedState`
- **A**: `ChaseDistanceState`
- **Priorità**: Seconda (controllato dopo MandatoryChase)

---

## Transizioni Dettagliate

### Da PatrolEnemyState
```
→ MandatoryChaseState: Se A >= 1.0 (trigger globale)
→ ChaseDistanceState: Se d <= 10m (trigger globale)
→ GettingConfusedState: Se ConfusingSound attivo e condizioni soddisfatte
→ (rimane in Patrol): Altrimenti
```

### Da MandatoryChaseState
**Dopo 3 secondi:**
```
→ AttackState: Se d <= AttackRange
→ ChaseDistanceState: Se (A >= 0.8 OPPURE d <= 10m) E d <= 10m
→ ChaseEnemyState: Se (A >= 0.8 OPPURE d <= 10m) E d > 10m
→ CheckSoundState: Se NON ci sono condizioni per chase E c'è sound position
→ PatrolState: Se NON ci sono condizioni per chase E NON c'è sound position
```

**Durante i 3 secondi:**
```
→ AttackState: Se d <= AttackRange
→ (rimane in MandatoryChase): Altrimenti
```

### Da ChaseEnemyState
```
→ AttackState: Se d <= AttackRange
→ LostTargetState: Se A < 0.8 E d > 10m
→ (rimane in Chase): Se A >= 0.8 OPPURE d <= 10m
```

### Da ChaseDistanceState
```
→ AttackState: Se d <= AttackRange
→ LostTargetState: Se d > 15m
→ (rimane in ChaseDistance): Se d <= 15m
```

### Da AttackEnemyState
**Durante attacco:**
```
→ (rimane in Attack): Finché attacco non finisce
```

**Dopo attacco (cooldown):**
- **Skip cooldown immediato (cooldown >= 0.1s):**
  - **Se HasConfirmedPlayer == true:**
    ```
    → ChaseDistanceState: Se d <= 15m E IsNoiseChaseActive == false
    → ChaseEnemyState: Se d <= 15m E IsNoiseChaseActive == true
    ```
  - **Se HasConfirmedPlayer == false:**
    ```
    → ChaseDistanceState: Se d <= 10m E IsNoiseChaseActive == false
    → ChaseEnemyState: Se d <= 10m E IsNoiseChaseActive == true
    ```

- **Se cooldown completo:**
  - **Caso 1: IsNoiseChaseActive == true (chase da rumore)**
    ```
    → ChaseEnemyState: Se A >= 1.0 OPPURE d <= 10m
    → CheckSoundState: Se A < 0.8
    → ChaseEnemyState: Se 0.8 <= A < 1.0 (caso intermedio)
    ```
  
  - **Caso 2: IsNoiseChaseActive == false (chase solo distanza)**
    ```
    → ChaseDistanceState: Se d <= 15m
    → LostTargetState: Se d > 15m
    ```

**Nota**: `Exit()` non gestisce transizioni, solo cleanup (reset animator, attackEnded, attackCollider).

### Da CheckSoundState
**Durante movimento verso suono:**
```
→ GettingConfusedState: Se ConfusingSound attivo e più forte
→ (rimane in CheckSound): Altrimenti
```

**Quando arriva al punto del suono (distanza <= 2m):**
```
→ ChaseDistanceState: Se d <= 10m (imposta IsNoiseChaseActive = true)
→ ChaseEnemyState: Se A >= 1.0 (imposta IsNoiseChaseActive = true)
→ StandAndExaminateState: Altrimenti (sempre, indipendentemente da attraction)
```

**Se non c'è sound position:**
```
→ PatrolState: Immediatamente
```

### Da StandAndExaminateState
```
→ MandatoryChaseState: Se A >= 1.0 (trigger globale)
→ ChaseDistanceState: Se d <= 10m (trigger globale)
→ GettingConfusedState: Se ConfusingSound attivo
→ PatrolState: Se (dopo 7s) A < 0.8 E d > 10m
→ (rimane in StandAndExamine): Se (dopo 7s) A >= 0.8 OPPURE d <= 10m
```

**Nota**: Se `HasConfirmedPlayer == true` quando entra, va a `LostTargetState` (errore).

### Da LostTargetState
**Dopo 2 secondi:**
```
→ GettingConfusedState: Se ConfusingSound attivo
→ PatrolState: Altrimenti
```

**Durante i 2 secondi:**
```
→ ChaseDistanceState: Se d <= 10m (trigger globale - può interrompere)
→ (rimane in LostTarget): Altrimenti
```

### Da GettingConfusedState
**Durante movimento verso sorgente:**
```
→ ChaseDistanceState: Se d <= 10m
→ MandatoryChaseState: Se A >= 1.0
→ (rimane in GettingConfused): Altrimenti
```

**Quando arriva alla sorgente:**
```
→ (rimane fermo e confuso): Per ConfusingSoundDuration (6s)
```

**Dopo confusione:**
```
→ ChaseDistanceState: Se d <= 10m
→ MandatoryChaseState: Se A >= 1.0
→ PatrolState: Altrimenti
```

**Se sorgente si disattiva:**
```
→ ChaseDistanceState: Se d <= 10m
→ MandatoryChaseState: Se A >= 1.0
→ PatrolState: Altrimenti
```

---

## Diagramma di Flusso

```
                    [START]
                      |
                      v
                 ┌─────────┐
                 │ Patrol  │
                 └────┬────┘
                      |
        ┌─────────────┼─────────────┐
        |             |             |
   A>=1.0         d<=10m      ConfusingSound
        |             |             |
        v             v             v
┌───────────────┐ ┌──────────┐ ┌──────────────┐
│ MandatoryChase│ │ChaseDist │ │GettingConfused│
│   (3 secondi) │ │          │ │              │
└───────┬───────┘ └────┬──────┘ └──────┬───────┘
        |              |               |
        |         d>15m|               |
        |              v               |
        |         ┌─────────┐          |
        |         │LostTarget│         |
        |         └────┬────┘          |
        |              |               |
        v              |               |
   ┌────────┐         |               |
   │ Chase  │         |               |
   └───┬────┘         |               |
       |              |               |
   A<0.8              |               |
   E d>10m            |               |
       |              |               |
       v              |               |
  ┌─────────┐         |               |
  │LostTarget│        |               |
  └────┬────┘         |               |
       |              |               |
       └──────┬───────┘               |
              |                       |
              v                       |
         ┌─────────┐                 |
         │ Patrol  │◄────────────────┘
         └─────────┘

Da MandatoryChase (dopo 3s):
  - Se condizioni OK → Chase/ChaseDistance
  - Se no condizioni → CheckSound (se c'è suono) o Patrol

Da CheckSound (quando arriva):
  - Se d<=10m → ChaseDistance
  - Se A>=1.0 → Chase
  - Altrimenti → StandAndExamine

Da StandAndExamine (dopo 7s):
  - Se A<0.8 E d>10m → Patrol
  - Altrimenti → rimane in StandAndExamine

Da Attack:
  - Se player vicino:
    * HasConfirmedPlayer == true: d <= 15m → Chase immediato (skip cooldown)
    * HasConfirmedPlayer == false: d <= 10m → Chase immediato (skip cooldown)
  - Altrimenti (dopo cooldown completo) dipende da IsNoiseChaseActive:
    * true: usa A/threshold → Chase o CheckSound
    * false: usa solo d → ChaseDistance (se d <= 15m) o LostTarget (se d > 15m)
```

---

## Note Importanti

### Differenza tra StandAndExamine e LostTarget
- **StandAndExamine**: Stato di SOSPETTO prima di confermare il player (`HasConfirmedPlayer == false`)
- **LostTarget**: Stato di PERDITA dopo un vero chase (`HasConfirmedPlayer == true`)

### Differenza tra Chase e ChaseDistance
- **Chase**: Basato su attraction (A >= 0.8) OPPURE distanza (d <= 10m)
- **ChaseDistance**: Basato SOLO su distanza (d <= 15m), ignora attraction

### Differenza tra noise-chase e distance-chase
- **Noise-chase** (`IsNoiseChaseActive == true`): Dopo attacco può andare a CheckSound se A scende
- **Distance-chase** (`IsNoiseChaseActive == false`): Dopo attacco usa solo distanza, non va mai a CheckSound

### Comportamento Post-Attacco
- **Skip cooldown**: Se il player è ancora in range dopo l'attacco, il nemico può saltare il cooldown e riprendere subito la chase
  - Se `HasConfirmedPlayer == true`: Usa `D_exit` (15m) come soglia per skip cooldown
  - Se `HasConfirmedPlayer == false`: Usa `D_enter` (10m) come soglia per skip cooldown
- **Dopo cooldown completo**: La logica dipende da `IsNoiseChaseActive` (vedi sezione "Comportamento Post-Attacco" per dettagli)

### Trigger Globali
I trigger globali hanno priorità su qualsiasi logica interna degli stati. Vengono controllati PRIMA di `CurrentState.Update()`.

---

## Esempi di Flussi

### Flusso 1: Player fa rumore forte
```
Patrol → (A>=1.0) → MandatoryChase (3s) → Chase → Attack → Chase → ...
```

### Flusso 2: Player si avvicina silenziosamente
```
Patrol → (d<=10m) → ChaseDistance → Attack → ChaseDistance → (d>15m) → LostTarget → Patrol
```

### Flusso 3: Player fa rumore, poi scappa
```
Patrol → (A>=1.0) → MandatoryChase (3s) → CheckSound → StandAndExamine → (A<0.8) → Patrol
```

### Flusso 4: Player fa rumore, nemico lo perde dopo chase
```
Patrol → (A>=1.0) → MandatoryChase → Chase → (A<0.8 E d>10m) → LostTarget → Patrol
```

### Flusso 5: Nemico distratto da ConfusingSound
```
Patrol → GettingConfused → (arriva) → (6s confuso) → Patrol
```

### Flusso 6: Player attacca e scappa (distanza 10-15m)
```
ChaseDistance → Attack → (HasConfirmedPlayer==true, d=12m) → ChaseDistance immediato (skip cooldown)
```

### Flusso 7: Player attacca e scappa oltre 15m
```
ChaseDistance → Attack → (HasConfirmedPlayer==true, d=18m) → (cooldown completo) → LostTarget
```

---

## Comportamento Post-Attacco

### Logica Skip Cooldown (cooldown >= 0.1s)
Quando il player è ancora in range dopo l'attacco, il nemico può saltare il cooldown completo e riprendere subito la chase:

- **Se `HasConfirmedPlayer == true`**: Usa `D_exit` (15m) come soglia
  - Se `d <= 15m` → Riprende chase immediatamente (skip cooldown)
  
- **Se `HasConfirmedPlayer == false`**: Usa `D_enter` (10m) come soglia
  - Se `d <= 10m` → Riprende chase immediatamente (skip cooldown)

Questo garantisce che dopo un vero chase, il nemico continui a inseguire finché il player è entro 15m, non solo 10m.

### Logica Dopo Cooldown Completo
Se il player non è abbastanza vicino per saltare il cooldown, il nemico aspetta il cooldown completo (`AttackCoolDown`) e poi decide:

- **Noise-chase** (`IsNoiseChaseActive == true`): Usa attraction e threshold
- **Distance-chase** (`IsNoiseChaseActive == false`): Usa solo distanza (D_exit = 15m)

---

*Ultimo aggiornamento: Dopo implementazione sistema trigger, IsNoiseChaseActive e logica post-attacco con D_exit*

