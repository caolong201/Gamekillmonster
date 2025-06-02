using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtStage;

    public void Init()
    {
        gameObject.SetActive( true );
        txtStage.text = "Stage " + GameManager.Instance.CurrentStage;
       
    }

    public void OnbtnNewGameClick()
    {
        GameManager.Instance.NextStage();
        GameManager.Instance.NewGame();
        gameObject.SetActive(false);
    }
}
