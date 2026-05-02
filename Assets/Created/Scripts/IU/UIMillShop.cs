using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIMillShop : MonoBehaviour
{
    [SerializeField] private TMP_Text millCostText;
    [SerializeField] private TMP_Text nbMillText;
    [SerializeField] private BuyItemButton buyMillButton;

    void Awake()
    {
        var millItem = new MillItem();
        /*ATTENTION : Ici par simplicité, on mets en dur GetDataItem("mill") ce qui n'est pas dérangeant vu qu'on ne
         possède qu'un type de mill. Cependant  si on décide de mettre plusieurs type de mill (si il y a plusieurs espèce
         et qu'on se dit que les humains produisent plus mais pour plus cher que les orcs) alors il faudra revoir
         le système d'accès a la data par la DataBaseItem
        */
        
        var millSO = (MillSO)DataBaseItem.Instance.GetDataItem("mill");
        millItem.SetData(millSO);
        buyMillButton.SetItem(millItem);
    }

    /*public void BuyNewMill(MillItem item)
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
    }*/

    public void SetUI(int nbMills, int cost)
    {
        ChangeNbMillText(nbMills);
        ChangeMillCostText(cost);
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
