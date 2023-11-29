using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class ServerGameEnteredMission : MissionBase
{
    protected override async Task MissionProcess()
    {
        if (OnProcess != null)
            await OnProcess.Invoke();

        CompleteCount = 1;

        NetworkHandler.Instance.RegisterMessageListener(MsgType.NetMsg_GameEnteredRespond, OnGameEnteredRespond);
        EventManager.Instance.RegisterEventListener<ServerConnectedEvent>(((int)RemoteConnetionType.Game), OnGameServerConnected);
        NetworkHandler.Instance.Connect(RemoteConnetionType.Game, ClientData.Instance.GameServerAddreas, GameManager.Instance.MultiplayerServerName);

        CompleteCount = 2;

        await Task.CompletedTask;
    }

    private void OnGameEnteredRespond(int connectionId, Dictionary<int, object> message)
    {
        Debug.Log("OnGameEnteredRespond" + JsonConvert.SerializeObject(message));
        CompleteCount = 5;
        Task.Run(this.MissionComplete);

        if (message.TryGetValue(((int)GameEnterRespond.GameStaticInfo), out object gameStaticInfo))
        {
            GameControlEvent controlEvent = new GameControlEvent(GameControlMessageEvent.GameReconnectLobbyToGame);
            controlEvent.data = gameStaticInfo;
            EventManager.Instance.SendEvent(controlEvent);
        }
    }

    private void OnGameServerConnected(IEvent obj)
    {
        CompleteCount = 3;
        var msgBuilder = new MessageBuilder();
        msgBuilder.AddMsg(((int)LobbyLoginRequest.SessionId), ClientData.Instance.SessionId, NetMsgFieldType.Int);
        NetworkHandler.Instance.Send(RemoteConnetionType.Game, MsgType.NetMsg_GameEnteredRequest, msgBuilder);

        Debug.Log("Send NetMsg_GameEnteredRequest message " + ClientData.Instance.SessionId);

        CompleteCount = 4;
    }

    protected override async Task MissionFinish()
    {
        IsFinish = true;

        if (OnFinish != null)
        {
            await OnFinish.Invoke();
            await Task.Delay(TimeSpan.FromMilliseconds(0.5));
        }

        NetworkHandler.Instance.UnRegisterMessageListener(MsgType.NetMsg_GameEnteredRespond, OnGameEnteredRespond);
        EventManager.Instance.UnRegisterEventListener<ServerConnectedEvent>(((int)RemoteConnetionType.Game), OnGameServerConnected);
        await Task.CompletedTask;
    }

}
