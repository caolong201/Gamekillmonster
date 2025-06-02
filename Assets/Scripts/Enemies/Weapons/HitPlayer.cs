using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPlayer : MonoBehaviour
{
    [SerializeField] private int attackPower = 3;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            var player = other.gameObject.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(attackPower);
                
                gameObject.SetActive(false);
            }
        }
    }
}
