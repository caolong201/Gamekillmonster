using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RangedEnemy : Enemy
{
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float timeWaitAnimation = 1f;
    private Tween _tweenDelay = null;
    
    public override void Init(Transform player)
    {
        base.Init(player);
        // maxHealth = 15;
        // attackPower = 3;
        // attackSpeed = 0.5f; // Tấn công chậm hơn nhưng mạnh hơn
    }

    protected override void Attack()
    {
        base.Attack();
    }
    
    //Animation event
    public void MacgicEffect()
    {
        var rig = EffectManager.Instance.PlayEffect<Rigidbody>(EEffectType.RangedEnemyFX, firePoint.position,
            Quaternion.identity);

        Vector3 direction = (player.position - firePoint.position).normalized;
        direction.y = 0f;
        rig.velocity = direction * projectileSpeed;
        //projectile.GetComponent<EnemyProjectile>().damage = attackPower;
    }
}