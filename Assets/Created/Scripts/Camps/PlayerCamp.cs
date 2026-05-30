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
    public void ServerRemoveCardFromCampToHand(Localisation loc, string cardId, NetworkConnection conn = null)
    {
        RemoveCardFromCampToHand(loc,cardId);

    }

    [ServerRpc(RequireOwnership = false)]
    public void ServerPutCardOnCamp(string cardId, Localisation loc, Localisation newCardLoc, NetworkConnection conn = null)
    {
        
        if (!IsPossibleToPutCardOnCamp(cardId, loc, newCardLoc)) TargetRemoveCardFromCamp(conn,loc);
        else
        {
            
            if (newCardLoc != null) //Si la carte vient déjà de camp
            {
                if (cardsOnCamp.ContainsKey(loc)) //Si il y a déjà un carte en loc
                {
                    Debug.Log($"Swap in camp loc {loc.Row},{loc.Col} and new loc {newCardLoc.Row},{newCardLoc.Col}");
                    SwapCards(loc, newCardLoc);
                    return;
                }
                cardsOnCamp.Remove(newCardLoc);
                cardsOnCamp[loc] = cardId;
                return;
            }
            cardsOnCamp[loc] = cardId;
            Debug.Log($"cardsOnCamp had set {cardsOnCamp[loc]} in {loc.Col}, {loc.Row}");
            CardCollection.RemoveCard(ps.cardsInHand, cardId);
        }
    }

    [Server]
    private void SwapCards(Localisation oldLoc, Localisation newLoc)
    {
        Debug.Log($"Before swap card1 {cardsOnCamp[oldLoc]}, card2 {cardsOnCamp[newLoc]}");
        (cardsOnCamp[oldLoc], cardsOnCamp[newLoc]) = (cardsOnCamp[newLoc], cardsOnCamp[oldLoc]);
        Debug.Log($"After swap card1 {cardsOnCamp[oldLoc]}, card2 {cardsOnCamp[newLoc]}");
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

    private bool IsPossibleToPutCardOnCamp(string cardId, Localisation loc, Localisation newCardLoc)
    {
        if (loc.Row < 0 || loc.Row >= _nbRow) return WhyItsImpossibleToPutCardOnCamp("Row invalid");
        if (loc.Col < 0 || loc.Col >= _nbCol) return  WhyItsImpossibleToPutCardOnCamp("Col invalid");
        DebugAllKeyInCardsOnCamp();
        /*if (cardsOnCamp.ContainsKey(loc)) return  WhyItsImpossibleToPutCardOnCamp("Loc already used");*/
        if (!CardCollection.HasCard(ps.cardsInHand, cardId))
        { 
            if (newCardLoc == null) return WhyItsImpossibleToPutCardOnCamp("previous location is null");
            if (!cardsOnCamp.ContainsKey(newCardLoc)) return WhyItsImpossibleToPutCardOnCamp($"Card's from nowhere {newCardLoc.Col}, {newCardLoc.Row}"); 
            if (cardsOnCamp[newCardLoc] != cardId) return WhyItsImpossibleToPutCardOnCamp("Card id mismatch");
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
