using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemTable : TableBase
{
    private const string s_ItemID = "ItemID";
    private const string s_ItemName = "ItemName";
    private const string s_ItemPrice = "ItemPrice";

    private Dictionary<int, SlotItemInfo> itemIdShopItemInfoTable =
        new Dictionary<int, SlotItemInfo>();


    public SlotItemInfo GetShopItemInfo(int itemID)
    {
        itemIdShopItemInfoTable.TryGetValue(itemID, out SlotItemInfo shopItemInfo);
        return shopItemInfo;
    }

    protected override void OnRowParsed(List<object> rowContent)
    {
        int itemID = rowContent[GetColumnNameIndex(s_ItemID)] as ValueTypeWrapper<int>;
        string itemName = rowContent[GetColumnNameIndex(s_ItemName)] as ValueTypeWrapper<string>;
        string itemPrice = rowContent[GetColumnNameIndex(s_ItemPrice)] as ValueTypeWrapper<string>;

        SlotItemInfo shopItemInfo = new SlotItemInfo()
        {
            ItemID = itemID,
            ItemName = itemName,
            ItemPrice = itemPrice,
        };

        if (!itemIdShopItemInfoTable.ContainsKey(itemID))
        {
            itemIdShopItemInfoTable.Add(itemID, shopItemInfo);
        }
        else
        {
            itemIdShopItemInfoTable[itemID] = shopItemInfo;
            Debug.LogWarningFormat("itemIdShopItemInfoTable has have shopItemInfo for {0} name", itemName);
        }
    }

    protected override void OnTableParsed()
    {

    }
}
