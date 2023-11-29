using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class GameStaticInfo
{
    public int PlayerCount;
    public List<GamePlayerInfo> GameRoomInfo = new List<GamePlayerInfo>();

    public void InitGameStaticInfo(int playerCount, long serverTime)
    {
        PlayerCount = playerCount;
    }

    public void SyncGameRoomStaticInfo(List<GamePlayerInfo> gameRoomData)
    {
        GameRoomInfo = gameRoomData;
    }

    public void DeserializeGameStaticObject(object[] retv)
    {
        PlayerCount = Convert.ToInt32(retv[0]);

        object[] playerData = (object[])(retv[1]);

        Array.ForEach(playerData, playerInfo =>
        {
            GamePlayerInfo gamePlayerInfo = new GamePlayerInfo();
            gamePlayerInfo.DeserializeObject((object[])playerInfo);
            GameRoomInfo.Add(gamePlayerInfo);
        });
    }
}