using GameKit.Dependencies.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICamp : MonoBehaviour
{
    [SerializeField] private Image colorAlly;
    [SerializeField] private Transform allyCamp;
    [SerializeField] private Image colorEnnemy;
    [SerializeField] private Transform ennemyCamp;
    [SerializeField] private GameObject noEnnemyPanel;
    
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private GameObject slotInCampPrefab;
    

    public void SetUI(PlayerState ps, int nbRow, int nbCol)
    {
        ClearUI();
        colorAlly.color = ps.playerColor.Value;
        for (var i = 0; i < nbCol; i++)
        {
            var line = Instantiate(linePrefab, allyCamp);
            for (var j = 0; j < nbRow; j++)
            {
                var slotInCamp = Instantiate(slotInCampPrefab, line.transform).GetComponent<SlotInCamp>();
                slotInCamp.SetupAlly(j,i, ps.Camp);
            }
        }
    }
    public void SetEnnemy(PlayerState ps, int nbRow, int nbCol)
    {
        ClearEnnemyUI();
        colorEnnemy.color = ps.playerColor.Value;
        for (var i = 0; i < nbCol; i++)
        {
            var line = Instantiate(linePrefab, ennemyCamp);
            for (var j = 0; j < nbRow; j++)
            {
                var slotInCamp = Instantiate(slotInCampPrefab, line.transform).GetComponent<SlotInCamp>();
                slotInCamp.SetupEnnemy(j, i);
            }
        }
    }

    public void SetNoEnnemy()
    {
        noEnnemyPanel.SetActive(true);
    }
    


    public void RemoveCardFromCamp(CampType campType, Localisation loc)
    {
        var slot = (campType == CampType.Ally) ? 
            allyCamp.GetChild(loc.Col).GetChild(loc.Row).GetComponent<SlotInCamp>() :
            ennemyCamp.GetChild(loc.Col).GetChild(loc.Row).GetComponent<SlotInCamp>();
        slot.DragRejected();
    }

    private void ClearUI()
    {
        allyCamp.DestroyChildren();
    }

    private void ClearEnnemyUI()
    {
        ennemyCamp.DestroyChildren();
    }
    
}
