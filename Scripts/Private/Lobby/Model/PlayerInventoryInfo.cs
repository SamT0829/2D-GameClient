using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public enum SlotItemAllName
{
    Gold,
    Sword,
    Apple,
    Candy,
}

public enum SlotItemType
{
    None,
    Item,
    Equip,
}

public enum ItemName
{
    None,
    Gold,
    Apple,
    Candy
}

public enum EquipName
{
    None,
    Sword,
    Weapon,
    Armor,
    Shoe,
}

public class SlotItemInfo
{
    public int ItemID;
    public SlotItemType SlotItemType;
    public SlotItemAllName SlotItemNameType;

    public int Amount;
    public int ItemIndex;
    public string ItemName;
    public string ItemPrice;

    public SlotItemInfo()
    {
        Amount = 1;
    }

    public void InitInventory(SlotItemInfo slotItemInfo)
    {
        //Amount = shopItemInfo.ItemAmount;
        SlotItemType = slotItemInfo.SlotItemType;
        SlotItemNameType = slotItemInfo.SlotItemNameType;
        ItemID = slotItemInfo.ItemID;
        ItemName = slotItemInfo.ItemName;
        ItemPrice = slotItemInfo.ItemPrice;
    }

    public List<object> CreateSerializeObject()
    {
        List<object> retv = new List<object>();
        retv.Add(ItemID);
        retv.Add(ItemIndex);
        retv.Add(ItemName);
        retv.Add(ItemPrice);

        return retv;
    }

    public void DeserializeObject(object[] retv)
    {
        ItemID = Convert.ToInt32(retv[0]);
        ItemIndex = Convert.ToInt32(retv[1]);
        ItemName = retv[2].ToString();
        ItemPrice = retv[3].ToString();
    }
}

public class PlayerInventoryInfo
{
    private Dictionary<ItemName, SlotItemInfo> itemInventoryTable =
        new Dictionary<ItemName, SlotItemInfo>();

    public Dictionary<ItemName, SlotItemInfo> PlayerItemInventoryTable => itemInventoryTable;

    private Dictionary<EquipName, SlotItemInfo> equipInventoryTable =
        new Dictionary<EquipName, SlotItemInfo>();

    public Dictionary<EquipName, SlotItemInfo> PlayerEquipInventoryTable => equipInventoryTable;

    public void AddInventroy(SlotItemType slotItemType, int inventoryType, SlotItemInfo inventoryInfo)
    {
        SlotItemInfo inventory;

        switch (slotItemType)
        {
            case SlotItemType.Item:
                if (!Enum.IsDefined(typeof(ItemName), inventoryType))
                {
                    Debug.Log("Cant Find inventory type for ItemType value " + inventoryType);
                    return;
                }

                if (!itemInventoryTable.TryGetValue((ItemName)inventoryType, out inventory))
                {
                    itemInventoryTable.Add((ItemName)inventoryType, inventoryInfo);
                }
                else
                {
                    inventory.Amount += inventoryInfo.Amount;
                }
                break;
            case SlotItemType.Equip:
                if (!Enum.IsDefined(typeof(EquipName), inventoryType))
                {
                    Debug.Log("Cant Find inventory type for EquipType value " + inventoryType);
                    return;
                }

                if (!equipInventoryTable.TryGetValue((EquipName)inventoryType, out inventory))
                {
                    equipInventoryTable.Add((EquipName)inventoryType, inventoryInfo);
                }
                else
                {
                    inventory.Amount += inventoryInfo.Amount;
                }
                break;
        }
    }

    public void DeserializePlayerInventoryTable(string[] data)
    {
        itemInventoryTable = JsonConvert.DeserializeObject<Dictionary<ItemName, SlotItemInfo>>(data[0]);
        equipInventoryTable = JsonConvert.DeserializeObject<Dictionary<EquipName, SlotItemInfo>>(data[1]);
    }

    public string SerializePlayerInventoryTable()
    {
        return JsonConvert.SerializeObject(itemInventoryTable);
    }
}