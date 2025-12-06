# Descrizione Scena: First-Level (Noah's Lab)

## Panoramica

La scena **First-Level** rappresenta il livello tutorial del gioco **EchoCity**, corrispondente al laboratorio sotterraneo di Noah ("Noah's Lab"). Questa è la prima scena di gioco dove il giocatore viene introdotto alla storia, prova il prototipo di ecolocalizzazione e incontra i primi nemici.

## Informazioni Tecniche

- **Nome File**: `First-Level.unity`
- **Percorso**: `EchoCity-Game/Assets/Scenes/First-Level.unity`
- **Dimensione File**: 65,644 righe
- **Tipo**: Scena Unity (YAML)

## Configurazioni Globali della Scena

### Render Settings
- **Fog**: Disabilitato
- **Ambient Sky Color**: RGB(0.212, 0.227, 0.259)
- **Ambient Equator Color**: RGB(0.114, 0.125, 0.133)
- **Ambient Ground Color**: RGB(0.047, 0.043, 0.035)
- **Skybox**: Default Unity Skybox

### Lightmap Settings
- **Baked Lightmaps**: Abilitato
- **Realtime Lightmaps**: Disabilitato
- **Bake Resolution**: 40
- **Atlas Size**: 1024
- **Lighting Data Asset**: Presente (GUID: 279b357bb214a594887b18e3ee12b0a0)

### NavMesh Settings
- **Agent Radius**: 0.5
- **Agent Height**: 2
- **Agent Slope**: 45°
- **Agent Climb**: 0.4
- **Cell Size**: 0.16666667
- **Tile Size**: 256
- **Min Region Area**: 2

## Struttura della Scena

### GameObjects Principali

#### 1. Ambientazione Bunker
- **Bunker-Factory Alex**: Area principale della fabbrica/bunker
- **Bunker-Corridor Alex**: Corridoio del bunker
- **Noah's Lab Alex**: Laboratorio principale di Noah

#### 2. Elementi Architettonici

##### Pareti (Walls)
- Multiple istanze di pareti del bunker
- Pareti con prefab `AI_Env_Bunker_Wall_Ladder` (scale)
- Pareti con prefab `AI_Env_Sewer_Wall_CornerBlock` (blocchi angolari)

##### Pavimenti (Floors)
- Pavimenti in pietra (`SM_Bld_Bunker_Floor_Stone_01`)
- Multiple istanze posizionate in diverse aree

##### Tetti (Roofs)
- Tetto del bunker (`AI_Env_Bunker_Roof`)
- Multiple istanze per coprire le diverse aree

##### Porte (Doors)
- Sistema di porte per la navigazione tra aree

#### 3. Illuminazione

La scena contiene **19 Point Lights** numerati:
- Point Light (3)
- Point Light (4)
- Point Light (5)
- Point Light (6)
- Point Light (7)
- Point Light (8)
- Point Light (10)
- Point Light (11)
- Point Light (12)
- Point Light (13)
- Point Light (14)
- Point Light (15)
- Point Light (18)
- Point Light (19)
- Point Light (principale)

Le luci sono organizzate in gruppi padre chiamati "Lights" per facilitare la gestione.

#### 4. Fonti Sonore Ambientali (Environmental Sources)

##### Generatori
- **High_Generator_Hum**: Generatore ad alta frequenza con ronzio continuo
- **Low_Generator_Hum**: Generatore a bassa frequenza con ronzio continuo
- **SM_Prop_Generator_01**: Prop di generatore posizionato nella scena
- **AI_Prop_Generator**: Generatore per l'AI

##### Radio
- **SM_Prop_Radio_01_Mid**: Radio che emette suoni a frequenza media

#### 5. Sistema di Puzzle - Steps

La scena contiene un sistema di puzzle progressivo con i seguenti step:
- **Step0**: Step iniziale del puzzle (2 istanze)
- **Step1**: Primo step del puzzle (2 istanze)
- **Step2**: Secondo step del puzzle (2 istanze)
- **Step3**: Terzo step del puzzle (2 istanze)
- **Step4**: Quarto step del puzzle (2 istanze)

Questi step probabilmente rappresentano le fasi del puzzle tutorial descritto nel Game Design Document.

#### 6. Frequency Setters

La scena contiene numerosi oggetti per impostare le frequenze di ecolocalizzazione:

##### High Frequency Setters
- Multiple istanze di `HighFrequencySetter` per suoni ad alta frequenza
- Utilizzati per rivelare dettagli dell'ambiente ma attirano maggiormente i nemici

##### Mid Frequency Setters
- Multiple istanze di `MidFrequencySetter` per suoni a frequenza media
- Bilanciamento tra dettaglio e rischio

##### Low Frequency Setters
- Utilizzati per suoni a bassa frequenza
- Rivelano l'ambiente in modo grossolano ma con minor rischio

#### 7. Props e Oggetti Interattivi

##### Mobili e Arredi
- **Desk**: Scrivanie (multiple istanze)
- **Desk-Shelf**: Scrivania con scaffale
- **Shelf**: Scaffali (multiple istanze)
- **SideTable**: Tavolino laterale
- **Table**: Tavolo
- **ProvisionsShelf**: Scaffale per provviste
- **Medic**: Probabilmente un kit medico o area medica

##### Oggetti Tecnologici
- **EmergencyBox**: Scatola di emergenza
- **Atomic bomb**: Oggetto decorativo o narrativo

##### Servizi
- **SM_Prop_Toilet_Pneumatic_01**: Toilette pneumatica (multiple istanze)
- **SM_Prop_Toilet_Pneumatic_Lid_01**: Coperchio toilette pneumatica (multiple istanze)

##### Altri Props
- **SM_Prop_PalletStack_01**: Pila di pallet

#### 8. Colliders e Navigazione

##### Colliders per Interazione
- **Collider**: Collider generico
- **Collider1**, **Collider2**, **Collider3**, **Collider4**: Collider numerati
- **ColliderObject**: Oggetto con collider
- **ColliderObject2**: Secondo oggetto con collider
- **COLLIDER FLOOR**: Collider per il pavimento

##### NavMesh
- **StairsStartForNavMesh**: Punto di partenza scale per NavMesh
- **Stairs_NavCollider**: Collider per scale nel NavMesh
- **Ladder**: Scala per la navigazione

#### 9. Aree e Zone

- **ToBomb**: Area che porta alla bomba atomica
- **ToLab**: Area che porta al laboratorio
- **OtherDecorations**: Decorazioni aggiuntive

## Scripts e Componenti

### Scripts Identificati

1. **ObjectFrequencySetter** (GUID: 9a73a507f72a331229877f92ec6e7fce)
   - Utilizzato su numerosi oggetti per impostare la frequenza di ecolocalizzazione
   - Presente su: HighFrequencySetter, MidFrequencySetter, e altri oggetti

2. **AudioEmitter** (GUID: 61993809d763062a4a9ff0e6b4227239)
   - Componente per emettere suoni ambientali
   - Presente su: High_Generator_Hum, Low_Generator_Hum

3. **Script Sconosciuto** (GUID: 474bcb49853aa07438625e644c072ee6)
   - Utilizzato su alcuni Point Lights
   - Probabilmente un componente per la gestione delle luci

### Componenti Standard Unity

- **Transform**: Tutti i GameObjects
- **MeshFilter** e **MeshRenderer**: Per gli oggetti 3D
- **BoxCollider**: Per collisioni e interazioni
- **MeshCollider**: Per collisioni complesse
- **Light**: Per le Point Lights
- **NavMeshAgent**: Probabilmente presente per i nemici

## Meccaniche di Gioco Implementate

### 1. Sistema di Ecolocalizzazione
- Multiple fonti sonore ambientali (generatori, radio)
- Frequency setters per controllare il tipo di rivelazione
- Sistema di suoni a bassa, media e alta frequenza

### 2. Puzzle Tutorial
- Sistema di step progressivi (Step0-Step4)
- Probabilmente collegato al puzzle descritto nel GDD dove i mostri bloccano l'uscita

### 3. Navigazione
- NavMesh configurato per il movimento dei nemici
- Scale e ladder per la navigazione verticale
- Colliders per definire aree interattive

### 4. Illuminazione
- Sistema di luci puntiformi per creare atmosfera
- Lightmaps baked per performance ottimali

## Note di Design

### Atmosfera
La scena rappresenta un ambiente sotterraneo claustrofobico con:
- Strutture in cemento/pietra (bunker)
- Illuminazione limitata (Point Lights)
- Fonti sonore meccaniche (generatori, radio)

### Gameplay
- Ambiente tutorial con puzzle guidato
- Introduzione alle meccaniche di ecolocalizzazione
- Primo incontro con i nemici
- Sistema di step per guidare il giocatore

### Performance
- Lightmaps baked per ottimizzare le performance
- Static Editor Flags impostati su molti oggetti per il batching statico
- NavMesh pre-calcolato per l'AI

## Riferimenti al Game Design Document

Questa scena implementa la sezione **5.1. Noah's Lab - Tutorial Level** del Game Design Document:

- ✅ Introduzione alla storia
- ✅ Test del prototipo di ecolocalizzazione
- ✅ Primo incontro con i nemici
- ✅ Puzzle guidato (sistema Step0-Step4)
- ✅ Fonti sonore ambientali (generatori, radio)
- ✅ Struttura del laboratorio sotterraneo

## File Correlati

- **Game Design Document**: `Documents/GameDesignDocument.md`
- **Scene Loader Script**: `EchoCity-Game/Assets/Scripts/Managers/SceneManager/SceneLoader.cs`
- **Audio Emitter Script**: `EchoCity-Game/Assets/Scripts/Managers/EcholocationManager/AudioEmitter.cs`
- **Object Frequency Setter Script**: `EchoCity-Game/Assets/Scripts/Managers/EcholocationManager/ObjectFrequencySetter.cs`

## Statistiche Approximate

- **GameObjects Totali**: ~200+ (basato sul numero di nomi unici trovati)
- **Point Lights**: 19
- **Frequency Setters**: ~20+ (High/Mid/Low combinati)
- **Puzzle Steps**: 5 step (Step0-Step4) con multiple istanze
- **Fonti Sonore Ambientali**: 2+ (generatori, radio)
- **Props Interattivi**: 15+ (desk, shelf, toilet, etc.)

---

*Documento generato analizzando il file Unity scene First-Level.unity*

