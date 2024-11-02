using UnityEngine;

public struct ObjectPoolAssetRefIDPair {
    public string ID { get { return id; } private set { } }
    private string id;
    private ObjectSpawner<SpawnedObject> objectPool;
    private ObjectSpawner<FXInstance> fxObjectPool;
    private const int DEFAULT_NEW_POOL_SIZE = 5;

    public void CreatePool(GameObject prefabToPool, string idOverride = null) {
        string newID = idOverride == null ? prefabToPool.name : idOverride;
        this.id = newID;
        objectPool = new ObjectSpawner<SpawnedObject>(prefabToPool, DEFAULT_NEW_POOL_SIZE, objectPoolTransformContainer: GameManager.Instance.poolHolder);
    }

    public void CreateFXInstancePool(GameObject prefabToPool, string idOverride = null) {
        string newID = idOverride == null ? prefabToPool.name : idOverride;
        this.id = newID;
        fxObjectPool = new ObjectSpawner<FXInstance>(prefabToPool, DEFAULT_NEW_POOL_SIZE, newID, GameManager.Instance.poolHolder);
    }

    public ObjectSpawner<SpawnedObject> GetPool() {
        return objectPool;
    }

    public ObjectSpawner<FXInstance> GetFXPool() {
        return fxObjectPool;
    }

    public SpawnedObject Pull(GameObject source) {
        return objectPool.Pull(source);
    }

    public SpawnedObject Pull(GameObject source, Vector3 position, Quaternion rotation, bool worldSpace = true) {
        return objectPool.Pull(source, position, rotation, worldSpace);
    }

    public FXInstance PullFX(GameObject source = null) {
        return fxObjectPool.Pull(source);
    }
}