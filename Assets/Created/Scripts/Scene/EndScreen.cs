using FishNet;
using UnityEngine;
using TMPro;

public class EndScreen : MonoBehaviour
{
    public TMP_Text winnerText;

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
