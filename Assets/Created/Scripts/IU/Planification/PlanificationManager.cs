using System.Collections;
using System.Collections.Generic;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlanificationManager : MonoBehaviour
{
    [Header("Conteneurs camps")]
    [SerializeField] private RectTransform allyContainer;
    [SerializeField] private RectTransform enemyContainer;
    [SerializeField] private GameObject noEnemyPanel;

    [Header("Prefabs")]
    [Tooltip("Carte avec CardUI + PlanificationCard.")]
    [SerializeField] private GameObject cardPrefab;
    [Tooltip("Slot instancié pour CHAQUE cellule (rempli ou vide) afin de garder la grille uniforme. La carte est placée dedans.")]
    [SerializeField] private GameObject slotPrefab;
    [Tooltip("Image fine, pivot (0, 0.5), pour tracer les lignes.")]
    [SerializeField] private GameObject lineImagePrefab;
    [Tooltip("Parent des lignes (RectTransform plein écran, au-dessus des cartes).")]
    [SerializeField] private RectTransform lineLayer;

    [Header("Couleurs contour")]
    [SerializeField] private Color noTargetColor = Color.red;
    [SerializeField] private Color hasTargetColor = Color.green;

    private PlayerState _local;
    private PlayerState _enemy;

    private readonly Dictionary<Localisation, PlanificationCard> _allyCards = new();
    private readonly Dictionary<int, PlanificationCard> _cardsByGroupId = new();
    private readonly Dictionary<Localisation, RectTransform> _committedLines = new();
    private RectTransform _dragLine;

    private IEnumerator Start()
    {
        while (PlayerState.Local == null) yield return null;
        _local = PlayerState.Local;

        int oppId = GameManager.Instance.GetOpponent(_local.IdPlayer);
        _enemy = PlayerRegistry.GetPlayerState(oppId);

        int nbRow = GameManager.Instance.nbRow.Value;
        int nbCol = GameManager.Instance.nbCol.Value;

        BuildCamp(allyContainer, _local, true, nbRow, nbCol);

        if (_enemy != null) BuildCamp(enemyContainer, _enemy, false, nbRow, nbCol);
        else if (noEnemyPanel != null) noEnemyPanel.SetActive(true);

        _local.Camp.CardTargets.OnChange += OnTargetsChanged;
        RefreshAll();
    }

    private void OnDestroy()
    {
        if (_local != null && _local.Camp != null)
            _local.Camp.CardTargets.OnChange -= OnTargetsChanged;
    }

    private void BuildCamp(RectTransform container, PlayerState owner, bool isAlly, int nbRow, int nbCol)
    {
        var cardsOnCamp = owner.Camp.CardsOnCamp;

        for (int col = 0; col < nbCol; col++)
        {
            var colGo = new GameObject($"Col_{col}", typeof(RectTransform));
            colGo.transform.SetParent(container, false);
            var vlg = colGo.AddComponent<VerticalLayoutGroup>();
            vlg.childControlWidth = vlg.childControlHeight = true;
            vlg.childForceExpandWidth = vlg.childForceExpandHeight = true;
            vlg.spacing = 10f;

            for (int row = 0; row < nbRow; row++)
            {
                // Un slot pour CHAQUE cellule : garde la grille uniforme même avec des cases vides
                var slot = Instantiate(slotPrefab, colGo.transform).transform;

                var loc = new Localisation(row, col);
                if (!cardsOnCamp.TryGetValue(loc, out string cardId) || string.IsNullOrEmpty(cardId))
                    continue;

                // La carte est placée DANS le slot (elle s'étire pour le remplir)
                var go = Instantiate(cardPrefab, slot);

                var cardUI = go.GetComponent<CardUI>();
                var data = DataBaseItem.Instance.GetDataItem(cardId) as CardsSO;
                if (cardUI != null && data != null) cardUI.SetCardUI(data);

                var pc = go.GetComponent<PlanificationCard>();
                pc.Init(this, loc, owner.IdPlayer, isAlly);

                int gid = FightManager.ComputeGroupId(owner.IdPlayer, row, col);
                _cardsByGroupId[gid] = pc;
                if (isAlly) _allyCards[loc] = pc;
            }
        }
    }

    // ==========================================
    // DRAG & DROP DE CIBLAGE
    // ==========================================

    public void BeginTargeting(PlanificationCard from, Vector2 screenPos)
    {
        if (_dragLine == null && lineImagePrefab != null)
            _dragLine = Instantiate(lineImagePrefab, lineLayer).GetComponent<RectTransform>();
        if (_dragLine != null) _dragLine.gameObject.SetActive(true);
        UpdateDragLine(from.Rect, screenPos);
    }

    public void UpdateTargeting(Vector2 screenPos)
    {
        if (_dragLineOrigin != null) UpdateDragLine(_dragLineOrigin, screenPos);
    }

    private RectTransform _dragLineOrigin;

    private void UpdateDragLine(RectTransform from, Vector2 screenPos)
    {
        _dragLineOrigin = from;
        if (_dragLine == null) return;
        PositionLine(_dragLine, from.position, screenPos);
    }

    public void EndTargeting(PlanificationCard from, PointerEventData e)
    {
        if (_dragLine != null) _dragLine.gameObject.SetActive(false);
        _dragLineOrigin = null;

        PlanificationCard target = FindCardUnderPointer(e);

        if (target == null || target == from)
        {
            // Drop dans le vide (ou sur soi-même) : on retire la cible existante
            _local.Camp.ServerClearCardTarget(from.Loc);
            return;
        }

        int targetGroupId = FightManager.ComputeGroupId(target.OwnerPlayerId, target.Loc.Row, target.Loc.Col);
        _local.Camp.ServerSetCardTarget(from.Loc, targetGroupId);
    }

    // Trouve la carte sous le curseur, que le raycast tombe sur la carte ou sur son slot parent.
    private PlanificationCard FindCardUnderPointer(PointerEventData e)
    {
        if (e.pointerEnter != null)
        {
            var pc = e.pointerEnter.GetComponentInParent<PlanificationCard>()
                     ?? e.pointerEnter.GetComponentInChildren<PlanificationCard>();
            if (pc != null) return pc;
        }

        if (EventSystem.current == null) return null;

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(e, results);
        foreach (var r in results)
        {
            var pc = r.gameObject.GetComponentInParent<PlanificationCard>()
                     ?? r.gameObject.GetComponentInChildren<PlanificationCard>();
            if (pc != null) return pc;
        }
        return null;
    }

    // ==========================================
    // CONTOURS + LIGNES VALIDÉES
    // ==========================================

    private void OnTargetsChanged(SyncDictionaryOperation op, Localisation key, int value, bool asServer)
    {
        RefreshAll();
    }

    private void RefreshAll()
    {
        var targets = _local.Camp.CardTargets;

        // Contours
        foreach (var kvp in _allyCards)
            kvp.Value.SetOutline(targets.ContainsKey(kvp.Key), noTargetColor, hasTargetColor);

        // Lignes : on reconstruit l'ensemble
        foreach (var line in _committedLines.Values)
            if (line != null) Destroy(line.gameObject);
        _committedLines.Clear();

        if (lineImagePrefab == null || lineLayer == null) return;

        foreach (var kvp in targets)
        {
            if (!_allyCards.TryGetValue(kvp.Key, out var fromCard)) continue;
            if (!_cardsByGroupId.TryGetValue(kvp.Value, out var toCard)) continue;

            var line = Instantiate(lineImagePrefab, lineLayer).GetComponent<RectTransform>();
            PositionLine(line, fromCard.Rect.position, toCard.Rect.position);
            _committedLines[kvp.Key] = line;
        }
    }

    private void PositionLine(RectTransform line, Vector3 fromWorld, Vector3 toWorld)
    {
        line.position = fromWorld;
        Vector3 dir = toWorld - fromWorld;
        float dist = dir.magnitude;
        line.sizeDelta = new Vector2(dist, line.sizeDelta.y);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        line.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
