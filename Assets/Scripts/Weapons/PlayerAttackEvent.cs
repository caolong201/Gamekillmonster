using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackEvent : MonoBehaviour
{
    private Sword sword;
    public void Init(Sword sword)
    {
        this.sword = sword;
    }

    //Animation event
    public void MacgicEffect()
    {
        sword.TakeDamage();
        // // Phát hiện enemy trong phạm vi
        // Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRadius, LayerMask.GetMask("Enemy"));
        // if (hitEnemies.Length == 0) return;
        //
        // foreach (Collider enemy in hitEnemies)
        // {
        //     enemy.GetComponent<Enemy>().TakeDamage(damageByLevel[level - 1]);
        // }
    }
}
