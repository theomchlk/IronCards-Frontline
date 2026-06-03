using UnityEngine;
using TMPro;

public class PreparationTimerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;

    private void Start()
    {
        GameStateController gsc = GameStateController.Instance;
        if (gsc == null) return;

        gsc.preparationTimeRemaining.OnChange += OnTimeChanged;
        Refresh(gsc.preparationTimeRemaining.Value);
    }

    private void OnDestroy()
    {
        if (GameStateController.Instance == null) return;
        GameStateController.Instance.preparationTimeRemaining.OnChange -= OnTimeChanged;
    }

    private void OnTimeChanged(int prev, int next, bool asServer)
    {
        if (asServer) return;
        Refresh(next);
    }

    private void Refresh(int totalSeconds)
    {
        int m = totalSeconds / 60;
        int s = totalSeconds % 60;
        timerText.text = $"{m:00}:{s:00}";
    }
}
