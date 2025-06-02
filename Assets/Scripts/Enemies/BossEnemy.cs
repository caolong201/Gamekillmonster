using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : Enemy
{
    public GameObject areaAttackPrefab;
    public float areaAttackCooldown = 5f;
    private float nextAreaAttackTime;

    public override void Init(Transform player)
    {
        base.Init(player);
        maxHealth = 150;
        attackPower = 8;
        moveSpeed = 1.5f;
    }

    protected override void Update()
    {
        base.Update();
        if (Time.time >= nextAreaAttackTime)
        {
            AreaAttack();
            nextAreaAttackTime = Time.time + areaAttackCooldown;
        }
    }

    void AreaAttack()
    {
        Instantiate(areaAttackPrefab, transform.position, Quaternion.identity);
        // Logic gây damage được xử lý trong script AreaAttackEffect
    }

}