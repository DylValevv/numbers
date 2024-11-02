public struct PlayerCharacterData {
    private string playerID;
    private string connectedDeviceName;
    private int score;
    private TeamDataSO teamData;

    public void Init(string playerID, string connectedDeviceName) {
        this.connectedDeviceName = connectedDeviceName;
        this.playerID = playerID;
    }

    public string GetPlayerID() {
        return playerID;
    }

    public string ConnectedDeviceID() {
        return connectedDeviceName;
    }

    public void SetTeamData(TeamDataSO teamData) {
        this.teamData = teamData;
    }

    public TeamDataSO GetTeamData() {
        return teamData;
    }

    public int GetScore() {
        return score;
    }

    public void UpdateScore(int scoreToAdd) {
        score += scoreToAdd;
    }

    public void SetScore(int newScore) {
        score = newScore;
    }
}