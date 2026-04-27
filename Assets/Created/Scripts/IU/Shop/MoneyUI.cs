using System;
using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    
    [SerializeField] private TMP_Text moneyText;

    public void OnEnable()
    {
        throw new NotImplementedException();
    }

    public void ChangeMoneyText(int money)
    {
        moneyText.text = money.ToString();
    }
}
