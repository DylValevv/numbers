using UnityEngine;

public struct SpawnEventData {
    public string optionalSpawnedByID;//can save us on GetComponent() calls
    public GameObject spawnedBy;
    public string optionalWhatSpawnedID;//can save us on GetComponent() calls
    public GameObject whatSpawned;
}