using TMPro;
using UnityEngine;

public class SoldierUIManager : MonoBehaviour
{
    [Header("Stats Components")]
    [SerializeField] private TextMeshProUGUI soldierNameText;
    [SerializeField] private RectTransform soldierHealthBar;
    [SerializeField] private TextMeshProUGUI soldierHealthText;
    [SerializeField] private TextMeshProUGUI soldierRangeText;
    [SerializeField] private TextMeshProUGUI soldierDamageText;
    [SerializeField] private TextMeshProUGUI soldierAttackSpeedText;
    [SerializeField] private TextMeshProUGUI soldierMoveSpeedText;

    public void ShowAndUpdateUI(Soldier soldier)
    {
        if (soldier == null) return;

        gameObject.SetActive(true);

        float health = soldier.GetHealth();
        float maxHealth = soldier.GetMaxHealth();
        soldierHealthBar.anchorMax = new Vector2(health / maxHealth, soldierHealthBar.anchorMax.y);

        soldierNameText.text = soldier.GetName();
        soldierHealthText.text = $"{health} / {maxHealth}";
        
        soldierRangeText.text = soldier.GetRange().ToString().Replace(',', '.');
        soldierDamageText.text = soldier.GetDamage().ToString().Replace(',', '.');
        soldierAttackSpeedText.text = soldier.GetAttackSpeed().ToString().Replace(',', '.');
        soldierMoveSpeedText.text = soldier.GetMoveSpeed().ToString().Replace(',', '.');
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
    }
}