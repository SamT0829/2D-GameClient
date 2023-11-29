using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class ServerLobbyEnterMission : MissionBase
{
    public override Task MissionPrepare()
    {
        SubMissionCount = 6;
        CompleteCount = 0;
        return Task.CompletedTask;
    }

    // public override async Task MissionWork()
    // {
    //     await MissionProcess();

    //     while (!this.IsComplete)
    //     {
    //         if (this.OnProgress != null)
    //         {
    //             await this.OnProgress.Invoke();
    //         }

    //         await Task.Delay(TimeSpan.FromSeconds(0.05));
    //     }

    //     await MissionFinish();
    // }

    protected override Task MissionProcess()
    {
        if (OnProcess != null)
            OnProcess.Invoke();

        CompleteCount = 1;

        NetworkHandler.Instance.RegisterMessageListener(MsgType.NetMsg_LobbyLoginRespond, OnLobbyLoginRespond);
        EventManager.Instance.RegisterEventListener<ServerConnectedEvent>(((int)RemoteConnetionType.Lobby), OnLobbyServerConnected);

        NetworkHandler.Instance.Connect(RemoteConnetionType.Lobby, ClientData.Instance.LobbyServerAddreas, GameManager.Instance.MultiplayerServerName);

        Debug.Log(Name + "Mission Process , Lobby connection" + ClientData.Instance.LobbyServerAddreas + GameManager.Instance.MultiplayerServerName);

        CompleteCount = 2;

        return Task.CompletedTask;
    }

    protected async override Task MissionComplete()
    {
        CompleteCount = SubMissionCount;
        IsComplete = true;
        Debug.Log(Name + "Mission Complete" + ClientData.Instance.LobbyPlayerInfo.AccountId);
        EventManager.Instance.UnRegisterEventListener<ServerConnectedEvent>(((int)RemoteConnetionType.Lobby), OnLobbyServerConnected);

        if (OnComplete != null)
        {
            await OnComplete.Invoke();
        }

        await Task.CompletedTask;
    }

    protected override Task MissionFail()
    {
        IsFail = true;

        if (OnFail != null)
            OnFail.Invoke();

        //GOTO : GameClose Event

        return Task.CompletedTask;
    }

    protected async override Task MissionFinish()
    {
        IsFinish = true;
        Debug.Log(Name + "Mission MissionFinish" + ClientData.Instance.LobbyPlayerInfo.AccountId);

        if (OnFinish != null)
        {
            await OnFinish.Invoke();
        }

        await Task.CompletedTask;
    }

    private void OnLobbyLoginRespond(int connectionId, Dictionary<int, object> message)
    {
        Debug.Log("OnLobbyLoginRespond message " + JsonConvert.SerializeObject(message));

        CompleteCount = 5;
        ErrorCode errorCode = (ErrorCode)message[((int)LobbyLoginRespond.ErrorCode)];
        if (errorCode == ErrorCode.Success)
        {
            var lobbyPlayerData = (object[])message[((int)LobbyLoginRespond.PlayerData)];
            var playerAccountData = (object[])message[((int)LobbyLoginRespond.AccountData)];

            ClientData.Instance.LobbyPlayerInfo.DeserializeObject(lobbyPlayerData);
            ClientData.Instance.PlayerAccountInfo.DeserializeObject(playerAccountData);
            ClientData.Instance.PlayerAccountInfo.ServerTime = (long)message[((int)LobbyLoginRespond.ServerTime)];
            ClientData.Instance.PlayerInventoryInfo.DeserializePlayerInventoryTable((string[])message[((int)LobbyLoginRespond.InventoryInfo)]);
            Task.Run(this.MissionComplete);
        }
    }

    private void OnLobbyServerConnected(IEvent obj)
    {
        CompleteCount = 3;
        var msgBuilder = new MessageBuilder();
        msgBuilder.AddMsg(((int)LobbyLoginRequest.SessionId), ClientData.Instance.SessionId, NetMsgFieldType.Int);
        NetworkHandler.Instance.Send(RemoteConnetionType.Lobby, MsgType.NetMsg_LobbyLoginRequest, msgBuilder);

        Debug.Log("Send NetMsg_LobbyLoginRequest message " + ClientData.Instance.SessionId);

        CompleteCount = 4;
    }
}