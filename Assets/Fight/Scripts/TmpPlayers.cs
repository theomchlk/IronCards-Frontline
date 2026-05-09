using UnityEngine;

public class TmpPlayer : MonoBehaviour
{
    [SerializeField] private CardsSO[] playerLeft;
    [SerializeField] private CardsSO[] playerRight;
    [SerializeField] private Canvas canvasLeft;
    [SerializeField] private Canvas canvasRight;
    [SerializeField] private Spawner spawner;


    void Start()
    {
        HideCards();
        CardUI[] cardUILeft = canvasLeft.GetComponentsInChildren<CardUI>();
        CardUI[] cardUIRight = canvasRight.GetComponentsInChildren<CardUI>();

        for (int i = 0; i < playerLeft.Length; i++)
        {
            if (playerLeft[i] != null)
            {
                cardUILeft[i].SetCardUI(playerLeft[i]);
                CanvasGroup canvasGroup = cardUILeft[i].GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f;
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }
            }
        }
        for (int i = 0; i < playerRight.Length; i++)
        {
            if (playerRight[i] != null)
            {
                cardUIRight[i].SetCardUI(playerRight[i]);
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
        if (spawner != null)
        {
            spawner.SpawnSoldiers(playerLeft, cardUILeft, 0);
            spawner.SpawnSoldiers(playerRight, cardUIRight, 1);
        }
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

}
