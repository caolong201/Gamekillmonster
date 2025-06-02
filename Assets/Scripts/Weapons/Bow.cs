using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public int level = 1;
    public GameObject arrowPrefab;
    [SerializeField] List<Color> arrowColors;
    public float attackCooldown = 2f;
    private float nextAttackTime;
    public float attackRadius = 5f;
    public int attackPower = 2;

    public bool isCanAttack = false;
    private GameObject currentSkin;
    private int maxSkin = 5;
    private int skinID = 1;

    private Tween delayTween = null;
    private Transform player;

    public void Init(Transform player, BackpackSystem.SkillData data)
    {
        this.player = player;
        level = data.level;
        attackPower = data.attackPower;
        attackRadius = 4 + level;
        isCanAttack = false;

        skinID = level;
        if (skinID > maxSkin) skinID = maxSkin;
        
        arrowPrefab.GetComponent<Renderer>().sharedMaterial.color = arrowColors[skinID - 1];
    }

    private void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            isCanAttack = true;
        }
    }

    public void BowAttack()
    {
        Shoot();
        isCanAttack = false;
    }

    void Shoot()
    {
        
        int numOfArrows = level + 4;
        float angleStep = 360f / numOfArrows;
        float angle = 0f;

        for (int i = 0; i < numOfArrows; i++)
        {
            // Tính hướng dựa trên góc
            float dirX = Mathf.Cos(angle * Mathf.Deg2Rad);
            float dirZ = Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector3 direction = new Vector3(dirX, 0f, dirZ).normalized;

            // Tạo mũi tên tại vị trí player
            GameObject arrow = Instantiate(arrowPrefab, player.position, Quaternion.LookRotation(direction));
            Arrow arrowScript = arrow.GetComponent<Arrow>();
            arrowScript.damage = attackPower;
            arrowScript.SetDirection(direction, player.position, attackRadius);

            angle += angleStep;
        }
    }

    private void OnDisable()
    {
        isCanAttack = false;
    }
}