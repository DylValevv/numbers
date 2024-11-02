using UnityEngine;

public struct DeathEventData {
    public string optionalKilledByID;//can save us on GetComponent() calls
    public GameObject killedBy;
    public string optionalWhatDiedID;//can save us on GetComponent() calls
    public GameObject whatDied;
    public int scoreReward;
}