using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public int damage = 2;
    public float lifetime = 3f;
    private Transform target = null;
    private Vector3 direction;
    public float speed = 10f;
    private Vector3 startPosition;
    private float maxDistance;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void SetDirection(Vector3 dir, Vector3 startPos, float radius)
    {
        direction = dir;
        startPosition = startPos;
        maxDistance = radius;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }
   

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().TakeDamage(damage);
        }
    }
}