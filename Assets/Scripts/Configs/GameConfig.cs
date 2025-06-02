using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class StageConfig
{
    public int stage;
    public int TotalEnemiesSpaw = 10;
    public float winTimer = 2;// minus
    public List<EnemyConfig> enemies;

}


[Serializable]
public class EnemyConfig
{
    public EnemyType type;
    public float maxHealth;
    public float attackPower;
}

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game Configuration", order = 1)]
public class GameConfig : ScriptableObject
{
    public List<StageConfig> stages;
}

