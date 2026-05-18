using System;

[Serializable]
public struct LobbyData
{
    public string id;
    public string lobbyName;
    public string address;
    public ushort port;
    /*public int hostClientId;*/
    public int nbPlayers;
    public int maxPlayers;
    public bool isOpen;
    public float lastSeen;
    public bool hasBeenModified;

    public LobbyData(string lobbyName, string address, ushort port, /*int hostClientId,*/ int maxPlayers)
    {
        id = Guid.NewGuid().ToString();
        this.lobbyName = lobbyName;
        this.address = address;
        this.port = port;
        /*this.hostClientId = hostClientId;*/
         nbPlayers = 1;
        this.maxPlayers = maxPlayers;
        isOpen = true;
        lastSeen = -1;
        hasBeenModified = true;
    }
    
    public bool IsFull() => nbPlayers >= maxPlayers;
}
