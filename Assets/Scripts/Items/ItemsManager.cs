using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsManager : MonoBehaviour
{
    [SerializeField] private int spawnCount = 50;
    [SerializeField] private Transform player;
    [SerializeField] private float distanceCollect = 2f;
    void Start()
    {
        LoadItems();
    }

    public void LoadItems()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            ItemHandler item =EffectManager.Instance.LoadObject<ItemHandler>(EEffectType.ItemCollect, GetRandomSpawnPosition(), Quaternion.identity, transform);
            item.Init(player, distanceCollect);
        }
    }
    
    Vector3 GetRandomSpawnPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere.normalized;
        float randomDistance = Random.Range(10, 30);
        Vector3 spawnPos = player.position + randomDirection * randomDistance;

        spawnPos.y = 0f;

        return spawnPos;
    }
}
