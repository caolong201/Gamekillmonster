using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum EGameStage
{
    Prepare,
    Live,
    Dead,
    Win
}


public class GameManager : SingletonMonoAwake<GameManager>
{
    public delegate void OnStageChanged(EGameStage stage);

    public event OnStageChanged onStageChanged;

    [SerializeField] private GameConfig gameConfig;
    public GameConfig GameConfig => gameConfig;

    private EGameStage gameStage = EGameStage.Prepare;
    public EGameStage GameStage => gameStage;

    [SerializeField] HUDGamePlay hud;

    public int CurrentStage = 1;

    public int PlayerEXP = 0;
    private int conditionUnlockSkill = 10;
    private int wave = 0;

    public bool IsTutorial = true;
    private bool isBackpackShowing = false;
    public override void OnAwake()
    {
        DOTween.defaultTimeScaleIndependent = true;
        base.OnAwake();
        Application.targetFrameRate = 60;

        ChangeStage(EGameStage.Live);
        IsTutorial = PlayerPrefs.GetInt("kTutorial", 1) == 1 ? true : false;
        IsTutorial = false;
        CurrentStage = PlayerPrefs.GetInt("kCurrentStage", 1);
        CurrentStage = 1;
        PlayerEXP = 0;
        wave = 0;
        conditionUnlockSkill = 10;
        GetHUD().PlayerOnEXPChanged(PlayerEXP, conditionUnlockSkill);
    }

    public void NextStage()
    {
        CurrentStage++;
        PlayerPrefs.SetInt("kCurrentStage", CurrentStage);
    }

    public HUDGamePlay GetHUD()
    {
        if (hud == null)
        {
            hud = FindObjectOfType<HUDGamePlay>();
        }

        return hud;
    }

    public void ChangeStage(EGameStage stage)
    {
        if (stage != gameStage)
        {
            gameStage = stage;
            onStageChanged?.Invoke(stage);
        }
    }

    public StageConfig GetStageConfig()
    {
        if (CurrentStage <= gameConfig.stages.Count)
        {
            return gameConfig.stages[CurrentStage - 1];
        }

        int rand = UnityEngine.Random.Range(1, gameConfig.stages.Count);
        return gameConfig.stages[rand];
    }

    public void NewGame()
    {
        PlayerEXP = 0;
        wave = 0;
        conditionUnlockSkill = 10;

        EffectManager.Instance.ClearEffectPool();

        ScreenFader.Instance.FadeIn(() =>
        {
            SceneManager.LoadScene("Game");
            EnemiesManager.Instance.NewGame();
            ChangeStage(EGameStage.Live);
            GetHUD().PlayerOnEXPChanged(PlayerEXP, conditionUnlockSkill);
            ScreenFader.Instance.FadeOut();
        });
    }

    public void IncreasePlayerEXP(int amount, bool isReset = false)
    {
        return;
        if (isReset)
        {
            isBackpackShowing = false;
            PlayerEXP = 0;
            conditionUnlockSkill += 5;
            GetHUD().PlayerOnEXPChanged(PlayerEXP, conditionUnlockSkill);
            return;
        }
        
        if(isBackpackShowing) return;

        PlayerEXP += amount;
        if (PlayerEXP >= conditionUnlockSkill)
        {
            isBackpackShowing = true;
            GetHUD().GetBackpackSystem().ShowBackpack(true);
            GetHUD().PlayerOnEXPChanged(conditionUnlockSkill, conditionUnlockSkill);
           
        }
        else
        {
            GetHUD().PlayerOnEXPChanged(PlayerEXP, conditionUnlockSkill);
        }
    }

    public void EndGame(bool isWin)
    {
        Debug.Log("EndGame: " + isWin);
        if (isWin) GetHUD().ShowWinPanel();
        else
        {
            DOVirtual.DelayedCall(1, () => GetHUD().ShowLosePanel());
        }

        ChangeStage(isWin ? EGameStage.Win : EGameStage.Dead);
    }
}