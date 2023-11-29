using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemPrefab : MonoBehaviour
{
    public SlotItemInfo Info;

    [Header("Shop Prefab Component")]
    [SerializeField] private Image ItemImage;
    [SerializeField] private Text ItemNameText;
    [SerializeField] private Text ItemPriceText;
    [SerializeField] private Button ItemButton;

    public void InitShopItem(SlotItemInfo shopItemInfo, Action<ShopItemPrefab> clickItemAction)
    {
        Info = shopItemInfo;

        string path = "ShopItem/" + shopItemInfo.ItemName;
        Sprite sp = Resources.Load<Sprite>(path);

        ItemImage.sprite = sp;
        ItemNameText.text = shopItemInfo.ItemName;
        ItemPriceText.text = shopItemInfo.ItemPrice;

        ItemButton.onClick.AddListener(() =>
        {
            OnClickItemButton();
            clickItemAction.Invoke(this);
        });

    }

    private void OnClickItemButton()
    {
        // Debug.Log("OnClickItemButton");
    }
}
