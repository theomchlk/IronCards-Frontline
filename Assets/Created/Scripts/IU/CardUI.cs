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
    [SerializeField] private TMP_Text cardNotFoundText;
    [SerializeField] private BuyItemButton buyButton;
    

    private CardItem _cardItem;


    public void SetCardUI(CardsSO data)
    {
        cardNameText.text = data.name;
        cardHPText.text = data.health.ToString("F0");
        cardDPSText.text = data.dps.ToString("F0");
        cardRangeText.text = data.range.ToString("F0");
        cardNbText.text = data.nbSoldiers.ToString("F0");
        cardMSText.text = data.movementSpeed.ToString("F0");
        if (data.sprite) cardImage.sprite = data.sprite;
        else cardNotFoundText.gameObject.SetActive(true);
        
    }

    public void SetButton(CardsSO data, ShopItem shopItem)
    {
        buyButton.SetButton(data, shopItem);
    }
    
    public void Bind(CardItem cardItem) => _cardItem = cardItem;

    public void EnableBuyMode()
    {
        buyButton.gameObject.SetActive(true);
        //Désactiver le drag mode
    }

    public void EnableDragMode()
    {
        buyButton.gameObject.SetActive(false);
        //Activer le drag mode
    }
}
