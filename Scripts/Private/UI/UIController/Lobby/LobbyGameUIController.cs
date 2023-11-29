using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class LobbyGameUIController : MonoBehaviour
{
    [Header("Lobby Button")]
    [SerializeField] Button CreateRoomButton;
    [SerializeField] Button JoinRoomButton;

    [Header("Lobby Panel UI")]
    [SerializeField] GameObject LobbyGamePanel;
    [SerializeField] GameObject LobbyMainPanel;

    [Header("CreateRoomUI")]
    [SerializeField] GameObject CreateRoomPanel;
    [SerializeField] InputField RoomNameInputField;
    [SerializeField] Dropdown MaxPlayerDropDown;
    [SerializeField] Button CreateRoomConfirmButton;

    [Header("JoinRoomUI")]
    [SerializeField] GameObject JoinRoomPanel;
    [SerializeField] InputField RoomNameJoinInputField;
    [SerializeField] InputField MaxPlayerJoinInputField;
    [SerializeField] Button JoinRoomConfirmButton;

    [Header("GameRoomPrefab")]
    [SerializeField] GameRoomPrefab gameRoomPrefab;
    [SerializeField] Transform GameRoomListPanelTransform;

    [SerializeField] GameRoomPrefab[] GameNameRoomPrefabArray = new GameRoomPrefab[10];

    int roomId;

    // 不用經過 LoginUI
    [SerializeField] bool TestMode = false;

    private void OnEnable()
    {
        // BackgroundThread
        NetworkHandler.Instance.RegisterMessageListener(MsgType.NetMsg_LobbyBackgroundThread, OnLobbyBackgroundThread);

        // Lobby room Event
        EventManager.Instance.RegisterEventListener<GameUIEvent>(((int)GameUIMessageEvent.EnterGameRoom), OnEnterGameRoomEvent);
        EventManager.Instance.RegisterEventListener<GameUIEvent>(((int)GameUIMessageEvent.LeaveGameRoom), OnLeaveGameRoomEvent);
        EventManager.Instance.RegisterEventListener<GameControlEvent>(((int)GameControlMessageEvent.GameReconnectLoginToLobby), OnReconnectGame);

        // Lobby Button Callback
        CreateRoomButton.onClick.AddListener(OnClickCreateRoomButton);
        JoinRoomButton.onClick.AddListener(OnClickJoinRoomButton);

        CreateRoomConfirmButton.onClick.AddListener(OnClickCreateRoomConfirmButton);
    }
    private void OnDisable()
    {
        NetworkHandler.Instance.UnRegisterMessageListener(MsgType.NetMsg_LobbyBackgroundThread, OnLobbyBackgroundThread);

        EventManager.Instance.UnRegisterEventListener<GameUIEvent>(((int)GameUIMessageEvent.EnterGameRoom), OnEnterGameRoomEvent);
        EventManager.Instance.UnRegisterEventListener<GameUIEvent>(((int)GameUIMessageEvent.LeaveGameRoom), OnLeaveGameRoomEvent);
        EventManager.Instance.UnRegisterEventListener<GameControlEvent>(((int)GameControlMessageEvent.GameReconnectLoginToLobby), OnReconnectGame);
    }
    private void Start()
    {
        // Test Mode
        if (TestMode)
        {
            NetworkHandler.Instance.Connect(RemoteConnetionType.Account, GameManager.Instance.ServerAddreas, GameManager.Instance.MultiplayerServerName);
            UIManager.Instance.CreateLoadingMission(new ServerLoginMission("sam", "sam"));
            UIManager.Instance.CreateLoadingMission(new ServerLobbyEnterMission()
            {
                OnFinish = () =>
                {
                    UIManager.Instance.FinishLoadingMission();
                    return null;
                }
            });
            UIManager.Instance.StartLoadingMission();
        }
    }


    #region Button Callback
    private void OnClickCreateRoomButton()
    {
        CreateRoomPanel.SetActive(true);
    }
    private void OnClickJoinRoomButton()
    {
        CreateRoomPanel.SetActive(false);
        // GOTO : Show JoinRoomPanel 
    }
    private void OnClickCreateRoomConfirmButton()
    {
        var roomName = RoomNameInputField.text;
        var maxPlayer = MaxPlayerDropDown.captionText.text;

        //Send Network Message
        MessageBuilder msgBuilder = new MessageBuilder();
        msgBuilder.AddMsg(((int)CreateGameRoomRequest.RoomName), roomName, NetMsgFieldType.String);
        msgBuilder.AddMsg(((int)CreateGameRoomRequest.RoomPassword), "", NetMsgFieldType.String);
        msgBuilder.AddMsg(((int)CreateGameRoomRequest.MaxPlayer), Convert.ToInt32(maxPlayer), NetMsgFieldType.Int);
        NetworkHandler.Instance.Send(RemoteConnetionType.Lobby, MsgType.NetMsg_PlayerCreateLobbyRoomRequest, msgBuilder);
    }
    #endregion

    #region Net Message Callback
    private void OnLobbyBackgroundThread(int connectionId, Dictionary<int, object> message)
    {
        object[] roomListData = (object[])message[LobbyBackgroundThread.RoomListData.GetHashCode()];

        List<LobbyRoomInfo> gameRoomDataList = new List<LobbyRoomInfo>();
        foreach (object[] roomData in roomListData)
        {
            LobbyRoomInfo gameRoomData = new LobbyRoomInfo();
            gameRoomData.DeserializeObject(roomData);
            gameRoomDataList.Add(gameRoomData);
        }

        UpdateGameRoomUI(gameRoomDataList);
    }
    #endregion
    // Lobby Create Game Room UI
    private void UpdateGameRoomUI(List<LobbyRoomInfo> lobbyRoomInfoList)
    {
        if (GameRoomListPanelTransform == null)
        {
            var gameListPanel = transform.Find("LobbyPanel").Find("GameRoomListPanel");
            if (gameListPanel != null)
                GameRoomListPanelTransform = gameListPanel;
            else
                return;
        }

        for (int i = 0; i < GameNameRoomPrefabArray.Length; i++)
        {
            if (lobbyRoomInfoList.Count <= i)
            {
                if (GameNameRoomPrefabArray[i] != null)
                    Destroy(GameNameRoomPrefabArray[i].gameObject);

                continue;
            }

            if (GameNameRoomPrefabArray[i] != null && GameNameRoomPrefabArray[i].RoomId == lobbyRoomInfoList[i].RoomId)
                continue;

            if (GameNameRoomPrefabArray[i] != null)
                Destroy(GameNameRoomPrefabArray[i].gameObject);

            GameRoomPrefab gameRoom = Instantiate(gameRoomPrefab, GameRoomListPanelTransform);
            gameRoom.SettingRoomPrefab(lobbyRoomInfoList[i]);
            GameNameRoomPrefabArray[i] = gameRoom;
        }
    }

    #region Event Message Callback
    private void OnEnterGameRoomEvent(IEvent obj)
    {
        UIManager.Instance.UIStatus = UIStatus.Room;
        // ResetLobbyUI();
    }
    private void OnLeaveGameRoomEvent(IEvent obj)
    {
        UIManager.Instance.UIStatus = UIStatus.Lobby;
        GetComponentInParent<LobbyMainUIController>().ActivePanel(GetComponent<RectTransform>());
    }
    private void OnReconnectGame(IEvent obj)
    {
        Action OnBeforTeleportEvent = () =>
        {

        };

        Action OnAfterTeleportEvent = () =>
        {
            UIManager.Instance.CreateLoadingMission(new ServerLobbyReconnectGameMission());
            UIManager.Instance.CreateLoadingMission(new ServerGameEnteredMission() { OnFinish = LoadingFinish });
            UIManager.Instance.StartLoadingMission();
        };

        GameManager.Instance.TeleportToScene(GameScene.Lobby.ToString(), GameScene.Game.ToString(), OnBeforTeleportEvent, OnAfterTeleportEvent);
    }
    private Task LoadingFinish()
    {
        Debug.Log("finish");
        UIManager.Instance.FinishLoadingMission();
        return Task.CompletedTask;
    }
    #endregion

    private void ResetLobbyUI()
    {
        // LobbyGamePanel.SetActive(false);
        CreateRoomPanel.SetActive(false);
        // JoinRoomPanel.SetActive(false);
        RoomNameInputField.text = string.Empty;
        MaxPlayerDropDown.value = 0;

        Array.ForEach(GameNameRoomPrefabArray, prefab => { if (prefab != null) Destroy(prefab.gameObject); });
    }
}
