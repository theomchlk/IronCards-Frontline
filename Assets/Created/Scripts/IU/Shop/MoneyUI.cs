using System;
using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    
    [SerializeField] private TMP_Text moneyText;
    

    public void ChangeMoneyText(int money)
    {
        moneyText.text = money.ToString();
    }
}
