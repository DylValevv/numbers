using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ResourceManager : MonoBehaviour {
    public static ResourceManager Instance {
        get { return _instance; }
        private set { }
    }
    public static ResourceManager _instance;

    //these are where all of the pools are stored alongside a reference to the asset's name
    private List<ObjectPoolAssetRefIDPair> poolIDPairs= new List<ObjectPoolAssetRefIDPair>();
    [SerializeField] private GameObject fxInstance;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public SpawnedObject SpawnObject(GameObject prefabToSpawn, GameObject source, Vector3 position, Quaternion rotation, bool worldSpace = true) {
        SpawnedObject spawnedObject = GetPoolOfPrefab(prefabToSpawn).Pull(source, position, rotation, worldSpace);
        return spawnedObject;
    }

    public FXInstance PlayFXRequest(FXRequest fxRequest) {
        FXInstance fxInstance = null;
        if (fxRequest.particleSystemAssetReference != null) {
            fxInstance = GetPoolOfFX(fxRequest.particleSystemAssetReference.gameObject).PullFX();
        }
        else if (fxRequest.sfxData.clip != null) {//sfx only, need to be its own object so can have dedicated audio source component
            fxInstance = GetPoolOfFX(this.fxInstance.gameObject, fxRequest.sfxData.clip.name).PullFX();
        }
        else {
            Debug.LogWarning($"FXRequest: No data provided! Source: {fxRequest.source}");
            return null;
        }
        
        fxInstance.LoadFXRequest(fxRequest);
        return fxInstance;
    }

    /// <summary>
    /// Searches for and creates pools of given prefab
    /// </summary>
    /// <param name="prefab">The Object of the pool</param>
    /// <returns>The prefab's pool</returns>
    private ObjectPoolAssetRefIDPair GetPoolOfFX(GameObject prefab, string idOverride = null) {
        string id = idOverride == null ? prefab.name : idOverride;
        foreach (ObjectPoolAssetRefIDPair poolIDPair in poolIDPairs) {
            if (poolIDPair.ID == id) {//pool for this item exists
                return poolIDPair;
            }
        }

        //first time. make a new pool.
        ObjectPoolAssetRefIDPair newPool = new ObjectPoolAssetRefIDPair();
        GameObject prefabClone = Instantiate(prefab);//create a clone so we don't modify the actual asset
        prefabClone.AddComponent<FXInstance>();//do this automatically here so you don't have to worry about dependencies in-inspector. make spawning assets work and as easy as possible!
        newPool.CreateFXInstancePool(prefabClone, id);
        prefabClone.transform.SetParent(newPool.GetFXPool().GetObjectPoolTransform());
        poolIDPairs.Add(newPool);
        //Destroy(prefabClone.gameObject);
        return newPool;
    }

    /// <summary>
    /// Searches for and creates pools of given prefab
    /// </summary>
    /// <param name="prefab">The Object of the pool</param>
    /// <returns>The prefab's pool</returns>
    private ObjectPoolAssetRefIDPair GetPoolOfPrefab(GameObject prefab) {
        foreach (ObjectPoolAssetRefIDPair poolIDPair in poolIDPairs) {
            if (poolIDPair.ID == prefab.name) {//pool for this item exists
                return poolIDPair;
            }
        }

        //first time. make a new pool.
        ObjectPoolAssetRefIDPair newPool = new ObjectPoolAssetRefIDPair();
        GameObject prefabClone = Instantiate(prefab);//create a clone so we don't modify the actual asset
        prefabClone.gameObject.SetActive(false);
        if (prefabClone.GetComponent<SpawnedObject>() == null) {
            prefabClone.AddComponent<SpawnedObject>();//make sure we have the SpawnedObject component on the pooled object (makes it compatible with pooling and 'source' attribute becomes accessible if bespoke script caches a reference to SpawnedObject component
        }
        newPool.CreatePool(prefabClone, prefab.name);
        prefabClone.transform.SetParent(newPool.GetPool().GetObjectPoolTransform());
        poolIDPairs.Add(newPool);
        return newPool;
    }

    //public void PlaySFX(string name) {
    //    Sound foundSound = Array.Find(sounds, sound => sound.name == name);
    //    if (foundSound == null) {
    //        Debug.LogWarning($"Sound: {name} + not found!", gameObject);
    //        return;
    //    }

    //    foundSound.audioSource.Play();
    //}
}