# InputManagerTest - Script Placeholder per Test Sistema Nemico

## 📋 Panoramica

Questi script sono **placeholder** basati sul branch Develop per testare il sistema nemico. Permettono di simulare azioni del player e verificare che i nemici reagiscano correttamente all'attraction.

---

## 🚀 Setup Rapido

### 1. Creare SOPlayerActionEvent

1. In Unity Editor: `Create → ECHO CITY → Events → PlayerActionEventSO`
2. Assegna un nome (es. `PlayerActionEvent`)
3. **Importante**: Usa la stessa istanza per InputManagerTest e tutti gli EnemyAI

### 2. Configurare InputManagerTest

1. Crea un GameObject vuoto chiamato "InputManagerTest"
2. Aggiungi il componente `InputManagerTest`
3. Nella sezione "Invoking Events":
   - Assegna `SOPlayerActionEvent` (quello creato al punto 1)
4. Nella sezione "Player Action Test Settings":
   - **Action Position Source**: Lascia null (userà player) oppure assegna un Transform
   - **Test Positions List**: (Opzionale) Lista di posizioni alternative per test
5. Nella sezione "Action Parameters":
   - **Base Intensity**: 0.5 (valore base)
   - **Base Duration**: 1.0 secondi
   - **Base Radius**: 5 metri
6. Nella sezione "Frequency Presets":
   - **Low Freq Intensity**: 0.3 (per test Low Frequency)
   - **Mid Freq Intensity**: 0.6 (per test Mid Frequency)
   - **High Freq Intensity**: 1.0 (per test High Frequency)

### 3. Configurare EnemyAI

Per ogni nemico:
1. Seleziona il GameObject con `EnemyAI`
2. Nella sezione "Observed Events":
   - Assegna lo stesso `SOPlayerActionEvent` usato in InputManagerTest

---

## 🎮 Controlli Test

| Tasto | Azione | Descrizione |
|-------|--------|-------------|
| **E** | Azione Base | Emette un'azione con parametri base (Mid Frequency) |
| **1** | Low Frequency | Emette azione Low Frequency (intensity bassa) |
| **2** | Mid Frequency | Emette azione Mid Frequency (intensity media) |
| **3** | High Frequency | Emette azione High Frequency (intensity alta) |
| **X** | Stress Test | Emette 100 azioni rapidamente (test performance) |

---

## 📊 Parametri Testabili

### Frequency (Frequenza)
- **0 = Low**: Intensity moltiplicata per 1x
- **1 = Mid**: Intensity moltiplicata per 1.5x
- **2 = High**: Intensity moltiplicata per 2x

### Intensity (Intensità)
- Valore base che contribuisce all'attraction
- Più alto = più attraction generata
- Decade con la distanza (formula Attraction.cs)

### Duration (Durata)
- Quanto tempo l'azione rimane "attiva"
- Durante questo tempo, l'azione contribuisce all'attraction ogni frame
- Dopo la durata, l'azione scade e l'attraction inizia a decadere

### Radius (Raggio)
- Usato nel calcolo della distanza (formula Attraction.cs)
- Non è un limite assoluto, ma influisce sul falloff

---

## 🧪 Scenari di Test

### Test 1: Azione Base
1. Premi **E** vicino a un nemico
2. **Aspettato**: Il nemico dovrebbe iniziare a inseguirti (attraction >= 1.0)

### Test 2: Frequency Impact
1. Premi **1** (Low) - intensity bassa
2. Premi **2** (Mid) - intensity media
3. Premi **3** (High) - intensity alta
4. **Aspettato**: High Frequency dovrebbe generare più attraction

### Test 3: Distanza
1. Premi **E** lontano dal nemico (> 15m)
2. **Aspettato**: Attraction dovrebbe essere più bassa (decadimento distanza)
3. Avvicinati e premi di nuovo **E**
4. **Aspettato**: Attraction dovrebbe aumentare

### Test 4: MinChaseDuration
1. Premi **E** per triggerare chase
2. Aspetta 2 secondi
3. **Aspettato**: Il nemico continua a inseguirti (min 3 secondi)
4. Dopo 3 secondi, se attraction < 0.8, dovrebbe passare a ChaseSoundState

### Test 5: Stress Test
1. Premi **X** (100 azioni rapide)
2. **Aspettato**: Il sistema dovrebbe gestire correttamente (solo 1 azione attiva alla volta)

---

## 🔍 Debug

### Log Attivi
Se `logActions = true` (default), vedrai in console:
```
Base Action triggered #1 | Pos: (x, y, z) | Intensity: 0.50 | Duration: 1.00s | Frequency: Mid | Radius: 5.00m
```

### Gizmos
In Scene View (con GameObject selezionato):
- **Giallo**: `actionPositionSource` position
- **Cyan**: Posizioni in `testPositionsList`

---

## 📝 Note

1. **Un'azione alla volta**: Se premi un tasto mentre un'azione è ancora attiva, la nuova sostituisce la precedente
2. **Posizione azione**: Se `testPositionsList` ha elementi, sceglie una posizione random. Altrimenti usa `actionPositionSource` o player position
3. **Placeholder**: Questi script sono per test. In produzione, le azioni verranno da oggetti/items con parametri reali
4. **Frequency**: I valori 0/1/2 corrispondono a Low/Mid/High. Il sistema nemico moltiplica l'intensity di conseguenza

---

## 🔄 Prossimi Passi

Quando implementerai il sistema reale:
1. Sostituisci `InputManagerTest` con il vero InputManager
2. Le azioni verranno da oggetti/items (ScriptableObject con parametri)
3. I parametri (intensity, duration, frequency, radius) verranno dall'item usato
4. Rimuovi questi script placeholder

---

**Creato per**: Test sistema nemico basato su azioni player
**Basato su**: Branch Develop - InputManagerTest pattern

