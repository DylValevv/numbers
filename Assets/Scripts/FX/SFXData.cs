using UnityEngine;

[System.Serializable]
public struct SFXData {
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;

    public bool loop;
}