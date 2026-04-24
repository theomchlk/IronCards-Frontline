using UnityEngine;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;


public static class CardCollection
{

    public static bool HasCard(SyncDictionary<string,int> cards, string id) => cards.ContainsKey(id) && cards[id] > 0;

    public static void AddCard(SyncDictionary<string,int> cards, string id)
    {
        cards.TryAdd(id, 0);
        cards[id]++;
    }

    public static bool RemoveCard(SyncDictionary<string,int> cards, string id)
    {
        if (!HasCard(cards,id)) return false;
        cards[id]--;
        return true;
    }
}
