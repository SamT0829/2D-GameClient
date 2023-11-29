
public enum GameUIMessageEvent
{
    EnterGameLobby,
    EnterGameRoom,
    LeaveGameRoom,
    EnterGamePlay,
}

public enum GameControlMessageEvent
{
    GameDisconnected,
    BeforeLobbyTeleportToGame,
    AfterLobbyTeleportToGame,
    AfterGameTeleportToLobby,

    GameReconnectLoginToLobby,
    GameReconnectLobbyToGame,
}

public enum PlayerControlMessageEvent
{
    PlayerTakeCoin,
}