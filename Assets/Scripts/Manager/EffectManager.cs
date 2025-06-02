using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEffectType
{
    DamageFX = 3,
    RangedEnemyFX = 4,
    EnemyAdvancedFX = 5,
    ItemCollect = 6,
    PoisonFX = 7,
    PlusHPFX = 8,
    PoisonFX_2 = 9,
    PoisonFX_3 = 10,
    PoisonFX_4 = 11,
    PoisonFX_5 = 12,
}

[Serializable]
public class EffectPrefab
{
    public EEffectType type;
    public GameObject prefab;
}

public class EffectManager : SingletonMonoAwake<EffectManager>
{
    [SerializeField] private List<EffectPrefab> effectsPrefab = new List<EffectPrefab>();

    // Pool for effects
    private Dictionary<string, Queue<GameObject>> effectPool = new Dictionary<string, Queue<GameObject>>();


    public override void OnAwake()
    {
        base.OnAwake();
    }

    /// <summary>
    /// Preloads effects into the pool.
    /// </summary>
    /// <param name="effectPrefab">Effect prefab.</param>
    /// <param name="amount">Number of instances to preload.</param>
    public void PreloadEffect(EEffectType type, int amount)
    {
        var findEffect = effectsPrefab.Find(x => x.type == type);
        if (findEffect == null || findEffect.prefab == null)
        {
            Debug.Log("Effect not found");
            return;
        }

        string effectName = findEffect.prefab.name;
        if (!effectPool.ContainsKey(effectName))
        {
            effectPool[effectName] = new Queue<GameObject>();
        }

        for (int i = 0; i < amount; i++)
        {
            GameObject effectInstance = Instantiate(findEffect.prefab, transform);
            effectInstance.SetActive(false);
            effectPool[effectName].Enqueue(effectInstance);
        }
    }

    /// <summary>
    /// Spawns an effect from the pool at the specified position and rotation.
    /// </summary>
    /// <param name="effectPrefab">Effect prefab.</param>
    /// <param name="position">Spawn position.</param>
    /// <param name="rotation">Spawn rotation.</param>
    public T PlayEffect<T>(EEffectType type, Vector3 position, Quaternion rotation)
    {
        var findEffect = effectsPrefab.Find(x => x.type == type);
        if (findEffect == null || findEffect.prefab == null)
        {
            Debug.Log("Effect not found");
            return default(T);
        }

        string effectName = findEffect.prefab.name;

        if (!effectPool.ContainsKey(effectName) || effectPool[effectName].Count == 0)
        {
            PreloadEffect(type, 1); // Preload if no available effects
        }

        GameObject effectInstance = effectPool[effectName].Dequeue();
        effectInstance.transform.position = position;
        effectInstance.transform.rotation = rotation;
        effectInstance.SetActive(true);

        // Schedule effect deactivation
        StartCoroutine(DeactivateEffect(effectInstance, findEffect.prefab));

        return effectInstance.GetComponent<T>();
    }

    public T LoadObject<T>(EEffectType type, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        var findEffect = effectsPrefab.Find(x => x.type == type);
        if (findEffect == null || findEffect.prefab == null)
        {
            Debug.Log("Effect not found");
            return default(T);
        }
        
        var effectInstance = Instantiate(findEffect.prefab, position, rotation, parent);
        effectInstance.SetActive(true);

        return effectInstance.GetComponent<T>();
    }

    /// <summary>
    /// Deactivates the effect after it has finished playing.
    /// </summary>
    /// <param name="effectInstance">Effect instance.</param>
    /// <param name="effectPrefab">Effect prefab.</param>
    private IEnumerator DeactivateEffect(GameObject effectInstance, GameObject effectPrefab)
    {
        ParticleSystem ps = effectInstance.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            yield return new WaitForSeconds(ps.main.duration + ps.main.startLifetime.constantMax);
        }
        else
        {
            yield return new WaitForSeconds(2f); // Default time for non-particle effects
        }

        if (effectInstance != null)
        {
            effectInstance.SetActive(false);
            if (effectPool.ContainsKey(effectPrefab.name)) effectPool[effectPrefab.name].Enqueue(effectInstance);
        }
    }

    public void DeactivateEffect(GameObject effectInstance)
    {
        effectInstance.SetActive(false);
        if (effectPool.ContainsKey(effectInstance.name)) effectPool[effectInstance.name].Enqueue(effectInstance);
    }

    public void ClearEffectPool()
    {
        foreach (var queue in effectPool.Values) // Duyệt từng hàng đợi (Queue)
        {
            while (queue.Count > 0)
            {
                GameObject obj = queue.Dequeue(); // Lấy object ra khỏi Queue
                if (obj != null)
                {
                    Destroy(obj); // Hủy GameObject
                }
            }
        }

        effectPool.Clear(); // Xóa toàn bộ Dictionary
    }

    public void ClearObjectPool()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        effectPool.Clear(); // Xóa toàn bộ Dictionary
    }
}