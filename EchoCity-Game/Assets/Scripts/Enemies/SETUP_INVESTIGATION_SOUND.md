# Setup Emissione Suono Investigazione - Unity

## 📋 Panoramica

Quando il nemico entra in `StandAndExaminateState`, emette:
1. **Frase audio** (già presente): AudioClip random da `InvestigationPhrases[]` - riprodotto tramite `AudioSource`
2. **Suono echolocation** (nuovo): Suono per il sistema echolocation - emesso tramite `SONewAudioSphereEvent`

---

## 🎵 La Frase Audio (Già Presente)

### Come Funziona
- `PlayInvestigationPhrase()` viene chiamato in `StandAndExaminateState.Enter()`
- Seleziona un `AudioClip` random da `SOEnemyData.InvestigationPhrases[]`
- Riproduce l'audio tramite `AudioSource.PlayOneShot()`

### Setup
1. Apri `SOEnemyData` (es. `BasicEnemy.asset`)
2. Nella sezione **"Investigation"**:
   - **InvestigationPhrases**: Aggiungi gli `AudioClip` che vuoi far riprodurre al nemico
   - Se vuoto o null, nessuna frase viene riprodotta

### AudioSource nel Nemico
- **Ruolo**: Riproduce l'audio della frase di investigazione
- **Come funziona**: `audioSource.PlayOneShot(selectedClip)` riproduce l'audio
- **Setup**: L'`AudioSource` è richiesto automaticamente da `[RequireComponent(typeof(AudioSource))]`
- **Configurazione**: Puoi configurare volume, spatial blend, ecc. nell'Inspector del componente AudioSource

---

## 🔊 Il Suono Echolocation (Nuovo)

### Come Funziona
- `EmitInvestigationSound()` viene chiamato in `StandAndExaminateState.Enter()`
- Crea `SoundEmissionData` con parametri da `SOEnemyData`
- Solleva `SONewAudioSphereEvent` per notificare l'echolocation system

### Setup Step-by-Step

#### 1. Creare SONewAudioSphereEvent
1. In Unity Editor: `Create → ECHO CITY → NewAudioSphereEventSO`
2. Assegna un nome (es. `EnemySoundEmissionEvent`)
3. **Importante**: Questo evento viene usato per l'echolocation system

#### 2. Configurare SOEnemyData
1. Apri `SOEnemyData` (es. `BasicEnemy.asset`)
2. Nella sezione **"Investigation Sound Emission"**:
   - **InvestigationSoundIntensity**: 0.3 (intensità del suono)
   - **InvestigationSoundDuration**: 1.0 (durata in secondi)
   - **InvestigationSoundFrequency**: 0 (0=Low, 1=Mid, 2=High)
   - **InvestigationSoundRadius**: 5.0 (raggio del suono in metri)

#### 3. Assegnare Evento a EnemyAI
1. Seleziona il GameObject del nemico con `EnemyAI`
2. Nella sezione **"Invoking Events"**:
   - Assegna `SONewAudioSphereEvent` nel campo **"Enemy Sound Emission Event"**
3. **Nota**: Questo è solo un riferimento - la logica è nello stato, EnemyAI è solo un "holder"

---

## 🎯 Differenza tra i Due Suoni

| Caratteristica | Frase Audio | Suono Echolocation |
|---------------|-------------|-------------------|
| **Scopo** | Audio narrativo (il nemico "parla") | Sistema echolocation (rivelazione ambiente) |
| **Componente** | `AudioSource` (riproduce AudioClip) | `SONewAudioSphereEvent` (notifica sistema) |
| **Configurazione** | `InvestigationPhrases[]` in SOEnemyData | Parametri `InvestigationSound*` in SOEnemyData |
| **Chi lo usa** | Player (ascolta il nemico) | Echolocation system (rivelazione visiva) |

---

## 🔧 Setup Completo in Unity

### Checklist

- [ ] Creato `SONewAudioSphereEvent` ScriptableObject
- [ ] Configurato `SOEnemyData`:
  - [ ] `InvestigationPhrases[]` popolato con AudioClip
  - [ ] `InvestigationSoundIntensity` configurato
  - [ ] `InvestigationSoundDuration` configurato
  - [ ] `InvestigationSoundFrequency` configurato
  - [ ] `InvestigationSoundRadius` configurato
- [ ] Assegnato `SONewAudioSphereEvent` a `EnemyAI.enemySoundEmissionEvent`
- [ ] Verificato che il nemico abbia `AudioSource` component (richiesto automaticamente)

---

## 🎮 Test

1. Fai emettere un'azione dal player (premi 'E' in InputManagerTest)
2. Aspetta che il nemico entri in chase
3. Aspetta che l'attraction scenda sotto 0.8
4. Il nemico dovrebbe:
   - Andare al punto del suono (ChaseSoundState)
   - Arrivare e fermarsi (StandAndExaminateState)
   - **Riprodurre frase audio** (tramite AudioSource)
   - **Emettere suono echolocation** (tramite SONewAudioSphereEvent)

---

## 📝 Note

1. **AudioSource**: Serve SOLO per riprodurre l'audio della frase. Non ha logica, è solo un componente Unity standard.
2. **SONewAudioSphereEvent**: Serve per notificare l'echolocation system. La logica di emissione è in `StandAndExaminateState`, EnemyAI è solo un riferimento.
3. **Separazione**: 
   - Frase audio = narrativa (il nemico "parla")
   - Suono echolocation = gameplay (rivelazione ambiente)
4. **EnemyAI pulito**: EnemyAI contiene solo riferimenti, la logica è negli stati.

---

**Ultimo aggiornamento**: Sistema con logica negli stati, EnemyAI come riferimento holder

