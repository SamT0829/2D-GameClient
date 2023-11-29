using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEditor;

public class UIManager : MonoBehaviour
{
    Dictionary<Type, MonoBehaviour> prefabUITable = new Dictionary<Type, MonoBehaviour>();
    Dictionary<SlotItemAllName, Sprite> slotItemTypeItemSpriteTable = new Dictionary<SlotItemAllName, Sprite>();
    public static UIManager Instance;

    [Header("MessageTipsPrefab")]
    [SerializeField] MessageTipsPrefab messageTipsPrefab;
    [SerializeField] GameObject MessageTipsPanel;

    [Header("LoadingUIPrefab")]
    [SerializeField] LoadingUIPrefab loadingUIPrefab;
    [SerializeField] LoadingUIPrefab loadingUI;

    [Header("GameMessagePrefab")]
    [SerializeField] GameMessagePrefab gameMessagePrefab;
    [SerializeField] GameMessagePrefab gameMessageUI;

    public UIStatus UIStatus = UIStatus.None;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(this);
        // GetShopItemImageFromAsset();

        var tablePath = Application.dataPath + "/Table/";
        TableManager.Instance.CreateTable<ShopItemTable>(tablePath + nameof(ShopItemTable) + ".txt");
        TableManager.Instance.CreateTable<SlotItemTable>(tablePath + nameof(SlotItemTable) + ".txt");
        TableManager.Instance.CreateTable<MultiPlayerGameStartTable>(tablePath + nameof(MultiPlayerGameStartTable) + ".txt");

        InitSlotGameSpriteUI();
    }

    private void Start()
    {
        AddUIPrefab(loadingUIPrefab);
    }

    public void AddUIPrefab<T>(T prefab) where T : MonoBehaviour
    {
        if (!prefabUITable.ContainsKey(typeof(T)))
        {
            prefabUITable.Add(typeof(T), prefab);
        }
    }
    public T GetUIPrefab<T>() where T : MonoBehaviour
    {
        if (prefabUITable.TryGetValue(typeof(T), out MonoBehaviour gameObject))
        {
            return (T)gameObject;
        }

        return null;
    }
    public Sprite GetSlotItemSprite(SlotItemAllName slotItemName)
    {
        if (slotItemTypeItemSpriteTable.TryGetValue(slotItemName, out Sprite sprite))
        {
            return sprite;
        }

        Debug.LogWarningFormat("Cant find slotItemType {0} from slotItemTypeItemSpriteTable", slotItemName);
        return null;
    }

    #region Loading UI Setting
    public void CreateLoadingMission(MissionBase mission)
    {
        if (loadingUI == null)
            loadingUI = Instantiate(loadingUIPrefab);

        loadingUI.CreateMission(mission);
    }
    public async void StartLoadingMission()
    {
        if (loadingUI != null)
        {
            await loadingUI.LoadingStart();
        }
        else
            Debug.Log("StartLoadingMission error");
    }
    public void FinishLoadingMission()
    {
        if (loadingUI != null)
            loadingUI.OnLoadingUIFinish();
    }
    #endregion

    #region Game Message UI Setting 
    public void CreateGameMessage(string gameMessageText, string gameMessageButtonText, Action gameMessageCallBack = null, Action buttonCallBack = null)
    {
        if (gameMessageUI == null)
        {
            gameMessageUI = Instantiate(gameMessagePrefab);
        }
        else
            gameMessageUI.ResetGameMessage();

        gameMessageUI.BuildGameMessage(gameMessageText, gameMessageButtonText, gameMessageCallBack, buttonCallBack);
    }
    public void DestroyGameMessage()
    {
        gameMessageUI.DestroyGameMessage();
    }
    #endregion

    // Init Game Sprite
    private void InitSlotGameSpriteUI()
    {
        string path = "SlotItem/";
        Sprite[] spArray = Resources.LoadAll<Sprite>(path);
        foreach (Sprite sp in spArray)
        {
            if (Enum.TryParse(sp.name, out SlotItemAllName itemType))
            {
                slotItemTypeItemSpriteTable.Add(itemType, sp);
            }
            else
            {
                Debug.LogWarningFormat("Cant find slotItemType from sprite name {0}", sp.name);
            }
        }
    }
}