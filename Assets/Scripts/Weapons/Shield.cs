using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public int level = 1;
    
    private GameObject currentSkin;
    private int maxSkin = 5;
    private int skinID = 0;

    public void Init(BackpackSystem.SkillData data)
    {
        level = data.level;

        int newSkinID = level;
        if (newSkinID > maxSkin)
        {
            newSkinID = maxSkin;
        }

        if (newSkinID == skinID && currentSkin != null)
        {
            currentSkin.SetActive(true);
        }
        else
        {
            skinID = newSkinID;
            //destroy old skin
            if (currentSkin != null) Destroy(currentSkin);
            //load new skin
            GameObject bowPrefab = Resources.Load<GameObject>("PlayerWeapons/Shields/Shield_" + skinID);
            if (bowPrefab != null)
            {
                currentSkin = Instantiate(bowPrefab, transform);
            }
        }
    }
}