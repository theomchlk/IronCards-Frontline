using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CardDataBase : MonoBehaviour
{
    public static CardDataBase Instance;
    
    [SerializeField] private List<CardsSO> cardsSo;

    private Dictionary<string, CardsSO> _cardsDictionary;

    void Awake()
    {
        Instance = this;
        _cardsDictionary = cardsSo.ToDictionary(c => c.cardID, c => c);
    }

    public CardsSO GetCard(string cardID) => _cardsDictionary[cardID];
}
