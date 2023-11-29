using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotItemTable : TableBase
{
    private const string s_ItemID = "ItemID";
    private const string s_ItemName = "ItemName";
    private const string s_ItemPrice = "ItemPrice";

    private Dictionary<int, SlotItemInfo> itemIdShopItemInfoTable =
        new Dictionary<int, SlotItemInfo>();


    public SlotItemInfo GetSlotItemInfo(int itemID)
    {
        itemIdShopItemInfoTable.TryGetValue(itemID, out SlotItemInfo slotItemInfo);
        return slotItemInfo;
    }

    protected override void OnRowParsed(List<object> rowContent)
    {
        int itemID = rowContent[GetColumnNameIndex(s_ItemID)] as ValueTypeWrapper<int>;
        string itemName = rowContent[GetColumnNameIndex(s_ItemName)] as ValueTypeWrapper<string>;
        string itemPrice = rowContent[GetColumnNameIndex(s_ItemPrice)] as ValueTypeWrapper<string>;

        SlotItemInfo slotItemInfo = new SlotItemInfo()
        {
            ItemID = itemID,
            ItemName = itemName,
            ItemPrice = itemPrice,
        };

        if (!itemIdShopItemInfoTable.ContainsKey(itemID))
        {
            itemIdShopItemInfoTable.Add(itemID, slotItemInfo);
        }
        else
        {
            itemIdShopItemInfoTable[itemID] = slotItemInfo;
            Debug.LogWarningFormat("itemIdShopItemInfoTable has have shopItemInfo for {0} name", itemName);
        }
    }

    protected override void OnTableParsed()
    {

    }
}
