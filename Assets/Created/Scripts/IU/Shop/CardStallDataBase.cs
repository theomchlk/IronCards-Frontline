using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CardStallDataBase : MonoBehaviour
{
    public static CardStallDataBase Instance;
    [SerializeField] private List<CardStallSO> cards;
    private Dictionary<string, CardStallSO> _stallDictionary;

    void Awake()
    {
        Instance = this;
        _stallDictionary = cards.ToDictionary(cs => cs.cardStallID, cs => cs);
    }
    
    public CardStallSO GetCardStall(string cardStallID) => _stallDictionary[cardStallID];
}
