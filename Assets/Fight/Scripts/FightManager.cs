using UnityEngine;

public class FightManager : MonoBehaviour
{
    [SerializeField] private float spawnHeight = 1f;
    [SerializeField] private TmpPlayerCamp playerLeft;
    [SerializeField] private TmpPlayerCamp playerRight;
    [SerializeField] private Canvas canvasLeft;
    [SerializeField] private Canvas canvasRight;

    void Start()
    {
        HideCards();
        CardUI[] cardUILeft = canvasLeft.GetComponentsInChildren<CardUI>();
        CardUI[] cardUIRight = canvasRight.GetComponentsInChildren<CardUI>();

        for (int i = 0; i < playerLeft.camp.Length; i++)
        {
            if (playerLeft.camp[i] != null)
            {
                cardUILeft[i].SetCardUI(playerLeft.camp[i]);
                CanvasGroup canvasGroup = cardUILeft[i].GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f;
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }
            }
        }
        for (int i = 0; i < playerRight.camp.Length; i++)
        {
            if (playerRight.camp[i] != null)
            {
                cardUIRight[i].SetCardUI(playerRight.camp[i]);
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
        SpawnSoldiers(playerLeft.camp, cardUILeft, 0);
        SpawnSoldiers(playerRight.camp, cardUIRight, 1);
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