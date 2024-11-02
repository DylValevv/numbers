using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour {
    private static DebugManager _instance;
    public static DebugManager Instance {
        get { return _instance; }
        private set { }
    }
    private bool enable = false;

    [Header("Player Attributes")]
    public bool autoSortTeamJoin;
    [Header("Scene Load Attributes")]
    public InputAction loadSceneInput;
    public string testLoadScene;
    [Header("UI")]
    public bool logWhatClickedOn;
    public GraphicRaycaster graphicRaycaster;
    private PointerEventData pointerEventData;
    private List<RaycastResult> raycastResults = new List<RaycastResult>();

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;

        Init();

        pointerEventData = new PointerEventData(EventSystem.current);
    }

    private void OnDisable() {
        loadSceneInput.Disable();
    }

    private void Init() {
#if UNITY_EDITOR
        enable = true;
        loadSceneInput.Enable();
#endif
    }

    private void Update() {
        if (!enable) {
            return;
        }

        if (loadSceneInput.triggered) {
            GameManager.Instance.LoadScene(testLoadScene);
        }

        if (logWhatClickedOn) {
            // Check if the left mouse button was clicked and no object is selected
            if (Input.GetMouseButtonDown(0)) {
                pointerEventData.position = Mouse.current.position.ReadValue();
                raycastResults.Clear();

                graphicRaycaster.Raycast(pointerEventData, raycastResults);
                if (raycastResults.Count > 0) {
                    Debug.Log("UI Object clicked: " + raycastResults[0].gameObject.name);
                }
                else {
                    Debug.Log("No UI object clicked.");
                }
            }
        }
    }

    public void Ping() {
        Debug.Log("hellow orld !");
    }
}