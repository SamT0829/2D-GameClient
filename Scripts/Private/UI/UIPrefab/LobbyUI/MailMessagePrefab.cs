using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class MailMessagePrefab : MonoBehaviour
{
    public MailMessageInfo Info;

    [Header("Child")]
    [SerializeField] Button MailMessageButton;
    [SerializeField] Image MailMessageImage;
    [SerializeField] Text MailMessageSubject;
    [SerializeField] Text MailMessageDate;
    [SerializeField] Image MailMessageBadge;


    [Header("Mail Sprite")]
    [SerializeField] Sprite MailSpriteOpen;
    [SerializeField] Sprite MailSpriteClose;

    public void InitMailMessage(MailMessageInfo mailMessageInfo, Action onClickMailMessage)
    {
        Info = mailMessageInfo;

        if (Info.isNew)
            MailMessageImage.sprite = MailSpriteClose;
        else
            MailMessageImage.sprite = MailSpriteOpen;

        if (!Info.isClaimed)
            MailMessageBadge.gameObject.SetActive(true);
        else
            MailMessageBadge.gameObject.SetActive(false);

        MailMessageSubject.text = Info.titleText;
        MailMessageDate.text = Info.date;

        MailMessageButton.onClick.AddListener(() =>
        {
            if (onClickMailMessage != null)
                onClickMailMessage.Invoke();

            OnClickMailMessage();
        });
    }

    public void UpdateMessage(MailMessageInfo mailMessageInfo)
    {
        Info = mailMessageInfo;
        ResetMailMessage();
    }

    public void ResetMailMessage()
    {
        if (Info.isNew)
            MailMessageImage.sprite = MailSpriteClose;
        else
            MailMessageImage.sprite = MailSpriteOpen;

        if (!Info.isClaimed)
            MailMessageBadge.gameObject.SetActive(true);
        else
            MailMessageBadge.gameObject.SetActive(false);
    }

    // open messageImage sprite
    private void OnClickMailMessage()
    {
        ResetMailMessage();
    }
}
