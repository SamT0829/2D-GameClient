using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public readonly string ServerAddreas = "192.168.8.111:5055";
    public readonly string MultiplayerServerName = "MultiplayerGame";

    public static GameManager Instance;
    public GameType GameType = GameType.MultiplayerGame;
    public PlayerStatus PlayerStatus = PlayerStatus.Idle;
    public LobbyRoomInfo LobbyRoomInfo;

    [Header("Game Player Prefab")]
    public List<GamePlayerPrefab> gamePlayerPrefabList;

    [Header("Game Bullet Prefab")]
    [SerializeField] GameBulletPrefab gameBulletPrefab;


    public Camera mainCamera;
    public Texture2D cursorIdle;
    public Texture2D cursorClick;


    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        Application.runInBackground = true;
        DontDestroyOnLoad(gameObject);

        GetPlayerPrefab();

        if (!mainCamera)
            mainCamera = Camera.main;
    }

    public void ClickCursor()
    {
        Cursor.SetCursor(cursorClick, Vector2.zero, CursorMode.Auto);
    }

    public void IdleCursor()
    {
        Cursor.SetCursor(cursorIdle, Vector2.zero, CursorMode.Auto);
    }

    public void TeleportToScene(string from, string to, Action beforeTeleportAction = null, Action afterTeleportAction = null)
    {
        StartCoroutine(TransitionToScene(from, to, beforeTeleportAction, afterTeleportAction));
    }

    private IEnumerator TransitionToScene(string from, string to, Action beforeTeleportAction, Action afterTeleportAction)
    {
        if (SceneManager.GetActiveScene().name != to)
        {
            if (beforeTeleportAction != null)
                beforeTeleportAction.Invoke();

            yield return SceneManager.LoadSceneAsync(to, LoadSceneMode.Single);

            if (afterTeleportAction != null)
                afterTeleportAction.Invoke();
        }
    }

    private void GetPlayerPrefab()
    {
        gamePlayerPrefabList.AddRange(Resources.LoadAll<GamePlayerPrefab>("Player"));
    }
}