using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/* Base de donées des différentes étales de cartes
 * On va regrouper les cartes selon leurs catégories (Méthode d'attaque, MS, Level, etc)
 * C'est ce que l'on va afficher dans le shop
 */
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
