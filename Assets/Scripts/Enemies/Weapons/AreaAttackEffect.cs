using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAttackEffect : MonoBehaviour
{
    public int damage = 10;
    public float radius = 5f;
    public float duration = 2f;

    void Start() {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hit in hits) {
            // if (hit.CompareTag("Player")) {
            //     hit.GetComponent<PlayerHealth>().TakeDamage(damage);
            //}
        }
        Destroy(gameObject, duration);
    }
}
