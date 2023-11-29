using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameRoomState
{
    WaitingEnter,
    EnterFinish,
    GameStart,
}

public class GameRoomInfo
{
    public int RoomId { get; set; }
    public string RoomName { get; set; }
    public GameType RoomGameType { get; set; }
    public GameRoomState RoomState { get; set; }
    public List<LobbyPlayerInfo> playerDataList = new List<LobbyPlayerInfo>();

    public void InitGameData(LobbyRoomInfo lobbyRoomInfo)
    {
        RoomId = lobbyRoomInfo.RoomId;
        RoomName = lobbyRoomInfo.RoomName;
        RoomGameType = GameType.MultiplayerGame;
        RoomState = GameRoomState.WaitingEnter;

        foreach (var playerData in lobbyRoomInfo.LobbyPlayerInfoList)
        {
            playerDataList.Add(playerData);
        }
    }
}