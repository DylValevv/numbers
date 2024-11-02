using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour {
    private static UIManager _instance;
    public static UIManager Instance {
        get { return _instance; }
        private set { }
    }

    public Transform viewUIsContainer;
    private List<UIContainer> uiContainers = new List<UIContainer>();
    [SerializeField] private UIContainer pauseMenu;
    private List<PlayerUIControllerContainer> playerUIControllers = new List<PlayerUIControllerContainer>();

    #region Monobehaviour
    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;

        DontDestroyOnLoad(this);

        foreach (Transform child in viewUIsContainer) {
            UIContainer viewUI = child.GetComponent<UIContainer>();
            uiContainers.Add(viewUI);
        }
    }

    private void OnEnable() {
        AddListeners();
    }

    private void OnDisable() {
        RemoveListeners();
    }
    #endregion

    public PlayerUIControllerContainer RegisterPlayer(PlayerCharacter playerCharacterInstanceData) {
        PlayerUIControllerContainer playerUIController;
        playerUIController = DuplicatePlayerDataCheck(playerCharacterInstanceData);
        if (playerUIController) {
            return playerUIController;
        }

        Debug.Log($"Creating player-controlled UI Container: {playerCharacterInstanceData.GetPlayerID()}");
        playerUIController = Instantiate(playerCharacterInstanceData.playerUIControllerPrefab, viewUIsContainer);
        playerUIController.Init(playerCharacterInstanceData);
        playerUIControllers.Add(playerUIController);
        return playerUIController;
    }

    private PlayerUIControllerContainer DuplicatePlayerDataCheck(PlayerCharacter playerCharacterInstanceData) {
        //destroy any to-be duplicates.
        int count = playerUIControllers.Count;
        for (int i = 0; i < count; i++) {
            if (playerUIControllers[i].GetID() == playerCharacterInstanceData.GetPlayerID()) {
                return playerUIControllers[i];
            }
        }

        return null;
    }

    private void PauseGame(bool pause) {
        pauseMenu.Show(pause);

        //switch input modes from Player/Game -> Menu/UI

        //pauseMenu.Show(pause);
    }

    #region Listeners
    private void PauseListener(Message message) {
        if (message.Data is bool pause) {
            PauseGame(pause);
        }
    }

    private void HideAllViewUIs(Message message) {
        foreach(UIContainer viewUI in uiContainers) {
            viewUI.Show(false);
        }
    }

    private void AddListeners() {
        MessageDispatcher.AddListener(GlobalNames.UI.PAUSE, PauseListener);
        MessageDispatcher.AddListener(GlobalNames.UI.HIDE_ALL_VIEWS, HideAllViewUIs);
    }

    private void RemoveListeners() {
        MessageDispatcher.RemoveListener(GlobalNames.UI.PAUSE, PauseListener);
        MessageDispatcher.RemoveListener(GlobalNames.UI.HIDE_ALL_VIEWS, HideAllViewUIs);
    }
    #endregion
}