using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRoomPrefab : MonoBehaviour
{
    public int RoomId;
    public string RoomPassword;

    [SerializeField]
    Button GameRoomButton;
    [SerializeField]
    Text GameRoomId;
    [SerializeField]
    Text GameRoomName;
    [SerializeField]
    Text GamePlayerCount;


    private void Start()
    {
        GameRoomButton.onClick.AddListener(JoinRoom);
    }

    public void SettingRoomPrefab(LobbyRoomInfo lobbyRoomInfo)
    {
        RoomId = Convert.ToInt32(lobbyRoomInfo.RoomId);
        RoomPassword = lobbyRoomInfo.RoomPassword;

        GameRoomId.text = lobbyRoomInfo.RoomId.ToString();
        GameRoomName.text = lobbyRoomInfo.RoomName;
        GamePlayerCount.text = string.Format("Player {0}/{1}", lobbyRoomInfo.LobbyPlayerInfoList.Count, lobbyRoomInfo.MaxPlayer);
    }

    public void SettingRoomPrefab(string roomIdText, string gameNameText, string maxPlayerText)
    {
        RoomId = Convert.ToInt32(roomIdText);
        GameRoomId.text = roomIdText;
        GameRoomName.text = gameNameText;
        GamePlayerCount.text = maxPlayerText;
    }

    private void JoinRoom()
    {
        if (RoomPassword == string.Empty)
        {
            //Send Network Message
            MessageBuilder msgBuilder = new MessageBuilder();
            msgBuilder.AddMsg(((int)JoinGameRoomRequest.RoomId), Convert.ToInt32(GameRoomId.text), NetMsgFieldType.Int);
            msgBuilder.AddMsg(((int)JoinGameRoomRequest.RoomName), GameRoomName.text, NetMsgFieldType.String);
            msgBuilder.AddMsg(((int)JoinGameRoomRequest.RoomPassword), "", NetMsgFieldType.String);
            // msgBuilder.AddMsg(((int)JoinGameRomeRequest.PlayerData), GameManager.Instance.PlayerStatus, NetMsgFieldType.String);
            NetworkHandler.Instance.Send(RemoteConnetionType.Lobby, MsgType.NetMsg_PlayerJoinLobbyRoomRequest, msgBuilder);
        }
    }
}
