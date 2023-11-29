using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class LobbyPlayerInfo
{
    public long AccountId = -1;
    public string NickName;
    public PlayerStatus Status { get; set; }
    public GameType GameType;
    public int TotalPlayCount;
    public int TotalLoseCount;
    public int TotalWinCount;

    // Player in LobbyRoom Setting
    public bool isReady { get; set; }

    public void DeserializeObject(object[] retv)
    {
        AccountId = Convert.ToInt64(retv[0]);
        NickName = retv[1].ToString();
        Status = (PlayerStatus)Convert.ToInt32(retv[2]);
        GameType = (GameType)Convert.ToInt32(retv[3]);
        TotalPlayCount = Convert.ToInt32(retv[4]);
        TotalLoseCount = Convert.ToInt32(retv[5]);
        TotalWinCount = Convert.ToInt32(retv[6]);
    }
}