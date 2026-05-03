using UnityEngine;
using TMPro;

public class CardStallContainer : MonoBehaviour
{
    [SerializeField] private TMP_Text cardCostText;
    [SerializeField] private Transform cardLocation;
    
    public Transform CardLocation => cardLocation;

    public void SetCardStallContainer(int cardCost)
    {
        cardCostText.text = cardCost.ToString();
    }
}
