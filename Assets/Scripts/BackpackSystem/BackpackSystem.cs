using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BackpackSystem : MonoBehaviour
{
    [System.Serializable]
    public class SkillData
    {
        public int skillId;
        public string skillName;
        public int level = 1;
        public int size = 1;
        public int attackPower = 0;
    }

    public delegate void OnActiveSkillsChanged(List<SkillData> skills);

    public event OnActiveSkillsChanged ActiveSkillsChanged;


    [SerializeField] private List<SkillData> _activeSkills;
    public List<SkillData> ActiveSkills => _activeSkills;

    public List<SkillIcon> skillPool;
    public RectTransform backpackPopup;
    public Transform skillSpawnArea;
    public BackpackSlot[] slots;
    private BackpackSlot[,] grid = new BackpackSlot[3, 3];
    public RectTransform gridPanel;

    [SerializeField] private List<WeaponIcon> weaponIcons;
    private bool isShowing = false;

    private void Awake()
    {
        backpackPopup.anchoredPosition = new Vector2(0, -558);

        _activeSkills = new List<SkillData>();

        // Gán vào ma trận 2D
        for (int i = 0; i < slots.Length; i++)
        {
            int row = i / 3;
            int col = i % 3;

            grid[row, col] = slots[i];
        }

        UpdateWeaponPower();
        DOVirtual.DelayedCall(0.2f, () => { ActiveSkillsChanged?.Invoke(_activeSkills); });
    }

    public void ShowBackpack(bool show)
    {
        if (GameManager.Instance.GameStage != EGameStage.Live)
        {
            return;
        }

        if (show && isShowing) return;
        isShowing = true;

        GameManager.Instance.GetHUD().GetTimer().PauseTimer(show);
        if (show)
        {
            UpdateWeaponPower();

            foreach (Transform child in skillSpawnArea)
            {
                Destroy(child.gameObject);
            }

            backpackPopup.DOAnchorPos(new Vector2(0, 750), .3f).SetEase(Ease.OutExpo).SetUpdate(UpdateType.Normal, true)
                .OnComplete(() => { SpawnRandomSkills(3); });

            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1.0f;
            ActiveSkillsChanged?.Invoke(_activeSkills);
            backpackPopup.DOAnchorPos(new Vector2(0, -558), .3f).SetEase(Ease.InExpo)
                .OnComplete(() =>
                {
                    GameManager.Instance.IncreasePlayerEXP(0, true);
                    isShowing = false;
                });
        }
    }

    void SpawnRandomSkills(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var skill = Instantiate(skillPool[GetRandomSkill()], skillSpawnArea);
            skill.Initialize(this, grid, gridPanel);
            skill.transform.localScale = Vector3.zero;
            skill.transform.DOScale(Vector3.one, 0.7f).SetEase(Ease.OutBounce).SetUpdate(UpdateType.Normal, true);
        }
    }

    private int GetRandomSkill()
    {
        int indexSkill = 0;
        switch (GameManager.Instance.CurrentStage)
        {
            case 1:
            {
                indexSkill = 0;
            }
                break;
            case 2:
            {
                int randomIndex = Random.Range(0, 100);
                if (randomIndex < 70) indexSkill = 0; //sword
                else if (randomIndex < 90) indexSkill = 2; //posion
                else
                {
                    indexSkill = Random.Range(4, 7); //fruits
                }
            }
                break;
            case 3:
            {
                int randomIndex = Random.Range(0, 100);
                if (randomIndex < 50) indexSkill = 1; //bow
                else
                {
                    indexSkill = Random.Range(0, 7); //others
                }
            }
                break;

            default:
            {
                indexSkill = Random.Range(0, 7); //others
            }
                break;
        }

        return indexSkill;
    }

    public void UpdateWeaponPower()
    {
        float swordPower = 3;
        float bowPower = 0;
        int poisonBottlePower = 0;
        int shieldPower = 0;
        int applePower = 0;
        int watermelonPower = 0;
        int beetPower = 0;

        SkillData sword = null;
        SkillData bow = null;
        SkillData poison = null;
        SkillData shield = null;
        SkillData apple = null;
        SkillData watermelon = null;
        SkillData beet = null;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].currentSkill != null)
            {
                switch (slots[i].currentSkill.skillData.skillId)
                {
                    case 0: //sword
                    {
                        if (slots[i].currentSkill.skillData.level * 3 > swordPower)
                        {
                            swordPower = slots[i].currentSkill.skillData.level * 3;
                            if (sword == null)
                            {
                                sword = slots[i].currentSkill.skillData;
                            }

                            sword.attackPower = (int)swordPower;
                        }
                    }
                        break;
                    case 1: //Bow
                    {
                        if ((slots[i].currentSkill.skillData.level * 2) > bowPower)
                        {
                            bowPower = (slots[i].currentSkill.skillData.level * 2);
                            if (bow == null)
                            {
                                bow = slots[i].currentSkill.skillData;
                            }

                            bow.attackPower = (int)bowPower;
                        }
                    }
                        break;
                    case 2: //Poison Bottle
                    {
                        int level = slots[i].currentSkill.skillData.level;
                        int power = 0;
                        if (level > 0)
                        {
                            switch (level)
                            {
                                case 1:
                                    power = 5;
                                    break;
                                case 2:
                                    power = 8;
                                    break;
                                case 3:
                                    power = 11;
                                    break;
                                default:
                                    power = (13 + 3 * (level - 3));
                                    break;
                            }

                            if (power > poisonBottlePower)
                            {
                                poisonBottlePower = power;
                                if (poison == null)
                                {
                                    poison = slots[i].currentSkill.skillData;
                                }

                                poison.attackPower = (int)poisonBottlePower;
                            }
                        }
                    }
                        break;
                    case 3: //Shield
                    {
                        if (slots[i].currentSkill.skillData.level * 10 > shieldPower)
                        {
                            shieldPower = slots[i].currentSkill.skillData.level * 10;
                            shieldPower = Mathf.Clamp(shieldPower, 0, 90);
                            if (shield == null)
                            {
                                shield = slots[i].currentSkill.skillData;
                            }

                            shield.attackPower = (int)shieldPower;
                        }
                    }
                        break;
                    case 4: //Fruits
                    {
                        applePower += 10;
                        if (apple == null)
                        {
                            apple = slots[i].currentSkill.skillData;
                        }

                        apple.attackPower = (int)applePower;
                    }
                        break;
                    case 5: //Fruits
                    {
                        watermelonPower += 25;
                        if (watermelon == null)
                        {
                            watermelon = slots[i].currentSkill.skillData;
                        }

                        watermelon.attackPower = (int)watermelonPower;
                    }
                        break;
                    case 6: //Fruits
                    {
                        beetPower += 15;
                        if (beet == null)
                        {
                            beet = slots[i].currentSkill.skillData;
                        }

                        beet.attackPower = (int)beetPower;
                    }
                        break;
                }
            }
        }

        weaponIcons[0].SetPower(applePower + watermelonPower + beetPower);
        weaponIcons[1].SetPower((int)bowPower + (int)swordPower + poisonBottlePower);
        weaponIcons[2].SetPower(shieldPower);

        _activeSkills.Clear();
        if (sword != null) _activeSkills.Add(sword);
        else
        {
            //default is sword
            _activeSkills.Add(new SkillData()
            {
                skillId = 0,
                skillName = "Sword",
                size = 2,
                attackPower = 3
            });
        }

        if (bow != null) _activeSkills.Add(bow);
        if (poison != null) _activeSkills.Add(poison);
        if (shield != null) _activeSkills.Add(shield);
        if (apple != null) _activeSkills.Add(apple);
        if (watermelon != null) _activeSkills.Add(watermelon);
        if (beet != null) _activeSkills.Add(beet);
    }

    public void SkillBackToArea(Transform skill)
    {
        skill.SetParent(skillSpawnArea);
        UpdateWeaponPower();
    }

    public void ClearAllHighlight()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].ClearHighlight();
        }
    }

    public void OnbtnBattleClicked()
    {
        ShowBackpack(false);
    }
}