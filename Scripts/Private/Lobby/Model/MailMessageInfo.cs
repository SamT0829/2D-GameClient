using UnityEngine;
using System;
using System.Collections.Generic;

public class MailMessageInfo
{
    // mail message id
    public int messageId;

    // appears as a title 
    public string titleText;

    // format: MM/dd/yyyy
    public string date;

    // body of mail text
    [TextArea]
    public string mailText;

    // image at end of mail
    public Sprite mailPicAttachment;

    // type of free shopItem
    public SlotItemAllName rewardType;

    // footer of email shows a free shopItem
    public uint rewardValue;

    // has the gift been claimed
    public bool isClaimed;

    // important messages show a badge next to sender
    public bool isImportant;

    // has not been read
    public bool isNew;

    // deleted messages appear in the second tab
    public bool isDeleted;

    const int maxSubjectLine = 14;

    // validate DateTime for sorting
    public DateTime Date
    {
        get
        {
            DateTime dt;

            if (DateTime.TryParse(date, out dt))
            {
                String.Format("{0:MM/dd/yyyy}", dt);
            }
            else
            {
                dt = new DateTime();
            }

            return dt;
        }
    }

    public string SubjectLine
    {
        get
        {
            if (string.IsNullOrEmpty(titleText))
            {
                return "...";
            }
            return (titleText.Length < maxSubjectLine) ? titleText : titleText.Substring(0, Math.Min(titleText.Length, maxSubjectLine)) + "...";
        }
    }

    public List<object> SerializeObject()
    {
        List<object> retv = new List<object>();
        retv.Add(messageId);
        retv.Add(titleText);
        retv.Add(date);
        retv.Add(mailText);
        retv.Add(mailPicAttachment.name);
        retv.Add(rewardType);
        retv.Add(Convert.ToInt32(rewardValue));
        retv.Add(isClaimed);
        retv.Add(isImportant);
        retv.Add(isNew);
        retv.Add(isDeleted);

        return retv;
    }

    public void DeserializeObject(object[] retv)
    {
        messageId = Convert.ToInt32(retv[0]);
        titleText = retv[1].ToString();
        date = retv[2].ToString();
        mailText = retv[3].ToString();
        mailPicAttachment = Resources.Load<Sprite>("SlotItem/" + retv[4].ToString());
        rewardType = (SlotItemAllName)retv[5];
        rewardValue = Convert.ToUInt32(retv[6]);
        isClaimed = Convert.ToBoolean(retv[7]);
        isImportant = Convert.ToBoolean(retv[8]);
        isNew = Convert.ToBoolean(retv[9]);
        isDeleted = Convert.ToBoolean(retv[10]);
    }
}