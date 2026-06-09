using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuUI : MonoBehaviour
{
    [Header("Liste des joueurs")]
    [SerializeField] private Transform playerListContainer;
    [SerializeField] private GameObject playerEntryPrefab;

    [Header("Boutons")]
    [SerializeField] private Button surrenderButton;
    [SerializeField] private Button collapseButton;

    [Header("Animation collapse")]
    [SerializeField] private RectTransform menuPanel;
    [SerializeField] private float collapseAnimDuration = 0.3f;

    private readonly Dictionary<PlayerState, PlayerHealthEntry> _entries = new();
    private bool _collapsed;
    private float _expandedX;
    private Coroutine _animCoroutine;

    private void Start()
    {
        _expandedX = menuPanel.anchoredPosition.x;

        if (surrenderButton != null) surrenderButton.onClick.AddListener(OnSurrender);
        if (collapseButton != null)  collapseButton.onClick.AddListener(ToggleCollapse);

        BuildList();
    }

    private void BuildList()
    {
        foreach (var ps in PlayerRegistry.GetAll)
            AddEntry(ps);
        SortEntries();
    }

    private void AddEntry(PlayerState ps)
    {
        if (ps == null || _entries.ContainsKey(ps)) return;

        var go = Instantiate(playerEntryPrefab, playerListContainer);
        var entry = go.GetComponent<PlayerHealthEntry>();
        entry.Bind(ps, ps == PlayerState.Local);

        _entries[ps] = entry;
        ps.HpVar.OnChange += OnAnyHpChanged;
    }

    private void OnAnyHpChanged(int previous, int next, bool asServer) => SortEntries();

    private void SortEntries()
    {
        var sorted = new List<PlayerState>(_entries.Keys);
        sorted.Sort((a, b) =>
        {
            int cmp = b.Hp.CompareTo(a.Hp);
            if (cmp != 0) return cmp;
            return string.Compare(a.playerName.Value, b.playerName.Value, StringComparison.OrdinalIgnoreCase);
        });

        for (int i = 0; i < sorted.Count; i++)
            _entries[sorted[i]].transform.SetSiblingIndex(i);
    }

    private void OnSurrender()
    {
        if (PlayerState.Local != null)
            PlayerState.Local.ServerPlayerSurrender();
    }

    private void ToggleCollapse()
    {
        _collapsed = !_collapsed;
        float targetX = _collapsed ? _expandedX + menuPanel.rect.width : _expandedX;

        if (_animCoroutine != null) StopCoroutine(_animCoroutine);
        _animCoroutine = StartCoroutine(SlidePanel(targetX));
    }

    private IEnumerator SlidePanel(float targetX)
    {
        Vector2 start = menuPanel.anchoredPosition;
        Vector2 end = new Vector2(targetX, start.y);

        float t = 0f;
        while (t < collapseAnimDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / collapseAnimDuration);
            menuPanel.anchoredPosition = Vector2.Lerp(start, end, k);
            yield return null;
        }
        menuPanel.anchoredPosition = end;
    }

    private void OnDestroy()
    {
        foreach (var ps in _entries.Keys)
            if (ps != null) ps.HpVar.OnChange -= OnAnyHpChanged;
    }
}
