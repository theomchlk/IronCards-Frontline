using GameKit.Dependencies.Utilities;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICamp : MonoBehaviour
{
    [SerializeField] private Image colorAlly;
    [SerializeField] private Transform allyCamp;
    [SerializeField] private Image colorEnnemy;
    [SerializeField] private Transform ennemyCamp;
    [SerializeField] private GameObject noEnnemyPanel;

    [SerializeField] private GameObject linePrefab;
    [SerializeField] private GameObject slotInCampPrefab;
    [SerializeField] private GameObject cardInCampPrefab;

    private PlayerCamp _enemyCamp;
    private bool _allyBuilt;

    public void OnEnterPreparation(PlayerState ps, int nbRow, int nbCol)
    {
        if (!_allyBuilt)
        {
            SetUI(ps, nbRow, nbCol);
            _allyBuilt = true;
        }
        else
        {
            ReturnCampCardsToHand();
        }
    }

    public void SetUI(PlayerState ps, int nbRow, int nbCol)
    {
        ClearUI();
        colorAlly.color = ps.playerColor.Value;
        for (var i = 0; i < nbCol; i++)
        {
            var line = Instantiate(linePrefab, allyCamp);
            for (var j = 0; j < nbRow; j++)
            {
                var slotInCamp = Instantiate(slotInCampPrefab, line.transform).GetComponent<SlotInCamp>();
                slotInCamp.SetupAlly(j, i, ps.Camp);
            }
        }
    }

    public void ReturnCampCardsToHand()
    {
        foreach (Transform line in allyCamp)
        {
            foreach (Transform slot in line)
            {
                var card = slot.GetComponentInChildren<CardDragHandler>();
                if (card == null || card.SlotInHand == null) continue;

                card.LastSlotInCamp = null;
                card.CurrentSlotInCamp = null;
                card.SetNewParent(card.SlotInHand);
            }
        }
    }

    public void SetEnnemy(PlayerState ps, int nbRow, int nbCol)
    {
        if (ps == null) return;

        if (_enemyCamp != null)
            _enemyCamp.CardsOnCamp.OnChange -= OnEnemyCampChanged;

        ClearEnnemyUI();
        colorEnnemy.color = ps.playerColor.Value;

        for (var i = 0; i < nbCol; i++)
        {
            var line = Instantiate(linePrefab, ennemyCamp);
            for (var j = 0; j < nbRow; j++)
            {
                Debug.Log($"GameObjectName: {gameObject.name}");
                var slotInCamp = Instantiate(slotInCampPrefab, line.transform).GetComponent<SlotInCamp>();
                slotInCamp.SetupEnnemy(j, i);
            }
        }

        _enemyCamp = ps.Camp;
        _enemyCamp.CardsOnCamp.OnChange += OnEnemyCampChanged;

        foreach (var kvp in _enemyCamp.CardsOnCamp)
            RefreshEnemySlot(kvp.Key, kvp.Value);
    }

    private void OnEnemyCampChanged(SyncDictionaryOperation op, Localisation loc, string cardId, bool asServer)
    {
        switch (op)
        {
            case SyncDictionaryOperation.Add:
            case SyncDictionaryOperation.Set:
                RefreshEnemySlot(loc, cardId);
                break;
            case SyncDictionaryOperation.Remove:
                RefreshEnemySlot(loc, null);
                break;
            case SyncDictionaryOperation.Clear:
                ClearAllEnemyCards();
                break;
        }
    }

    private void ClearAllEnemyCards()
    {
        foreach (Transform col in ennemyCamp)
            foreach (Transform slotTr in col)
                slotTr.GetComponent<SlotInCamp>()?.HideCard();
    }

    private void RefreshEnemySlot(Localisation loc, string cardId)
    {
        if (ennemyCamp.childCount <= loc.Col) return;
        Transform col = ennemyCamp.GetChild(loc.Col);
        if (col.childCount <= loc.Row) return;

        var slot = col.GetChild(loc.Row).GetComponent<SlotInCamp>();
        if (slot == null) return;

        if (string.IsNullOrEmpty(cardId))
        {
            slot.HideCard();
            return;
        }

        if (cardInCampPrefab == null)
        {
            Debug.LogError("[UICamp] cardInCampPrefab n'est pas assigné dans l'Inspector !");
            return;
        }

        var card = DataBaseItem.Instance.GetDataItem(cardId) as CardsSO;
        if (card != null) slot.ShowCard(cardInCampPrefab, card);
    }

    private void OnDestroy()
    {
        if (_enemyCamp != null)
            _enemyCamp.CardsOnCamp.OnChange -= OnEnemyCampChanged;
    }

    public void SetNoEnnemy()
    {
        noEnnemyPanel.SetActive(true);
    }
    


    public void RemoveCardFromCamp(CampType campType, Localisation loc)
    {
        var slot = (campType == CampType.Ally) ? 
            allyCamp.GetChild(loc.Col).GetChild(loc.Row).GetComponent<SlotInCamp>() :
            ennemyCamp.GetChild(loc.Col).GetChild(loc.Row).GetComponent<SlotInCamp>();
        slot.DragRejected();
    }

    private void ClearUI()
    {
        allyCamp.DestroyChildren();
    }

    private void ClearEnnemyUI()
    {
        ennemyCamp.DestroyChildren();
    }
    
}
