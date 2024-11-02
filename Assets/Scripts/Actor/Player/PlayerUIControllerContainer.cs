using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;
using UnityEditor;

[RequireComponent(typeof(InputSystemUIInputModule), typeof(MultiplayerEventSystem))]
public class PlayerUIControllerContainer : UIContainer {
    private PlayerCharacter playerCharacter;
    public MultiplayerEventSystem multiplayerEventSystem;
    public InputSystemUIInputModule uiInputModule;
    private GameObject lastSelectedGameObject;
    private GameObject currentSelectedUIComponent;

    private void OnDisable() {
        RemoveListeners();
    }

    private void Update() {
        UpdateInfo();
    }

    public void Init(PlayerCharacter playerCharacterInstanceData) {
        playerCharacter = playerCharacterInstanceData;

        multiplayerEventSystem.playerRoot = gameObject;
        playerCharacter.GetPlayerInput().uiInputModule = uiInputModule;
        playerCharacter.GetPlayerInput().onActionTriggered += MenuAction;
        //Show(false);

        AddListeners();
    }

    private void UpdateInfo() {
        if (multiplayerEventSystem.currentSelectedGameObject == null) {
            currentSelectedUIComponent = null;
            return;
        }
        if (lastSelectedGameObject == multiplayerEventSystem.currentSelectedGameObject) {
            return;
        }

        currentSelectedUIComponent = multiplayerEventSystem.currentSelectedGameObject;
        lastSelectedGameObject = multiplayerEventSystem.currentSelectedGameObject;
    }

    public string GetID() {
        return playerCharacter.GetPlayerID();
    }


    public override void ShowInstant(bool show) {
        base.ShowInstant(show);
        if (show) { SelectFirstMenuItem(); }
    }

    public override void Show(bool show) {
        base.Show(show);
        if (show) { SelectFirstMenuItem(); }
    }

    public PlayerCharacter GetPlayerCharacter() {
        return playerCharacter;
    }

    public void MenuAction(InputAction.CallbackContext context) {
        if (context.phase != InputActionPhase.Started) {//Started = OnButtonDown()
            return;
        }
        switch (context.action.name) {
            case GlobalNames.InputIDs.CONFIRM_INPUT_ID:
                if (multiplayerEventSystem.currentSelectedGameObject == null) {
                    //Debug.LogError($"No selected MenuItem to click!");
                    return;
                }
                multiplayerEventSystem.currentSelectedGameObject.GetComponentInParent<UIComponent>().DoAction();
                break;
            case GlobalNames.InputIDs.CANCEL_INPUT_ID:
                Back();
                break;
            default: break;
        }
    }
    public void SelectFirstMenuItem() {
        for (int i = 0; i < uiComponents.Count; i++) {
            if (!uiComponents[i].isSelectable) {
                continue;
            }
            multiplayerEventSystem.SetSelectedGameObject(uiComponents[i].gameObject);
            return;
        }
        multiplayerEventSystem.SetSelectedGameObject(null);
    }

    public void RemoveMenuItem(UIComponent menuItemToRemove) {
        uiComponents.Remove(menuItemToRemove);
        SelectFirstMenuItem();
    }

    #region Listeners
    private void AddListeners() { }

    private void RemoveListeners() { }
    #endregion
}