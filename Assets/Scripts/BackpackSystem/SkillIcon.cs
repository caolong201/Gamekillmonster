using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private List<GameObject> levelObjs;
    public BackpackSystem.SkillData skillData;
    private CanvasGroup canvasGroup;
    public Transform originalParent;
    private BackpackSlot[,] grid;
    public RectTransform gridPanel;
    public List<BackpackSlot> occupiedSlots = new(); // slot đang chiếm
    private List<BackpackSlot> currentHighlightedSlots = new();
    private BackpackSystem parent;
    private bool IsInvalid = false;
    [SerializeField] public bool IsCanMerge = true;

    public void Initialize(BackpackSystem parent, BackpackSlot[,] grid, RectTransform gridPanel)
    {
        this.parent = parent;
        this.grid = grid;
        this.gridPanel = gridPanel;
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

        foreach (var obj in levelObjs)
        {
            if (obj == null) continue;
            obj.SetActive(false);
        }

        levelObjs[skillData.level - 1].SetActive(true);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        originalParent = transform.parent;
        transform.SetParent(transform.root);

        // clear occupied
        foreach (var slot in occupiedSlots)
        {
            slot.currentSkill = null;
        }

        occupiedSlots.Clear();
    }

    public void OnDrag(PointerEventData eventData)
    {
        IsInvalid = false;
        transform.position = Input.mousePosition;

        // Tìm xem vị trí chuột đang hover lên slot nào
        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(gridPanel, Input.mousePosition,
                eventData.pressEventCamera, out localPoint))
            return;

        int col = Mathf.FloorToInt((localPoint.x + gridPanel.rect.width / 2) /
                                   (gridPanel.rect.width / grid.GetLength(1)));
        int row = grid.GetLength(0) - 1 -
                  Mathf.FloorToInt((localPoint.y + gridPanel.rect.height / 2) /
                                   (gridPanel.rect.height / grid.GetLength(0)));

        ClearCurrentHighlights();

        if (CanPlaceSkillAt(row, col, out List<BackpackSlot> validSlots))
        {
            foreach (var slot in validSlots)
            {
                slot.SetHighlight(true);
            }

            currentHighlightedSlots = validSlots;
        }
        else
        {
            if (IsValidIndex(row, col))
            {
                grid[row, col].SetHighlight(false);
                currentHighlightedSlots.Add(grid[row, col]);
                IsInvalid = true;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsInvalid)
        {
            canvasGroup.blocksRaycasts = true;
            parent.SkillBackToArea(transform);
            return;
        }

        bool isUpLevel = false;
        if (currentHighlightedSlots.Count > 0 && currentHighlightedSlots.All(s => s.highlightBG.color == s.green))
        {
            // Đặt skill vào slot mới
            foreach (var slot in currentHighlightedSlots)
            {
                if (slot.currentSkill != null)
                {
                    if (slot.currentSkill.skillData.skillId == skillData.skillId &&
                        slot.currentSkill.skillData.level == skillData.level)
                    {
                        slot.currentSkill.LevelUpdate();
                        isUpLevel = true;
                    }
                }
                else
                {
                    slot.currentSkill = this;
                    transform.SetParent(currentHighlightedSlots[0].transform);
                    transform.position = currentHighlightedSlots[0].transform.position +
                                         new Vector3((skillData.size - 1) * 90, 0, 0);
                }

                occupiedSlots.Add(slot);
                parent.UpdateWeaponPower();
            }
        }
        else
        {
            // Quay lại vị trí ban đầu
            transform.SetParent(originalParent);
            transform.position = originalParent.position + new Vector3((skillData.size - 1) * 90, 0, 0);
        }

        canvasGroup.blocksRaycasts = true;
        ClearCurrentHighlights();

        if (isUpLevel) Destroy(gameObject);

        parent.ClearAllHighlight();
    }

    private void ClearCurrentHighlights()
    {
        foreach (var slot in currentHighlightedSlots)
        {
            slot.ClearHighlight();
        }

        currentHighlightedSlots.Clear();
    }

    private bool CanPlaceSkillAt(int row, int col, out List<BackpackSlot> result)
    {
        result = new();

        if (!IsValidIndex(row, col))
        {
            IsInvalid = true;
            return false;
        }

        int size = skillData.size;
        // if (col + size > grid.GetLength(1)) return false;

        for (int i = 0; i < size; i++)
        {
            if (size > 1 && col + i >= grid.GetLength(1))
            {
                BackpackSlot slotPrev = grid[row, col + (i - 1)];
                if (slotPrev.currentSkill == null)
                {
                    return false;
                }
                else
                {
                    if (slotPrev.currentSkill.IsCanMerge && slotPrev.currentSkill.skillData.skillId == skillData.skillId &&
                        slotPrev.currentSkill.skillData.level == skillData.level)
                    {
                        //can merge
                        result.Add(slotPrev);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                BackpackSlot slot = grid[row, col + i];
                if (slot.currentSkill != null)
                {
                    if (slot.currentSkill.IsCanMerge && slot.currentSkill.skillData.skillId == skillData.skillId &&
                        slot.currentSkill.skillData.level == skillData.level)
                    {
                        //can merge
                    }
                    else
                    {
                        return false;
                    }
                }
                result.Add(slot);
            }
        }

        return true;
    }

    private bool IsValidIndex(int row, int col)
    {
        return row >= 0 && row < grid.GetLength(0) && col >= 0 && col < grid.GetLength(1);
    }

    private void LevelUpdate()
    {
        int index = skillData.level;
        index++;
        if (index > levelObjs.Count)
        {
            index = levelObjs.Count - 1;
        }

        for (int i = 0; i < levelObjs.Count; i++)
        {
            levelObjs[i].SetActive(false);
        }

        levelObjs[index - 1].SetActive(true);
        levelObjs[index - 1].transform.localScale = Vector3.zero;
        levelObjs[index - 1].transform.DOScale(Vector3.one, 0.7f).SetEase(Ease.OutBounce)
            .SetUpdate(UpdateType.Normal, true);

        skillData.level += 1;
    }
}