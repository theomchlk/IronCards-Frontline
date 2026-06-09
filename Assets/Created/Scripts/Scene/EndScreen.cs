using FishNet;
using UnityEngine;
using TMPro;

public class EndScreen : MonoBehaviour
{
    public TMP_Text winnerText;

    private void Start()
    {
        if (GameManager.Instance == null) return;
        int id = GameManager.Instance.winnerId.Value;
        var ps = PlayerRegistry.GetPlayerState(id);
        if (ps != null) SetText(ps.playerName.Value, ps.playerColor.Value);
    }

    public void SetText(string playerName, Color color)
    {
        winnerText.text = playerName + " won!";
        winnerText.color = color;
    }

    public void OnClickReturnMenu()
    {
        InstanceFinder.ClientManager.StopConnection();
    }
}
