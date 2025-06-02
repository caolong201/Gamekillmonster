using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PoisonBottle : MonoBehaviour
{
    public int level = 1;
    private PoisonBottleExplosion bottlePrefab;
    public float throwForce = 10f;

    [SerializeField] private int attackPower = 5;
    [SerializeField] float radius = 2;
    [SerializeField] int damgePerSecond = 1;
    private float throwCooldown = 6f;
    private float nextThrowTime;

    private Enemy target = null;
    private bool isActive = false;

    private Animator animator;

    private int maxSkin = 5;
    private int skinID = 0;

    public void Init(Animator animator, BackpackSystem.SkillData data)
    {
        this.animator = animator;
        level = data.level;
        attackPower = 5 + ((level - 1) * 3);
        radius = 2 + ((level - 1) * 1);
        damgePerSecond = 1 + ((level - 1) * 1);
        throwCooldown = 6f;
        nextThrowTime = 0;
        isActive = true;

        int newSkinID = level;
        if (newSkinID > maxSkin)
        {
            newSkinID = maxSkin;
        }

        if (newSkinID == skinID && bottlePrefab != null)
        {
            //do notthing
        }
        else
        {
            skinID = newSkinID;
            //destroy old skin
            if (bottlePrefab != null) Destroy(bottlePrefab);
            //load new skin
            GameObject Prefab = Resources.Load<GameObject>("PlayerWeapons/Poisons/Poison_" + skinID);
            if (Prefab != null)
            {
                bottlePrefab = Prefab.GetComponent<PoisonBottleExplosion>();
            }
        }
    }

    public void Stop()
    {
        isActive = false;
    }

    void Update()
    {
        if (!isActive) return;

        nextThrowTime -= Time.deltaTime;
        if (nextThrowTime <= 0)
        {
            nextThrowTime = throwCooldown;
            ThrowBottle();
        }

    }

    void ThrowBottle()
    {
        target = EnemiesManager.Instance.GetNearestEnemy(10);
        if (target == null)
        {
            nextThrowTime = 1; //re-check after 1s
            return;
        }

        PoisonBottleExplosion bottle = Instantiate(bottlePrefab, transform.position, Quaternion.identity);
        Rigidbody rb = bottle.GetComponent<Rigidbody>();
        Vector3 throwDirection = (target.transform.position - transform.position).normalized;
        rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        bottle.SetStats(level, attackPower, radius, damgePerSecond);
    }
}