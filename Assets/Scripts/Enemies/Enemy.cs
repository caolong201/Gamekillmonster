using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public EnemyType type;
    [Header("Stats")] public int maxHealth = 10;
    public int attackPower = 2;
    public float attackSpeed = 1f;
    public float attackDistance = 1f;
    public float moveSpeed = 3f;
    [SerializeField] private int EXP = 1;
    [SerializeField] private Renderer meshRenderer;

    [SerializeField] protected int currentHealth;
    protected float nextAttackTime;
    protected Transform player;

    private float poisonTimer = 0f;
    private int currentPoisonDps = 0;

    protected Animator animator;
    private string currentAnimation = string.Empty;

    protected PlayerController playerController;
    public bool isDead = false;

    EGameStage gameStage = EGameStage.Live;
    private Tween delayTween = null;

    void Start()
    {
        GameManager.Instance.onStageChanged += OnStageChanged;
        animator = GetComponent<Animator>();
    }

    private void OnDestroy()
    {
        if (GameManager.IsInstanceValid()) GameManager.Instance.onStageChanged -= OnStageChanged;
    }

    private void OnStageChanged(EGameStage stage)
    {
        gameStage = stage;
    }

    public virtual void Init(Transform player)
    {
        isDead = false;
        currentHealth = maxHealth;
        poisonTimer = 0f;
        currentPoisonDps = 0;
        this.player = player;
        playerController = player.GetComponentInParent<PlayerController>();
    }

    protected virtual void Update()
    {
        if (isDead) return;

        if (gameStage != EGameStage.Live)
        {
            SwitchAnimation("Idle");
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= attackDistance)
        {
            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackSpeed;
            }
            else
            {
                if (currentAnimation == "Run") SwitchAnimation("Idle");
            }
        }
        else
        {
            Move();
        }

        transform.LookAt(player);
    }

    private void SwitchAnimation(string animName, bool force = false)
    {
        if (!force && currentAnimation == animName) return;

        animator.Play(animName);
        currentAnimation = animName;
    }

    protected virtual void Move()
    {
        SwitchAnimation("Run");
        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
        );
    }

    protected virtual void Attack()
    {
        SwitchAnimation("Attack", true);
    }

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
       
        FlashHit();
        EffectManager.Instance.PlayEffect<DamageFX>(EEffectType.DamageFX, transform.position, Quaternion.identity)
            .PlayFX(damage,
                new Vector3(transform.position.x + Random.Range(-0.2f, 0.2f), transform.position.y + 2,
                    transform.position.z + Random.Range(-0.2f, 0.2f)));
    }

    protected virtual void Die()
    {
        if (isDead) return;
        if (delayTween != null) delayTween.Kill();

        SwitchAnimation("Die");
        delayTween = DOVirtual.DelayedCall(1.1f, () =>
        {
            DropGem();
            //GameManager.Instance.IncreasePlayerEXP(EXP);
            EnemiesManager.Instance.RemoveEnemyDead(this);
        });

        isDead = true;
    }

    public void ApplyPoison(int dps, float duration)
    {
        currentPoisonDps = dps;
        poisonTimer = duration;

        if (!IsInvoking("PoisonDamage"))
        {
            InvokeRepeating("PoisonDamage", 1f, 1f); // Gây damage mỗi giây
        }
    }

    void PoisonDamage()
    {
        TakeDamage(currentPoisonDps);
        poisonTimer -= 1f;

        if (poisonTimer <= 0)
        {
            CancelInvoke("PoisonDamage");
        }
    }

    //Animation event
    public void MacgicEffect()
    {
       
        playerController.TakeDamage(attackPower);
    }

    private void FlashHit()
    {
        if (meshRenderer != null)
        {
            // Smoothly change to the flash color
            meshRenderer.material.DOColor(Color.red, 0.1f).OnComplete(() =>
            {
                // Smoothly revert to the original color
                meshRenderer.material.DOColor(Color.white, 0.1f);
            });
        }
    }

    private void DropGem()
    {
        for (int i = 0; i < EXP; i++)
        {
            ItemHandler item = EffectManager.Instance.LoadObject<ItemHandler>(EEffectType.ItemCollect,
                GetRandomSpawnPosition(), Quaternion.identity);
            item.Init(player, 2);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere.normalized;
        float randomDistance = Random.Range(0, 1);
        Vector3 spawnPos = transform.position + randomDirection * randomDistance;
        spawnPos.y = 0f;

        return spawnPos;
    }
}