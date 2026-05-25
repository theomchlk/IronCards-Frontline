using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReadyButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;
    [SerializeField] private Color readyColor = new Color(0.2f, 0.8f, 0.2f);
    [SerializeField] private Color defaultColor = Color.white;

    private PlayerState _ps;

    private void Start()
    {
        button.onClick.AddListener(OnClick);
        if (PlayerState.Local != null)
            Bind(PlayerState.Local);
        else
            StartCoroutine(WaitForLocalPlayer());
    }

    private IEnumerator WaitForLocalPlayer()
    {
        while (PlayerState.Local == null)
            yield return null;
        Bind(PlayerState.Local);
    }

    private void Bind(PlayerState ps)
    {
        _ps = ps;
        ps.isReady.OnChange += OnReadyChanged;
        Refresh(ps.isReady.Value);
    }

    private void OnDestroy()
    {
        if (_ps != null)
            _ps.isReady.OnChange -= OnReadyChanged;
        button.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        if (_ps == null) return;
        _ps.ServerToggleReady();
        StartCoroutine(PunchScale());
    }

    private void OnReadyChanged(bool prev, bool next, bool asServer)
    {
        if (asServer) return;
        Refresh(next);
    }

    private void Refresh(bool ready)
    {
        label.text = ready ? "Annuler" : "Prêt !";
        button.image.color = ready ? readyColor : defaultColor;
    }

    private IEnumerator PunchScale()
    {
        Transform t = button.transform;
        Vector3 original = t.localScale;
        Vector3 big = original * 1.15f;
        float half = 0.08f;

        for (float e = 0f; e < half; e += Time.deltaTime)
        {
            t.localScale = Vector3.Lerp(original, big, e / half);
            yield return null;
        }
        for (float e = 0f; e < half; e += Time.deltaTime)
        {
            t.localScale = Vector3.Lerp(big, original, e / half);
            yield return null;
        }
        t.localScale = original;
    }
}
