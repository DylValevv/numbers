using System.Collections.Generic;
using UnityEngine;
using System;

/// INFO
/// Object Pooling system derived from 'One Wheel Studio': https://github.com/onewheelstudio/Adventures-in-C-Sharp/blob/main/Object%20Pool%202/ObjectPool.cs
///Why?:
/// - No object pool manager
/// - Objects return themselves upon disable
/// - Store reference to component (avoids get component calls)
/// - Easy initialization with interfaces
///


/// <summary>
/// Contains functions needed by an Object Pooler to behave like one
/// </summary>
/// <typeparam name="T">The type of Object we are storing</typeparam>
public interface IPool<T> {
    T Pull();
    void Push(T t);
}

/// <summary>
/// Contains functions that allow an Object to be pooled
/// </summary>
/// <typeparam name="T">The type of Object we are storing</typeparam>
public interface IPoolable<T> {
    void Initialize(System.Action<T> returnAction);//The 'returnAction' is the function that returns the object to the pool (in our case, Push())
    void ReturnToPool();//Where the 'returnAction' function is invoked
    void SetSource(GameObject source);
}

public class ObjectSpawner<T> : IPool<T> where T : MonoBehaviour, IPoolable<T> {
    #region Attributes
    private System.Action<T> pullObjectCallback;//An action that can be assigned in a constructor, allows you to call a function every time an object is pulled out of the pool
    private System.Action<T> pushObjectCallback;//'' ditto except everytime it is pushed back to the pool
    private Stack<T> pooledObjects = new Stack<T>();//The pool itself
    private GameObject prefab;//Can assume GameObject because 'T' is a type with Monobehaviour implementing IPoolable, making it valid to pool
    public int poolSize { get { return pooledObjects.Count; } }
    //Addons
    private GameObject objectPoolHolder;//The parent transform of the pool
    #endregion

    #region Constructors
    /// <summary>
    /// Basic constructor for an object pool.
    /// </summary>
    /// <param name="objectToSpawnPrefab">The prefab to spawn</param>
    /// <param name="startingPoolSize">The amount of prefab to spawn upon pool creation</param>
    /// <param name="objectPoolContainer">Where the pooled objects are placed. Default to making its own container</param>
    public ObjectSpawner(GameObject objectToSpawnPrefab, int startingPoolSize = 0, string poolNameOverride = null, Transform objectPoolTransformContainer = null) {
        this.prefab = objectToSpawnPrefab;
        string poolName = poolNameOverride == null ? $"{objectToSpawnPrefab.name}" : poolNameOverride;
        objectPoolHolder = new GameObject($"{poolName}_Pool");
        if (objectPoolTransformContainer != null) {
            objectPoolHolder.transform.SetParent(objectPoolTransformContainer);
        }

        Spawn(startingPoolSize);
    }

    /// <summary>
    /// Constructor for object pool with Push() & Pull() invoke functions
    /// </summary>
    /// <param name="objectToSpawnPrefab">The prefab to spawn</param>
    /// <param name="pullObjectCallback">Function called when object is pulled out of pool</param>
    /// <param name="pushObjectCallback">Function called when object is pushed back to pool</param>
    /// <param name="startingPoolSize">The amount of prefab to spawn upon pool creation</param>
    ///     /// <param name="objectPoolContainer">Where the pooled objects are placed. Default to making its own container</param>
    public ObjectSpawner(GameObject objectToSpawnPrefab, Action<T> pullObjectCallback, Action<T> pushObjectCallback, int startingPoolSize = 0, string poolNameOverride = null, Transform objectPoolTransformContainer = null) {
        this.prefab = objectToSpawnPrefab;
        this.pullObjectCallback = pullObjectCallback;
        this.pushObjectCallback = pushObjectCallback;
        string poolName = poolNameOverride == null ? $"{objectToSpawnPrefab.name}" : poolNameOverride;
        objectPoolHolder = new GameObject($"{poolName}_Pool");
        if (objectPoolTransformContainer != null) {
            objectPoolHolder.transform.SetParent(objectPoolTransformContainer);
        }

        Spawn(startingPoolSize);
    }
    #endregion

    public Transform GetObjectPoolTransform() {
        return objectPoolHolder.transform;
    }

    #region Pull
    /// <summary>
    /// Retrieves Object from pool
    /// </summary>
    /// <returns>Object from pool</returns>
    public T Pull() {
        T t;

        if (poolSize == 0) {
            Spawn(1);//Auto grows stack if there is nothing remaining to pull
        }
        t = pooledObjects.Pop();

        t.gameObject.SetActive(true);
        t.Initialize(Push);//Assign reference to instance's Push() function in 'returnAction' so spawnedObject can return itself to this pool instance

        //If assigned/not null, invoke the Pull() callback
        pullObjectCallback?.Invoke(t);

        return t;
    }

    /// <summary>
    /// Retrieves Object from pool and assigns reference to who called Pull() 
    /// </summary>
    /// <returns>Object from pool</returns>
    public T Pull(GameObject source) {
        T t;
        if (poolSize == 0) {
            Spawn(1);//Auto grows stack if there is nothing remaining to pull
        }
        t = pooledObjects.Pop();
        t.SetSource(source);

        t.gameObject.SetActive(true);
        t.Initialize(Push);//Assign reference to instance's Push() function in 'returnAction' so spawnedObject can return itself to this pool instance

        //If assigned/not null, invoke the Pull() callback
        pullObjectCallback?.Invoke(t);

        return t;
    }

    /// <summary>
    /// Retrieves Object from pool and sets position
    /// </summary>
    /// <param name="position">Spawn position</param>
    /// <returns>Object from pool</returns>
    public T Pull(Vector3 position) {
        T t = Pull();
        t.transform.position = position;
        return t;
    }

    /// <summary>
    /// Retrieves Object from pool and sets position and reference to who called Pull()
    /// </summary>
    /// <param name="source">Who called Pull()</param>
    /// <param name="position">Spawn position</param>
    /// <returns>Object from pool</returns>
    public T Pull(GameObject source, Vector3 position) {
        T t = Pull(source);
        t.transform.position = position;
        return t;
    }

    /// <summary>
    /// Retrieves Object from pool and sets position & rotation
    /// </summary>
    /// <param name="position">Spawn position</param>
    /// <param name="rotation">Spawn position</param>
    /// <param name="worldSpace">Sets position and rotation to world space (true) or local space (false)</param>
    /// <returns>Object from pool</returns>
    public T Pull(Vector3 position, Quaternion rotation, bool worldSpace = true) {
        T t = Pull();
        if (worldSpace) {
            t.transform.SetPositionAndRotation(position, rotation);
        }
        else {
            t.transform.SetLocalPositionAndRotation(position, rotation);
        }
        
        return t;
    }

    /// <summary>
    /// Retrieves Object from pool and sets position & rotation and reference to who called Pull()
    /// </summary>
    /// <param name="source">Who called Pull()</param>
    /// <param name="position">Spawn position</param>
    /// <param name="rotation">Spawn position</param>
    /// <param name="worldSpace">Sets position and rotation to world space (true) or local space (false)</param>
    /// <returns>Object from pool</returns>
    public T Pull(GameObject source, Vector3 position, Quaternion rotation, bool worldSpace = true) {
        T t = Pull(source);
        if (worldSpace) {
            t.transform.SetPositionAndRotation(position, rotation);
        }
        else {
            t.transform.SetLocalPositionAndRotation(position, rotation);
        }

        return t;
    }

    /// <summary>
    /// Retrieves GameObject from pool
    /// </summary>
    /// <param name="source">Who called Pull()</param>
    /// <returns>GameObject from pool</returns>
    public GameObject PullGameObject(GameObject source) {
        return Pull().gameObject;
    }

    /// <summary>
    /// Retrieves GameObject from pool and sets position
    /// </summary>
    /// <param name="source">Who called Pull()</param>
    /// <param name="position">Spawn position</param>
    /// <returns>GameObject from pool</returns>
    public GameObject PullGameObject(GameObject source, Vector3 position) {
        GameObject go = Pull().gameObject;
        go.transform.position = position;
        return go;
    }

    /// <summary>
    /// Retrieves GameObject from pool and sets position & rotation
    /// </summary>
    /// <param name="source">Who called Pull()</param>
    /// <param name="position">Spawn position</param>
    /// <param name="rotation">Spawn position</param>
    /// <param name="worldSpace">Sets position and rotation to world space (true) or local space (false)</param>
    /// <returns>GameObject from pool</returns>
    public GameObject PullGameObject(GameObject source, Vector3 position, Quaternion rotation, bool worldSpace = true) {
        GameObject go = Pull().gameObject;
        if (worldSpace) {
            go.transform.SetPositionAndRotation(position, rotation);
        }
        else {
            go.transform.SetLocalPositionAndRotation(position, rotation);
        }

        return go;
    }
    #endregion

    #region Push
    /// <summary>
    /// Returns spawned objects to their pool
    /// </summary>
    /// <param name="t">The spawned object to return</param>
    public void Push(T t) {
        pooledObjects.Push(t);

        //If assigned/not null, invoke the Push() callback
        pushObjectCallback?.Invoke(t);

        t.gameObject.SetActive(false);
    }
    #endregion

    private bool Spawn(int amountToSpawn) {
        if (prefab == null) {
            return false;
        }

        T t;

        for (int i = 0; i < amountToSpawn; i++) {
            t = GameObject.Instantiate(prefab).GetComponent<T>();
            if (objectPoolHolder != null) {
                t.transform.SetParent(objectPoolHolder.transform, true);
            }
            pooledObjects.Push(t);
            t.gameObject.SetActive(false);
        }
        return true;
    }
}