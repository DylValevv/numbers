using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalNames {
    public static class InputIDs {
        //action names
        public const string MOVEMENT_INPUT_ID = "Move";
        public const string AIM_INPUT_ID = "Aim";
        public const string CONFIRM_INPUT_ID = "Confirm";
        public const string CANCEL_INPUT_ID = "Cancel";
        public const string SPAWN_INPUT_ID = "Spawn";

        //action map names
        public const string PLAYER_ACTION_MAP = "Player";
        public const string UI_ACTION_MAP = "UI";
    }

    public static class TagsAndLayers {
        //Tags
        public const string PLAYER_TAG = "Player";
        public const string SPAWN_POINT_TAG = "SpawnPoint";
        //Layers
        public const string ENVIRONMENT_LAYER = "Environment";
    }

    public static class SceneNames {
        public const string MENU = "Menu";
        public const string MAIN = "Main";
        public const string GREYBOX = "Greybox";
        public const string DEBUG = "Debug";
        public const string CREDITS = "Credits";
    }

    public static class UI {
        public const string PAUSE = "Pause";
        public const string HIDE_ALL_VIEWS = "HideAllViews";
    }

    public static class Data {
        public const string UPDATE_PLAYER_CHARACTER_DATA = "UpdatePlayerCharacterData";
    }

    public static class Game {
        public const string SCORE_EVENT = "ScoreEvent";
        public const string SPAWN_EVENT = "BirthEvent";
    }
}