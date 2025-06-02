using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtStage;

    public void Init()
    {
        gameObject.SetActive(true);
        txtStage.text = "Stage " + GameManager.Instance.CurrentStage;
    }

    public void OnbtnNewGameClick()
    {
        GameManager.Instance.NextStage();
        GameManager.Instance.NewGame();
        gameObject.SetActive(false);
    }


    [SerializeField] private Transform hand;
    [SerializeField] private Image imgbutton;

    private void Start()
    {
        hand.gameObject.SetActive(false);
        DOVirtual.DelayedCall(.5f, () => { hand.gameObject.SetActive(true); });
        DOVirtual.DelayedCall(1f, () =>
        {
            hand.DOScale(Vector3.one * 3 * 0.8f, 0.3f).SetLoops(2, LoopType.Yoyo);
        });
        DOVirtual.DelayedCall(1.15f, () =>
        {
            imgbutton.color = Color.green;
        });
    }
}