using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonMonoAwake<UIManager>
{
    [SerializeField] Image blackScreen;
    [SerializeField] private GameObject winPanel, losePanel;

    public Tutorial tutorial;

    public override void OnAwake()
    {
        base.OnAwake();
        Close();
    }

    public void ShowEndGame(bool isWin)
    {
        winPanel.SetActive(isWin);
        losePanel.SetActive(!isWin);

        ShowBlackScreen();
        if (isWin)
        {
            winPanel.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), .2f);
        }
        else
        {
            losePanel.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), .2f);
        }
    }

    public void ShowBlackScreen()
    {
        blackScreen.DOFade(0.8f, 0.2f);
        blackScreen.raycastTarget = true;
    }

    public void Close()
    {
        blackScreen.DOFade(0, 0f);
        blackScreen.raycastTarget = false;
    }
}