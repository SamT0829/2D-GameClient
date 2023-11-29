using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;

public class NetworkHandler : MonoBehaviour
{
    private static NetworkHandler _instance;
    public static NetworkHandler Instance { get { return _instance; } }

    private Dictionary<RemoteConnetionType, ClientPeer> remoteConnectorTable = new Dictionary<RemoteConnetionType, ClientPeer>();
    private Dictionary<MsgType, Action<int, Dictionary<int, object>>> messageDispatchTable = new Dictionary<MsgType, Action<int, Dictionary<int, object>>>();

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            _instance = this;


        Application.runInBackground = true;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        NetworkUpdate();
    }

    private void NetworkUpdate()
    {
        if (remoteConnectorTable.Count > 0)
        {
            var peerList = remoteConnectorTable.Values.ToList();
            peerList.ForEach(peer => peer.Update());
        }
    }

    public void Connect(RemoteConnetionType connectionId, string serverAddreas, string serverName)
    {
        if (remoteConnectorTable.TryGetValue(connectionId, out ClientPeer peer))
        {
            peer.Disconnect();
            remoteConnectorTable.Remove(connectionId);
            Debug.Log("peer Disconnect with ID: " + serverAddreas + connectionId);
        }

        remoteConnectorTable[connectionId] = new ClientPeer(connectionId);
        remoteConnectorTable[connectionId].Connect(serverAddreas, serverName);
        Debug.Log("peer connect with ID: " + serverAddreas + connectionId);
    }

    public void Disconnect(RemoteConnetionType connectionId)
    {
        if (remoteConnectorTable.TryGetValue(connectionId, out ClientPeer peer))
        {
            peer.Disconnect();
            remoteConnectorTable.Remove(connectionId);
            Debug.Log("peer Disconnect with ID: " + connectionId);

        }
    }

    public bool IsConnect(RemoteConnetionType connectionId)
    {
        return remoteConnectorTable.ContainsKey(connectionId);
    }

    /**註冊ClientCallBack訊息*/
    public void RegisterMessageListener(MsgType msgType, Action<int, Dictionary<int, object>> callback)
    {
        Action<int, Dictionary<int, object>> listener;
        if (!messageDispatchTable.TryGetValue(msgType, out listener))
            messageDispatchTable[msgType] = callback;

        else
        {
            if (messageDispatchTable[msgType] != callback)
                messageDispatchTable[msgType] += callback;
        }
    }

    public void UnRegisterMessageListener(MsgType msgType, Action<int, Dictionary<int, object>> callback)
    {
        if (messageDispatchTable.TryGetValue(msgType, out Action<int, Dictionary<int, object>> listener))
        {
            messageDispatchTable[msgType] -= callback;

            if (messageDispatchTable[msgType] == null)
                messageDispatchTable.Remove(msgType);
        }
    }

    public void Send(RemoteConnetionType connectionId, MsgType msgType, MessageBuilder message)
    {
        if (!IsConnect(connectionId))
        {
            Debug.Log($"Peer {connectionId} Disconnect");
            return;
        }

        MessageBuilder builtMessage = message;

        //<-----------------判斷是否為玩家訊息------------------------>
        if (msgType > MsgType.NetMsg_PlayerMessageBegin && msgType < MsgType.NetMsg_PlayerMessageEnd)
        {
            MessageBuilder playerMessage = new MessageBuilder();
            playerMessage.AddMsg(((int)PlayerFieldIndicator.MessageType), (int)msgType, NetMsgFieldType.Int);
            playerMessage.AddMsg(((int)PlayerFieldIndicator.MessageData), message.BuildMsg(), NetMsgFieldType.Object);

            msgType = MsgType.NetMsg_PlayerMessage;
            builtMessage = playerMessage;
        }

        if (msgType > MsgType.NetMsg_GamePlayerMessageBegin && msgType < MsgType.NetMsg_GamePlayerMessageEnd)
        {
            MessageBuilder gamePlayerMessage = new MessageBuilder();
            gamePlayerMessage.AddMsg(((int)PlayerFieldIndicator.MessageType), (int)msgType, NetMsgFieldType.Int);
            gamePlayerMessage.AddMsg(((int)PlayerFieldIndicator.MessageData), message.BuildMsg(), NetMsgFieldType.Object);
            msgType = MsgType.NetMsg_GamePlayerMessage;
            builtMessage = gamePlayerMessage;
        }

        MessageBuilder outMessage = new MessageBuilder();
        outMessage.AddNetMsg(((byte)FieldIndicator.MessageID), (int)msgType, NetMsgFieldType.Int);
        outMessage.AddNetMsg(((byte)FieldIndicator.RemoteType), (int)RemoteConnetionType.Client, NetMsgFieldType.Int);
        outMessage.AddNetMsg(((byte)FieldIndicator.Data), builtMessage.BuildMsg(), NetMsgFieldType.Object);
        outMessage.AddNetMsg(((byte)FieldIndicator.SelfDefinedType), true, NetMsgFieldType.Boolean);
        ((ClientPeer)remoteConnectorTable[connectionId]).Send(NetOperationCode.ClientServer, outMessage.BuildNetMsg());
    }

    public void OnMessageArrived(Dictionary<byte, object> rowMessage)
    {
        MsgType msgType = (MsgType)rowMessage[((byte)FieldIndicator.MessageID)];
        RemoteConnetionType remoteConnetionType = (RemoteConnetionType)rowMessage[((byte)FieldIndicator.RemoteType)];
        Dictionary<int, object> userData = (Dictionary<int, object>)rowMessage[((byte)FieldIndicator.Data)];

        if (msgType == MsgType.NetMsg_PlayerMessage)
        {
            msgType = (MsgType)userData[((byte)PlayerFieldIndicator.MessageType)];
            userData = (Dictionary<int, object>)userData[((byte)PlayerFieldIndicator.MessageData)];
        }

        if (msgType == MsgType.NetMsg_GamePlayerMessage)
        {
            msgType = (MsgType)userData[((byte)PlayerFieldIndicator.MessageType)];
            userData = (Dictionary<int, object>)userData[((byte)PlayerFieldIndicator.MessageData)];
        }

        DispatchEvent(msgType, remoteConnetionType, userData);
    }

    private void DispatchEvent(MsgType msgType, RemoteConnetionType remoteConnetionType, Dictionary<int, object> message)
    {
        if (messageDispatchTable.ContainsKey(msgType))
            messageDispatchTable[msgType].Invoke(((byte)remoteConnetionType), message);
    }
}
