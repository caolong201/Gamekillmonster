using System;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public int level = 1;
    public float attackCooldown = 1f;
    public float attackRadius = 2f;
    public int attackPower = 3;
    private float nextAttackTime;

    public bool isCanAttack = false;
    
    private GameObject currentSkin;
    private int maxSkin = 5;
    private int skinID = 0;
    
    public void Init(BackpackSystem.SkillData data)
    {
        level = data.level;
        attackPower = data.attackPower;
        
        int newSkinID = level;
        if (newSkinID > maxSkin)
        {
            newSkinID = maxSkin;
        }

        if (newSkinID == skinID && currentSkin != null)
        {
            currentSkin.SetActive(true);
        }
        else
        {
            skinID = newSkinID;
            //destroy old skin
            if (currentSkin != null) Destroy(currentSkin);
            //load new skin
            GameObject swordPrefab = Resources.Load<GameObject>("PlayerWeapons/Swords/Sword_" + skinID);
            if (swordPrefab != null)
            {
                currentSkin = Instantiate(swordPrefab, transform);
            }
        }
    }

    private void FixedUpdate()
    {
        if (Time.fixedTime >= nextAttackTime)
        {
            nextAttackTime = Time.fixedTime + attackCooldown;

            Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRadius, LayerMask.GetMask("Enemy"));
            if (hitEnemies.Length > 0)
            {
                isCanAttack = true;
            }
        }
    }

    public void SwordAttack()
    {
       // this.animator.Play("SwordAttack");
        isCanAttack = false;
    }

    //Animation event
    public void TakeDamage()
    {
        // Phát hiện enemy trong phạm vi
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRadius, LayerMask.GetMask("Enemy"));
        if (hitEnemies.Length == 0) return;

        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackPower);
        }
    }

    // Vẽ Gizmo để debug phạm vi tấn công
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}