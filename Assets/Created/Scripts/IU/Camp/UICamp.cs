using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICamp : MonoBehaviour
{
    [SerializeField] private Image colorAlly;
    [SerializeField] private Image colorEnnemy;
    [SerializeField] private GameObject noEnnemyPanel;

    public void SetUI(PlayerState ps) => colorAlly.color = ps.playerColor.Value;
    public void SetEnnemy(PlayerState ps)
    {
        colorEnnemy.color = ps.playerColor.Value;
    }

    public void SetNoEnnemy()
    {
        noEnnemyPanel.SetActive(true);
    }
}
