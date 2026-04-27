using UnityEngine;
using TMPro;

public class CardStallContainer : MonoBehaviour
{
    [SerializeField] private TMP_Text cardCostText;
    [SerializeField] private Transform slotLocation;
    
    public Transform SlotLocation => slotLocation;
}
