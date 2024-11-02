using System;
using UnityEngine;
using UnityEngine.VFX;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A GameObject with the ability to play sounds and control VFX
/// </summary>
[RequireComponent(typeof(SpawnedObject))]
[System.Serializable]
public class FXInstance : MonoBehaviour, IPoolable<FXInstance> {
    private Action<FXInstance> returnToPool;//Store a reference to the Push() function invoked by the object pool
    private GameObject source;

    //vfx
    private bool playParticleSystem;
    private ParticleSystem particleSystem;
    private PoolBarnicle poolBarnicle;
    //private VisualEffectAsset vfxGraph;
    private bool playVFXGraph;
    private Coroutine cleanupCoroutine;

    //sfx
    private AudioSource audioSource;
    private bool playSound;

    private const float POST_SPAWN_CLEANUP_DELAY = 0.5f;

    private void Awake() {
        particleSystem = GetComponent<ParticleSystem>();
    }
    private void OnEnable() {
        if (cleanupCoroutine != null) {
            StopCoroutine(cleanupCoroutine);
        }

        cleanupCoroutine = StartCoroutine(_Cleanup());//Timing.KillCoroutines(cleanupCoroutine);
    }

    private IEnumerator _Cleanup() {
        yield return new WaitForSeconds(POST_SPAWN_CLEANUP_DELAY);
        while (true) {
            ReturnToPoolHelper();
            yield return new WaitForEndOfFrame();
        }
    }


    private void ReturnToPoolHelper() {
        if (!gameObject.activeInHierarchy) {
            return;
        }

        if (playParticleSystem && (particleSystem.isPlaying || particleSystem.particleCount > 0)) {
            return;
        }
        if (playVFXGraph) {//todo idk what the criteria for this guy is
            return;
        }
        if (playSound && audioSource.isPlaying) {
            return;
        }

        //if we reach this point that means whatever should be playing is no longer playing, therefore it is done
        gameObject.SetActive(false);
    }

    public void LoadFXRequest(FXRequest fxRequest) {
        transform.position = fxRequest.spawnPosition;
        transform.rotation = Quaternion.Euler(fxRequest.spawnRotation);

        this.playParticleSystem = fxRequest.particleSystemAssetReference != null;
        //this.playVFXGraph = fxRequest.vfxGraphAssetReference != null;
        this.playSound = !fxRequest.Equals(default(SFXData));

        if (fxRequest.follow) {
            poolBarnicle = gameObject.AddComponent<PoolBarnicle>();
            poolBarnicle.LoadConstraintSettings(fxRequest.contraintSettings);
            poolBarnicle.SetTarget(fxRequest.source);
        }

        if (playParticleSystem) {
            if (particleSystem == null) {//instantiate the particle system
                particleSystem = Instantiate(fxRequest.particleSystemAssetReference, transform.position, Quaternion.identity, transform);
            }
            PlayParticleSystem(particleSystem);
        }
        //if (playVFXGraph) {
        //    //todo
        //    //PlayVFXGraph(fxRequest.vfxGraph);
        //}
        if (playSound) {//Two FXInstances can be two different noises but the same particle system. i think that's pretty cool :)
            PlaySFX(fxRequest.sfxData);
        }
    }

    public void Stop() {
        StopSFX();
        StopParticleSystem();
        //StopVFXGraph();
    }

    public void PlayParticleSystem(ParticleSystem particleSystem) {
        particleSystem.Play();
    }

    public void StopParticleSystem() {
        particleSystem.Stop();
    }

    //public void PlayVFXGraph(VisualEffectAsset vfxGraph) {
    //    //todo?
    //}

    //public void StopVFXGraph() {
    //    //todo?
    //}

    public void PlaySFX(SFXData data) {
        if (audioSource == null) {//add it as we go along
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = data.clip;
        audioSource.volume = data.volume;
        audioSource.pitch = data.pitch;
        audioSource.loop = data.loop;

        audioSource.Play();
    }

    public void StopSFX() {
        if (audioSource == null) {
            return;
        }
        audioSource.Stop();
    }

    #region IPoolable
    /// <summary>
    /// Sets up pooling ability for object
    /// </summary>
    /// <param name="returnAction">Reference to the Push() function invoked by the object pool</param>
    public void Initialize(Action<FXInstance> returnAction) {
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
        if (cleanupCoroutine != null) {
            StopCoroutine(cleanupCoroutine);
        }
        ReturnToPool();//Invoke function assigned in Initialize() when disabled (automatically pools itself)
    }
    #endregion
}