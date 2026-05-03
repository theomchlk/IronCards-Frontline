using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Created.Scripts.IU
{
    public class CardStallUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text cardStallNameText;
        [SerializeField] private Image cardStallImage;
        public Transform cardsContainerLocation;

        public void SetCardStall(CardStallSO data)
        {
            
            cardStallNameText.text = data.cardStallName;
            cardStallImage.color = data.cardStallColor;
        }

        public void SetNewCardContainer(GameObject cardContainer, CardsSO cardData)
        {
            var ccGO = Instantiate(cardContainer, cardsContainerLocation);
            var csc = ccGO.GetComponent<CardStallContainer>();
            csc.SetCardStallContainer(cardData.cost);
            var cGO = Instantiate(cardData.goItemUI, csc.CardLocation);
            var cUI = cGO.GetComponent<CardUI>();
            cUI.SetCardUI(cardData);
        }
        
        
    }
}