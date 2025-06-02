using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedEnemy : Enemy
{
    [SerializeField] private Transform posEff;
    
    public override void Init(Transform player)
    {
        base.Init(player);
    }
    
    //Animation event
    public void MacgicEffect()
    {
        Debug.Log("AdvancedEnemy MacgicEffect");
        EffectManager.Instance.PlayEffect<Rigidbody>(EEffectType.EnemyAdvancedFX, posEff.position,
            Quaternion.identity);

        
        playerController.TakeDamage(attackPower);
    }
}
