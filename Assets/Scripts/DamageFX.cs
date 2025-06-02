using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = System.Random;

public class DamageFX : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtDamage;
    public float fadeDuration = 0.5f; // Duration for fading out the damage number

    public void PlayFX(int damage, Vector3 position)
    {
        txtDamage.DOFade(1, 0.01f);
        txtDamage.text = damage + "";
        // Animate the damage number
        AnimateDamage(position);
    }

    private void AnimateDamage(Vector3 targetMove)
    {
        transform.DOMove(targetMove, 0.2f)
            .SetEase(Ease.OutQuad);

        // Fade out the number using DOTween
        txtDamage.DOFade(0, fadeDuration).OnKill(() => { gameObject.SetActive(false); });

        // //crit
        // if (isCritical)
        // {
        //     transform.DOPunchScale(new Vector3(0.02f, 0.02f, 0.02f), 0.2f).SetEase(Ease.OutQuad);
        // }
    }
}