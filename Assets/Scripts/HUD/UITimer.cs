using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float elapsedTime = 0f;
    private bool timerActive = false;

    private StageConfig stageConfig;

    private void Start()
    {
        stageConfig = GameManager.Instance.GetStageConfig();
    }

    public void StartTimer()
    {
        timerActive = true;
    }
    
    public void StopTimer()
    {
        timerActive = false;
    }

    public void PauseTimer(bool isPause)
    {
        timerActive = !isPause;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        timerActive = false;
        timerText.text = "00:00";
    }

    void Update()
    {
        if(!timerActive || GameManager.Instance.IsTutorial) return;
        
        elapsedTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        if ( elapsedTime >= stageConfig.winTimer * 60 && stageConfig != null && !GameManager.Instance.IsTutorial)
        {
          GameManager.Instance.EndGame(true);
          StopTimer();
        }
    }
}