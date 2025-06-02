using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum PlayerWeapon
{
    Sword = 0,
    Bow,
    Poison,
    Shield,
    Apple,
    Watermelon,
    Beet
}

public class PlayerController : MonoBehaviour
{
    public delegate void OnHPChanged(float current, float max);

    public event OnHPChanged HPChanged;

    [SerializeField] Slider sdPlayerHP;
    [SerializeField] private Image hpBarImage;

    public VariableJoystick joystick; // Reference to the joystick
    [SerializeField] private Transform player;
    public float moveSpeed = 5f; // Movement speed
    public float rotationSpeed = 10f; // Rotation speed
    private Rigidbody rb;

    [SerializeField] Animator animator;

    [SerializeField] private Renderer meshRenderer;

    public int MaxHP = 100;
    public float HP = 100;

    [SerializeField] Sword swordWeapon;
    [SerializeField] Bow bowWeapon;
    [SerializeField] PoisonBottle poisonBottle;
    [SerializeField] Shield shieldWeapon;
    [SerializeField] Transform shieldLeftPosition, shieldRightPosition;


    private Vector3 direction;

    private bool isDead = false;
    private List<BackpackSystem.SkillData> _activeSkills;
    [SerializeField] private BackpackSystem.SkillData currentSkill = null;

    private float TimeCheckAutoSwitchWeapon = 1;
    private float DamageReduction = 0f;
    private string currentAnimation = string.Empty;
    private bool isAttacking = false;

    [SerializeField] BoxCollider spawnZone;

    private void Awake()
    {
        SpawnPlayerInZone();
    }

    void Start()
    {
        GameManager.Instance.GetHUD().GetBackpackSystem().ActiveSkillsChanged -= ActiveSkillsChanged;
        GameManager.Instance.GetHUD().GetBackpackSystem().ActiveSkillsChanged += ActiveSkillsChanged;

        HP = MaxHP;
        isDead = false;
        if (animator == null) animator = GetComponent<Animator>();
        rb = player.GetComponent<Rigidbody>();

        player.GetComponent<PlayerAttackEvent>().Init(swordWeapon);
        player.GetComponent<PlayerEndAttackEvent>().Init(this);

        PlayerOnHPChanged(HP, MaxHP);

        DOVirtual.DelayedCall(0.1f, () => { HPChanged?.Invoke(HP, MaxHP); });
    }

    void SpawnPlayerInZone()
    {
        Bounds bounds = spawnZone.bounds;
        player.transform.position = new Vector3(Random.Range(bounds.min.x, bounds.max.x), 0.01f,
            Random.Range(bounds.min.z, bounds.max.z));
        
        spawnZone.enabled = false;
    }

    private void PlayerOnHPChanged(float playerHealth, float maxHealth)
    {
        sdPlayerHP.value = playerHealth / maxHealth;
        hpBarImage.color = Color.Lerp(Color.red, Color.green, sdPlayerHP.value);
    }

    private void ActiveSkillsChanged(List<BackpackSystem.SkillData> skills)
    {
        _activeSkills = skills;
        ResetWeapons();

        int plusHP = 0;
        for (int i = 0; i < _activeSkills.Count; i++)
        {
            switch ((PlayerWeapon)_activeSkills[i].skillId)
            {
                case PlayerWeapon.Sword:
                    currentSkill = _activeSkills[i];
                    swordWeapon.gameObject.SetActive(true);
                    swordWeapon.Init(currentSkill);
                    SetupShieldPos(true);
                    
                    break;
                case PlayerWeapon.Shield:
                    shieldWeapon.gameObject.SetActive(true);
                    shieldWeapon.Init(_activeSkills[i]);
                    DamageReduction = _activeSkills[i].attackPower;
                    break;
                case PlayerWeapon.Bow:
                    bowWeapon.gameObject.SetActive(true);
                    bowWeapon.Init(player, _activeSkills[i]);
                    break;
                case PlayerWeapon.Poison:
                    poisonBottle.Init(animator, _activeSkills[i]);
                    break;
                case PlayerWeapon.Apple:
                case PlayerWeapon.Watermelon:
                case PlayerWeapon.Beet:
                    plusHP += _activeSkills[i].attackPower;
                    break;
            }
        }

        if (currentSkill == null)
        {
            currentSkill = _activeSkills[0];
            swordWeapon.gameObject.SetActive(true);
            swordWeapon.Init(currentSkill);
            SetupShieldPos(true);
        }

        if (plusHP > 0)
        {
            DOVirtual.DelayedCall(0.2f, () =>
            {
                HP += plusHP;
                HP = Mathf.Clamp(HP, 0, MaxHP);
                HPChanged?.Invoke(HP, MaxHP);
                PlayerOnHPChanged(HP, MaxHP);
                var eff = EffectManager.Instance.PlayEffect<Transform>(EEffectType.PlusHPFX, transform.position,
                    Quaternion.identity);
                eff.SetParent(player);
                eff.localPosition = new Vector3(0, 1, 0);
            });
        }
    }

    private void ResetWeapons()
    {
        currentSkill = null;
        shieldWeapon.gameObject.SetActive(false);
        DamageReduction = 0;
        swordWeapon.gameObject.SetActive(false);
        bowWeapon.gameObject.SetActive(false);
        poisonBottle.Stop();
    }

    void FixedUpdate()
    {
        if (isDead || currentSkill == null) return;

        direction = new Vector3(joystick.Horizontal, 0f, joystick.Vertical).normalized;
        
        switch ((PlayerWeapon)currentSkill.skillId)
        {
            case PlayerWeapon.Sword:
            {
                if (swordWeapon.isCanAttack)
                {
                    isAttacking = true;
                    swordWeapon.SwordAttack();
                    if (direction.magnitude >= 0.01f)
                    {
                        SwitchAnimation("SwordRunAttack");
                    }
                    else
                    {
                        SwitchAnimation("SwordIdleAttack");
                    }
                }
            }
                break;
        }

        if (bowWeapon.isCanAttack)
        {
            bowWeapon.BowAttack();
        }

        if (!isAttacking)
        {
            if (direction.magnitude >= 0.01f)
            {
                SwitchAnimation("Run");
            }
            else
            {
                SwitchAnimation("Idle");
            }
        }

        MovePlayer();
    }

    void MovePlayer()
    {
        if (joystick == null) return;


        // Move player
        if (direction.magnitude >= 0.01f)
        {
            rb.velocity = direction * moveSpeed + new Vector3(0, rb.velocity.y, 0);
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            player.rotation = Quaternion.Slerp(player.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0); // Stop moving when no input
        }
    }

    private void OnDead()
    {
        SwitchAnimation("Die");
        isDead = true;
        rb.velocity = new Vector3(0, rb.velocity.y, 0); // Stop
        
        GameManager.Instance.EndGame(false);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        float takeDamge = damage - Mathf.Abs(DamageReduction / 100f * damage);

        HP -= takeDamge;
        if (HP <= 0)
        {
            HP = 0;
            OnDead();
        }

        FlashHit();

        HP = Mathf.Clamp(HP, 0, MaxHP);
        HPChanged?.Invoke(HP, MaxHP);
        PlayerOnHPChanged(HP, MaxHP);
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

    private void SetupShieldPos(bool isLeft)
    {
        if (isLeft) shieldWeapon.transform.SetParent(shieldLeftPosition);
        else shieldWeapon.transform.SetParent(shieldRightPosition);

        shieldWeapon.transform.localEulerAngles = Vector3.zero;
        shieldWeapon.transform.localPosition = Vector3.zero;
    }

    private void SwitchAnimation(string animName, bool force = false)
    {
        if (!force && currentAnimation == animName) return;

        animator.Play(animName);
        currentAnimation = animName;
    }

    public void PlayerEndAttackEvent()
    {
        isAttacking = false;
    }
}