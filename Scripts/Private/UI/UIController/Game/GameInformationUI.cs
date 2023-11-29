using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInformationUI : MonoBehaviour
{
    [Header("Componenent")]
    [SerializeField] private GameObject GameInformationPanel;
    [SerializeField] private RectTransform PlayerInformationGrid;

    [Header("Prefab")]
    [SerializeField] PlayerInformationPrefab PlayerInformationPrefab;

    List<GamePlayerInfo> gamePlayerInfos = new List<GamePlayerInfo>();
    Dictionary<long, PlayerInformationPrefab> accountIdPlayerInformationTable = new Dictionary<long, PlayerInformationPrefab>();

    private void Start()
    {
        NetworkHandler.Instance.RegisterMessageListener(MsgType.NetMsg_GamePlayerSyncRespond, OnGamePlayerSyncRespond);
    }
    
    private void OnDisable()
    {
        NetworkHandler.Instance.UnRegisterMessageListener(MsgType.NetMsg_GamePlayerSyncRespond, OnGamePlayerSyncRespond);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            GameInformationPanel.SetActive(true);
        if (Input.GetKeyUp(KeyCode.Tab))
            GameInformationPanel.SetActive(false);

    }

    public void SetActiveGameInformationPanel(bool active)
    {
        GameInformationPanel.SetActive(active);
    }

    private void OnGamePlayerSyncRespond(int connectionId, Dictionary<int, object> msg)
    {
        if (msg.TryGetValue(((int)GamePlayerSyncRespond.GameStaticInfo), out object gameStaticData))
        {
            GameStaticInfo gameStaticInfo = new GameStaticInfo();
            gameStaticInfo.DeserializeGameStaticObject((object[])gameStaticData);

            gameStaticInfo.GameRoomInfo.ForEach(gamePlayerInfo =>
            {
                PlayerInformationPrefab playerInformation;
                if (!accountIdPlayerInformationTable.TryGetValue(gamePlayerInfo.AccountId, out playerInformation))
                {
                    playerInformation = InitPlayerInformation(gamePlayerInfo);
                    accountIdPlayerInformationTable.Add(gamePlayerInfo.AccountId, playerInformation);
                }
                else
                {
                    playerInformation.InitPlayerInformation(gamePlayerInfo);
                }
            });
        }
    }

    private PlayerInformationPrefab InitPlayerInformation(GamePlayerInfo gamePlayerInfo)
    {
        var playerInformation = Instantiate(PlayerInformationPrefab, PlayerInformationGrid);
        playerInformation.InitPlayerInformation(gamePlayerInfo);
        return playerInformation;
    }
}
