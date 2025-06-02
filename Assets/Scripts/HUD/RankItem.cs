using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankingData
{
    public int id;
    public string rankName;
    public int rank;
    public int kill;
    public Color color = new Color(1f, 1f, 1f, 128f/255f);
}

public class RankItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtRank, txtName, txtKill;

    private List<RectTransform> listRankPos;
    private List<float> listRankWidthSize = new List<float>() { 700, 650, 600, 550 };
    private float rectHeight = 100;
    private RectTransform rect;
    public RankingData mData;
    public bool IsUpdated = false;

    public void Initialize(List<RectTransform> listRankPos, RankingData data)
    {
        mData = data;
        IsUpdated = false;
        this.listRankPos = listRankPos;
        rect = GetComponent<RectTransform>();
    }
   
    public void UpdateRank(RankingData data)
    {
        IsUpdated = true;
        RectTransform topRect = null;
        if (data.rank <= 4) //top 4 rank
        {
            //normal case
            topRect = listRankPos[data.rank - 1];
        }
        else
        {
            //get last
            topRect = listRankPos[listRankPos.Count - 1];
        }

        transform.SetParent(topRect);
        rect.DOAnchorPosY(0, .5f);
        txtRank.text = data.rank + "st";
        txtKill.text = data.kill.ToString();
        txtName.text = data.rankName;

        float widthSize = data.rank <= 4 ? listRankWidthSize[data.rank - 1] : listRankWidthSize[3];
        rect.DOSizeDelta(new Vector2(widthSize, rectHeight), .5f);
        GetComponent<Image>().color = data.color;
    }

}