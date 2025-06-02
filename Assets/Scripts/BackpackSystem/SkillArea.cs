using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillArea : MonoBehaviour, IDropHandler
{
    

    public void OnDrop(PointerEventData eventData)
    {
        Debug.LogError("SkillArea OnDrop");
    }
}
