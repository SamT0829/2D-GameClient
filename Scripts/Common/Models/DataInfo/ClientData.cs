public class ClientData
{
    private static ClientData instance = new ClientData();
    public static ClientData Instance { get { return instance; } }

    public PlayerAccountInfo PlayerAccountInfo = new PlayerAccountInfo();
    public LobbyPlayerInfo LobbyPlayerInfo = new LobbyPlayerInfo();
    public PlayerInventoryInfo PlayerInventoryInfo = new PlayerInventoryInfo();

    public int SessionId { get; set; }
    public string LobbyServerAddreas { get; set; }
    public string GameServerAddreas { get; set; }

}
