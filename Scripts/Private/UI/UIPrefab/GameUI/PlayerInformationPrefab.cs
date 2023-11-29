using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerInformationPrefab : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] Text PlayerNameText;
    [SerializeField] Text PlayerKillText;
    [SerializeField] Text PlayerDeathText;
    [SerializeField] Text PlayerCointText;

    public void InitPlayerInformation(GamePlayerInfo gamePlayerInfo)
    {
        PlayerNameText.text = gamePlayerInfo.NickName;
        PlayerKillText.text = gamePlayerInfo.KillCount.ToString();
        PlayerDeathText.text = gamePlayerInfo.DeathCount.ToString();
        PlayerCointText.text = gamePlayerInfo.CointCount.ToString();
    }
}
