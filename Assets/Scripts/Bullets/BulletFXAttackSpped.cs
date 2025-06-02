using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class BulletFXAttackSpped : MonoBehaviour
{
    public float speed = 20f; // Speed of bullet movement
    public float maxDistance = 50f; // Maximum distance
    private Vector3 startPosition;

    private void OnEnable()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Destroy bullet after it travels 50m
        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            gameObject.SetActive(false);
        }
    }
}