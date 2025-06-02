using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    public enum ItemState
    {
        Idle,
        Collecting,
        Collected,
    }
    private Transform player;
    private float distanceCollect;
    private float timer = 0.2f;
    private float moveSpeed = 20f;
    
    ItemState state = ItemState.Idle;
    public void Init(Transform player, float distanceCollect)
    {
        this.player = player;
        this.distanceCollect = distanceCollect;
        state = ItemState.Idle;
    }

    private void Update()
    {
        if(player == null) return;

        switch (state)
        {
            case ItemState.Idle:
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    if (Vector3.Distance(player.position, transform.position) < distanceCollect)
                    {
                        state = ItemState.Collecting;
                    }
                    timer =  0.2f;
                }
            }
                break;
            case ItemState.Collecting:
            {
                transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
                if (Vector3.Distance(player.position, transform.position) < 0.8f)
                {
                    state = ItemState.Collected;
                }
            }
                break;
            case ItemState.Collected:
            {
                GameManager.Instance.IncreasePlayerEXP(1);
                EffectManager.Instance.DeactivateEffect(gameObject);
                player = null;
            }
                break;
        }
        
    }
}
