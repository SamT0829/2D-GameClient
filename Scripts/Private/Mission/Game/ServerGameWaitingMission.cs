using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ServerGameWaitingMission : MissionBase
{
    protected override Task MissionProcess()
    {
        if (OnProcess != null)
            OnProcess.Invoke();

        CompleteCount = 1;

    //    NetworkHandler.Instance.RegisterMessageListener(MsgType.NetMsg_GameWaitingRespond, OnGameWaitingRespond);

    //     MessageBuilder msgBuilder = new MessageBuilder();
    //     msgBuilder.AddMsg(((int)GamePrepareRequest.AccountId), ((int)ClientData.Instance.PlayerData.AccountId), NetMsgFieldType.Long);
    //     NetworkHandler.Instance.Send(RemoteConnetionType.Game, MsgType.NetMsg_GameWaitingRequest, msgBuilder);
    //     Debug.Log("Send NetMsg_GameWaitingRespond");

        CompleteCount = 2;

        return Task.CompletedTask;
    }

    private void OnGameWaitingRespond(int connectionId, Dictionary<int, object> message)
    {

    }
}
