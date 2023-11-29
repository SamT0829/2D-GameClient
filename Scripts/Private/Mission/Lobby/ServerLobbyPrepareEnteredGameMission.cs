using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class ServerLobbyPrepareEnteredGameMission : MissionBase
{
    protected override Task MissionProcess()
    {
        if (OnProcess != null)
            OnProcess.Invoke();

        CompleteCount = 1;

        NetworkHandler.Instance.RegisterMessageListener(MsgType.NetMsg_LobbyPrepareEnterGameRespond, OnLobbyPrepareEnterGameRespond);

        MessageBuilder msgBuilder = new MessageBuilder();
        msgBuilder.AddMsg(((int)LobbyPrepareEnterGameRequest.AccountId), ClientData.Instance.LobbyPlayerInfo.AccountId, NetMsgFieldType.Long);
        NetworkHandler.Instance.Send(RemoteConnetionType.Lobby, MsgType.NetMsg_LobbyPrepareEnterGameRequest, msgBuilder);
        Debug.Log("Send NetMsg_GamePrepareRequest");

        CompleteCount = 2;

        return Task.CompletedTask;
    }

    private void OnLobbyPrepareEnterGameRespond(int connectionId, Dictionary<int, object> message)
    {
        Debug.Log("Player OnGamePrepareRespond" + JsonConvert.SerializeObject(message));

        CompleteCount = 3;
        ErrorCode errorCode = (ErrorCode)message[((int)LobbyPrepareEnterGameRespond.ErrorCode)];

        if (errorCode == ErrorCode.Success)
        {
            var gameServerIP = message[((int)LobbyPrepareEnterGameRespond.GameServerIP)].ToString();
            var GameServerPort = message[((int)LobbyPrepareEnterGameRespond.GameServerPort)].ToString();
            ClientData.Instance.GameServerAddreas = string.Format("{0}:{1}", gameServerIP, GameServerPort);
            Task.Run(this.MissionComplete);

        }
        else // PrepareFailed
        {
            UIManager.Instance.CreateGameMessage(errorCode.ToString(), "Confirm",
                () => NetworkHandler.Instance.Disconnect(RemoteConnetionType.Lobby),
                () => Application.Quit());
        }

        Debug.Log("Player OnGamePrepareRespond complete");
        CompleteCount = 4;
    }

    protected async override Task MissionComplete()
    {
        CompleteCount = SubMissionCount;
        IsComplete = true;
        Debug.Log(Name + "Mission Complete" + ClientData.Instance.LobbyPlayerInfo.AccountId);
        NetworkHandler.Instance.UnRegisterMessageListener(MsgType.NetMsg_LobbyPrepareEnterGameRespond, OnLobbyPrepareEnterGameRespond);

        if (OnComplete != null)
        {
            await OnComplete.Invoke();
        }

        await Task.CompletedTask;
    }
}
