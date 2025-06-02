using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonBottleExplosion : MonoBehaviour
{
    private int damage;
    private float radius;
    private int dotDps;
    private int level = 1;

    public void SetStats(int level, int dmg, float rad, int dps)
    {
        this.level = level;
        damage = dmg;
        radius = rad;
        dotDps = dps;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Explode();
            Destroy(gameObject);
        }
    }

    void Explode()
    {
        switch (level)
        {
            case 1:
                EffectManager.Instance.PlayEffect<GameObject>(EEffectType.PoisonFX, new Vector3(transform.position.x, 1,transform.position.z), Quaternion.identity);
                break;
            case 2:
                EffectManager.Instance.PlayEffect<GameObject>(EEffectType.PoisonFX_2, new Vector3(transform.position.x, 1,transform.position.z), Quaternion.identity);
                break;
            case 3:
                EffectManager.Instance.PlayEffect<GameObject>(EEffectType.PoisonFX_3, new Vector3(transform.position.x, 1,transform.position.z), Quaternion.identity);
                break;
            case 4:
                EffectManager.Instance.PlayEffect<GameObject>(EEffectType.PoisonFX_4, new Vector3(transform.position.x, 1,transform.position.z), Quaternion.identity);
                break;
            default:
                EffectManager.Instance.PlayEffect<GameObject>(EEffectType.PoisonFX_5, new Vector3(transform.position.x, 1,transform.position.z), Quaternion.identity);
                break;
        }
        
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("Enemy"));
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy is BoxCollider)
                continue;

            enemy.GetComponent<Enemy>().TakeDamage(damage);
            enemy.GetComponent<Enemy>().ApplyPoison(dotDps, 5f); // 5 giây DOT
        }
    }
}