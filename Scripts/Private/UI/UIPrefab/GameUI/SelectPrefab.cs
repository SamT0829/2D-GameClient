using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPrefab : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] private Text TitleText;
    [SerializeField] private Text RewardText;
    [SerializeField] private Image SlotImage1;
    [SerializeField] private Image SlotImage2;
    [SerializeField] private Image SlotImage3;
    [SerializeField] private Text SlotText1;
    [SerializeField] private Text SlotText2;
    [SerializeField] private Text SlotText3;
    [SerializeField] private Button MenuButton;
    [SerializeField] private Button PlayButton;
    [SerializeField] private Button BackButton;


    public void InitRewardPrefab(string titleText, string rewardText, string slotItem1, string slotItem2,
        string slotItem3, string slotText1, string slotText2, string slotText3)
    {
        TitleText.text = titleText;
        RewardText.text = rewardText;

        string path = "ShopItem/" + slotItem1;
        Sprite sp = Resources.Load<Sprite>(path);
        SlotImage1.sprite = sp;

        path = "ShopItem/" + slotItem2;
        sp = Resources.Load<Sprite>(path);
        SlotImage2.sprite = sp;

        path = "ShopItem/" + slotItem3;
        sp = Resources.Load<Sprite>(path);
        SlotImage3.sprite = sp;

        SlotText1.text = slotText1;
        SlotText2.text = slotText2;
        SlotText3.text = slotText3;
    }


}
