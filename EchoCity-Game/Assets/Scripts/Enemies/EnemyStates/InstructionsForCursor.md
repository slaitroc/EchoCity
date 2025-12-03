OBIETTIVO
=========
Aggiungere un flag che distingua:
- CHASE DA RUMORE (attrazione >= NoiseThreshold → MandatoryChase/CheckSound)
- CHASE SOLO PER DISTANZA (entra in ChaseDistance/Attack senza threshold superata)

e usare questo flag in AttackEnemyState per:
- permettere CheckSound SOLO nel caso di chase da rumore
- nel caso di chase per distanza NON andare mai in CheckSound (solo ChaseDistance/LostTarget/Patrol).

NON modificare:
- logica di calcolo attraction
- passaggi tra stati a parte il ramo in AttackEnemyState.Update indicato sotto
- parametri EnemyData esistenti
- altri stati oltre a quelli esplicitamente citati.

File da toccare:
- EnemyAI.cs
- MandatoryChaseState.cs
- CheckSoundState.cs
- PatrolEnemyState.cs
- LostTargetState.cs
- AttackEnemyState.cs


1) EnemyAI.cs – aggiungi flag IsNoiseChaseActive
-------------------------------------------------
Dentro la classe EnemyAI, aggiungi una proprietà pubblica (o pubblica con [HideInInspector]):

    /// <summary>
    /// True se il nemico sta inseguendo il player perché ha superato la soglia di rumore
    /// (NoiseThreshold) o è arrivato da un suono investigato. False se sta inseguendo solo
    /// per distanza (ChaseDistance/Attack senza threshold superata).
    /// </summary>
    public bool IsNoiseChaseActive { get; set; }

Inizializzala a false:
- o direttamente nella dichiarazione con `= false;`
- oppure in Awake()/Start, come preferisci.


2) MandatoryChaseState.cs – attiva il flag
------------------------------------------
Nel metodo Enter() di MandatoryChaseState, dopo aver settato CurrentState, imposta:

    _enemyAI.IsNoiseChaseActive = true;

Questo significa: da quando entro in MandatoryChase in poi, sto inseguendo per rumore.


3) CheckSoundState.cs – attiva il flag quando da suono passo a chase
---------------------------------------------------------------------
Nel file CheckSoundState.cs cerca il punto in cui, una volta raggiunto il punto dell’ultimo suono, passi a Chase/ChaseDistance.

Qualcosa del tipo:

    if (distToPlayer <= _enemyData.D_enter)
        _fsm.SwitchState(_fsm.chaseDistanceState);
    else if (attraction >= _enemyData.NoiseThreshold)
        _fsm.SwitchState(_fsm.chaseState);
    ...

Prima di ogni `SwitchState` verso chase/chaseDistance in questo contesto, imposta:

    _enemyAI.IsNoiseChaseActive = true;

Esempio:

    if (distToPlayer <= _enemyData.D_enter)
    {
        _enemyAI.IsNoiseChaseActive = true;
        _fsm.SwitchState(_fsm.chaseDistanceState);
    }
    else if (attraction >= _enemyData.NoiseThreshold)
    {
        _enemyAI.IsNoiseChaseActive = true;
        _fsm.SwitchState(_fsm.chaseState);
    }

L’idea: se arrivo da CheckSound verso un chase, è sempre perché ho investigato un SUONO → quindi è un noise-chase.


4) PatrolEnemyState.cs – reset del flag
---------------------------------------
Nel metodo Enter() di PatrolEnemyState, dopo aver settato CurrentState, aggiungi:

    _enemyAI.IsNoiseChaseActive = false;

Quando torno in patrol, considero chiusa qualsiasi noise-chase.


5) LostTargetState.cs – reset del flag
--------------------------------------
Nel metodo Enter() di LostTargetState, dopo aver settato CurrentState, aggiungi:

    _enemyAI.IsNoiseChaseActive = false;

Quando il nemico dice “ti ritroverò” e entra in questo stato, ha perso definitivamente il target → il noise-chase si considera chiuso.


6) AttackEnemyState.cs – modifica della logica dopo l’attacco
-------------------------------------------------------------
In AttackEnemyState NON cambiare la coroutine, né il modo in cui `_attackEnded` viene settato.  
Lavora SOLO nel metodo Update(float attraction), nella parte in cui, dopo il cooldown, si decide il prossimo stato.

Cerca il blocco tipo:

    if (_coolDownTimer >= _enemyData.AttackCoolDown)
    {
        float distToPlayer = Vector3.Distance(...);

        if (attraction >= _enemyData.NoiseThreshold || distToPlayer <= _enemyData.D_enter)
        {
            _fsm.SwitchState(_fsm.chaseState);
        }
        else if (attraction < _enemyData.NoiseLoseThreshold)
        {
            _fsm.SwitchState(_fsm.checkSoundState);
        }
        else
        {
            _fsm.SwitchState(_fsm.chaseState);
        }
    }

Sostituisci l’intero contenuto di quel blocco interno con una logica che separa i casi:

    if (_coolDownTimer >= _enemyData.AttackCoolDown)
    {
        float distToPlayer = Vector3.Distance(_enemyAI.transform.position, _enemyAI.player.position);
        bool noiseChase = _enemyAI.IsNoiseChaseActive;

        if (noiseChase)
        {
            // CASO 1: inseguimento nato da rumore (threshold superata oppure da CheckSound)
            // → ha senso usare attraction / threshold e poter tornare a CheckSound
            if (attraction >= _enemyData.NoiseThreshold || distToPlayer <= _enemyData.D_enter)
            {
                // ancora molto attratto o ancora vicino → continua a inseguire normalmente
                _fsm.SwitchState(_fsm.chaseState);
            }
            else if (attraction < _enemyData.NoiseLoseThreshold)
            {
                // attrazione bassa ma l'ultimo suono potrebbe essere ancora rilevante → CheckSound
                _fsm.SwitchState(_fsm.checkSoundState);
            }
            else
            {
                // caso intermedio: attraction tra LoseThreshold e Threshold → continua a cercare
                _fsm.SwitchState(_fsm.chaseState);
            }
        }
        else
        {
            // CASO 2: inseguimento nato SOLO da distanza (ChaseDistance/Attack senza threshold > 1)
            // → NON ha senso usare CheckSound, ci basiamo solo sulla distanza.
            if (distToPlayer <= _enemyData.D_enter)
            {
                // il player è ancora entro la distanza di "close chase"
                _fsm.SwitchState(_fsm.chaseDistanceState);
            }
            else if (distToPlayer <= _enemyData.D_exit)
            {
                // era stato visto, ma ora è fuori dal range di close chase → LostTarget
                _fsm.SwitchState(_fsm.lostTargetState);
            }
            else
            {
                // è davvero lontano → torna a pattugliare
                _fsm.SwitchState(_fsm.patrolState);
            }
        }
    }

NON cambiare altro nel file AttackEnemyState.cs.  
In particolare:
- mantieni `_attackEnded`, `_coolDownTimer` e AttackCoolDown come sono ora;
- NON aggiungere ulteriori transizioni a stati non citati sopra.


7) Controllo finale
-------------------
Dopo le modifiche, il comportamento atteso è:

- Se il nemico ha iniziato l’inseguimento perché è stata superata la NoiseThreshold (MandatoryChase o CheckSound → Chase/ChaseDistance):
  - `EnemyAI.IsNoiseChaseActive` è TRUE.
  - Dopo un attacco può ancora andare a CheckSound se l’attrazione scende sotto NoiseLoseThreshold.

- Se il nemico ha iniziato a inseguire / attaccare SOLO perché il player gli è passato vicino (ChaseDistance/Attack senza threshold superata):
  - `EnemyAI.IsNoiseChaseActive` resta FALSE.
  - Dopo l’attacco NON va mai in CheckSound:
      - se sei ancora vicino → ChaseDistance
      - se sei mediamente lontano → LostTarget
      - se sei lontano → Patrol

NON introdurre altre modifiche oltre a quelle esplicitamente richieste.
 