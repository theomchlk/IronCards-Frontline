using TMPro;
using UnityEngine;

public class BattleMessageUI : MonoBehaviour
{
    public static BattleMessageUI Instance;

    [Tooltip("CanvasGroup placé sur le Panel — gère le fondu (fond + texte ensemble).")]
    [SerializeField] private CanvasGroup canvasGroup;
    [Tooltip("Le texte du message.")]
    [SerializeField] private TMP_Text messageText;

    [Header("Durées (secondes)")]
    [Tooltip("Temps pendant lequel le message reste affiché en entier.")]
    [SerializeField] private float displayTime = 3f;
    [Tooltip("Temps de disparition en fondu après l'affichage.")]
    [SerializeField] private float fadeTime = 1f;

    private float _stay;
    private float _fade;

    private void Awake()
    {
        Instance = this;

        if (canvasGroup == null) canvasGroup = GetComponentInChildren<CanvasGroup>(true);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
        else
        {
            Debug.LogWarning("[BattleMessageUI] Aucun CanvasGroup trouvé — le message restera visible en permanence.");
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Show(string message)
    {
        if (messageText != null) messageText.text = message;
        if (canvasGroup != null) canvasGroup.alpha = 1f;

        _stay = displayTime;
        _fade = fadeTime;
    }

    private void Update()
    {
        if (canvasGroup == null) return;

        if (_stay > 0f)
        {
            _stay -= Time.deltaTime;
            return;
        }

        if (_fade > 0f)
        {
            _fade -= Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(_fade / Mathf.Max(0.0001f, fadeTime));
        }
    }
}
