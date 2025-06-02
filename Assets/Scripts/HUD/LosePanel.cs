using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LosePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtStage;

    public void Init()
    {
        gameObject.SetActive( true );
        txtStage.text = "Stage " + GameManager.Instance.CurrentStage;
    }

    public void OnbtnRetryClick()
    {
        GameManager.Instance.NewGame();
        gameObject.SetActive(false);
    }
}
