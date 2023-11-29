using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] bool alwaysVisible;

    [Header("PlayerUIPrefab")]
    [SerializeField] GameObject PlayerUIPrefab;

    [Header("PlayerStatusUIPrefab")]
    [SerializeField] GameObject PlayerStatus;

    [Header("PlayerStatusUIPrefabComponent")]
    [SerializeField] Text playerNameText;
    [SerializeField] Text coinCountText;
    [SerializeField] Image playerHealth;
    [SerializeField] Image playerHealthCurrent;
    [SerializeField] Image playerEnergy;
    [SerializeField] Image playerEnergyCurrent;

    [Header("Point")]
    [SerializeField] Transform playerNamePoint;
    [SerializeField] Transform coinCountPoint;
    [SerializeField] Transform healthPoint;
    [SerializeField] Transform energyPoint;

    GamePlayerPrefab gamePlayerPrefab;

    private void Awake()
    {
        gamePlayerPrefab = GetComponent<GamePlayerPrefab>();
    }

    private void Start()
    {
        playerNameText.text = gamePlayerPrefab.GamePlayerInfo.NickName;
        coinCountText.text = gamePlayerPrefab.GamePlayerInfo.CointCount.ToString();
        float healthSize = (float)gamePlayerPrefab.GamePlayerInfo.PlayerHealth / (float)gamePlayerPrefab.GamePlayerInfo.PlayerMaxHealth;
        playerHealthCurrent.fillAmount = healthSize;
    }

    private void OnEnable()
    {
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                PlayerStatus = Instantiate(PlayerUIPrefab, canvas.transform);
                playerNameText = PlayerStatus.transform.Find("PlayerName").GetComponent<Text>();
                coinCountText = PlayerStatus.transform.Find("CoinCount").GetComponent<Text>();
                playerHealth = PlayerStatus.transform.Find("HealthBackground").GetComponent<Image>();
                playerHealthCurrent = PlayerStatus.transform.Find("HealthBackground").Find("HealthCurrent").GetComponent<Image>();
                playerEnergy = PlayerStatus.transform.Find("EnergyBackground").GetComponent<Image>();
                playerEnergyCurrent = PlayerStatus.transform.Find("EnergyBackground").Find("EnergyCurrent").GetComponent<Image>();
                PlayerStatus.SetActive(alwaysVisible);
            }
        }
    }

    private void LateUpdate()
    {
        var cam = Camera.main.transform;

        if (playerNameText != null)
        {
            playerNameText.transform.position = playerNamePoint.position;
            playerNameText.transform.forward = cam.forward;
        }

        if (coinCountText != null)
        {
            coinCountText.text = gamePlayerPrefab.GamePlayerInfo.CointCount.ToString();
            coinCountText.transform.position = coinCountPoint.position;
            coinCountText.transform.forward = cam.forward;
        }

        if (playerHealth != null)
        {
            float healthSize = (float)gamePlayerPrefab.GamePlayerInfo.PlayerHealth / (float)gamePlayerPrefab.GamePlayerInfo.PlayerMaxHealth;
            playerHealthCurrent.fillAmount = healthSize;
            playerHealth.transform.position = healthPoint.position;
            playerHealth.transform.forward = cam.forward;
        }

        if (playerEnergy != null)
        {
            float energySize = (float)gamePlayerPrefab.GamePlayerInfo.PlayerEnergy / (float)gamePlayerPrefab.GamePlayerInfo.PlayerMaxEnergy;
            playerEnergyCurrent.fillAmount = energySize;
            playerEnergy.transform.position = energyPoint.position;
            playerEnergy.transform.forward = cam.forward;
        }
    }

    public void InitPlayerUIStatus(string playerName, int coinCount)
    {
        playerNameText.text = playerName;
        coinCountText.text = coinCount.ToString();
    }

    public void EnablePlayerUIStatus(bool enabled)
    {
        PlayerStatus.SetActive(enabled);
    }

    public void RemovePlayerUIStatus()
    {
        Destroy(gameObject);
    }
}