using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class PlayerCamp : NetworkBehaviour
{
    private int _nbRow, _nbCol;
    private readonly SyncDictionary<Localisation, string> cardsOnCamp = new();
    public SyncDictionary<Localisation, string> CardsOnCamp => cardsOnCamp;
    [SerializeField] private PlayerState ps;

    [Server]
    public void RemoveCardFromCampToHand(Localisation loc, string cardId)
    {
        if (cardsOnCamp.ContainsKey(loc) && cardsOnCamp[loc] == cardId)
        {
            CardCollection.AddCard(ps.cardsInHand, cardId);
            cardsOnCamp.Remove(loc);
            return;
        }
        Debug.LogWarning($"The card {cardId} from {loc.Col}, {loc.Row} isn't in camp.");
    }
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        var gameManager = GameManager.Instance;
        gameManager.nbRow.OnChange += SetNbRow;
        gameManager.nbCol.OnChange += SetNbCol;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        var gameManager = GameManager.Instance;
        gameManager.nbRow.OnChange -= SetNbRow;
        gameManager.nbCol.OnChange -= SetNbCol;   
    }

    private void SetNbRow(int prev, int next, bool asServer)
    {
        if (asServer) return;
        _nbRow = next;
    }
    private void SetNbCol(int prev, int next, bool asServer)
    {
        if (asServer) return;
        _nbCol = next;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ServerPutCardOnCamp(string cardId, Localisation loc, Localisation prev, NetworkConnection conn = null)
    {
        
        if (!IsPossibleToPutCardOnCamp(cardId, loc, prev)) TargetRemoveCardFromCamp(conn,loc);
        else
        {
            
            if (prev != null) //Si la carte vient déjà de camp
            {
                if (cardsOnCamp.ContainsKey(loc)) //Si il y a déjà un carte en loc
                {
                    Debug.Log("Swap in camp");
                    SwapCards(cardsOnCamp[loc], cardsOnCamp[prev]);
                    return;
                }
                cardsOnCamp.Remove(prev);
                cardsOnCamp[loc] = cardId;
                return;
            }
            cardsOnCamp[loc] = cardId;
            Debug.Log($"cardsOnCamp had set {cardsOnCamp[loc]} in {loc.Col}, {loc.Row}");
            CardCollection.RemoveCard(ps.cardsInHand, cardId);
        }
    }

    [Server]
    private void SwapCards(string card1, string card2)
    {
        (card1, card2) = (card2, card1);
    }

    [Server]
    private void RemoveToHand(NetworkConnection conn,Localisation loc)
    {
        CardCollection.AddCard(ps.cardsInHand,cardsOnCamp[loc]);
        TargetRemoveCardFromCamp(conn, loc);
    }

    [Server]
    public void ResetCardsOnCamp()
    {
        foreach (var cards in cardsOnCamp)
        {
            CardCollection.AddCard(ps.cardsInHand, cards.Value);
        }
        cardsOnCamp.Clear();
    }

    [TargetRpc]
    private void TargetRemoveCardFromCamp(NetworkConnection conn, Localisation loc)
    {
        Debug.Log("TargetPutCardFailed");
        UIManager.Instance.uiCamp.RemoveCardFromCamp(CampType.Ally, loc);
    }

    [TargetRpc]
    private void TargetPutCardOnSucceeded(NetworkConnection conn, string cardId, Localisation loc)
    {
        
    }

    private bool IsPossibleToPutCardOnCamp(string cardId, Localisation loc, Localisation prev)
    {
        if (loc.Row < 0 || loc.Row >= _nbRow) return WhyItsImpossibleToPutCardOnCamp("Row invalid");
        if (loc.Col < 0 || loc.Col >= _nbCol) return  WhyItsImpossibleToPutCardOnCamp("Col invalid");
        DebugAllKeyInCardsOnCamp();
        /*if (cardsOnCamp.ContainsKey(loc)) return  WhyItsImpossibleToPutCardOnCamp("Loc already used");*/
        if (!CardCollection.HasCard(ps.cardsInHand, cardId))
        { 
            if (prev == null) return WhyItsImpossibleToPutCardOnCamp("previous location is null");
            if (!cardsOnCamp.ContainsKey(prev)) return WhyItsImpossibleToPutCardOnCamp($"Card's from nowhere {prev.Col}, {prev.Row}"); 
            if (cardsOnCamp[prev] != cardId) return WhyItsImpossibleToPutCardOnCamp("Card id mismatch");
        }
       
        return true;
    }

    private void DebugAllKeyInCardsOnCamp()
    {
        foreach (var key in cardsOnCamp.Keys)
        {
            Debug.Log($"CardsOnCamps: Key (col,row) = {key.Col}{key.Row} ; Value = {cardsOnCamp[key]}");
        }
    }

    private bool WhyItsImpossibleToPutCardOnCamp(string reason)
    {
        Debug.LogWarning("WhyItsImpossibleToPutCardOnCamp: " + reason);
        return false;
    }
    
}
