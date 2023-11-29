using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class LobbyShopUIController : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] ShopItemPrefab shopItemPrefab;
    [SerializeField] SlotItemPrefab inventoryItemPrefab;

    [Header("Lobby Shop UI component child")]
    [SerializeField] Transform ShopGridGroup;
    [SerializeField] Transform InventoryGridGroup;
    [SerializeField] Button BackToLobbyButton;
    [SerializeField] Text GoldText;

    LobbyMainUIController LobbyMainUIController;
    private int MaxShopItemCount = 8;
    private Dictionary<int, ShopItemPrefab> itemIdShopItemTable = new Dictionary<int, ShopItemPrefab>();
    private Dictionary<ItemName, SlotItemPrefab> playerInventoryPrefabTable = new Dictionary<ItemName, SlotItemPrefab>();

    private void Awake()
    {
        LobbyMainUIController = GetComponentInParent<LobbyMainUIController>();
    }
    private void OnEnable()
    {
        NetworkHandler.Instance.RegisterMessageListener(MsgType.NetMsg_PlayerLobbyGoToShopRespond, OnPlayerLobbyGoToShopRespond);
        NetworkHandler.Instance.RegisterMessageListener(MsgType.NetMsg_PlayerLobbyBuyShopItemRespond, OnPlayerLobbyBuyShopItemRespond);
    }

    private void OnDisable()
    {
        NetworkHandler.Instance.UnRegisterMessageListener(MsgType.NetMsg_PlayerLobbyGoToShopRespond, OnPlayerLobbyGoToShopRespond);
        NetworkHandler.Instance.UnRegisterMessageListener(MsgType.NetMsg_PlayerLobbyBuyShopItemRespond, OnPlayerLobbyBuyShopItemRespond);
    }

    private void Update()
    {
        GoldText.text = ClientData.Instance.PlayerAccountInfo.Money.ToString();

        if (ShopGridGroup.transform.childCount > MaxShopItemCount)
        {
            MaxShopItemCount += 2;
            var height = shopItemPrefab.GetComponent<RectTransform>().sizeDelta.y;
            ShopGridGroup.GetComponent<RectTransform>().sizeDelta += new Vector2(0, height + 20);
        }
    }
    private void PlayerInventory()
    {
        if (ClientData.Instance.PlayerInventoryInfo.PlayerItemInventoryTable == null)
            return;

        foreach (var inventoryInfo in ClientData.Instance.PlayerInventoryInfo.PlayerItemInventoryTable)
        {
            // Create InventoryItemPrefab
            SlotItemPrefab inventoryItem;
            if (!playerInventoryPrefabTable.TryGetValue(inventoryInfo.Key, out inventoryItem))
            {
                inventoryItem = Instantiate(inventoryItemPrefab, InventoryGridGroup);
                inventoryItem.InitInventoryItem(inventoryInfo.Value);
                playerInventoryPrefabTable.Add(inventoryInfo.Key, inventoryItem);
            }
            else
            {
                inventoryItem.InitInventoryItem(inventoryInfo.Value);
            }
        }
    }

    #region Button Callback
    private void OnClickShopItemButton(ShopItemPrefab shopItem)
    {
        //Send Network Message
        MessageBuilder msgBuilder = new MessageBuilder();
        msgBuilder.AddMsg(((int)PlayerLobbyBuyShopItemRequest.ShopInfo), shopItem.Info.CreateSerializeObject().ToArray(), NetMsgFieldType.Array);
        NetworkHandler.Instance.Send(RemoteConnetionType.Lobby, MsgType.NetMsg_PlayerLobbyBuyShopItemRequest, msgBuilder);
    }
    #endregion

    #region Network Callback
    private void OnPlayerLobbyGoToShopRespond(int connectionId, Dictionary<int, object> message)
    {
        if (!DictionaryMethod.RetrivieSturctData(message, PlayerLobbyGoToShopRespond.ErrorCode, out ErrorCode errorCode)) return;
        if (!DictionaryMethod.RetrivieClassData(message, PlayerLobbyGoToShopRespond.ShopInfo, out object[] shopInfoDataArray)) { }

        if (errorCode == ErrorCode.Success)
        {
            LobbyMainUIController.ActivePanel(GetComponent<RectTransform>());
            PlayerInventory();

            Array.ForEach(shopInfoDataArray, (Action<object>)(shopInfoData =>
            {
                SlotItemInfo shopItemInfo = new SlotItemInfo();
                shopItemInfo.DeserializeObject((object[])shopInfoData);

                ShopItemPrefab shopItem;
                if (!itemIdShopItemTable.TryGetValue((int)shopItemInfo.ItemID, out shopItem))
                {
                    shopItem = Instantiate(shopItemPrefab, ShopGridGroup);
                    shopItem.InitShopItem(shopItemInfo, OnClickShopItemButton);
                    shopItem.transform.SetSiblingIndex((int)shopItem.Info.ItemIndex);
                    itemIdShopItemTable.Add((int)shopItemInfo.ItemID, shopItem);
                }
            }));
        }
        else
        {
            // error
        }
    }
    private void OnPlayerLobbyBuyShopItemRespond(int connectionId, Dictionary<int, object> message)
    {
        if (!DictionaryMethod.RetrivieSturctData(message, PlayerLobbyBuyShopItemRespond.ErrorCode, out ErrorCode errorCode)) return;
        if (!DictionaryMethod.RetrivieSturctData(message, PlayerLobbyBuyShopItemRespond.Money, out long money)) { };
        if (!DictionaryMethod.RetrivieClassData(message, PlayerLobbyBuyShopItemRespond.InventoryData, out string[] inventoryData)) { };

        if (errorCode == ErrorCode.Success)
        {
            ClientData.Instance.PlayerAccountInfo.Money = money;
            ClientData.Instance.PlayerInventoryInfo.DeserializePlayerInventoryTable(inventoryData);
            PlayerInventory();
        }
        else
        {
            // error
        }
    }
    #endregion
}
