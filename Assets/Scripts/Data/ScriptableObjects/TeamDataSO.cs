using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TeamData", fileName = "New TeamData")]
public class TeamDataSO : ScriptableObject {
    public enum Team {
        White = 0,
        Red = 1,
        Blue = 2,
        Green = 3,
        Yellow = 4,
    }

    public string teamName = "Team Name";
    public Team team;
    public Material teamMaterial;
    [ColorUsageAttribute(true, true)] public Color teamColor;
    [ColorUsageAttribute(true, true)] public Color teamColorUI;
}