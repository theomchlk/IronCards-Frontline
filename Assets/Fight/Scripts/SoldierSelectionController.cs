using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Gère la sélection, la commande et l'affichage UI des soldats contrôlés par le joueur.
/// Extrait de FreeCamController pour respecter le principe de responsabilité unique.
/// </summary>
[RequireComponent(typeof(Camera))]
public class SoldierSelectionController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private SoldierUIManager uiManager;

    [Header("Curseurs")]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D attackCursor;
    [SerializeField] private Texture2D stepCursor;

    [Header("Physique")]
    [SerializeField] private LayerMask groundLayer;

    private Camera cam;
    private Soldier hoveredSoldier;
    private Soldier selectedSoldier;

    public Soldier HoveredSoldier => hoveredSoldier;
    public Soldier SelectedSoldier => selectedSoldier;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        SetCursor(defaultCursor, Vector2.zero);
    }

    private void LateUpdate()
    {
        hoveredSoldier = GetSoldierUnderMouse();

        HandleEscapeKey();
        HandleMouseInteractions();
        UpdateCursorState();
        UpdateUI();
    }

    private void HandleMouseInteractions()
    {
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame) return;

        if (selectedSoldier == null && hoveredSoldier != null)
        {
            SelectSoldier(hoveredSoldier);
        }
        else if (selectedSoldier != null && hoveredSoldier != null
                 && hoveredSoldier != selectedSoldier
                 && selectedSoldier.GetOwnerId() != hoveredSoldier.GetOwnerId())
        {
            selectedSoldier.SetTarget(hoveredSoldier);
        }
        else if (selectedSoldier != null && hoveredSoldier == null)
        {
            CommandSoldierToMove();
        }
    }

    private void SelectSoldier(Soldier soldier)
    {
        selectedSoldier = soldier;
        selectedSoldier.SetIsControlledByPlayer(true);
    }

    private void CommandSoldierToMove()
    {
        Vector3 targetPoint = GetMouseClickedPoint();
        if (targetPoint != Vector3.zero)
        {
            selectedSoldier.HandleMovementRigidbody(targetPoint);
            selectedSoldier.SetTarget(null);
        }
    }

    private void HandleEscapeKey()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame && selectedSoldier != null)
        {
            selectedSoldier.SetIsControlledByPlayer(false);
            selectedSoldier.SetTarget(null);
            selectedSoldier = null;
        }
    }

    private void UpdateCursorState()
    {
        if (selectedSoldier != null)
        {
            if (hoveredSoldier != null && selectedSoldier.GetOwnerId() != hoveredSoldier.GetOwnerId())
                SetCursor(attackCursor, Vector2.zero);
            else
                SetCursor(stepCursor, new Vector2(stepCursor.width / 2f, stepCursor.height / 2f));
        }
        else
        {
            SetCursor(defaultCursor, Vector2.zero);
        }
    }

    private void UpdateUI()
    {
        if (hoveredSoldier != null)
        {
            hoveredSoldier.SetBloomMaterial();
            uiManager.ShowAndUpdateUI(hoveredSoldier);
        }
        else if (selectedSoldier != null)
        {
            selectedSoldier.SetBloomMaterial();
            uiManager.ShowAndUpdateUI(selectedSoldier);
        }
        else
        {
            uiManager.HideUI();
        }
    }

    public Soldier GetSoldierUnderMouse()
    {
        if (Mouse.current == null) return null;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 500f))
        {
            return hit.collider.GetComponentInParent<Soldier>();
        }

        return null;
    }

    private Vector3 GetMouseClickedPoint()
    {
        if (Mouse.current == null) return Vector3.zero;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 500f, groundLayer))
            return hit.point;

        return Vector3.zero;
    }

    private void SetCursor(Texture2D texture, Vector2 hotspot)
    {
        if (texture != null)
            Cursor.SetCursor(texture, hotspot, CursorMode.Auto);
    }
}
