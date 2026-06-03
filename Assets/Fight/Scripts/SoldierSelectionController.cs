using UnityEngine;
using UnityEngine.InputSystem;

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
    private int selectedGroupId = -1;
    private FightManager selectedFightManager;

    public Soldier HoveredSoldier => hoveredSoldier;
    public int SelectedGroupId => selectedGroupId;

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

        if (hoveredSoldier != null)
        {
            int localId = hoveredSoldier.GetFightManager().GetLocalPlayerState().IdPlayer;
            if (hoveredSoldier.GetOwnerId() == localId)
            {
                SelectGroup(hoveredSoldier);
            }
            else if (selectedGroupId >= 0)
            {
                selectedFightManager.CmdSetGroupTarget(selectedGroupId, hoveredSoldier.GetNetId());
            }
        }
        else if (selectedGroupId >= 0)
        {
            Vector3 targetPoint = GetMouseClickedPoint();
            if (targetPoint != Vector3.zero)
                selectedFightManager.CmdMoveGroupTo(selectedGroupId, targetPoint);
        }
    }

    private void SelectGroup(Soldier soldier)
    {
        if (selectedGroupId >= 0)
            selectedFightManager.CmdSetGroupControlled(selectedGroupId, false);

        selectedGroupId = soldier.GetGroupId();
        selectedFightManager = soldier.GetFightManager();
        selectedFightManager.CmdSetGroupControlled(selectedGroupId, true);
    }

    private void HandleEscapeKey()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame && selectedGroupId >= 0)
        {
            selectedFightManager.CmdSetGroupControlled(selectedGroupId, false);
            selectedGroupId = -1;
            selectedFightManager = null;
        }
    }

    private void UpdateCursorState()
    {
        if (selectedGroupId >= 0)
        {
            if (hoveredSoldier != null)
            {
                int localId = selectedFightManager.GetLocalPlayerState().IdPlayer;
                bool isEnemy = hoveredSoldier.GetOwnerId() != localId;
                SetCursor(isEnemy ? attackCursor : defaultCursor, Vector2.zero);
            }
            else
            {
                SetCursor(stepCursor, new Vector2(stepCursor.width / 2f, stepCursor.height / 2f));
            }
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
            return hit.collider.GetComponentInParent<Soldier>();

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
