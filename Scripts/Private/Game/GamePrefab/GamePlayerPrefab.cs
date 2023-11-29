using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Newtonsoft.Json;
using System.Threading.Tasks;

public enum PlayerSyncNetworkMessage
{
    PlayerAccountId,
    PlayerPosition,
    PlayerLocalScale,
    PlayerTakeCoin,
    PlayerGetBullet,
}


public class GamePlayerPrefab : MonoBehaviour
{
    // Component
    Rigidbody2D rb;
    Collider2D coll;
    SpriteRenderer spriteRenderer;
    PlayerUIController playerUIController;
    Animator animator;

    CinemachineVirtualCamera virtualCamera;

    public GamePlayerInfo GamePlayerInfo;

    // 是否為控制玩家
    public bool IsGamePlayer = false;

    [Header("Bullet Prefab")]
    [SerializeField] private GameBulletPrefab gameBulletPrefab;
    [SerializeField] private float bulletSpeed;

    [Header("Player Status")]
    public float speed;
    public bool isMove;
    float xVelocity;
    float yVelocity;
    bool isJumpPressed;
    bool isFirePressed;
    bool isThrowGranadeButtonPressed;
    bool isRocketLauncherFireButtonPressed;
    bool isPlayerDie = false;

    [Header("Gun Equip Setting")]
    public Transform Weapon;
    public Transform GunEquipTransform;
    private float gunRotationZ;
    private Vector3 gunAimDirection;

    // animator Id
    int isRunId;
    int isDieId;

    // Component
    Camera mainCamera;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerUIController = GetComponent<PlayerUIController>();
        animator = GetComponent<Animator>();

        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

        isRunId = Animator.StringToHash("isRun");
        isDieId = Animator.StringToHash("isDie");

        mainCamera = Camera.main;
    }
    private void Update()
    {
        NetworkButtonPressedSeting();
        NetworkPlayerSyncRequest();
    }

    private void FixedUpdate()
    {
        FlipDireaction();
        Movement();
        RotationOfWeapon();
        // Reset();
        animator.SetBool(isRunId, isMove);
        animator.SetBool(isDieId, isPlayerDie);
    }

    # region Trigger 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Coin")
        {
            var coinPrefab = other.gameObject.GetComponent<GameCoinPrefab>();
            if (coinPrefab != null)
                OnPlayerTakeCoin(coinPrefab);
        }

        if (other.gameObject.tag == "Bullet")
        {
            var bulletPrefab = other.gameObject.GetComponent<GameBulletPrefab>();
            if (bulletPrefab != null)
            {
                OnPlayerGetBullet(bulletPrefab);
                Destroy(bulletPrefab.gameObject);
            }
        }
    }
    private void OnPlayerTakeCoin(GameCoinPrefab coinPrefab)
    {
        MessageBuilder msgBuilder = new MessageBuilder();
        Dictionary<PlayerSyncNetworkMessage, object> playerNetworkMessageTable = new Dictionary<PlayerSyncNetworkMessage, object>();
        playerNetworkMessageTable.Add(PlayerSyncNetworkMessage.PlayerAccountId, GamePlayerInfo.AccountId);
        playerNetworkMessageTable.Add(PlayerSyncNetworkMessage.PlayerTakeCoin, coinPrefab.Coin.CoinID);
        msgBuilder.AddMsg(((int)GamePlayerSyncRequest.PlayerSyncMessage), playerNetworkMessageTable, NetMsgFieldType.Object);
        NetworkHandler.Instance.Send(RemoteConnetionType.Game, MsgType.NetMsg_GamePlayerSyncRequest, msgBuilder);
    }
    private void OnPlayerGetBullet(GameBulletPrefab bulletPrefab)
    {
        MessageBuilder msgBuilder = new MessageBuilder();
        Dictionary<PlayerSyncNetworkMessage, object> playerNetworkMessageTable = new Dictionary<PlayerSyncNetworkMessage, object>();
        playerNetworkMessageTable.Add(PlayerSyncNetworkMessage.PlayerAccountId, GamePlayerInfo.AccountId);
        playerNetworkMessageTable.Add(PlayerSyncNetworkMessage.PlayerGetBullet, bulletPrefab.BulletInfo.BulletId);
        msgBuilder.AddMsg(((int)GamePlayerSyncRequest.PlayerSyncMessage), playerNetworkMessageTable, NetMsgFieldType.Object);
        NetworkHandler.Instance.Send(RemoteConnetionType.Game, MsgType.NetMsg_GamePlayerSyncRequest, msgBuilder);
    }
    # endregion

    # region Network function 
    public void NetworkSyncInit(GamePlayerInfo gamePlayerInfo)
    {
        GamePlayerInfo = gamePlayerInfo;
        IsGamePlayer = ClientData.Instance.LobbyPlayerInfo.AccountId == gamePlayerInfo.AccountId;
        transform.position = gamePlayerInfo.PlayerPosition;
        transform.localScale = gamePlayerInfo.PlayerLocalScale;

        if (IsGamePlayer)
        {
            virtualCamera.LookAt = transform;
        }
    }
    public void NetworkSyncUpdate(GamePlayerInfo gamePlayerInfo)
    {
        GamePlayerInfo = gamePlayerInfo;

        if (gamePlayerInfo.IsPlayerDie && !isPlayerDie)
        {
            PlayerDie();
        }

        if (!gamePlayerInfo.IsPlayerDie && isPlayerDie)
        {
            // spriteRenderer.enabled = true;
            coll.enabled = true;
            rb.simulated = true;
            isPlayerDie = false;
            playerUIController.EnablePlayerUIStatus(true);
            transform.position = gamePlayerInfo.PlayerPosition;
        }
    }
    public void NetworkInputUpdate(PlayerNetworkInput playerNetworkInput)
    {
        if (isPlayerDie)
        {
            xVelocity = 0;
            yVelocity = 0;
            isJumpPressed = false;
            isFirePressed = false;
            return;
        }

        xVelocity = playerNetworkInput.movementInput.x;
        yVelocity = playerNetworkInput.movementInput.y;

        gunRotationZ = playerNetworkInput.gunRotationZ;
        gunAimDirection = playerNetworkInput.gunAimDirection;

        isJumpPressed = playerNetworkInput.GetNetworkButtonInputData(NetworkInputButtons.JUMP);
        isFirePressed = playerNetworkInput.GetNetworkButtonInputData(NetworkInputButtons.FIRE);
    }
    private void NetworkButtonPressedSeting()
    {
        if (!IsGamePlayer)
            return;

        // network move Input
        PlayerNetworkInput PlayerNetworkInput = new PlayerNetworkInput();
        PlayerNetworkInput.movementInput.x = Input.GetAxis("Horizontal");
        PlayerNetworkInput.movementInput.y = Input.GetAxis("Vertical");

        // Button input
        var isJumpButtonPressed = Input.GetKeyDown(KeyCode.Z);
        var isFireButtonPressed = Input.GetKeyDown(KeyCode.X);

        // Gun rotation & direction
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
        Vector3 target = mainCamera.ScreenToWorldPoint(mousePosition);
        Vector2 difference = target - GunEquipTransform.localPosition;

        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        difference.Normalize();

        PlayerNetworkInput.gunRotationZ = rotationZ;
        PlayerNetworkInput.gunAimDirection = difference;

        PlayerNetworkInput.SetNetworkButtonInputData(NetworkInputButtons.JUMP, isJumpButtonPressed);
        PlayerNetworkInput.SetNetworkButtonInputData(NetworkInputButtons.FIRE, isFireButtonPressed);

        PlayerNetworkInput.SendNetworkInputData();
    }
    private void NetworkPlayerSyncRequest()
    {
        if (NetworkHandler.Instance.IsConnect(RemoteConnetionType.Game) && IsGamePlayer)
        {
            MessageBuilder msgBuilder = new MessageBuilder();
            Dictionary<PlayerSyncNetworkMessage, object> playerNetworkMessageTable = new Dictionary<PlayerSyncNetworkMessage, object>();
            object[] playerPosition = Utility.VectorToList(transform.position);
            object[] playerLocalScale = Utility.VectorToList(transform.localScale);
            playerNetworkMessageTable.Add(PlayerSyncNetworkMessage.PlayerAccountId, GamePlayerInfo.AccountId);
            playerNetworkMessageTable.Add(PlayerSyncNetworkMessage.PlayerPosition, playerPosition);
            playerNetworkMessageTable.Add(PlayerSyncNetworkMessage.PlayerLocalScale, playerLocalScale);
            msgBuilder.AddMsg(((int)GamePlayerSyncRequest.PlayerSyncMessage), playerNetworkMessageTable, NetMsgFieldType.Object);
            NetworkHandler.Instance.Send(RemoteConnetionType.Game, MsgType.NetMsg_GamePlayerSyncRequest, msgBuilder);
        }
    }
    # endregion

    # region Player Controller function 
    private void FlipDireaction()
    {
        if (gunRotationZ > 90 || gunRotationZ < -90)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            Weapon.localScale = new Vector3(-1, -1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
            Weapon.localScale = new Vector3(1, 1, 1);
        }
    }
    private void Movement()
    {
        if (Mathf.Abs(xVelocity + yVelocity) > 0)
            isMove = true;
        else
            isMove = false;

        var dir = new Vector2(xVelocity, yVelocity);
        dir.Normalize();
        rb.velocity = speed * dir * Time.fixedDeltaTime;
    }
    public GameBulletPrefab FireBullet(PlayerNetworkInput playerNetworkInput, BulletInfo bulletInfo)
    {
        var gameBullet = Instantiate(gameBulletPrefab, GunEquipTransform.position + gunAimDirection * 1, Quaternion.Euler(0.0f, 0.0f, gunRotationZ));
        gameBullet.InitBullet(playerNetworkInput.gunAimDirection, bulletInfo, bulletSpeed, IsGamePlayer);

        isFirePressed = false;
        return gameBullet;
    }
    private void RotationOfWeapon()
    {
        Weapon.rotation = Quaternion.Euler(0.0f, 0.0f, gunRotationZ);
    }
    private void Reset()
    {
        xVelocity = 0;
        yVelocity = 0;
    }
    private void PlayerDie()
    {
        isPlayerDie = true;
        // spriteRenderer.enabled = false;
        coll.enabled = false;
        rb.simulated = false;
        playerUIController.EnablePlayerUIStatus(false);

        transform.position = GamePlayerInfo.PlayerPosition;
        transform.localScale = GamePlayerInfo.PlayerLocalScale;

        // MessageBuilder msgBuilder = new MessageBuilder();
        // Dictionary<PlayerNetworkMessage, object> playerNetworkMessageTable = new Dictionary<PlayerNetworkMessage, object>();
        // playerNetworkMessageTable.Add(PlayerNetworkMessage.PlayerDie, true);
        // msgBuilder.AddMsg(((int)GamePlayerSyncRequest.PlayerMessage), playerNetworkMessageTable, NetMsgFieldType.Object);
        // NetworkHandler.Instance.Send(RemoteConnetionType.Game, MsgType.NetMsg_GamePlayerSyncRequest, msgBuilder);

        // yield return new WaitForSeconds(3);

        // MessageBuilder msgBuilder2 = new MessageBuilder();
        // Dictionary<PlayerNetworkMessage, object> playerNetworkMessageTable2 = new Dictionary<PlayerNetworkMessage, object>();
        // playerNetworkMessageTable2.Add(PlayerNetworkMessage.PlayerDie, false);
        // msgBuilder2.AddMsg(((int)GamePlayerSyncRequest.PlayerMessage), playerNetworkMessageTable2, NetMsgFieldType.Object);
        // NetworkHandler.Instance.Send(RemoteConnetionType.Game, MsgType.NetMsg_GamePlayerSyncRequest, msgBuilder2);
    }
    # endregion

}