using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyRoomPlayerInfo
{
    public int RoomId { get; set; }
    public long AccountId { get; set; }
    public string NickName { get; set; }
    public GameType GameType { get; set; }
    public string PlayerDataStatus { get; set; }

    // Player in LobbyRoom Setting
    public bool isReady { get; set; }


    public LobbyRoomPlayerInfo()
    {
        isReady = false;
    }

    public void DeserializeObject(object[] retv)
    {
        NickName = retv[0].ToString();
        isReady = Convert.ToBoolean(retv[1]);
    }
}
