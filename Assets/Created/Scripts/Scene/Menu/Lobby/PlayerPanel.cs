using UnityEngine;
using TMPro;

public class PlayerPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text playerName;
    public int index;

    public void SetPlayerPanel(string namePlayer, Color color)
    {
        playerName.text = namePlayer;
        playerName.color = color;;
    }
    
}