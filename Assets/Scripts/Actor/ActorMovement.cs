using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class ActorMovement : MonoBehaviour {
    #region Attributes
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform bodyComponent;
    [SerializeField] private Transform aimComponent;

    [Header("Locomotion Attributes")]
    public float moveSpeed;
    public float momentumDecay = 0.9f;
    private Vector3 inputVector;
    private Vector3 previousInputVector;
    private bool canMove = true;

    [Header("First Person Locomotion Attributes")]
    //tood? look at fps controller shit

    [Header("Third Person Locomotion Attributes")]
    public float bodyRotateSpeed;
    public float aimRotationSpeed;
    private Vector2 rotateVector;
    private bool isReversing;
    float movementVectorDifference;
    public float groundCheckDistanceThreshold = 0.05f;
    public enum MovementType {
        Normal,
        Tank
    }
    public MovementType movementType;

    [Header("Tank Movement Nuance")]
    public float bodyRotateThreshold = 5f;
    [Tooltip("Angle to stop movement in order to rotate towards it")]  public float stopAngle;

    private const float JOYSTICK_CANNON_ROTATION_SPEED_SCALAR = 0.0025f;
    private const float BUMPER_CANNON_ROTATION_SPEED_SCALAR = 0.25f;
    private const float MOVE_SPEED_SCALAR = 0.15f;
    #endregion

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    //private void FixedUpdate() {
    //    GroundCorrection();
    //}

    private void GroundCorrection() {
        RaycastHit hit;
        Vector3 direction = Vector3.down;
        //Debug.DrawRay(playerMovement.GetAimComponent().position, direction, Color.red, 1);
        if (Physics.Raycast(transform.position, direction, out hit, 100)) {
            if (Vector3.Distance(hit.point, transform.position) > groundCheckDistanceThreshold) {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, 0.01f, gameObject.transform.position.z);
            }
        }
    }

    public void Movement(Vector2 moveVector) {
        switch (movementType) {
            case MovementType.Normal:
                NormalMovement(moveVector);
                break;
            case MovementType.Tank:
                TankMovement(moveVector);
                break;
        }
    }

    private void NormalMovement(Vector2 input) {
        input.Normalize();
        inputVector = new Vector3(input.x, 0, input.y);
        Vector3 moveVector = (inputVector + previousInputVector);
        rb.linearVelocity = moveVector;
        previousInputVector = moveVector * momentumDecay;

        movementVectorDifference = Mathf.Abs(Vector3.Angle(inputVector, bodyComponent.forward));
        isReversing = movementVectorDifference > 100f;
        //Debug.DrawRay(transform.position, inputVector, Color.red);
        NormalBodyRotationHandler(input, isReversing);
    }

    private void NormalBodyRotationHandler(Vector2 moveVector, bool reverse) {
        //if (movementVectorDifference < bodyRotateThreshold) {
        //    return;
        //}

        float targetAngle = Mathf.Atan2(moveVector.x, moveVector.y) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

        if (reverse) {//slide reverse-y 
            rb.linearVelocity = Vector3.zero;
            bodyComponent.rotation = Quaternion.Euler(0, bodyComponent.rotation.eulerAngles.y + 180f, 0);
            Debug.Log("ADD ANIMATION AND FX SLIDE HERE!");
        }
        else {
            bodyComponent.rotation = Quaternion.RotateTowards(bodyComponent.rotation, targetRotation, bodyRotateSpeed * Time.deltaTime);
        }
    }

    private void TankMovement(Vector2 moveVector) {
        if (moveVector == Vector2.zero) {
            //rb.velocity = Vector3.zero;
            return;
        }

        moveVector.Normalize();
        inputVector = new Vector3(moveVector.x, 0, moveVector.y);

        movementVectorDifference = Mathf.Abs(Vector3.Angle(inputVector, bodyComponent.forward));
        isReversing = movementVectorDifference > 100f;
        TankBodyRotationHandler(moveVector, isReversing);

        if (movementVectorDifference >= stopAngle) {
            canMove = false;
            //rb.velocity = Vector3.zero;
            return;
        }
        else {
            canMove = true;
        }

        Vector3 newPosition = rb.position + bodyComponent.forward * (moveSpeed * MOVE_SPEED_SCALAR) * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    private void TankBodyRotationHandler(Vector2 moveVector, bool reverse) {
        if (movementVectorDifference < bodyRotateThreshold) {
            return;
        }

        float targetAngle = Mathf.Atan2(moveVector.x, moveVector.y) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

        if (reverse) {
            rb.linearVelocity = Vector3.zero;
            bodyComponent.rotation = Quaternion.Euler(0, bodyComponent.rotation.eulerAngles.y + 180f, 0);
        }
        else {
            bodyComponent.rotation = Quaternion.RotateTowards(bodyComponent.rotation, targetRotation, bodyRotateSpeed * Time.deltaTime);
        }
    }

    //public void Move(Vector2 moveVector) {//from -1 to 1
    //    if (!canMove) {
    //        rb.velocity = Vector3.zero;
    //        return;
    //    }

    //    playerMovementVector = new Vector3(moveVector.x, 0f, moveVector.y);
    //    playerMovementVector *= moveSpeed;
    //    rb.velocity = playerMovementVector;
    //}

    public void Aim(Vector2 aimVector) {
        float currentYRotation;
        rotateVector = aimVector;
        if (rotateVector.magnitude < 0.01f) {
            return;
        }
        float targetAngle = Mathf.Atan2(rotateVector.x, rotateVector.y) * Mathf.Rad2Deg;
        currentYRotation = aimComponent.localEulerAngles.y;
        float angleDifference = Mathf.DeltaAngle(currentYRotation, targetAngle);
        aimComponent.Rotate(0, angleDifference * aimRotationSpeed * JOYSTICK_CANNON_ROTATION_SPEED_SCALAR, 0);
    }

    public void RotateCannon(Vector3 rotateAxis) {
        aimComponent.Rotate(
            rotateAxis.x * aimRotationSpeed * BUMPER_CANNON_ROTATION_SPEED_SCALAR,
            rotateAxis.y * aimRotationSpeed * BUMPER_CANNON_ROTATION_SPEED_SCALAR,
            rotateAxis.z * aimRotationSpeed * BUMPER_CANNON_ROTATION_SPEED_SCALAR);
    }

    public Transform GetAimComponent() {
        return aimComponent;
    }
}