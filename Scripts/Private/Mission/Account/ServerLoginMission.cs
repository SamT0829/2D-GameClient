using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;


public class ServerLoginMission : MissionBase
{
    private string _gameId;
    private string _gamePassword;

    public ServerLoginMission(string gameId, string gamePassword)
    {
        _gameId = gameId;
        _gamePassword = gamePassword;
    }

    public override Task MissionPrepare()
    {
        SubMissionCount = 6;
        CompleteCount = 0;
        return Task.CompletedTask;
    }

    public override async Task MissionWork()
    {
        await MissionProcess();

        while (!this.IsComplete)
        {
            if (this.OnProgress != null)
            {
                await this.OnProgress.Invoke();
            }
            await Task.Delay(TimeSpan.FromSeconds(0.05));
        }

        await MissionFinish();
    }

    protected override async Task MissionProcess()
    {
        CompleteCount = 1;

        if (OnProcess != null)
        {
            await OnProcess.Invoke();
        }

        NetworkHandler.Instance.RegisterMessageListener(MsgType.NetMsg_AccountLoginRespond, OnAccountLoginRespond);
        EventManager.Instance.RegisterEventListener<ServerConnectedEvent>(((int)RemoteConnetionType.Account), OnAccountServerConnected);

        NetworkHandler.Instance.Connect(RemoteConnetionType.Account, GameManager.Instance.ServerAddreas, GameManager.Instance.MultiplayerServerName);

        CompleteCount = 2;
    }

    protected override async Task MissionComplete()
    {
        CompleteCount = SubMissionCount;
        IsComplete = true;
        Debug.Log(Name + "Mission Complete");
        EventManager.Instance.UnRegisterEventListener<ServerConnectedEvent>(((int)RemoteConnetionType.Account), OnAccountServerConnected);

        // LoadingController.SendLoginLog(LoginLogEnum.AccountLoginRespond);
        if (OnComplete != null)
        {
            await OnComplete.Invoke();
        }
    }

    protected override Task MissionFail()
    {
        IsFail = true;

        if (OnFail != null)
            OnFail.Invoke();

        //GOTO : GameClose Event

        return Task.CompletedTask;
    }

    protected override async Task MissionFinish()
    {
        IsFinish = true;

        if (OnFinish != null)
        {
            await OnFinish.Invoke();
        }

        await Task.CompletedTask;
    }

    private void OnAccountLoginRespond(int connectionId, Dictionary<int, object> message)
    {
        Debug.Log(JsonConvert.SerializeObject(message));
        CompleteCount = 5;

        //處理Server傳來的登入資訊
        ErrorCode errorCode = ((ErrorCode)message[((int)AccountLoginRespond.ErrorCode)]);

        if (errorCode == ErrorCode.Success)
        {
            ClientData.Instance.SessionId = (int)message[((int)AccountLoginRespond.SessionId)];
            var lobbyServerIp = message[((int)AccountLoginRespond.LobbyServerIP)];
            var lobbyServerPort = message[((int)AccountLoginRespond.LobbyServerPort)];
            ClientData.Instance.LobbyServerAddreas = string.Format("{0}:{1}", lobbyServerIp, lobbyServerPort);
            Task.Run(MissionComplete);
        }
        else
        {

        }
    }

    private void OnAccountServerConnected(IEvent obj)
    {
        CompleteCount = 3;

        MessageBuilder msgBuilder = new MessageBuilder();

        msgBuilder.AddMsg(((int)AccountLoginRequest.GameType), ((int)GameManager.Instance.GameType), NetMsgFieldType.String);
        msgBuilder.AddMsg(((int)AccountLoginRequest.GameId), _gameId, NetMsgFieldType.String);
        msgBuilder.AddMsg(((int)AccountLoginRequest.Password), _gamePassword, NetMsgFieldType.String);
        NetworkHandler.Instance.Send(RemoteConnetionType.Account, MsgType.NetMsg_AccountLoginRequest, msgBuilder);

        CompleteCount = 4;
    }
}