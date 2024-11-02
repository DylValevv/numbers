using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This is the instance of a player character, a manifestation of its player character data
/// </summary>
public class PlayerCharacter : Actor {
    [Header("PlayerCharacter Attributes")]
    public int pointValue = 5;
    private PlayerCharacterData playerCharacterDataInstance = new PlayerCharacterData();
    private Message updateDataMessage;
    private PlayerInput playerInput;

    public PlayerUIControllerContainer playerUIControllerPrefab;
    private PlayerUIControllerContainer playerUIController;

    private void Awake() {
        Initialize();
    }

    private void FixedUpdate() {
        if (isDead) {
            return;
        }

        ConstantPlayerInputHandler();
    }

    protected override void Initialize() {
        base.Initialize();
        playerInput = GetComponent<PlayerInput>();

        playerCharacterDataInstance = PlayerManager.Instance.RegisterPlayer(this);
        updateDataMessage = new Message(GlobalNames.Data.UPDATE_PLAYER_CHARACTER_DATA, playerCharacterDataInstance);

        if (DebugManager.Instance.autoSortTeamJoin) {
            TeamDataSO.Team team = (TeamDataSO.Team)(GetPlayerInput().playerIndex + 1);
            SetTeamData(PlayerManager.Instance.GetTeamData(team));
        }
        else {
            SetTeamData(playerCharacterDataInstance.GetTeamData());
        }
        playerUIController = UIManager.Instance.RegisterPlayer(this);
        isDead = false;
    }

    private void OnEnable() {
        AddListeners();
    }

    private void OnDisable() {
        RemoveListeners();
    }

    public void PlayerInputListener(InputAction.CallbackContext context) {
        switch (context.action.name) {
            case GlobalNames.InputIDs.SPAWN_INPUT_ID:
                if (context.phase == InputActionPhase.Performed) {
                    if (isDead) {
                        Initialize();
                    }
                }
                break;
            default:
                break;
        }
    }

    private void ConstantPlayerInputHandler() {
        actorMovement.Movement(GetPlayerInput().actions[GlobalNames.InputIDs.MOVEMENT_INPUT_ID].ReadValue<Vector2>());
        actorMovement.Aim(GetPlayerInput().actions[GlobalNames.InputIDs.AIM_INPUT_ID].ReadValue<Vector2>());
    }

    public void SetTeamData(TeamDataSO newTeamData) {
        playerCharacterDataInstance.SetTeamData(newTeamData);
        updateDataMessage.Data = playerCharacterDataInstance;
        UpdatePlayerCharacterData();
    }

    //Appended to any thing that the player needs saved through a game session
    public void UpdatePlayerCharacterData() {
        MessageDispatcher.SendMessage(updateDataMessage);
    }

    public override void Die(GameObject source = null) {
        //very temp code. might want to do a material thing or animation call here in the future. for now just move them off screen
        transform.position += new Vector3(0, 1000, 0);

        deathData.scoreReward = pointValue;

        base.Die(source);
        PlayerManager.Instance.RemovePlayer(this);
        //Destroy(this.gameObject);
    }

    #region Getters
    public PlayerInput GetPlayerInput() {
        return playerInput;
    }
    
    public int GetScore() {
        return PlayerManager.Instance.GetPlayerCharacterData(GetPlayerID()).GetScore();//the dictionary will have most up to date value
    }

    public TeamDataSO GetTeamData() {
        return PlayerManager.Instance.GetPlayerCharacterData(GetPlayerID()).GetTeamData();//the dictionary will have most up to date value
    }

    public string GetPlayerID() {
        return playerCharacterDataInstance.GetPlayerID();
    }

    public string GetPlayerDeviceID() {
        return playerCharacterDataInstance.ConnectedDeviceID();
    }
    #endregion

    #region Listeners
    private void AddListeners() {
        playerInput.onActionTriggered += PlayerInputListener;
    }

    private void RemoveListeners() {
        playerInput.onActionTriggered -= PlayerInputListener;
    }
    #endregion
}