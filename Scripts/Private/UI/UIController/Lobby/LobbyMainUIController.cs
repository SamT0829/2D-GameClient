using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;


public class LobbyMainUIController : MonoBehaviour
{
    [Header("Lobby Main Button")]
    [SerializeField] Button HomeButton;
    [SerializeField] Button GameButton;
    [SerializeField] Button NotificationButton;
    [SerializeField] Button ShopButton;
    [SerializeField] Button MailButton;

    [Header("Lobby Main Panel")]
    [SerializeField] RectTransform HomePanel;
    [SerializeField] RectTransform GamePanel;
    [SerializeField] RectTransform NotificationPanel;
    [SerializeField] RectTransform ShopPanel;
    [SerializeField] RectTransform MailPanel;

    RectTransform nowPanel;

    [SerializeField] RectTransform ClickMarkerRectTransform;

    [Header("Lobby Panel UI")]
    [SerializeField] RectTransform LobbyGamePanel;
    [SerializeField] GameObject LobbyMainPanel;

    [Header("Lobby Main Child Component")]
    [SerializeField] private Text GoldText;
    [SerializeField] private Text DiamondText;

    private void OnEnable()
    {
        EventManager.Instance.RegisterEventListener<GameControlEvent>(GameControlMessageEvent.GameDisconnected.GetHashCode(), OnGameDisconnected);
    }

    private void OnDisable()
    {
        EventManager.Instance.UnRegisterEventListener<GameControlEvent>(GameControlMessageEvent.GameDisconnected.GetHashCode(), OnGameDisconnected);
    }

    private void Awake()
    {
        // Lobby Button Callback
        HomeButton.onClick.AddListener(() => OnClickHomeButton(HomeButton));
        GameButton.onClick.AddListener(() => OnClickGameButton(GameButton));
        NotificationButton.onClick.AddListener(() => OnClickNotificationButton(NotificationButton));
        ShopButton.onClick.AddListener(() => OnClickShopButton(ShopButton));
        MailButton.onClick.AddListener(() => OnClickMailButton(MailButton));
    }


    private void Update()
    {
        GoldText.text = ClientData.Instance.PlayerAccountInfo.Money.ToString();
        DiamondText.text = ClientData.Instance.PlayerAccountInfo.Diamond.ToString();
    }

    // Button Listener
    private void OnClickHomeButton(Button buttonObject)
    {
        ActivateButton(buttonObject);
        ActivePanel(HomePanel);
    }
    private void OnClickGameButton(Button buttonObject)
    {
        ActivateButton(buttonObject);
        ActivePanel(GamePanel);
    }
    private void OnClickNotificationButton(Button buttonObject)
    {
        ActivateButton(buttonObject);
    }
    private void OnClickShopButton(Button buttonObject)
    {
        ActivateButton(buttonObject);
        MessageBuilder msgBuilder = new MessageBuilder();
        NetworkHandler.Instance.Send(RemoteConnetionType.Lobby, MsgType.NetMsg_PlayerLobbyGoToShopRequest, msgBuilder);
    }
    private void OnClickMailButton(Button buttonObject)
    {
        ActivateButton(buttonObject);
        MessageBuilder msgBuilder = new MessageBuilder();
        NetworkHandler.Instance.Send(RemoteConnetionType.Lobby, MsgType.NetMsg_PlayerLobbyGoToMailRequest, msgBuilder);
    }

    // Lobby Main Button function
    private void ActivateButton(Button buttonObject)
    {
        ResetAllButton();
        MoveClickMarker(buttonObject);

        // Change button sizeDelta
        var rectTransform = buttonObject.GetComponent<RectTransform>();
        rectTransform.pivot = new Vector2(0, 0.5f);
        rectTransform.anchoredPosition = new Vector2(-100, rectTransform.anchoredPosition.y);
        rectTransform.DOSizeDelta(new Vector2(250, 200), 0.5f).SetEase(Ease.OutQuint);

        // Change Image localPosition
        var image = buttonObject.transform.Find("Image");
        image.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 25, 0);

        //Set Active Click object text
        var text = buttonObject.transform.Find("Text");
        text.gameObject.SetActive(true);
    }
    private void ResetAllButton()
    {
        Action<Button> resetButton = buttonObject =>
        {
            // Change button RectTransform
            buttonObject.GetComponent<RectTransform>().DOSizeDelta(new Vector2(200, 200), 0.5f).SetEase(Ease.OutQuint);

            // Change Image localPosition
            var image = buttonObject.transform.Find("Image");
            image.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);

            //Set Active Click object text
            var text = buttonObject.transform.Find("Text");
            text.gameObject.SetActive(false);

        };

        // Reset All button property
        resetButton.Invoke(HomeButton);
        resetButton.Invoke(GameButton);
        resetButton.Invoke(NotificationButton);
        resetButton.Invoke(ShopButton);
        resetButton.Invoke(MailButton);
    }
    private void MoveClickMarker(Button buttonObject)
    {
        ClickMarkerRectTransform.SetParent(buttonObject.transform);
        ClickMarkerRectTransform.localPosition = new Vector3(ClickMarkerRectTransform.localPosition.x, 0, 0);
    }
    public void ActivePanel(RectTransform panel)
    {
        if (nowPanel && nowPanel == panel)
        {
            return;
        }

        if (nowPanel)
            nowPanel.DOLocalMove(new Vector3(nowPanel.localPosition.x, nowPanel.sizeDelta.y), 0.5f).SetEase(Ease.OutQuint);

        nowPanel = panel;
        panel.DOLocalMove(new Vector3(panel.localPosition.x, 0), 0.5f).SetEase(Ease.OutQuint);
    }

    // Event Function
    private void OnGameDisconnected(IEvent e)
    {
        UIManager.Instance.CreateGameMessage("Game Disconnect", "Quit Game", null, Application.Quit);
    }
}