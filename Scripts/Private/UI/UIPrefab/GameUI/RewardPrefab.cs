using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;

public class RewardPrefab : MonoBehaviour
{
    private const int MaxSlotItemGridXCount = 5;

    [Header("Component")]
    [SerializeField] GameObject[] RewardStars;
    [SerializeField] RectTransform SlotItemGrid;
    [SerializeField] Text TitleText;
    [SerializeField] Text RewardText;


    [Header("Prefab")]
    [SerializeField] SlotItemPrefab SlotItemPrefab;

    public void InitRewardPrefab(string titleText, string rewardText, List<SlotItemInfo> slotItemInfoList, uint startCount)
    {
        GetStar(startCount);
        TitleText.text = titleText;
        RewardText.text = rewardText;

        // Initiate SlotItem
        List<SlotItemPrefab> rewardItem = new List<SlotItemPrefab>();
        foreach (var slotItemInfo in slotItemInfoList)
        {
            var slotItem = Instantiate(SlotItemPrefab, SlotItemGrid.transform);
            slotItem.InitInventoryItem(slotItemInfo);
            slotItem.GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);
            rewardItem.Add(slotItem);
        }

        // Change SlotItem Grid size
        int slotItemCount = rewardItem.Count;
        Vector2 maxSlotItemGridCount = new Vector2(slotItemCount, 1);
        while (slotItemCount > MaxSlotItemGridXCount)
        {
            maxSlotItemGridCount.x = MaxSlotItemGridXCount;
            maxSlotItemGridCount.y += 1;
            slotItemCount -= MaxSlotItemGridXCount;
        }
        var slotSize = SlotItemPrefab.GetComponent<RectTransform>().sizeDelta;
        SlotItemGrid.sizeDelta = new Vector2(maxSlotItemGridCount.x * slotSize.x, maxSlotItemGridCount.y * slotSize.y);

        // SlotItem Animation
        StartCoroutine(ShowSlotItemAnimation(rewardItem));
    }
    private IEnumerator ShowSlotItemAnimation(List<SlotItemPrefab> slotItem)
    {
        foreach (var item in slotItem)
        {
            item.transform.localScale = Vector3.zero;
        }

        foreach (var item in slotItem)
        {
            item.transform.DOScale(Vector3.one, 1).SetEase(Ease.OutBounce);
            yield return new WaitForSeconds(0.25f);
        }
    }
    private void GetStar(uint starCount)
    {
        if (starCount > 3)
        {
            Debug.LogWarningFormat("Get Star Function failed star Count > 3");
            return;
        }

        foreach (var star in RewardStars)
        {
            star.SetActive(false);
        }

        for (int i = 0; i < starCount; i++)
        {
            RewardStars[i].SetActive(true);
        }
    }

}
