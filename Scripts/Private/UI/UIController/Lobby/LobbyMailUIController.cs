using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using Newtonsoft.Json;

public class LobbyMailUIController : MonoBehaviour
{
    [Header("Mail Inbox Selection Button")]
    [SerializeField] Button InboxButton;
    [SerializeField] Button DeletedButton;

    [Header("Mail Inbox Selection Sprite")]
    [SerializeField] Sprite OnSelectSprite;
    [SerializeField] Sprite OnDesableSprite;
    private Button OnSelectionButton;

    [Header("Mail Message Prefab")]
    [SerializeField] MailMessagePrefab MailMessagePrefab;

    [Header("Mail Inbox Parent")]
    [SerializeField] Transform inboxMailMessageParent;
    [SerializeField] Transform deletedMailMessageParent;

    [Header("Mail Inbox Panel")]
    [SerializeField] GameObject InboxMailMessagePanel;
    [SerializeField] GameObject DeletedMailMessagePanel;

    [Header("Mail Container Componenet")]
    [SerializeField] Text MailTitle;
    [SerializeField] Text MailText;
    [SerializeField] Image ContentImage;
    [SerializeField] Image RewardImage;
    [SerializeField] Text RewardText;
    [SerializeField] Button RewardButton;
    [SerializeField] Button DeleteButton;

    LobbyMainUIController LobbyMainUIController;

    // mail messages stored as ScriptableObjects to simulate mail data
    List<MailMessageInfo> m_MailMessages = new List<MailMessageInfo>();
    List<MailMessagePrefab> m_InboxMessages = new List<MailMessagePrefab>();
    List<MailMessagePrefab> m_DeletedMessages = new List<MailMessagePrefab>();

    private void OnEnable()
    {
        NetworkHandler.Instance.RegisterMessageListener(MsgType.NetMsg_PlayerLobbyGoToMailRespond, OnPlayerLobbyGoToMailRespond);
    }

    private void OnDisable()
    {
        NetworkHandler.Instance.UnRegisterMessageListener(MsgType.NetMsg_PlayerLobbyGoToMailRespond, OnPlayerLobbyGoToMailRespond);
    }

    private void Awake()
    {
        LobbyMainUIController = GetComponentInParent<LobbyMainUIController>();

        InboxButton.onClick.AddListener(() => OnClickInboxButton(InboxButton));
        DeletedButton.onClick.AddListener(() => OnClickDeletedButton(DeletedButton));
    }

    private void OnClickInboxButton(Button button)
    {
        InboxMailMessagePanel.SetActive(true);
        DeletedMailMessagePanel.SetActive(false);
        ChooseMailSelectionButton(button);
    }
    private void OnClickDeletedButton(Button button)
    {
        InboxMailMessagePanel.SetActive(false);
        DeletedMailMessagePanel.SetActive(true);
        ChooseMailSelectionButton(button);
    }
    private void ChooseMailSelectionButton(Button selectionButton)
    {
        if (OnSelectionButton == selectionButton)
            return;

        SetActiveMailMessagePanel(false);

        InboxButton.GetComponent<Image>().sprite = OnDesableSprite;
        DeletedButton.GetComponent<Image>().sprite = OnDesableSprite;

        var image = selectionButton.GetComponent<Image>();
        if (image)
        {
            image.sprite = OnSelectSprite;
        }

        OnSelectionButton = selectionButton;
    }

    private void LoadMailMessages(List<MailMessageInfo> mailMessageInfoList)
    {
        // load the ScriptableObjects from the Resources directory (default = Resources/GameData/MailMessages)
        m_MailMessages = mailMessageInfoList;

        // separate lists for easier display
        BuildMailMessage(m_MailMessages.Where(x => !x.isDeleted).ToList(), m_InboxMessages, inboxMailMessageParent);
        BuildMailMessage(m_MailMessages.Where(x => x.isDeleted).ToList(), m_DeletedMessages, deletedMailMessageParent);

        UpdateView();
    }
    // Instantiate MailMessagePrefab
    private void BuildMailMessage(List<MailMessageInfo> mailMessageInfoList, List<MailMessagePrefab> messagePrefabs, Transform messageParent)
    {
        List<MailMessagePrefab> removePrefab = new List<MailMessagePrefab>();
        messagePrefabs.ForEach(prefab =>
        {
            var messageInfo = mailMessageInfoList.FirstOrDefault(info => info.messageId == prefab.Info.messageId);
            if (messageInfo == null)
            {
                removePrefab.Add(prefab);
            }
        });

        removePrefab.ForEach(prefab =>
        {
            messagePrefabs.Remove(prefab);
            Destroy(prefab.gameObject);
        });

        mailMessageInfoList.ForEach(info =>
        {
            var message = messagePrefabs.FirstOrDefault(prefab => prefab.Info.messageId == info.messageId);
            if (message)
            {
                message.UpdateMessage(info);
            }
            else
            {
                MailMessagePrefab mailMessage = Instantiate(MailMessagePrefab, messageParent);
                mailMessage.InitMailMessage(info, () => OnClickMailMessage(mailMessage));
                messagePrefabs.Add(mailMessage);
            }
        });
    }
    private void OnClickMailMessage(MailMessagePrefab mailMessagePrefab)
    {
        SetActiveMailMessagePanel(true);

        MailTitle.text = mailMessagePrefab.Info.titleText;
        MailText.text = mailMessagePrefab.Info.mailText;
        ContentImage.sprite = mailMessagePrefab.Info.mailPicAttachment;

        Sprite sprite = UIManager.Instance.GetSlotItemSprite(mailMessagePrefab.Info.rewardType);
        RewardImage.sprite = sprite;
        RewardText.text = mailMessagePrefab.Info.rewardValue.ToString();

        RewardButton.onClick.RemoveAllListeners();
        if (!mailMessagePrefab.Info.isClaimed)
        {
            RewardButton.interactable = true;
            RewardButton.onClick.AddListener(() => OnClickRewardButton(mailMessagePrefab));
        }
        else
        {
            RewardButton.interactable = false;
        }

        DeleteButton.onClick.RemoveAllListeners();
        if (mailMessagePrefab.Info.isDeleted)
        {
            DeleteButton.gameObject.SetActive(false);
        }
        else
        {
            DeleteButton.onClick.AddListener(() => OnClickDeleteButton(mailMessagePrefab));
        }

        mailMessagePrefab.Info.isNew = false;
        SendSyncMailMessageRequest(mailMessagePrefab.Info);
    }
    private void OnClickRewardButton(MailMessagePrefab mailMessagePrefab)
    {
        switch (mailMessagePrefab.Info.rewardType)
        {
            case SlotItemAllName.Gold:
                ClientData.Instance.PlayerAccountInfo.Money += mailMessagePrefab.Info.rewardValue;
                break;
        }

        mailMessagePrefab.Info.isClaimed = true;
        mailMessagePrefab.ResetMailMessage();
        RewardButton.onClick.RemoveAllListeners();
        SendSyncMailMessageRequest(mailMessagePrefab.Info);
    }
    private void OnClickDeleteButton(MailMessagePrefab mailMessagePrefab)
    {
        DeleteMessage(mailMessagePrefab);
        DeleteButton.onClick.RemoveAllListeners();
        SetActiveMailMessagePanel(false);
        SendSyncMailMessageRequest(mailMessagePrefab.Info);
    }

    // show the mailboxes in the MailScreen interface
    private void UpdateView()
    {
        //sort and generate elements from MailScreen
        m_InboxMessages = SortMailbox(m_InboxMessages);
        m_InboxMessages.ForEach(mailMessage => mailMessage.transform.SetSiblingIndex(m_InboxMessages.IndexOf(mailMessage)));

        m_DeletedMessages = SortMailbox(m_DeletedMessages);
        m_DeletedMessages.ForEach(mailMessage => mailMessage.transform.SetSiblingIndex(m_DeletedMessages.IndexOf(mailMessage)));
    }

    // order messages by validated Date property
    private List<MailMessagePrefab> SortMailbox(List<MailMessagePrefab> originalList)
    {
        return originalList.OrderBy(x => x.Info.Date).Reverse().ToList();
    }

    // returns one mail message from the inbox by index
    private MailMessagePrefab GetInboxMessage(int index)
    {
        if (index < 0 || index >= m_InboxMessages.Count)
            return null;

        return m_InboxMessages[index];
    }
    private MailMessagePrefab GetDeletedMessage(int index)
    {
        if (index < 0 || index >= m_DeletedMessages.Count)
            return null;

        return m_DeletedMessages[index];
    }
    private void MarkMessageAsRead(int indexToRead)
    {
        MailMessagePrefab msgToRead = GetInboxMessage(indexToRead);

        if (msgToRead != null && msgToRead.Info.isNew)
        {
            msgToRead.Info.isNew = false;
        }
    }
    private void DeleteMessage(int indexToDelete)
    {
        MailMessagePrefab msgToDelete = GetInboxMessage(indexToDelete);

        if (msgToDelete == null)
            return;

        DeleteMessage(msgToDelete);
    }
    private void DeleteMessage(MailMessagePrefab mailMessagePrefab)
    {
        // mark as deleted move from Inbox to Deleted List
        mailMessagePrefab.Info.isDeleted = true;
        m_DeletedMessages.Add(mailMessagePrefab);
        m_InboxMessages.Remove(mailMessagePrefab);

        mailMessagePrefab.transform.SetParent(deletedMailMessageParent);

        // rebuild the interface
        UpdateView();
    }
    private void SetActiveMailMessagePanel(bool active)
    {
        MailTitle.gameObject.SetActive(active);
        MailText.gameObject.SetActive(active);
        ContentImage.gameObject.SetActive(active);
        RewardImage.gameObject.SetActive(active);
        RewardText.gameObject.SetActive(active);
        RewardButton.gameObject.SetActive(active);
        DeleteButton.gameObject.SetActive(active);
    }
    private void OnPlayerLobbyGoToMailRespond(int connectionId, Dictionary<int, object> message)
    {
        Debug.Log(JsonConvert.SerializeObject(message));
        List<MailMessageInfo> mailMessageInfos = new List<MailMessageInfo>();
        if (DictionaryMethod.RetrivieClassData(message, PlayerLobbyGoToMailRespond.MailInfo, out object mailMessage))
        {
            foreach (var mailInfo in (object[])mailMessage)
            {
                MailMessageInfo mailMessageInfo = new MailMessageInfo();
                mailMessageInfo.DeserializeObject((object[])mailInfo);
                mailMessageInfos.Add(mailMessageInfo);
            }
        }

        LobbyMainUIController.ActivePanel(GetComponent<RectTransform>());
        LoadMailMessages(mailMessageInfos);
        SetActiveMailMessagePanel(false);
    }
    private void RemoveAllMessage()
    {
        m_InboxMessages.ForEach(mail => Destroy(mail.gameObject));
        m_DeletedMessages.ForEach(mail => Destroy(mail.gameObject));
    }
    private void SendSyncMailMessageRequest(MailMessageInfo mailMessageInfo)
    {
        List<object> outMessage = new List<object>();
        m_MailMessages.ForEach(info => outMessage.Add(info.SerializeObject().ToArray()));

        MessageBuilder messageBuilder = new MessageBuilder();
        messageBuilder.AddMsg(((int)PlayerLobbySyncMailMessageRequest.MailInfo), outMessage.ToArray(), NetMsgFieldType.Array);
        NetworkHandler.Instance.Send(RemoteConnetionType.Lobby, MsgType.NetMsg_PlayerLobbySyncMailMessageRequest, messageBuilder);
    }
}
