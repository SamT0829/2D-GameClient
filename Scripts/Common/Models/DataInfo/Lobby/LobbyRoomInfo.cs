using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;


public enum RoomMessage
{
    InfoMessage,
    WarningMessage,
    PlayerMessage,
}

[Serializable]
public class LobbyRoomInfo
{
    public int RoomId;
    public string RoomPassword;
    public string RoomName;
    public int MaxPlayer;
    public string HostPlayer;
    public List<LobbyPlayerInfo> LobbyPlayerInfoList = new List<LobbyPlayerInfo>();
    public List<LobbyRoomPlayerInfo> LobbyRoomPlayerInfosList = new List<LobbyRoomPlayerInfo>();

    public LobbyRoomInfo()
    {

    }

    public LobbyRoomInfo(int roomId, string roomName, int maxPlayerSetting, List<LobbyPlayerInfo> lobbyPlayerInfoList)
    {
        RoomId = roomId;
        RoomName = roomName;
        MaxPlayer = maxPlayerSetting;
        LobbyPlayerInfoList = lobbyPlayerInfoList;
    }

    public void DeserializeObject(object[] retv)
    {
        RoomId = Convert.ToInt32(retv[0]);
        RoomPassword = retv[1].ToString();
        RoomName = retv[2].ToString();
        MaxPlayer = Convert.ToInt32(retv[3]);
        HostPlayer = retv[4].ToString();

        object[] lobbyPlayerDataList = (object[])retv[5];
        foreach (var lobbyPlayerData in lobbyPlayerDataList)
        {
            LobbyPlayerInfo lobbyPlayerInfo = new LobbyPlayerInfo();
            lobbyPlayerInfo.DeserializeObject((object[])lobbyPlayerData);
            LobbyPlayerInfoList.Add(lobbyPlayerInfo);
        }

        object[] lobbyRommPlayerDataList = (object[])retv[6];
        foreach (var lobbyRoomPlayerData in lobbyRommPlayerDataList)
        {
            LobbyRoomPlayerInfo lobbyRoomPlayerInfo = new LobbyRoomPlayerInfo();
            lobbyRoomPlayerInfo.DeserializeObject((object[])lobbyRoomPlayerData);
            LobbyRoomPlayerInfosList.Add(lobbyRoomPlayerInfo);
        }
    }
}