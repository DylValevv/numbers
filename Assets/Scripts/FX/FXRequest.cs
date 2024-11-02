using UnityEngine.VFX;
using UnityEngine;

[System.Serializable]
public struct FXRequest {
    [Header("SFX")]
    public SFXData sfxData;

    [Header("VFX")]
    public ParticleSystem particleSystemAssetReference;
    //public VisualEffectAsset vfxGraphAssetReference;

    [Header("Position & Rotation")]
    [Tooltip("'Source' is assigned via script when the Play() function is called")]
    public bool inheritSpawnPositionFromSource;
    public Vector3 spawnPosition;
    public bool inheritSpawnRotationFromSource;
    public Vector3 spawnRotation;
    [HideInInspector] public GameObject source;
    [Header("Transform Tracking")]
    public bool follow;//pool barnicle
    public TransformContraintSettings contraintSettings;

    public void Play(GameObject source) {
        //init
        this.source = source;
        spawnPosition = inheritSpawnPositionFromSource ? source.transform.position : spawnPosition;
        spawnRotation = inheritSpawnRotationFromSource ? source.transform.rotation.eulerAngles : spawnRotation;
        ResourceManager.Instance.PlayFXRequest(this);//request and play FX (pooled)
    }
}