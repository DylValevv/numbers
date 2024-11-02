using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
    private static InputManager _instance;
    public static InputManager Instance {
        get {
            return _instance;
        }
    }
    private PlayerInputs playerInputs;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        }
        else {
            _instance = this;
        }

        playerInputs = new PlayerInputs();

        DontDestroyOnLoad(this);
    }

    private void OnEnable() {
        playerInputs.Enable();
    }

    private void OnDisable() {
        playerInputs.Disable();
    }

    public Vector2 GetMoveVector() {
        return playerInputs.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetAimDelta() {
        return playerInputs.Player.Aim.ReadValue<Vector2>();
    }

    public bool ShootRightActionTriggered() {
        return playerInputs.Player.ShootRight.triggered;
    }

    public bool ShootLeftActionTriggered() {
        return playerInputs.Player.ShootLeft.triggered;
    }

    public bool ShootActionRightHeld() {
        return playerInputs.Player.ShootRight.ReadValue<float>() == 1;
    }

    public bool ShootActionLefttHeld() {
        return playerInputs.Player.ShootLeft.ReadValue<float>() == 1;
    }

    public bool BumperRightTriggered() {
        return playerInputs.Player.SpecialOne.triggered;
    }

    public bool BumperRightHeld() {
        return playerInputs.Player.SpecialOne.ReadValue<float>() == 1;
    }

    public bool BumperLeftTriggered() {
        return playerInputs.Player.SpecialTwo.triggered;
    }

    public bool BumperLeftHeld() {
        return playerInputs.Player.SpecialTwo.ReadValue<float>() == 1;
    }

    public bool ConfirmActionTriggered() {
        return playerInputs.Menu.Confirm.triggered;
    }

    public bool CancelActionTriggered() {
        return playerInputs.Menu.Cancel.triggered;
    }

    public bool PauseTriggered() {
        return playerInputs.UI.Submit.triggered;
    }
}