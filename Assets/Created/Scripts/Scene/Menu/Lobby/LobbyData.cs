using System;

[Serializable]
public struct LobbyData
{
    public int id;
    public string lobbyName;
    public string address;
    public int port;
    public int hostClientId;
    public int nbPlayers;
    public int maxPlayers;
    public bool isOpen;

    public LobbyData(int id, string lobbyName, string address, int port, int hostClientId,int maxPlayers)
    {
        this.id = id;
        this.lobbyName = lobbyName;
        this.address = address;
        this.port = port;
        this.hostClientId = hostClientId;
         nbPlayers = 1;
        this.maxPlayers = maxPlayers;
        isOpen = true;
    }
    
    public bool IsFull() => nbPlayers >= maxPlayers;
}
