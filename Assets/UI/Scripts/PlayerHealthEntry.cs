using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Une ligne de la liste des joueurs : chiffre de vie à gauche,
/// pseudo (au-dessus) + barre de vie (en-dessous) à droite.
/// </summary>
public class PlayerHealthEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image healthBarFill;

    [Header("Couleurs barre de vie")]
    [SerializeField] private Color localColor = new Color(1f, 0.6f, 0f); // orange
    [SerializeField] private Color enemyColor = Color.red;

    private PlayerState _ps;
    private int _maxHp = 1;

    public PlayerState Player => _ps;

    public void Bind(PlayerState ps, bool isLocal)
    {
        _ps = ps;
        _maxHp = Mathf.Max(1, ps.MaxHp);

        Color playerColor = ps.playerColor.Value;
        nameText.text = ps.playerName.Value;
        nameText.color = playerColor;

        healthBarFill.color = isLocal ? localColor : enemyColor;

        _ps.HpVar.OnChange += OnHpChanged;
        Refresh(ps.Hp);
    }

    private void OnHpChanged(int previous, int next, bool asServer) => Refresh(next);

    private void Refresh(int hp)
    {
        hpText.text = hp.ToString();
        healthBarFill.fillAmount = Mathf.Clamp01((float)hp / _maxHp);
    }

    private void OnDestroy()
    {
        if (_ps != null) _ps.HpVar.OnChange -= OnHpChanged;
    }
}
