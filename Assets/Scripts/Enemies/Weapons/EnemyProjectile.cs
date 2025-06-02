using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 3;
    public float lifetime = 3f;

    void Start() {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            //other.GetComponent<PlayerHealth>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
