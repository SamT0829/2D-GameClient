using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class AccountLoginUIController : MonoBehaviour
{
    [Header("GameLoginUI")]
    [SerializeField] InputField GameIdInputField;
    [SerializeField] InputField GamePasswordInputField;
    [SerializeField] Button ConfirmButton;


    private void Awake()
    {
    }

    private void Start()
    {
        NetworkHandler.Instance.Connect(RemoteConnetionType.Account, GameManager.Instance.ServerAddreas, GameManager.Instance.MultiplayerServerName);
        NetworkHandler.Instance.RegisterMessageListener(MsgType.NetMsg_AccountLoginRespond, OnAccountLoginRespond);

        EventManager.Instance.RegisterEventListener<GameUIEvent>(((int)GameUIMessageEvent.EnterGameLobby), OnEnterGameLobbyEvent);

        ConfirmButton.onClick.AddListener(OnClickConfirmButton);
    }

    private void OnClickConfirmButton()
    {
        var gameId = GameIdInputField.text;
        var gamePassword = GamePasswordInputField.text;

        MessageBuilder msgBuilder = new MessageBuilder();
        msgBuilder.AddMsg(((int)AccountLoginRequest.GameType), ((int)GameManager.Instance.GameType), NetMsgFieldType.String);
        msgBuilder.AddMsg(((int)AccountLoginRequest.GameId), gameId, NetMsgFieldType.String);
        msgBuilder.AddMsg(((int)AccountLoginRequest.Password), gamePassword, NetMsgFieldType.String);
        NetworkHandler.Instance.Send(RemoteConnetionType.Account, MsgType.NetMsg_AccountLoginRequest, msgBuilder);
    }

    private void OnAccountLoginRespond(int connectionId, Dictionary<int, object> message)
    {
        Debug.Log(JsonConvert.SerializeObject(message));

        //處理Server傳來的登入資訊
        ErrorCode errorCode = ((ErrorCode)message[((int)AccountLoginRespond.ErrorCode)]);
        ClientData.Instance.SessionId = (int)message[((int)AccountLoginRespond.SessionId)];
        var lobbyServerIp = message[((int)AccountLoginRespond.LobbyServerIP)];
        var lobbyServerPort = message[((int)AccountLoginRespond.LobbyServerPort)];
        ClientData.Instance.LobbyServerAddreas = string.Format("{0}:{1}", lobbyServerIp, lobbyServerPort);

        if (errorCode == ErrorCode.Success)
        {
            UIManager.Instance.CreateLoadingMission(new ServerLobbyEnterMission()
            {
                OnFinish = () =>
                {
                    GameUIEvent e = new GameUIEvent(GameUIMessageEvent.EnterGameLobby);
                    EventManager.Instance.SendEvent(e);
                    return Task.CompletedTask;
                }
            });
            UIManager.Instance.StartLoadingMission();
        }
        else
        {
            Debug.Log(errorCode);
        }
    }

    #region Event Message Callback
    private void OnEnterGameLobbyEvent(IEvent obj)
    {
        Action beforeTeleportAction = (() =>
        {
            EventManager.Instance.UnRegisterEventListener<GameUIEvent>(((int)GameUIMessageEvent.EnterGameLobby), OnEnterGameLobbyEvent);
        });

        Action afterTeleportAction = (() =>
        {
            UIManager.Instance.UIStatus = UIStatus.Lobby;
            UIManager.Instance.FinishLoadingMission();

            // 判斷是否在遊戲狀態
            if (ClientData.Instance.LobbyPlayerInfo.Status == PlayerStatus.InGameRoom)
            {
                var e = new GameControlEvent(GameControlMessageEvent.GameReconnectLoginToLobby);
                EventManager.Instance.SendEvent(e);
                Debug.Log("ReconnectGame");
            }
        });

        GameManager.Instance.TeleportToScene(GameScene.Login.ToString(), GameScene.Lobby.ToString(), beforeTeleportAction, afterTeleportAction);
    }
    #endregion
}
