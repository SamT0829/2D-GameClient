using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotItemPrefab : MonoBehaviour
{
    public SlotItemInfo ItemInfo;

    [Header("Component")]
    [SerializeField] private Image ItemBackgroundImage;
    [SerializeField] private Image ItemImage;
    [SerializeField] private Text ItemAmount;

    public void InitInventoryItem(SlotItemInfo slotItemInfo)
    {
        string path = "ShopItem/" + slotItemInfo.ItemName;
        Sprite sp = Resources.Load<Sprite>(path);

        ItemInfo = slotItemInfo;
        ItemImage.sprite = sp;
        ItemAmount.text = slotItemInfo.Amount.ToString();
    }
}
