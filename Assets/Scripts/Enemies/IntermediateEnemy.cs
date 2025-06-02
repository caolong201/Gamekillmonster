using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermediateEnemy : Enemy
{
    [SerializeField] private Transform posEff;
    public override void Init(Transform player)
    {
        base.Init(player);
       
    }

    protected override void Attack()
    {
        base.Attack();
    }
    
    //Animation event
    public void MacgicEffect()
    {
      
        playerController.TakeDamage(attackPower);
    }
}
