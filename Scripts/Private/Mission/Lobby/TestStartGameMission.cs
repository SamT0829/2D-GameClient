using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class TestStartGameMission : MissionBase
{
    protected override async Task MissionProcess()
    {
        if (OnProcess != null)
            await OnProcess.Invoke();

        CompleteCount = 1;

        NetworkHandler.Instance.RegisterMessageListener(MsgType.NetMsg_LobbyGameRoomStartGame, OnLobbyGameRoomStartGame);
        MessageBuilder outMessage = new MessageBuilder();
        outMessage.AddMsg(((int)LobbyTestStartGameRequest.AccountId), ClientData.Instance.LobbyPlayerInfo.AccountId, NetMsgFieldType.Long);

        NetworkHandler.Instance.Send(RemoteConnetionType.Lobby, MsgType.NetMsg_LobbyTestGameRoomStartGame, outMessage);

        Debug.Log("Send NetMsg_LobbyTestGameRoomStartGame");

        CompleteCount = 2;

        await Task.CompletedTask;
    }

    private void OnLobbyGameRoomStartGame(int connectionId, Dictionary<int, object> message)
    {
        CompleteCount = 3;
        Debug.Log("OnLobbyGameRoomStartGame message " + JsonConvert.SerializeObject(message));

        ErrorCode errorCode = (ErrorCode)message[((int)LobbyGameRoomStartGame.ErrorCode)];
        if (errorCode == ErrorCode.Success)
        {
            Task.Run(this.MissionComplete);
        }

        CompleteCount = 4;
    }
}

