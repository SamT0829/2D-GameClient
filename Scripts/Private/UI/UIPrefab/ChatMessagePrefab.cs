using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ChatMessagePrefab : MonoBehaviour
{
    public string text;
    [SerializeField] Text messageText;

    private void Awake()
    {
        if (messageText == null)
            messageText = GetComponent<Text>();
    }
    public void BuildChatMessage(List<object> chatData)
    {
        RoomMessage roomMessage = (RoomMessage)Convert.ToInt32(chatData[0]);
        var name = chatData[1].ToString();
        var chatMessage = chatData[2].ToString();

        switch (roomMessage)
        {
            case RoomMessage.InfoMessage:
                messageText.color = Color.blue;
                text = string.Format("{0} {1}", name, chatMessage);
                break;
            case RoomMessage.PlayerMessage:
                messageText.color = Color.black;
                text = string.Format("{0} : {1}", name, chatMessage);
                break;
            case RoomMessage.WarningMessage:
                messageText.color = Color.red;
                text = string.Format("{0} {1}", name, chatMessage);
                break;
        }

        messageText.text = text;
    }
}
