using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cards : MonoBehaviour
{
    [SerializeField] private int nbSoldiers;
    [SerializeField] private int cost;
    [SerializeField] private Soldier soldier;
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI cardHPText;
    [SerializeField] private TextMeshProUGUI cardDPSText;
    [SerializeField] private TextMeshProUGUI cardRangeText;
    [SerializeField] private TextMeshProUGUI cardNbText;
    [SerializeField] private TextMeshProUGUI cardMSText;
    [SerializeField] private Color contourColor = Color.black;
    [SerializeField] private Image contourImage;
    [SerializeField] private Color backgroundColor = Color.white;
    [SerializeField] private List<Image> backgroundImages;
    


    // OnValidate is called when the script is loaded or a value is changed in the inspector (Editor only)
    void OnValidate()
    {
        cardNameText.text = soldier.GetName();
        contourImage.color = contourColor;
        foreach (var background in backgroundImages)
        {
            background.color = backgroundColor;
        }
        cardHPText.text = soldier.GetMaxHealth().ToString();
        cardDPSText.text = ((1/soldier.GetAttackSpeed())*soldier.GetDamage()).ToString("F1").Replace(",", ".");
        cardRangeText.text = soldier.GetRange().ToString();
        cardNbText.text = nbSoldiers.ToString();
        cardMSText.text = soldier.GetMoveSpeed().ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        cardNameText.text = soldier.GetName();
        contourImage.color = contourColor;
        foreach (var background in backgroundImages)
        {
            background.color = backgroundColor;
        }
        cardHPText.text = soldier.GetMaxHealth().ToString();
        cardDPSText.text = ((1/soldier.GetAttackSpeed())*soldier.GetDamage()).ToString("F1").Replace(",", ".");
        cardRangeText.text = soldier.GetRange().ToString();
        cardNbText.text = nbSoldiers.ToString();
        cardMSText.text = soldier.GetMoveSpeed().ToString();
    }

}
