using System;
using UnityEngine;

public class PlayerControlEvent : IEvent
{
    private PlayerControlMessageEvent _message;
    public GamePlayerInfo GamePlayerInfo;
    public GameObject TargetGameObject;

    public PlayerControlEvent(PlayerControlMessageEvent message, GamePlayerInfo playerInfo, GameObject targetGameObject)
    {
        _message = message;
        GamePlayerInfo = playerInfo;
        TargetGameObject = targetGameObject;
    }

    public int GetMessageKey()
    {
        return Convert.ToInt32(_message);
    }

    public bool GetSendAll()
    {
        return false;
    }
}
