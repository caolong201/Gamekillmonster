using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDGamePlay : MonoBehaviour
{
    [SerializeField] PlayerController player;
    
    [SerializeField] UITimer timer;
    public UITimer GetTimer() => timer;
    
    [SerializeField] Slider sdPlayerEXP;
    [SerializeField] TextMeshProUGUI txtEXP, txtStage;

    [SerializeField] BackpackSystem backpackSystem;
    public BackpackSystem GetBackpackSystem() => backpackSystem;
    
    [SerializeField] WinPanel winPanel;
    [SerializeField] LosePanel losePanel;
    private void Start()
    {
        timer.ResetTimer();
        timer.StartTimer();
        SetStage("Stage " + GameManager.Instance.CurrentStage);
        PlayerOnEXPChanged(0,10);
    }

    public void PlayerOnEXPChanged(float current, float maxEXP)
    {
        sdPlayerEXP.value = current / maxEXP;
        txtEXP.text = ((int)current) + "/" + maxEXP;
    }

    public void SetStage(string stage)
    {
        txtStage.text = stage;
    }

    public void ShowWinPanel()
    {
        Time.timeScale = 1;
        winPanel.Init();
    }
    public void ShowLosePanel()
    {
        Time.timeScale = 1;
        losePanel.Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            backpackSystem.ShowBackpack(true);
            timer.PauseTimer(true);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            backpackSystem.ShowBackpack(false);
            timer.PauseTimer(false);
        }
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            ShowWinPanel();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            ShowLosePanel();
        }
    }
}