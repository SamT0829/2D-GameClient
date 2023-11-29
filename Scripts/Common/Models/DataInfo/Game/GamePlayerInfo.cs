using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public enum PlayerCharacterType
{
    None,
    Player,
    Farmer,
}
public enum GamePlayerState
{
    WaitJoin,
    JoinFinish,
    StartGame,
}

public class GamePlayerInfo
{
    public long AccountId { get; set; }
    public string NickName { get; set; }
    public int RoomId { get; set; }
    public GamePlayerState GamePlayerState { get; set; }

    // Game Player Data
    public PlayerCharacterType PlayerCharacter{get;set;}
    public Vector3 PlayerPosition { get; set; }
    public Vector3 PlayerLocalScale { get; set; }
    public int PlayerHealth { get; set; }
    public int PlayerMaxHealth { get; set; }
    public float PlayerEnergy { get; set; }
    public float PlayerMaxEnergy { get; set; }
    public int PlayerDamage { get; set; }

    // Game Player bool
    public bool IsPlayerDie { get; set; }

    // GameData 
    public int KillCount { get; set; }
    public int DeathCount { get; set; }
    public int CointCount { get; set; }

    public GamePlayerInfo()
    {
    }

    public void DeserializeObject(object[] retv)
    {
        AccountId = Convert.ToInt64(retv[0]);
        NickName = retv[1].ToString();
        PlayerPosition = new Vector3(Convert.ToSingle(retv[2]), Convert.ToSingle(retv[3]), Convert.ToSingle(retv[4]));
        PlayerLocalScale = new Vector3(Convert.ToSingle(retv[5]), Convert.ToSingle(retv[6]), Convert.ToSingle(retv[7]));
        PlayerHealth = Convert.ToInt32(retv[8]);
        PlayerMaxHealth = Convert.ToInt32(retv[9]);
        PlayerEnergy = Convert.ToSingle(retv[10]);
        PlayerMaxEnergy = Convert.ToSingle(retv[11]);
        PlayerDamage = Convert.ToInt32(retv[12]);
        IsPlayerDie = Convert.ToBoolean(retv[13]);

        // Game Data
        KillCount = Convert.ToInt32(retv[14]);
        DeathCount = Convert.ToInt32(retv[15]);
        CointCount = Convert.ToInt32(retv[16]);
    }
}
