using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRoomPlayerPrefab : MonoBehaviour
{
    public LobbyPlayerInfo LobbyPlayerInfo;
    [SerializeField] Text PlayerName;
    [SerializeField] Image ReadyImage;
    [SerializeField] Image HostImage;

    [SerializeField] Sprite PlayerReadyImage;
    [SerializeField] Sprite PlayerNotReadyImage;


    private void Awake()
    {
        PlayerName = GetComponentInChildren<Text>();
        HostImage.gameObject.SetActive(false);
    }

    public void SettingGameRoomPlayerPrefab(LobbyPlayerInfo lobbyPlayerInfo)
    {
        LobbyPlayerInfo = lobbyPlayerInfo;
        PlayerName.text = lobbyPlayerInfo.NickName;
    }

    //Network Update
    public void UpdateNetworkSetting(LobbyRoomPlayerInfo lobbyRoomPlayerInfo, LobbyRoomInfo lobbyRoomInfo)
    {
        SetReady(lobbyRoomPlayerInfo.isReady);
        HostImageSetActive(lobbyRoomPlayerInfo.NickName == lobbyRoomInfo.HostPlayer);
    }

    private void SetReady(bool ready)
    {
        if (ready)
        {
            ReadyImage.sprite = PlayerReadyImage;
        }
        else
            ReadyImage.sprite = PlayerNotReadyImage;
    }

    private void HostImageSetActive(bool value)
    {
        HostImage.gameObject.SetActive(value);
    }
}
