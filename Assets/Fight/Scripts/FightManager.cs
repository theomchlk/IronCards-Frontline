using UnityEngine;

public class FightManager : MonoBehaviour
{
    [SerializeField] private float spawnHeight = 1f;
    [SerializeField] private Canvas canvasLeft;
    [SerializeField] private Canvas canvasRight;
    [SerializeField] private SoldierRegistry soldierRegistry;
    [SerializeField] private AllCardsSO allCardsSO;
    private PlayerState localPlayerState;
    private CardsSO[] playerLeftCamp;
    private CardsSO[] playerRightCamp;
    public static FightManager Instance;


    void Awake()
    {
        Instance = this;
    }

    public static void RegisterPlayerState(PlayerState psOwner, PlayerState psLeft, PlayerState psRight)
    {
        PlayerRegistry.DisplayAllInformations();
        if (Instance == null)
        {
            Debug.LogError("FightManager instance is null. Cannot register local player.");
            return;
        }

        if (psOwner == null || psLeft == null || psRight == null)
        {
            Debug.LogError("One or more PlayerState references are null. Cannot register local player.");
            return;
        }


        Instance.localPlayerState = psOwner;
        Instance.playerLeftCamp = new CardsSO[35];
        Instance.playerRightCamp = new CardsSO[35];

        for (int i = 0; i < psLeft.playerCamp.Value.campCardsId.Length; i++)
        {
            int cardId = psLeft.playerCamp.Value.campCardsId[i];
            if (cardId >= 0 && cardId < Instance.allCardsSO.allCards.Length)
            {
                Instance.playerLeftCamp[i] = Instance.allCardsSO.allCards[cardId];
            }
        }

        for (int i = 0; i < psRight.playerCamp.Value.campCardsId.Length; i++)
        {
            int cardId = psRight.playerCamp.Value.campCardsId[i];
            if (cardId >= 0 && cardId < Instance.allCardsSO.allCards.Length)
            {
                Instance.playerRightCamp[i] = Instance.allCardsSO.allCards[cardId];
            }
        }


        if (Instance.soldierRegistry == null)
            Instance.soldierRegistry = Instance.GetComponentInChildren<SoldierRegistry>();

        Instance.HideCards();
        CardUI[] cardUILeft = Instance.canvasLeft.GetComponentsInChildren<CardUI>();
        CardUI[] cardUIRight = Instance.canvasRight.GetComponentsInChildren<CardUI>();

        for (int i = 0; i < Instance.playerLeftCamp.Length; i++)
        {
            if (Instance.playerLeftCamp[i] != null)
            {
                cardUILeft[i].SetCardUI(Instance.playerLeftCamp[i]);
                CanvasGroup canvasGroup = cardUILeft[i].GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f;
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }
            }
        }

        for (int i = 0; i < Instance.playerRightCamp.Length; i++)
        {
            if (Instance.playerRightCamp[i] != null)
            {
                cardUIRight[i].SetCardUI(Instance.playerRightCamp[i]);
                CanvasGroup canvasGroup = cardUIRight[i].GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f;
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }
            }
        }

        Canvas.ForceUpdateCanvases();
        Instance.SpawnSoldiers(Instance.playerLeftCamp, cardUILeft, 0);
        Instance.SpawnSoldiers(Instance.playerRightCamp, cardUIRight, 1);
    }

    private void HideCards()
    {
        CardUI[] cardUILeft = canvasLeft.GetComponentsInChildren<CardUI>();
        CardUI[] cardUIRight = canvasRight.GetComponentsInChildren<CardUI>();

        for (int i = 0; i < cardUILeft.Length; i++)
        {
            CanvasGroup canvasGroup = cardUILeft[i].GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }
        for (int i = 0; i < cardUIRight.Length; i++)
        {
            CanvasGroup canvasGroup = cardUIRight[i].GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }
    }

    private void SpawnSoldiers(CardsSO[] cards, CardUI[] cardUIs, int playerId)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] != null)
            {
                GameObject soldierPrefab = cards[i].soldierPrefab;
                RectTransform cardRect = cardUIs[i].GetComponent<RectTransform>();
                for (int j = 0; j < cards[i].nbSoldiers; j++)
                {                    
                    float angle = j * Mathf.PI * 2 / cards[i].nbSoldiers;
                    float radius = 2f;
                    Vector3 circleOffset = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

                    Vector3 spawnPosition = cardRect.position + new Vector3(0, spawnHeight, 0) + circleOffset;
                    SpawnSoldier(soldierPrefab, spawnPosition, playerId);
                }
            }
        }
    }

    private void SpawnSoldier(GameObject soldierPrefab, Vector3 spawnPosition, int playerId)
    {
        GameObject soldier = Instantiate(soldierPrefab, spawnPosition, Quaternion.identity);
        soldier.GetComponent<Soldier>().SetOwnerId(playerId);
        soldier.transform.SetParent(transform);
    }
}