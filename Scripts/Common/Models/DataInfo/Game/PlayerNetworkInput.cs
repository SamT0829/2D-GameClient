using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Newtonsoft.Json;
using System.Linq;
using System;
using UnityEngine;

public enum NetworkInputButtons
{
    JUMP,
    FIRE,
    ThrowGrenade,
    RocketLauncherFire,
}

public class PlayerNetworkInput
{
    private Dictionary<NetworkInputButtons, bool> buttonsTable = new Dictionary<NetworkInputButtons, bool>();
    public Vector2 movementInput;
    public float gunRotationZ;
    public Vector3 gunAimDirection;

    public PlayerNetworkInput()
    {
    }

    public PlayerNetworkInput(float movementInputX, float movementInputY)
    {
        movementInput = new Vector2(movementInputX, movementInputY);
    }

    public bool OnClick()
    {
        bool onClick = buttonsTable.Values.FirstOrDefault(onButton => onButton == true);
        return onClick;
    }

    public void SetNetworkButtonInputData(NetworkInputButtons networkInputButtons, bool buttonState)
    {
        buttonsTable[networkInputButtons] = buttonState;
    }

    public bool GetNetworkButtonInputData(NetworkInputButtons networkInputButtons)
    {
        if (buttonsTable.TryGetValue(networkInputButtons, out bool _buttonState))
        {
            return _buttonState;
        }
        return false;
    }

    public void SendNetworkInputData()
    {
        MessageBuilder message = new MessageBuilder();
        message.AddMsg(((int)GamePlayerNetworkInputRequest.xVelocity), movementInput.x, NetMsgFieldType.Float);
        message.AddMsg(((int)GamePlayerNetworkInputRequest.yVelocity), movementInput.y, NetMsgFieldType.Float);
        message.AddMsg(((int)GamePlayerNetworkInputRequest.NetworkButon), buttonsTable, NetMsgFieldType.Object);

        // Weapon
        message.AddMsg(((int)GamePlayerNetworkInputRequest.GunRotationZ), gunRotationZ, NetMsgFieldType.Float);
        message.AddMsg(((int)GamePlayerNetworkInputRequest.GunAimDirection), gunAimDirection, NetMsgFieldType.UnityVector3);

        NetworkHandler.Instance.Send(RemoteConnetionType.Game, MsgType.NetMsg_GamePlayerNetworkInputRequest, message);
    }

    public List<object> CreateSerializedObject()
    {
        List<object> retv = new List<object>();
        retv.Add(movementInput.x);
        retv.Add(movementInput.y);
        retv.Add(gunRotationZ);
        retv.Add(gunAimDirection.x);
        retv.Add(gunAimDirection.y);

        var buttons = buttonsTable.ToDictionary(x => (int)x.Key, x => (object)x.Value);
        retv.Add(buttons);
        return retv;
    }

    public void DeserializeObject(object[] retv)
    {
        movementInput = new Vector2(Convert.ToSingle(retv[0]), Convert.ToSingle(retv[1]));
        gunRotationZ = Convert.ToSingle(retv[2]);
        gunAimDirection = new Vector2(Convert.ToSingle(retv[3]), Convert.ToSingle(retv[4]));

        if (retv[5].ToString() != null)
        {
            Dictionary<int, object> buttons = (Dictionary<int, object>)(retv[5]);
            buttonsTable = buttons.ToDictionary(x => (NetworkInputButtons)x.Key, x => (bool)x.Value);
        }
    }
}