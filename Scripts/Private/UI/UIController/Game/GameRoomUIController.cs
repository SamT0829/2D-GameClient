using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.UI;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using System.Linq;

public class GameRoomUIController : MonoBehaviour
{
    public enum GameResultType
    {
        WinPlayer,
    }
    private enum MultiPlayerGameState
    {
        None,
        Prepare,
        Start,
        Bonus,
        Finish,
        Ending,
    }

    [Header("Game Child Component UI")]
    [SerializeField] GameObject GamePanel;
    [SerializeField] Text GamePanelText;


    [Header("PlayerPrefab")]
    public GamePlayerPrefab gamePlayerPrefab;
    public Dictionary<long, GamePlayerPrefab> accountIdGamePlayerTable = new Dictionary<long, GamePlayerPrefab>();

    [Header("CoinPrefab")]
    public GameCoinPrefab gameCoinPrefab;

    [Header("MonsterPrefab")]
    public GameMonsterPrefab gameMonsterPrefab;
    public Dictionary<int, GameCoinPrefab> coinIdGameCoinTable = new Dictionary<int, GameCoinPrefab>();
    public Dictionary<int, GameMonsterPrefab> monsterIdGameCoinTable = new Dictionary<int, GameMonsterPrefab>();
    public Dictionary<int, GameBulletPrefab> bulletIdGameBulletTable = new Dictionary<int, GameBulletPrefab>();

    [Header("GameRoomSetting")]
    [SerializeField] private Text GameTimerText;
    [SerializeField] private MultiPlayerGameState GameState;
    bool gameStart = false;

    [SerializeField]
    bool TestMode;

    private int TotalPlayerCount;

    public TimeSpan PrepareTimer;
    public TimeSpan GameTimer;
    public TimeSpan BonusTimer;

    public RewardPrefab RewardPrefab;

    private void OnEnable()
    {
        NetworkHandler.Instance.RegisterMessageListener(MsgType.NetMsg_GameStart, OnGameStart);
        NetworkHandler.Instance.RegisterMessageListener(MsgType.NetMsg_GameOver, OnGameOver);

        NetworkHandler.Instance.RegisterMessageListener(MsgType.NetMsg_GamePlayerSyncRespond, OnGamePlayerSyncRespond);
        NetworkHandler.Instance.RegisterMessageListener(MsgType.NetMsg_GamePlayerNetworkInputRespond, OnGamePlayerNetworkInputRespond);

        EventManager.Instance.RegisterEventListener<GameControlEvent>(((int)GameControlMessageEvent.AfterLobbyTeleportToGame), OnAfterLobbyTeleportToGame);
        EventManager.Instance.RegisterEventListener<GameControlEvent>(((int)GameControlMessageEvent.GameReconnectLobbyToGame), OnGameReconnectLobbyToGame);
    }

    private void Start()
    {
        if (TestMode)
        {
            UIManager.Instance.CreateLoadingMission(new ServerLoginMission("sam", "sam"));
            UIManager.Instance.CreateLoadingMission(new ServerLobbyEnterMission());
            UIManager.Instance.CreateLoadingMission(new TestStartGameMission());
            UIManager.Instance.CreateLoadingMission(new ServerLobbyPrepareEnteredGameMission());
            UIManager.Instance.CreateLoadingMission(new ServerGameEnteredMission() { OnFinish = LoadingFinish });
            UIManager.Instance.StartLoadingMission();
        }
    }

    private void FixedUpdate()
    {
        switch (GameState)
        {
            case MultiPlayerGameState.Prepare:
                GamePanel.SetActive(true);
                GamePanelText.text = "Ready";
                break;
            case MultiPlayerGameState.Start:
                GamePanel.SetActive(false);
                break;
            case MultiPlayerGameState.Ending:
                CalculateGameResult();
                break;
        }
    }
    private void OnDisable()
    {
        NetworkHandler.Instance.UnRegisterMessageListener(MsgType.NetMsg_GameStart, OnGameStart);
        NetworkHandler.Instance.UnRegisterMessageListener(MsgType.NetMsg_GameOver, OnGameOver);

        NetworkHandler.Instance.UnRegisterMessageListener(MsgType.NetMsg_GamePlayerSyncRespond, OnGamePlayerSyncRespond);
        NetworkHandler.Instance.UnRegisterMessageListener(MsgType.NetMsg_GamePlayerNetworkInputRespond, OnGamePlayerNetworkInputRespond);

        EventManager.Instance.UnRegisterEventListener<GameControlEvent>(((int)GameControlMessageEvent.AfterLobbyTeleportToGame), OnAfterLobbyTeleportToGame);
        EventManager.Instance.UnRegisterEventListener<GameControlEvent>(((int)GameControlMessageEvent.GameReconnectLobbyToGame), OnGameReconnectLobbyToGame);
    }
    private void OnDestroy()
    {
        gameStart = true;
    }

    // Event Callback
    private void OnAfterLobbyTeleportToGame(IEvent e)
    {
        UIManager.Instance.CreateLoadingMission(new ServerLobbyPrepareEnteredGameMission());
        UIManager.Instance.CreateLoadingMission(new ServerGameEnteredMission() { OnFinish = LoadingFinish });
        UIManager.Instance.StartLoadingMission();
    }
    private void OnGameReconnectLobbyToGame(IEvent e)
    {
        Debug.Log("OnGameReconnectLobbyToGame");
        if (e.GetEventObject(out GameControlEvent gameControlEvent))
        {
            GameStaticInfo gameStaticInfo = new GameStaticInfo();
            gameStaticInfo.DeserializeGameStaticObject((object[])gameControlEvent.data);

            gameStaticInfo.GameRoomInfo.ForEach(gamePlayerInfo =>
            {
                GamePlayerPrefab gamePlayer;
                if (!accountIdGamePlayerTable.TryGetValue(gamePlayerInfo.AccountId, out gamePlayer))
                {
                    gamePlayer = Instantiate(gamePlayerPrefab, gamePlayerInfo.PlayerPosition, Quaternion.identity);
                    gamePlayer.NetworkSyncInit(gamePlayerInfo);

                    accountIdGamePlayerTable.Add(gamePlayerInfo.AccountId, gamePlayer);
                }
                else
                {
                    Debug.LogErrorFormat("accountIdGamePlayerTable is have gameplayer for {0} accountid", gamePlayerInfo.AccountId);
                }
            });
            Debug.Log("OnGameReconnectLobbyToGame finish");
        }
    }

    // Network Callback
    private void OnGameStart(int connectionId, Dictionary<int, object> msg)
    {
        Debug.Log(JsonConvert.SerializeObject(msg));

        if (msg.TryGetValue(((int)GameStart.GameStaticInfo), out object data))
        {
            GameStaticInfo gameStaticInfo = new GameStaticInfo();
            gameStaticInfo.DeserializeGameStaticObject((object[])data);

            gameStaticInfo.GameRoomInfo.ForEach(gamePlayerInfo =>
            {
                GamePlayerPrefab gamePlayer;
                if (!accountIdGamePlayerTable.TryGetValue(gamePlayerInfo.AccountId, out gamePlayer))
                {
                    gamePlayer = Instantiate(gamePlayerPrefab, gamePlayerInfo.PlayerPosition, Quaternion.identity);
                    gamePlayer.NetworkSyncInit(gamePlayerInfo);

                    accountIdGamePlayerTable.Add(gamePlayerInfo.AccountId, gamePlayer);
                }
                else
                {
                    Debug.LogErrorFormat("gameNameGamePlayerTable is has have player by {0}", gamePlayerInfo.NickName);
                }
            });
        }

        gameStart = true;
    }
    private void OnGameOver(int connectionId, Dictionary<int, object> msg)
    {
        Debug.Log("OnOnGameOver" + JsonConvert.SerializeObject(msg));

        object[] roomData = (object[])msg[(int)GameOver.LobbyRoomInfo];
        TeleportToMainLobby(roomData);

    }
    private void OnGamePlayerSyncRespond(int connectionId, Dictionary<int, object> msg)
    {
        // Debug.Log("OnGamePlayerSyncRespond" + JsonConvert.SerializeObject(msg));

        if (msg.TryGetValue(((int)GamePlayerSyncRespond.GameStaticInfo), out object gameStaticData))
        {
            // Debug.Log("OnGamePlayerSyncRespond" + JsonConvert.SerializeObject(data) + data.GetType());
            GameStaticInfo gameStaticInfo = new GameStaticInfo();
            gameStaticInfo.DeserializeGameStaticObject((object[])gameStaticData);

            gameStaticInfo.GameRoomInfo.ForEach(gamePlayerInfo =>
            {
                GamePlayerPrefab gamePlayer;
                if (!accountIdGamePlayerTable.TryGetValue(gamePlayerInfo.AccountId, out gamePlayer))
                {
                    gamePlayer = Instantiate(gamePlayerPrefab, gamePlayerInfo.PlayerPosition, Quaternion.identity);
                    gamePlayer.NetworkSyncInit(gamePlayerInfo);
                    accountIdGamePlayerTable.Add(gamePlayerInfo.AccountId, gamePlayer);
                }
                else
                {
                    gamePlayer.NetworkSyncUpdate(gamePlayerInfo);
                }
            });
        }

        if (msg.TryGetValue(((int)GamePlayerSyncRespond.GameDynamicInfo), out object gameDynamicData))
        {
            if ((gameDynamicData is Dictionary<string, object>) || gameDynamicData != null)
            {
                RetrivieGameDynamicData((Dictionary<string, object>)gameDynamicData);
            }
        }

        if (msg.TryGetValue(((int)GamePlayerSyncRespond.GameResult), out object gameResult))
        {
            if ((gameResult is Dictionary<int, object>) || gameResult != null)
            {
                RetriveGameResultData((Dictionary<int, object>)gameResult);
            }
        }
    }
    private void OnGamePlayerNetworkInputRespond(int connectionId, Dictionary<int, object> msg)
    {
        // Debug.Log("OnGamePlayerNetworkInputRespond" + JsonConvert.SerializeObject(msg));

        if (msg.TryGetValue(((int)GamePlayerNetworkInputRespond.PlayerNetworkInput), out object gamePlayerNetworkData))
        {
            if (msg.TryGetValue(((int)GamePlayerNetworkInputRespond.AccountId), out object accountId))
            {
                if (accountIdGamePlayerTable.TryGetValue(long.Parse(accountId.ToString()), out GamePlayerPrefab gamePlayer))
                {
                    PlayerNetworkInput playerNetworkInput = new PlayerNetworkInput();
                    playerNetworkInput.DeserializeObject((object[])gamePlayerNetworkData);
                    gamePlayer.NetworkInputUpdate(playerNetworkInput);

                    object bulletData;
                    if (msg.TryGetValue(((int)GamePlayerNetworkInputRespond.PlayerBulletData), out bulletData))
                    {
                        BulletInfo bulletInfo = new BulletInfo();
                        bulletInfo.DeserializeObject((object[])bulletData);
                        var bullet = gamePlayer.FireBullet(playerNetworkInput, bulletInfo);
                        bulletIdGameBulletTable.Add(bullet.BulletInfo.BulletId, bullet);
                    }
                }
                else
                {
                    Debug.LogError("cant find accountId " + accountId);
                }
            }
        }
    }
    private void RetrivieGameDynamicData(Dictionary<string, object> gameDynamicData)
    {
        GameDynamicInfo<MultiPlayerGameDynamicInfo> gameDynamicInfo = new GameDynamicInfo<MultiPlayerGameDynamicInfo>();
        gameDynamicInfo.DeserializeGameStaticObject(gameDynamicData);

        if (gameDynamicInfo.GetData(MultiPlayerGameDynamicInfo.GameState, out int state))
        {
            GameState = (MultiPlayerGameState)state;
        }

        if (gameDynamicInfo.GetData(MultiPlayerGameDynamicInfo.Timer, out object[] timer))
        {
            if ((double)timer[1] > 0)
                GameTimerText.text = timer[1].ToString();
        }

        if (gameDynamicInfo.GetData(MultiPlayerGameDynamicInfo.CoinSpawner, out Dictionary<int, object> coinSpawnerData))
        {
            foreach (var coinData in coinSpawnerData.Values)
            {
                Coin coin = new Coin();
                coin.DeserializeObject((object[])coinData);

                if (!coinIdGameCoinTable.TryGetValue(coin.CoinID, out GameCoinPrefab coinPrefab))
                {
                    coinPrefab = Instantiate(gameCoinPrefab, coin.Position, Quaternion.identity);
                    coinPrefab.Init(coin);
                    coinIdGameCoinTable.Add(coin.CoinID, coinPrefab);
                }
                else
                {
                    coinPrefab.NetworkUpdate(coin);
                }
            }
        }

        if (gameDynamicInfo.GetData(MultiPlayerGameDynamicInfo.BulletSpawner, out Dictionary<int, object> bulletSpawnerData))
        {
            foreach (var bulletData in bulletSpawnerData.Values)
            {
                BulletInfo bulletInfo = new BulletInfo();
                bulletInfo.DeserializeObject((object[])bulletData);

                if (bulletIdGameBulletTable.TryGetValue(bulletInfo.BulletId, out GameBulletPrefab bulletPrefab))
                {
                    if (!bulletInfo.alive)
                        bulletIdGameBulletTable.Remove(bulletInfo.BulletId);

                    bulletPrefab.NetworkUpdate(bulletInfo);
                }
            }
        }

        if (gameDynamicInfo.GetData(MultiPlayerGameDynamicInfo.MonsterSpawner, out Dictionary<int, object> monsterSpawnerData))
        {
            foreach (var monsterData in monsterSpawnerData.Values)
            {
                MonsterInfo monsterInfo = new MonsterInfo();
                monsterInfo.DeserializeObject((object[])monsterData);

                if (!monsterIdGameCoinTable.TryGetValue(monsterInfo.MonsterID, out GameMonsterPrefab monsterPrefab))
                {
                    monsterPrefab = Instantiate(gameMonsterPrefab, monsterInfo.Position, Quaternion.identity);
                    monsterPrefab.Init(monsterInfo);
                    monsterIdGameCoinTable.Add(monsterInfo.MonsterID, monsterPrefab);
                }
                else
                {
                    monsterPrefab.NetworkUpdate(monsterInfo);
                }
            }
        }
    }

    private void RetriveGameResultData(Dictionary<int, object> gameResultData)
    {
        object winPlayerMessage;
        if (gameResultData.TryGetValue(GameResultType.WinPlayer.GetHashCode(), out winPlayerMessage))
        {
            List<GamePlayerInfo> winPlayerList = new List<GamePlayerInfo>();
            if (winPlayerMessage != null)
            {
                foreach (var winPlayerdata in (object[])winPlayerMessage)
                {
                    GamePlayerInfo winPlayer = new GamePlayerInfo();
                    winPlayer.DeserializeObject((object[])winPlayerdata);
                    winPlayerList.Add(winPlayer);
                }

                var winnerPlayerName = string.Empty;
                winPlayerList.ForEach(info => winnerPlayerName += " " + info.NickName);
                GamePanelText.text = "GameWinner :" + winnerPlayerName;
            }
        }
    }

    private async Task LoadingFinish()
    {
        while (!gameStart)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.1));
        }

        UIManager.Instance.FinishLoadingMission();
        await Task.CompletedTask;
    }

    private void TeleportToMainLobby(object[] roomData)
    {
        Action afterTeleportAction = () =>
        {
            UIManager.Instance.UIStatus = UIStatus.Room;

            GameControlEvent afterGameTeleportToLobby = new GameControlEvent(GameControlMessageEvent.AfterGameTeleportToLobby);
            afterGameTeleportToLobby.data = roomData;
            EventManager.Instance.SendEvent(afterGameTeleportToLobby);
        };

        Action beforeTeleportAction = () =>
        {
        };

        NetworkHandler.Instance.Disconnect(RemoteConnetionType.Game);
        // GoTo Lobby
        GameManager.Instance.TeleportToScene(GameScene.Game.ToString(), GameScene.Lobby.ToString(), beforeTeleportAction, afterTeleportAction);
    }

    private void CalculateGameResult()
    {
        GamePanel.SetActive(true);
    }
}