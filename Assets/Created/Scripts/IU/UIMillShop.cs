using UnityEngine;
using TMPro;

public class UIMillShop : MonoBehaviour
{
    [SerializeField] private TMP_Text millCostText;
    [SerializeField] private TMP_Text nbMillText;



    public void BuyNewMill(MillItem item)
    {
        Debug.Log("New mill purchased !");
        SetUI();
    }

    public void OnEnable()
    {
        UIManager.Instance.OnSetUIsItems += SetUI;
    }

    public void OnDisable()
    {
        UIManager.Instance.OnSetUIsItems -= SetUI;
    }

    private void SetUI()
    {
        ChangeMillCostText(PlayerState.Local.millCost.Value);
        ChangeNbMillText(PlayerState.Local.nbMills.Value);
    }
    
    private void ChangeMillCostText(int cost)
    {
        millCostText.text = cost.ToString();
    }
    
    
    private void ChangeNbMillText(int nbMills)
    {
        nbMillText.text = nbMills.ToString();
    }
    

    private void ResetMillCost(int millCost)
    {
        ChangeMillCostText(millCost);
    }

    private void ResetNbMills(int nbMills)
    {
        ChangeNbMillText(nbMills);
    }
    
    
    
    
}
