using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using UnityEngine.Events;

public class GameMessagePrefab : MonoBehaviour
{
    [SerializeField] Text GameMessageText;
    [SerializeField] Button GameMessageButton;
    [SerializeField] Text GameMessageButtonText;

    public void BuildGameMessage(string gameMessageText, string gameMessageButtonText, Action gameMessageCallBack, Action buttonCallBack)
    {
        GameMessageText.text = gameMessageText;
        GameMessageButtonText.text = gameMessageButtonText;

        if (gameMessageCallBack != null)
            gameMessageCallBack.Invoke();

        GameMessageButton.onClick.AddListener(() =>
        {
            if (buttonCallBack != null)
                buttonCallBack.Invoke();

            DestroyGameMessage();
        });
    }

    public void ResetGameMessage()
    {
        GameMessageText.text = string.Empty;
        GameMessageButtonText.text = string.Empty;

        GameMessageButton.onClick.RemoveAllListeners();
    }

    public void DestroyGameMessage()
    {
        ResetGameMessage();
        Destroy(gameObject);
    }
}