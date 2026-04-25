using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Created.Scripts.IU
{
    public class CardStallUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text cardStallNameText;
        [SerializeField] private Image cardStallImage;
        //DATA
        
        [SerializeField] private CardStallSO data;

        public void Awake()
        {
            cardStallNameText.text = data.cardStallName;
            cardStallImage.color = data.cardStallColor;
        }
    }
}