using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ActorMovement))]
public abstract class Actor : MonoBehaviour {
    [Header("Actor Attributes")]
    [SerializeField] protected MeshRenderer teamFlag;
    protected ActorMovement actorMovement;

    [Header("Spawn")]
    public FXRequest spawnFX;
    protected Message spawnMessage = new Message(GlobalNames.Game.SPAWN_EVENT, null);
    protected SpawnEventData spawnData = new SpawnEventData();

    [Header("Death")]
    public FXRequest killedByPlayerFX;
    public FXRequest genericDeathFX;
    protected Message scoreDeathMessage = new Message(GlobalNames.Game.SCORE_EVENT, null);
    protected DeathEventData deathData = new DeathEventData();
    protected bool isDead;
    public bool IsDead {
        get { return isDead; }
        private set { }
    }

    protected virtual void Initialize() {
        actorMovement = GetComponent<ActorMovement>();
    }

    public ActorMovement GetActorMovement() {
        return actorMovement;
    }

    public virtual void Die(GameObject source) {
        if (source?.GetComponent<PlayerCharacter>() is PlayerCharacter playerCharacter) {
            deathData.optionalKilledByID = playerCharacter.GetPlayerID();
            killedByPlayerFX.Play(gameObject);
        }
        else {
            genericDeathFX.Play(gameObject);
        }

        isDead = true;
        deathData.killedBy = source;
        deathData.whatDied = gameObject;
        scoreDeathMessage.Data = deathData;
        MessageDispatcher.SendMessage(scoreDeathMessage);
    }
}