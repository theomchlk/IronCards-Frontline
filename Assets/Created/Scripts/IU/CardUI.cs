using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text cardNameText;
    [SerializeField] private TMP_Text cardHPText;
    [SerializeField] private TMP_Text cardDPSText;
    [SerializeField] private TMP_Text cardRangeText;
    [SerializeField] private TMP_Text cardNbText;
    [SerializeField] private TMP_Text cardMSText;
    [SerializeField] private Image cardImage;
    [SerializeField] private Sprite cardSprite;
    //DATA
    [SerializeField] private CardsSO data;

    private CardItem _cardItem;


    void Awake()
    {
        cardNameText.text = data.name;
        cardHPText.text = data.health.ToString("F0");
        cardDPSText.text = data.dps.ToString("F0");
        cardRangeText.text = data.range.ToString("F0");
        cardNbText.text = data.nbSoldiers.ToString("F0");
        cardMSText.text = data.movementSpeed.ToString("F0");
        cardImage.sprite = cardSprite;

    }
    
    public void Bind(CardItem cardItem) => _cardItem = cardItem;
}
