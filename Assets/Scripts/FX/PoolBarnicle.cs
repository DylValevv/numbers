using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TransformContraintSettings {
    public bool positionX;
    public bool positionY;
    public bool positionZ;

    public bool rotationX;
    public bool rotationY;
    public bool rotationZ;
}

public class PoolBarnicle : MonoBehaviour {
    public GameObject target;
    public TransformContraintSettings transformContraintSettings;
    private float posX;
    private float posY;
    private float posZ;
    private float rotX;
    private float rotY;
    private float rotZ;

    public void Update() {
        FollowTarget();
    }

    private void FollowTarget() {
        if (target == null) {
            return;
        }

        posX = transformContraintSettings.positionX ? 0 : target.transform.position.x;
        posY = transformContraintSettings.positionY ? 0 : target.transform.position.y;
        posZ = transformContraintSettings.positionZ ? 0 : target.transform.position.z;
        transform.position = new Vector3(posX, posY, posZ);

        rotX = transformContraintSettings.rotationX ? 0 : target.transform.rotation.x;
        rotY = transformContraintSettings.rotationY ? 0 : target.transform.rotation.y;
        rotZ = transformContraintSettings.rotationZ ? 0 : target.transform.rotation.z;

        transform.position = new Vector3(posX, posY, posZ);
        transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);
    }

    public void SetTarget(GameObject newTarget) {
        target = newTarget;
    }

    public void LoadConstraintSettings(TransformContraintSettings transformContraintSettings) {
        this.transformContraintSettings.positionX = transformContraintSettings.positionX;
        this.transformContraintSettings.positionY = transformContraintSettings.positionY;
        this.transformContraintSettings.positionZ = transformContraintSettings.positionZ;
        this.transformContraintSettings.rotationX = transformContraintSettings.rotationX;
        this.transformContraintSettings.rotationY = transformContraintSettings.rotationY;
        this.transformContraintSettings.rotationZ = transformContraintSettings.rotationZ;
    }
}