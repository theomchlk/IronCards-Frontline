using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CardStallSO", menuName = "CardStallSO")]
public class CardStallSO : ScriptableObject
{
    public string cardStallID;
    public string cardStallName;
    public List<CardsSO> cardsSo;
}
