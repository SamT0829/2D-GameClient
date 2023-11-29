using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class ServerLobbyReconnectGameMission : MissionBase
{
    protected override Task MissionProcess()
    {
        if (OnProcess != null)
            OnProcess.Invoke();

        CompleteCount = 1;

        NetworkHandler.Instance.RegisterMessageListener(MsgType.NetMsg_LobbyReconnectGameRespond, OnLobbyReconnectGameRespond);

        MessageBuilder msgBuilder = new MessageBuilder();
        msgBuilder.AddMsg(((int)LobbyPrepareEnterGameRequest.AccountId), ((int)ClientData.Instance.LobbyPlayerInfo.AccountId), NetMsgFieldType.Long);
        NetworkHandler.Instance.Send(RemoteConnetionType.Lobby, MsgType.NetMsg_LobbyReconnectGameRequest, msgBuilder);
        Debug.Log("Send NetMsg_LobbyReconnectGameRequest");

        CompleteCount = 2;

        return Task.CompletedTask;
    }

    private void OnLobbyReconnectGameRespond(int connectionId, Dictionary<int, object> message)
    {
        Debug.Log("Send NetMsg_LobbyReconnectGameRequest" + JsonConvert.SerializeObject(message));

        CompleteCount = 3;
        ErrorCode errorCode = (ErrorCode)message[((int)LobbyPrepareEnterGameRespond.ErrorCode)];

        if (errorCode == ErrorCode.Success)
        {
            var gameServerIP = message[((int)LobbyPrepareEnterGameRespond.GameServerIP)].ToString();
            var GameServerPort = message[((int)LobbyPrepareEnterGameRespond.GameServerPort)].ToString();
            ClientData.Instance.GameServerAddreas = string.Format("{0}:{1}", gameServerIP, GameServerPort);
            Task.Run(this.MissionComplete);
        }
        CompleteCount = 4;
    }

    protected async override Task MissionComplete()
    {
        CompleteCount = SubMissionCount;
        IsComplete = true;
        Debug.Log(Name + "Mission Complete" + ClientData.Instance.LobbyPlayerInfo.AccountId);
        NetworkHandler.Instance.UnRegisterMessageListener(MsgType.NetMsg_LobbyReconnectGameRespond, OnLobbyReconnectGameRespond);

        if (OnComplete != null)
        {
            await OnComplete.Invoke();
        }

        await Task.CompletedTask;
    }
}