using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    private static GameManager _instance;
    public static GameManager Instance {
        get { return _instance; }
        private set { }
    }
    public bool IsPaused { get { return paused; } private set { } }
    private bool paused;
    Message pauseMessage = new Message(GlobalNames.UI.PAUSE, true);

    //Attributes
    [Header("General Attributes")]
    public Transform poolHolder;

    [Header("FXRequests")]
    public FXRequest pauseFXRequest;
    public FXRequest unpauseFXRequest;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;

        DontDestroyOnLoad(this);
    }

    private void Update() {
        //Example of flexibility with prototyping with this code, can get very custom very easily
        //This is a global call, so anyone can trigger this command. For single player experiences this'll be very robust.
        //The PlayerCharacter script shows how it should be done per-player instance. 
        if (InputManager.Instance.PauseTriggered()) {
            paused = !paused;
            PauseGame(paused);
        }
    }

    public void PauseGame(bool pause) {
        paused = pause;
        pauseMessage.Data = paused;
        MessageDispatcher.SendMessage(pauseMessage);
        Time.timeScale = paused ? 0 : 1;
        if (paused) {
            PlayerManager.Instance.SwitchActionMap(GlobalNames.InputIDs.UI_ACTION_MAP);
            pauseFXRequest.Play(gameObject);
            Time.timeScale = 0;
        }
        else {
            unpauseFXRequest.Play(gameObject);
            PlayerManager.Instance.SwitchActionMap(GlobalNames.InputIDs.PLAYER_ACTION_MAP);
            Time.timeScale = 1;
        }
    }

    public void LoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}