using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingItem : MonoBehaviour
{
    public enum ItemType { Apple, Watermelon, Beets }
    public ItemType type;
    private int[] healAmounts = { 10, 25, 15 };

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            // other.GetComponent<PlayerHealth>().Heal(healAmounts[(int)type]);
            // Destroy(gameObject);
        }
    }
}
