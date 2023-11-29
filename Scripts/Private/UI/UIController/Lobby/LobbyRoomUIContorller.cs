using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class LobbyRoomUIContorller : MonoBehaviour
{
    [Header("GameRoomUI")]
    [SerializeField] GameObject GameRoomPanel;
    [SerializeField] Text GameRoomID;
    [SerializeField] Text GameRoomName;
    [SerializeField] GameRoomPlayerPrefab[] GameRoomPlayerPrefabArray;

    [Header("GameRoomChatUI")]
    [SerializeField] InputField GameRoomChatInputField;

    [Header("GameRoomButton")]
    [SerializeField] Button StartGameButton;
    [SerializeField] Button LeaveGameButton;
    [SerializeField] Button ReadyGameButton;

    [Header("GameRoomPlayerPrefab")]
    [SerializeField] GameRoomPlayerPrefab gameRoomPlayerPrefab;
    [SerializeField] Transform GameRoomPlayerListPanelTransform;

    [Header("ChatTextPrefab")]
    [SerializeField] ChatMessagePrefab chatMessagePrefab;
    [SerializeField] Transform GameRoomMessageContentTransform;
    [SerializeField] List<ChatMessagePrefab> ChatMessagePrefabList = new List<ChatMessagePrefab>();
    [SerializeField] int maxMessage;

    private void Start()
    {
        // Player
        NetworkHandler.Instance.RegisterMessageListener(MsgType.NetMsg_PlayerCreateLobbyRoomRespond, OnPlayerCreateGameRoomRespond);
        NetworkHandler.Instance.RegisterMessageListener(MsgType.NetMsg_PlayerJoinLobbyRoomRespond, OnPlayerJoinGameRoomRespond);
        NetworkHandler.Instance.RegisterMessageListener(MsgType.NetMsg_PlayerLeaveLobbyRoomRespond, OnPlayerLeaveGameRoomRespond);
        NetworkHandler.Instance.RegisterMessageListener(MsgType.NetMsg_PlayerStartLobbyRoomRespond, OnPlayerStartGameRoomRespond);
        NetworkHandler.Instance.RegisterMessageListener(MsgType.NetMsg_PlayerReadyLobbyRoomRespond, OnPlayerReadyLobbyRoomRespond);

        // Lobby Room
        NetworkHandler.Instance.RegisterMessageListener(MsgType.NetMsg_LobbyGameRoomStartGame, OnNetMsg_LobbyGameRoomStartGame);

        // BackgroundThread
        NetworkHandler.Instance.RegisterMessageListener(MsgType.NetMsg_LobbyGameRoomBackgroundThread, OnLobbyRoomBackgroundThread);

        EventManager.Instance.RegisterEventListener<GameUIEvent>(((int)GameUIMessageEvent.EnterGamePlay), OnEnterGamePlayEvent);
        EventManager.Instance.RegisterEventListener<GameControlEvent>(((int)GameControlMessageEvent.AfterGameTeleportToLobby), OnAfterGameTeleportToLobby);

        StartGameButton.onClick.AddListener(OnClickStartGameButton);
        LeaveGameButton.onClick.AddListener(OnClickLeaveGameButton);
        ReadyGameButton.onClick.AddListener(OnClickReadyGameButton);
        
    }

    private void OnDisable()
    {
        // Player
        NetworkHandler.Instance.UnRegisterMessageListener(MsgType.NetMsg_PlayerCreateLobbyRoomRespond, OnPlayerCreateGameRoomRespond);
        NetworkHandler.Instance.UnRegisterMessageListener(MsgType.NetMsg_PlayerJoinLobbyRoomRespond, OnPlayerJoinGameRoomRespond);
        NetworkHandler.Instance.UnRegisterMessageListener(MsgType.NetMsg_PlayerLeaveLobbyRoomRespond, OnPlayerLeaveGameRoomRespond);
        NetworkHandler.Instance.UnRegisterMessageListener(MsgType.NetMsg_PlayerStartLobbyRoomRespond, OnPlayerStartGameRoomRespond);

        // Lobby Room
        NetworkHandler.Instance.UnRegisterMessageListener(MsgType.NetMsg_LobbyGameRoomStartGame, OnNetMsg_LobbyGameRoomStartGame);

        // BackgroundThread
        NetworkHandler.Instance.UnRegisterMessageListener(MsgType.NetMsg_LobbyGameRoomBackgroundThread, OnLobbyRoomBackgroundThread);

        EventManager.Instance.UnRegisterEventListener<GameUIEvent>(((int)GameUIMessageEvent.EnterGamePlay), OnEnterGamePlayEvent);
        EventManager.Instance.UnRegisterEventListener<GameControlEvent>(((int)GameControlMessageEvent.AfterGameTeleportToLobby), OnAfterGameTeleportToLobby);
    }

    private void Update()
    {
        if (GameRoomPanel.activeInHierarchy)
        {
            if (!GameRoomChatInputField.isFocused)
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    GameRoomChatInputField.ActivateInputField();
                }
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                SendChatMessage();
            }
        }
    }

    private void SendChatMessage()
    {
        if (GameRoomChatInputField.text != string.Empty)
        {
            MessageBuilder msgBuilder = new MessageBuilder();
            msgBuilder.AddMsg(((int)GameRoomChatRequest.ChatMsg), GameRoomChatInputField.text, NetMsgFieldType.String);
            NetworkHandler.Instance.Send(RemoteConnetionType.Lobby, MsgType.NetMsg_PlayerLobbyRoomChatRequest, msgBuilder);

            GameRoomChatInputField.text = string.Empty;
        }
    }

    #region Click Button Event
    private void OnClickStartGameButton()
    {
        MessageBuilder msgBuilder = new MessageBuilder();
        NetworkHandler.Instance.Send(RemoteConnetionType.Lobby, MsgType.NetMsg_PlayerStartLobbyRoomRequest, msgBuilder);
    }
    private void OnClickLeaveGameButton()
    {
        MessageBuilder msgBuilder = new MessageBuilder();
        NetworkHandler.Instance.Send(RemoteConnetionType.Lobby, MsgType.NetMsg_PlayerLeaveLobbyRoomRequest, msgBuilder);
    }
    private void OnClickReadyGameButton()
    {
        MessageBuilder msgBuilder = new MessageBuilder();
        NetworkHandler.Instance.Send(RemoteConnetionType.Lobby, MsgType.NetMsg_PlayerReadyLobbyRoomRequest, msgBuilder);
    }
    #endregion

    #region Network Message Callback
    private void OnPlayerCreateGameRoomRespond(int connectionId, Dictionary<int, object> message)
    {
        Debug.Log("OnPlayerCreateRoomOnlineGame" + JsonConvert.SerializeObject(message));
        ErrorCode err = (ErrorCode)message[((int)CreateGameRoomRespond.ErrorCode)];

        if (err == ErrorCode.Success)
        {
            var roomData = (object[])message[((int)CreateGameRoomRespond.RoomData)];
            LobbyRoomInfo lobbyRoomInfo = new LobbyRoomInfo();
            lobbyRoomInfo.DeserializeObject(roomData);
            ShowGameRoomUI(lobbyRoomInfo);
        }
        else
        {
            // UIManager.Instance.CreteNotice();
        }
    }
    private void OnPlayerJoinGameRoomRespond(int connectionId, Dictionary<int, object> message)
    {
        Debug.Log("OnPlayerJoinGameRoomRespond" + JsonConvert.SerializeObject(message));
        ErrorCode err = (ErrorCode)message[((int)CreateGameRoomRespond.ErrorCode)];

        if (err == ErrorCode.Success)
        {
            var roomData = (object[])message[((int)CreateGameRoomRespond.RoomData)];
            LobbyRoomInfo lobbyRoomInfo = new LobbyRoomInfo();
            lobbyRoomInfo.DeserializeObject(roomData);
            ShowGameRoomUI(lobbyRoomInfo);
        }
        else
        {
            UIManager.Instance.CreateGameMessage(err.ToString(), "Confirm", null, null);
        }
    }
    private void OnPlayerLeaveGameRoomRespond(int connectionId, Dictionary<int, object> message)
    {
        var e = new GameUIEvent(GameUIMessageEvent.LeaveGameRoom);
        EventManager.Instance.SendEvent(e);

        ResetGameRoomUI();
    }
    private void OnPlayerStartGameRoomRespond(int connectionId, Dictionary<int, object> message)
    {
        ErrorCode errorCode;
        if (DictionaryMethod.RetrivieSturctData(message, StartGameRoomRespond.ErrorCode, out errorCode))
        {
            if (errorCode != ErrorCode.Success)
                Debug.Log("StartFiled");
        }
    }
    private void OnPlayerReadyLobbyRoomRespond(int connectionId, Dictionary<int, object> message)
    {

    }
    private void OnNetMsg_LobbyGameRoomStartGame(int connectionId, Dictionary<int, object> message)
    {
        ErrorCode errorCode = (ErrorCode)message[((int)LobbyGameRoomStartGame.ErrorCode)];

        if (errorCode == ErrorCode.Success)
        {
            IEvent e = new GameUIEvent(GameUIMessageEvent.EnterGamePlay);
            EventManager.Instance.SendEvent(e);
        }
    }
    private void OnLobbyRoomBackgroundThread(int connectionId, Dictionary<int, object> message)
    {
        object[] roomData;
        if (DictionaryMethod.RetrivieClassData(message, RoomBackgroundThread.RoomData, out roomData))
        {
            LobbyRoomInfo lobbyRoomInfo = new LobbyRoomInfo();
            lobbyRoomInfo.DeserializeObject(roomData);
            UpdateGameRoomPlayerUI(lobbyRoomInfo);
        }

        if (message.TryGetValue((int)RoomBackgroundThread.RoomMessage, out object chatMessage))
        {
            List<object> chatData = JsonConvert.DeserializeObject<List<object>>(chatMessage.ToString());
            UpdateGameRoomMessageUI(chatData);
        }
    }
    #endregion

    #region Event Message Callback
    private void OnEnterGamePlayEvent(IEvent obj)
    {
        TeleportToGameScene();
    }
    private void OnAfterGameTeleportToLobby(IEvent obj)
    {
        Debug.Log("OnAfterGameTeleportToLobby");

        if (obj.GetEventObject(out GameControlEvent gameControlEvent))
        {
            LobbyRoomInfo lobbyRoomInfo = new LobbyRoomInfo();
            lobbyRoomInfo.DeserializeObject((object[])gameControlEvent.data);
            ShowGameRoomUI(lobbyRoomInfo);
        }

        EventManager.Instance.UnRegisterEventListener<GameControlEvent>(((int)GameControlMessageEvent.AfterGameTeleportToLobby), OnAfterGameTeleportToLobby);
    }
    #endregion

    // Game Room UI Setting
    private void ShowGameRoomUI(LobbyRoomInfo lobbyRoomInfo)
    {
        GameUIEvent e = new GameUIEvent(GameUIMessageEvent.EnterGameRoom);
        EventManager.Instance.SendEvent(e);

        GetComponentInParent<LobbyMainUIController>().ActivePanel(GameRoomPanel.GetComponent<RectTransform>());
        GameRoomID.text = lobbyRoomInfo.RoomId.ToString();
        GameRoomName.text = lobbyRoomInfo.RoomName.ToString();
        GameRoomPlayerPrefabArray = new GameRoomPlayerPrefab[lobbyRoomInfo.MaxPlayer];

        UpdateGameRoomPlayerUI(lobbyRoomInfo);
    }

    // Game Room Create Player UI
    private void UpdateGameRoomPlayerUI(LobbyRoomInfo lobbyRoomInfo)
    {
        List<LobbyPlayerInfo> lobbyRoomPlayerData = lobbyRoomInfo.LobbyPlayerInfoList;

        if (GameRoomPlayerListPanelTransform == null)
        {
            var gamePlayerListPanel = transform.Find("GameRoomPanel").Find("GameRoomPlayerListPanel");
            if (gamePlayerListPanel != null)
                GameRoomPlayerListPanelTransform = gamePlayerListPanel;
            else
            {
                Debug.Log("GameRoomPlayerListPanelTransform is Null");
                return;
            }
        }

        for (int i = 0; i < GameRoomPlayerPrefabArray.Length; i++)
        {
            if (lobbyRoomPlayerData.Count <= i)
            {
                if (GameRoomPlayerPrefabArray[i] != null)
                    Destroy(GameRoomPlayerPrefabArray[i].gameObject);

                continue;
            }
            LobbyRoomPlayerInfo lobbyRoomPlayerInfo;
            if (GameRoomPlayerPrefabArray[i] != null && GameRoomPlayerPrefabArray[i].LobbyPlayerInfo.NickName == lobbyRoomPlayerData[i].NickName)
            {
                lobbyRoomPlayerInfo = lobbyRoomInfo.LobbyRoomPlayerInfosList.FirstOrDefault(playerInfo =>
                    playerInfo.NickName == lobbyRoomPlayerData[i].NickName);

                if (lobbyRoomPlayerInfo != null)
                    GameRoomPlayerPrefabArray[i].UpdateNetworkSetting(lobbyRoomPlayerInfo, lobbyRoomInfo);

                continue;
            }

            if (GameRoomPlayerPrefabArray[i] != null)
            {
                Destroy(GameRoomPlayerPrefabArray[i].gameObject);
            }

            GameRoomPlayerPrefab gameRoomPlayer = Instantiate(gameRoomPlayerPrefab, GameRoomPlayerListPanelTransform);
            gameRoomPlayer.SettingGameRoomPlayerPrefab(lobbyRoomPlayerData[i]);
            GameRoomPlayerPrefabArray[i] = gameRoomPlayer;

            lobbyRoomPlayerInfo = lobbyRoomInfo.LobbyRoomPlayerInfosList.FirstOrDefault(playerInfo => playerInfo.NickName == lobbyRoomPlayerData[i].NickName);
            if (lobbyRoomPlayerInfo != null)
                gameRoomPlayer.UpdateNetworkSetting(lobbyRoomPlayerInfo, lobbyRoomInfo);
        }
    }
    private void UpdateGameRoomMessageUI(List<object> chatData)
    {
        if (ChatMessagePrefabList.Count >= maxMessage)
        {
            Destroy(ChatMessagePrefabList[0].gameObject);
            ChatMessagePrefabList.Remove(ChatMessagePrefabList[0]);
        }

        var messagePrefab = Instantiate(chatMessagePrefab, GameRoomMessageContentTransform);
        messagePrefab.BuildChatMessage(chatData);
        ChatMessagePrefabList.Add(messagePrefab);
    }
    private void ResetGameRoomUI()
    {
        GameRoomChatInputField.text = string.Empty;
        ChatMessagePrefabList.ForEach(prefab => { if (prefab != null) Destroy(prefab.gameObject); });
        Array.ForEach(GameRoomPlayerPrefabArray, prefab => { if (prefab != null) Destroy(prefab.gameObject); });
    }

    // Teleport to Game Scene
    private void TeleportToGameScene()
    {
        GameManager.Instance.TeleportToScene(GameScene.Lobby.ToString(), GameScene.Game.ToString(), OnBeforTeleportEvent, OnAfterLobbyTeleportToGame);
    }
    private void OnBeforTeleportEvent()
    {

    }
    private void OnAfterLobbyTeleportToGame()
    {
        UIManager.Instance.UIStatus = UIStatus.Game;

        GameControlEvent afterLobbyTeleportToGame = new GameControlEvent(GameControlMessageEvent.AfterLobbyTeleportToGame);
        EventManager.Instance.SendEvent(afterLobbyTeleportToGame);
    }
}