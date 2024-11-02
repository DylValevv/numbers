using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour {
    private static PlayerManager _instance;
    public static PlayerManager Instance {
        get { return _instance; }
        private set { }
    }

    //instance per-scene attributes
    private List<PlayerCharacter> activePlayerCharacters = new List<PlayerCharacter>();
    private int numPlayersReady;
    public GameObject[] spawnPoints;
    public int numPlayersNeeded = 2;

    //index, unique ID glyph
    private Dictionary<string, string> devicePlayerIDMap = new Dictionary<string, string>();
    private const string playerIdGlyphs = "abcdefghijklmnopqrstuvwxyz0123456789";
    private Dictionary<string, PlayerCharacterData> playerDataDictionary = new Dictionary<string, PlayerCharacterData>();

    private TeamDataSO[] teamData;
    public Dictionary<TeamDataSO.Team, TeamDataSO> teamDataDictionary = new Dictionary<TeamDataSO.Team, TeamDataSO>();
    public const TeamDataSO.Team DEFAULT_TEAM = TeamDataSO.Team.White;

    private void Update() {
        //Debug.Log($"{playerDataDictionary[activePlayerCharacters[0].GetPlayerID()].GetScore()} :: {playerDataDictionary[activePlayerCharacters[1].GetPlayerID()].GetScore()}");
    }

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this);
        AddListeners();

        InitTeamData();
        LoadNewSceneData();
    }

    private void OnDisable() {
        RemoveListeners();
    }

    private void InitTeamData() {
        teamData = Resources.LoadAll<TeamDataSO>("Data/TeamData");

        // Load the team data objects into a dictionary, using the team enum as the key
        foreach (TeamDataSO teamData in teamData) {
            teamDataDictionary.Add(teamData.team, teamData);
        }
    }

    /// <summary>
    /// Assigns the player
    /// </summary>
    /// <param name="playerCharacter">The player</param>
    /// <param name="teamData">Optional team setting if we want to set their team</param>
    public PlayerCharacterData RegisterPlayer(PlayerCharacter playerCharacter) {
        string deviceName = playerCharacter.GetPlayerInput().devices[0].ToString();
        activePlayerCharacters.Add(playerCharacter);

        if (devicePlayerIDMap.ContainsKey(deviceName)) {
            SpawnPlayerTransform(playerCharacter);
            return GetPlayerCharacterData(devicePlayerIDMap[deviceName]);
        }

        // Make new player ID if this is a new device
        string playerID = PlayerIdHash();
        devicePlayerIDMap.Add(deviceName, playerID);

        PlayerCharacterData playerCharacterData = new PlayerCharacterData();
        playerCharacterData.Init(playerID, deviceName);
        playerDataDictionary.Add(playerID, playerCharacterData);
        SpawnPlayerTransform(playerCharacter);

        return GetPlayerCharacterData(devicePlayerIDMap[deviceName]);
    }

    private string PlayerIdHash() {
        int charAmount = Random.Range(8, 14);
        string result = "";
        for (int i = 0; i < charAmount; i++) {
            result += playerIdGlyphs[Random.Range(0, playerIdGlyphs.Length)];
        }
        return result;
    }

    public bool RemovePlayer(PlayerCharacter playerCharacter) {
        if (devicePlayerIDMap == null) {
            return false;
        }
        return activePlayerCharacters.Remove(playerCharacter);
        //keep the dictionary reference in case the player wants back in again- we can just load their data
        //return devicePlayerIDMap.Remove(playerCharacter.GetPlayerDeviceID());
    }

    private void UpdatePlayerCharacterData(PlayerCharacterData data) {
        playerDataDictionary[data.GetPlayerID()] = data;
    }

    //Custom spawn code here
    private void SpawnPlayerTransform(PlayerCharacter playerCharacter) {
        if (spawnPoints == null || spawnPoints.Length == 0) {
            return;
        }
        int index = UnityEngine.Random.Range(0, spawnPoints.Length);
        playerCharacter.transform.SetPositionAndRotation(spawnPoints[index].transform.position, Quaternion.identity);
        playerCharacter.spawnFX.Play(playerCharacter.gameObject);
    }

    private void LoadNewSceneData() {
        activePlayerCharacters.Clear();
        spawnPoints = GameObject.FindGameObjectsWithTag(GlobalNames.TagsAndLayers.SPAWN_POINT_TAG);
    }

    public int GetActivePlayerCount() {
        return activePlayerCharacters.Count;
    }

    public bool AllPlayersReady() {
        return GetActivePlayerCount() >= numPlayersNeeded;
    }

    public PlayerCharacterData GetPlayerCharacterData(string playerID) {
        return playerDataDictionary[playerID];
    }

    public string GetPlayerIdWithDeviceName(string deviceName) {
        string playerId;
        devicePlayerIDMap.TryGetValue(deviceName, out playerId);
        return playerId;
    }

    public TeamDataSO GetTeamData(TeamDataSO.Team team) {
        return teamDataDictionary[team];
    }

    public TeamDataSO.Team GetPlayerTeam(string playerID) {
        return GetPlayerCharacterData(playerID).GetTeamData().team;
    }

    public void SwitchActionMap(string actionMapName) {
        foreach (PlayerCharacter playerCharacter in activePlayerCharacters) {
            playerCharacter.GetPlayerInput().SwitchCurrentActionMap(actionMapName);
        }
    }

    #region Listeners
    private void ScoreEventListener(Message message) {
        if (message.Data is DeathEventData deathData) {
            //who to reward points to
            PlayerCharacter playerCharacter = deathData.killedBy.GetComponent<PlayerCharacter>();
            if (playerCharacter == null) {
                return;
            }
            if (deathData.killedBy == deathData.whatDied) {//there are no points for suicide
                return;
            }
            PlayerCharacterData playerCharacterData = GetPlayerCharacterData(playerCharacter.GetPlayerID());
            playerCharacterData.UpdateScore(deathData.scoreReward);
            UpdatePlayerCharacterData(playerCharacterData);
        }
        else if (message.Data is ScoreEventData scoreData) {
            PlayerCharacterData playerCharacterData = GetPlayerCharacterData(scoreData.playerCharacter.GetPlayerID());
            playerCharacterData.UpdateScore(scoreData.scoreReward);
            UpdatePlayerCharacterData(playerCharacterData);//dictionary doesn't update the instance value it has stored, must manually assign the updated data in the struct
        }
    }

    //Update the dictionary save data here
    private void UpdatePlayerCharacterDataListener(Message message) {
        if (message.Data is PlayerCharacterData playerCharacterData) {
            UpdatePlayerCharacterData(playerCharacterData);
        }
    }

    private void UpdateCurrentSceneListener(Scene scene, LoadSceneMode loadSceneMode) {
        LoadNewSceneData();
    }

    private void AddListeners() {
        MessageDispatcher.AddListener(GlobalNames.Game.SCORE_EVENT, ScoreEventListener);
        MessageDispatcher.AddListener(GlobalNames.Data.UPDATE_PLAYER_CHARACTER_DATA, UpdatePlayerCharacterDataListener);
        SceneManager.sceneLoaded += UpdateCurrentSceneListener;
    }

    private void RemoveListeners() {
        MessageDispatcher.RemoveListener(GlobalNames.Game.SCORE_EVENT, ScoreEventListener);
        MessageDispatcher.RemoveListener(GlobalNames.Data.UPDATE_PLAYER_CHARACTER_DATA, UpdatePlayerCharacterDataListener);
        SceneManager.sceneLoaded -= UpdateCurrentSceneListener;
    }
    #endregion
}