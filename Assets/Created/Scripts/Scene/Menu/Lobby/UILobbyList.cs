using UnityEngine;

public class UILobbyList : MonoBehaviour
{
    public static UILobbyList Local;
    [SerializeField] private GameObject lobbyListPanel;
    [SerializeField] private Transform lobbyListLocation;
    
    void Awake()
    {
        Local = this;
    }

    public void AddNewPanel(LobbyData data)
    {
        Debug.Log("Panel instantiated");
        LobbyUI lobbyUI = Instantiate(lobbyListPanel, lobbyListLocation).GetComponent<LobbyUI>();
        lobbyUI.SetUI(data);
    }

    public void DestroyPanel(int index)
    {
        Destroy(lobbyListLocation.GetChild(index).gameObject);
    }
    
    public void UpdatePanel(int index, LobbyData data)
    {
        LobbyUI lobbyUI = lobbyListLocation.GetChild(index).GetComponent<LobbyUI>();
        lobbyUI.SetUI(data);
    }
    
    public bool PanelExists(int lobbyId)
    {
        foreach (Transform child in lobbyListLocation)
        {
            if (child.GetComponent<LobbyUI>().LobbyId == lobbyId)
                return true;
        }
        return false;
    }
    

}
