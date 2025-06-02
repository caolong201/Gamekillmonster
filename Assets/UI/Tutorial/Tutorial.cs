using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public delegate void TutorialChangeHandler(ETutStep step);

    // Event using the delegate
    public event TutorialChangeHandler OnTutorialChange;

    public enum ETutStep
    {
        None,
        Step1,
        Step2,
        Done
    }

    private float timer = 3;

    public ETutStep tutorialStep = ETutStep.None;
    [SerializeField] private List<GameObject> tutorialObjs;

    private void Start()
    {
        if (!GameManager.Instance.IsTutorial)
        {
            gameObject.SetActive(false);
        }
        else
        {
            SwitchStep(ETutStep.Step1);
        }
    }

    public void SwitchStep(ETutStep step)
    {
        foreach (var obj in tutorialObjs)
        {
            if (obj != null) obj.gameObject.SetActive(false);
        }

        if (step == ETutStep.Done)
        {
            OnTutorialCompleted();
        }
        else
        {
            tutorialObjs[((int)step) - 1].gameObject.SetActive(true);
        }

        tutorialStep = step;
        OnTutorialChange?.Invoke(step);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.IsTutorial && Input.GetMouseButtonDown(0))
        {
            SwitchStep(ETutStep.Step2);
        }

        if (GameManager.Instance.IsTutorial && tutorialStep == ETutStep.Step2)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                SwitchStep(ETutStep.Done);
                timer = 10;
            }
        }
    }

    public void OnTutorialCompleted()
    {
        PlayerPrefs.SetInt("kTutorial", 0);
        GameManager.Instance.IsTutorial = false;
        gameObject.SetActive(false);
    }
}