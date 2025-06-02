using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BackpackSlot : MonoBehaviour, IDropHandler
{
    public SkillIcon currentSkill = null;
    [SerializeField] public Image highlightBG;

    [HideInInspector] public Color green = new Color(0f, 1f, 0f, 0.5f);
    private Color red = new Color(1f, 0f, 0f, 0.5f);

    private void Start()
    {
        ClearHighlight();
    }

    public void SetHighlight(bool isGreen)
    {
        if (isGreen) highlightBG.color = green;
        else highlightBG.color = red;

        highlightBG.enabled = true;
    }

    public void ClearHighlight()
    {
        if (highlightBG != null) highlightBG.enabled = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
    }
}