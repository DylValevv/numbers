using System;
using UnityEngine;

/// <summary>
/// Assign to any prefab to make object poolable
/// </summary>
public class SpawnedObject : MonoBehaviour, IPoolable<SpawnedObject> {
    private Action<SpawnedObject> returnToPool;//Store a reference to the Push() function invoked by the object pool
    private GameObject source;

    /// <summary>
    /// Sets up pooling ability for object
    /// </summary>
    /// <param name="returnAction">Reference to the Push() function invoked by the object pool</param>
    public void Initialize(Action<SpawnedObject> returnAction) {
        this.returnToPool = returnAction;
    }

    /// <summary>
    /// Invoke and return this object to pool
    /// </summary>
    public void ReturnToPool() {
        returnToPool?.Invoke(this);
    }

    public GameObject GetSource() {
        return source;
    }

    public void SetSource(GameObject source) {
        this.source = source;
    }

    private void OnDisable() {
        ReturnToPool();//Invoke function assigned in Initialize() when disabled (automatically pools itself)
    }
}