using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EnemyType
{
    EnemyBasic = 0,
    EnemyRanged,
    EnemyIntermediate,
    EnemyAdvanced,
    EnemyTutorial
}

public class EnemiesManager : SingletonMono<EnemiesManager>
{
    public float minDistanceFromPlayer = 5f;
    public float maxDistanceFromPlayer = 15f;

    [SerializeField] private Transform player;

    private List<Enemy> enemies = null;
    Enemy nearest = null;
    [SerializeField] StageConfig currentStageConfig;

    [SerializeField] List<Enemy> enemiesPrefab;

    private float timerCheckSpawBonusEnemies = 5;

    private float timerCheckSpawEnemies = 1;
    private int wave = 0;

    //Tutorial
    private bool isSpawEnmiesTutorial = false;
    private bool isSpawEnmiesTutorialDone = false;

    void Start()
    {
        NewGame();
    }

    public void Clear()
    {
        CancelInvoke();
        if (enemies != null)
        {
            foreach (var child in enemies)
            {
                Destroy(child.gameObject);
            }
        }

        enemies = new List<Enemy>();
        nearest = null;
    }

    public void NewGame()
    {
        Clear();
    }

    void SpawnEnemies()
    {
        currentStageConfig = GameManager.Instance.GetStageConfig();
        Enemy ePrefab = null;
        for (int i = 0; i < currentStageConfig.TotalEnemiesSpaw; i++)
        {
            Enemy enemy = null;
            var enemyType = currentStageConfig.enemies[Random.Range(0, currentStageConfig.enemies.Count)].type;
            enemy = enemies.Find(x => !x.gameObject.activeSelf && x.type == enemyType);
            Vector3 spawnPos = GetRandomSpawnPosition();
            if (enemy == null)
            {
                foreach (var prefab in enemiesPrefab)
                {
                    if (prefab.type == enemyType)
                    {
                        ePrefab = prefab;
                        break;
                    }
                }

                enemy = Instantiate(ePrefab, spawnPos, Quaternion.identity, transform);
                enemies.Add(enemy);
            }
            else
            {
                enemy.transform.position = spawnPos;
            }

            enemy.Init(player);
            enemy.gameObject.SetActive(true);
        }
    }

    void SpawnEnemies(EnemyType enemyType, int numEnemies)
    {
        Enemy ePrefab = null;
        for (int i = 0; i < numEnemies; i++)
        {
            Enemy enemy = null;
            enemy = enemies.Find(x => !x.gameObject.activeSelf && x.type == enemyType);
            Vector3 spawnPos = GetRandomSpawnPosition();
            if (enemy == null)
            {
                foreach (var prefab in enemiesPrefab)
                {
                    if (prefab.type == enemyType)
                    {
                        ePrefab = prefab;
                        break;
                    }
                }

                enemy = Instantiate(ePrefab, spawnPos, Quaternion.identity, transform);
                enemies.Add(enemy);
            }
            else
            {
                enemy.transform.position = spawnPos;
            }

            enemy.Init(player);
            enemy.gameObject.SetActive(true);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere.normalized;
        float randomDistance = Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);
        Vector3 spawnPos = player.position + randomDirection * randomDistance;

        spawnPos.y = 0f;

        return spawnPos;
    }

    public Enemy GetNearestEnemy(float maxDistance)
    {
        float minDistance = Mathf.Infinity;
        float distance = 0;
        foreach (var enemy in enemies)
        {
            if (!enemy.gameObject.activeSelf || enemy.isDead) continue;
            distance = Vector3.Distance(player.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemy;
            }
        }

        if (minDistance > maxDistance) return null;

        return nearest;
    }

    public List<Enemy> GetRandomEnemies(float maxDistance, int maxEnemies)
    {
        List<Enemy> validEnemies = new List<Enemy>();

        foreach (var enemy in enemies)
        {
            if (!enemy.gameObject.activeSelf || enemy.isDead) continue;

            float distance = Vector3.Distance(player.position, enemy.transform.position);
            if (distance <= maxDistance)
            {
                validEnemies.Add(enemy);
            }
        }

        // Trả về danh sách với số lượng tối đa
        return validEnemies.Take(maxEnemies).ToList();
    }

    public void RemoveEnemyDead(Enemy e)
    {
        e.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (GameManager.Instance.IsTutorial || GameManager.Instance.GameStage != EGameStage.Live) return;

        timerCheckSpawEnemies -= Time.deltaTime;
        if (GameManager.Instance.CurrentStage == 1) //tutorial
        {
            if (timerCheckSpawEnemies <= 0)
            {
                timerCheckSpawEnemies = 5;
                SpawnEnemies();
            }
        }
        else if (GameManager.Instance.CurrentStage == 2)
        {
            if (timerCheckSpawEnemies <= 0 && wave < 3)
            {
                SpawnEnemies(EnemyType.EnemyTutorial, 16);
                timerCheckSpawEnemies = 20;
                wave++;
            }
            else  if (timerCheckSpawEnemies <= 0 && wave >= 3)
            {
                SpawnEnemies(EnemyType.EnemyRanged, 10);
                timerCheckSpawEnemies = 30;
                wave++;
            }
            
            CheckSpawEnemiesBonus(EnemyType.EnemyTutorial, 8);
        }
        else
        {
            if (timerCheckSpawEnemies <= 0)
            {
                timerCheckSpawEnemies = 30;
                SpawnEnemies();
            }

            CheckSpawEnemiesBonus(EnemyType.EnemyBasic, 5);
        }
    }

    private void CheckSpawEnemiesBonus(EnemyType type, int numEnemies)
    {
        timerCheckSpawBonusEnemies -= Time.deltaTime;
        if (timerCheckSpawBonusEnemies <= 0)
        {
            timerCheckSpawBonusEnemies = 1;
            var checkLive = enemies.Find(x => x.isDead == false);
            if (checkLive == null)
            {
                SpawnEnemies(type, numEnemies);
            }
        }
    }
}