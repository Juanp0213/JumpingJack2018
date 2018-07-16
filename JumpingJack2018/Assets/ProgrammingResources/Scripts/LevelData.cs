using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LevelData")]
public class LevelData : ScriptableObject
{
    public LevelInfo[] Levels;
}

[System.Serializable]
public class LevelInfo
{
    public bool ExtraLife;
    public string FinishMessage;
    public EnemyData[] Enemies;
}

[System.Serializable]
public class EnemyData
{
    public int StartFloor;
    public float StartXPos;
    public Color EnemyColor;
}