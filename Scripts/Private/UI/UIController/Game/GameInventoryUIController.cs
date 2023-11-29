using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class GameInventoryUIController : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private SlotItemPrefab slotItemPrefab;

    [Header("Inventory Tab")]
    [SerializeField] private Button ItemInventoryTabButton;
    [SerializeField] private Button EquiInventoryTabButton;
    [SerializeField] private Button DllInventoryTabButton;

    [Header("Inventory Panel")]
    [SerializeField] private GameObject ItemInventoryPanel;
    [SerializeField] private GameObject EquipInventoryPanel;
    [SerializeField] private GameObject DllInventoryPanel;

    [SerializeField] private Transform ItemInventoryParent;
    [SerializeField] private Transform EquipInventoryParent;
    [SerializeField] private Transform DllInventoryParent;

    [Header("Sprite")]
    [SerializeField] private Sprite InventoryActiveSprite;
    [SerializeField] private Sprite InventoryNonActiveSprite;


    [Header("Inventory")]
    private Dictionary<ItemName, SlotItemPrefab> playerItemInventoryPrefabTable = new Dictionary<ItemName, SlotItemPrefab>();
    private Dictionary<EquipName, SlotItemPrefab> playerEquipInventoryPrefabTable = new Dictionary<EquipName, SlotItemPrefab>();


    private void Awake()
    {
        ItemInventoryTabButton.onClick.AddListener(() => OnClickItemInventoryTabButton(ItemInventoryTabButton));
        EquiInventoryTabButton.onClick.AddListener(() => OnClickEquiInventoryTabButton(EquiInventoryTabButton));
        DllInventoryTabButton.onClick.AddListener(() => OnClickDllInventoryTabButton(DllInventoryTabButton));
    }

    private void Update()
    {
        OpenInventory();
    }

    private void OpenInventory()
    {

        if (ClientData.Instance.PlayerInventoryInfo.PlayerItemInventoryTable == null)
            return;

        foreach (var inventoryInfo in ClientData.Instance.PlayerInventoryInfo.PlayerItemInventoryTable)
        {
            // Create InventoryItemPrefab
            AddInventory(inventoryInfo.Value);
        }

        foreach (var inventoryInfo in ClientData.Instance.PlayerInventoryInfo.PlayerEquipInventoryTable)
        {
            // Create InventoryItemPrefab
            AddInventory(inventoryInfo.Value);
        }
    }

    public void AddInventory(SlotItemInfo slotItemInfo)
    {
        SlotItemPrefab slotItem;

        switch (slotItemInfo.SlotItemType)
        {
            case SlotItemType.None:
                return;

            case SlotItemType.Item:
                ItemName itemType = ItemNameToItemType(slotItemInfo.ItemName);
                if (itemType == ItemName.None)
                    return;

                if (!playerItemInventoryPrefabTable.TryGetValue(itemType, out slotItem))
                {
                    slotItem = InitSlotItem(slotItemInfo, ItemInventoryParent);
                    playerItemInventoryPrefabTable.Add(itemType, slotItem);
                }
                else
                {
                    slotItem.InitInventoryItem(slotItemInfo);
                }

                break;

            case SlotItemType.Equip:
                EquipName equipType = ItemNameToEquipType(slotItemInfo.ItemName);
                if (equipType == EquipName.None)
                    return;

                if (!playerEquipInventoryPrefabTable.TryGetValue(equipType, out slotItem))
                {
                    slotItem = InitSlotItem(slotItemInfo, EquipInventoryParent);
                    playerEquipInventoryPrefabTable.Add(equipType, slotItem);
                }
                else
                {
                    slotItem.InitInventoryItem(slotItemInfo);
                }

                break;
        }
    }

    private SlotItemPrefab InitSlotItem(SlotItemInfo slotItemInfo, Transform parent)
    {
        SlotItemPrefab slotItem = Instantiate(slotItemPrefab, parent);
        slotItem.InitInventoryItem(slotItemInfo);

        return slotItem;
    }

    private ItemName ItemNameToItemType(string itemName)
    {
        ItemName itemType = ItemName.None;
        if (!Enum.TryParse<ItemName>(itemName, out itemType))
        {
            Debug.LogErrorFormat("ItemNameToItemType ItemType can't parse for itemName " + itemName);
        }
        return itemType;
    }

    private EquipName ItemNameToEquipType(string itemName)
    {
        EquipName equipType = EquipName.None;
        if (!Enum.TryParse<EquipName>(itemName, out equipType))
        {
            Debug.LogErrorFormat("ItemNameToEquipType EquipType can't parse for itemName " + itemName);
        }
        return equipType;
    }

    private void OnClickItemInventoryTabButton(Button button)
    {
        ItemInventoryPanel.SetActive(true);
        EquipInventoryPanel.SetActive(false);
        DllInventoryPanel.SetActive(false);
        SetButtonMenuTabActive(button);
    }

    private void OnClickEquiInventoryTabButton(Button button)
    {
        ItemInventoryPanel.SetActive(false);
        EquipInventoryPanel.SetActive(true);
        DllInventoryPanel.SetActive(false);
        SetButtonMenuTabActive(button);
    }

    private void OnClickDllInventoryTabButton(Button button)
    {
        ItemInventoryPanel.SetActive(false);
        EquipInventoryPanel.SetActive(false);
        DllInventoryPanel.SetActive(true);
        SetButtonMenuTabActive(button);
    }

    private void SetButtonMenuTabActive(Button button)
    {
        ItemInventoryTabButton.GetComponent<Image>().sprite = ItemInventoryTabButton == button ? InventoryActiveSprite : InventoryNonActiveSprite;
        EquiInventoryTabButton.GetComponent<Image>().sprite = EquiInventoryTabButton == button ? InventoryActiveSprite : InventoryNonActiveSprite;
        DllInventoryTabButton.GetComponent<Image>().sprite = DllInventoryTabButton == button ? InventoryActiveSprite : InventoryNonActiveSprite;
    }

}
