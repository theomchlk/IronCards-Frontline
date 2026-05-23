using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class FreeCamController : MonoBehaviour
{
    [Header("Mouvement")]
    [SerializeField] private float moveSpeed = 20f;
    private float speedMultiplier = 1f;
    [SerializeField] private float lookSpeed = 0.5f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 5f;

    [Header("Collision")]
    [SerializeField] private float minDistanceToGround = 1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Constraints")]
    [SerializeField] private float maxHeight = 80f;
    [SerializeField] private GameObject[] ground;

    private float pitch = 0f;
    private float yaw = 0f;
    private Camera cam;
    private int fpsFrameCount = 0;
    private float fpsElapsed = 0f;
    private float fps = 0f;
    private float minConstraintX, maxConstraintX, minConstraintZ, maxConstraintZ;
    private bool hasGroundConstraints = false;

    void Awake()
    {
        cam = GetComponent<Camera>();
        Vector3 angles = transform.eulerAngles;
        pitch = angles.x;
        yaw = angles.y;
        
        CalculateGroundBounds();    
    }

    void LateUpdate()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
        HandleGroundCollision();
        HandleConstraints();

        // DisplayFPS();
    }

    private void CalculateGroundBounds()
    {
        minConstraintX = float.MaxValue; maxConstraintX = float.MinValue; 
        minConstraintZ = float.MaxValue; maxConstraintZ = float.MinValue;

        for (int i = 0; i < ground.Length; i++)
        {
            Renderer groundRenderer = ground[i].GetComponent<Renderer>();
            if (groundRenderer != null)
            {
                hasGroundConstraints = true;
                Bounds bounds = groundRenderer.bounds;
                minConstraintX = Mathf.Min(minConstraintX, bounds.min.x);
                maxConstraintX = Mathf.Max(maxConstraintX, bounds.max.x);
                minConstraintZ = Mathf.Min(minConstraintZ, bounds.min.z);
                maxConstraintZ = Mathf.Max(maxConstraintZ, bounds.max.z);
            }
        }
    }

    private void HandleMovement()
    {
        Vector3 move = Vector3.zero;
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null) return;

        if (keyboard.wKey.isPressed || keyboard.zKey.isPressed) move += transform.forward;
        if (keyboard.sKey.isPressed) move += -transform.forward;
        if (keyboard.aKey.isPressed || keyboard.qKey.isPressed) move += -transform.right;
        if (keyboard.dKey.isPressed) move += transform.right;

        if (keyboard.leftCtrlKey.isPressed) move.y -= 1f;
        if (keyboard.spaceKey.isPressed) move.y += 1f;

        if (keyboard.leftShiftKey.isPressed) speedMultiplier = 2f;
        else speedMultiplier = 1f;
        
        transform.position += move.normalized * moveSpeed * speedMultiplier * Time.deltaTime;
    }

    private void HandleRotation()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.rightButton.isPressed)
        {
            Vector2 mouseDelta = mouse.delta.ReadValue();
            
            yaw += mouseDelta.x * lookSpeed;
            pitch -= mouseDelta.y * lookSpeed;
            pitch = Mathf.Clamp(pitch, -90f, 90f);

            transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }
    }

    private void HandleZoom()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        float scrollY = mouse.scroll.ReadValue().y;

        if (scrollY != 0)
        {
            Ray ray = cam.ScreenPointToRay(mouse.position.ReadValue());
            transform.position += ray.direction * scrollY * 0.01f * zoomSpeed;
        }
    }

    private void HandleGroundCollision()
    {
        Vector3 rayOrigin = new Vector3(transform.position.x, maxHeight + 5f, transform.position.z);
        Ray ray = new Ray(rayOrigin, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxHeight + 10f, groundLayer))
        {
            float limitY = hit.point.y + minDistanceToGround;
            if (transform.position.y < limitY)
            {
                transform.position = new Vector3(transform.position.x, limitY, transform.position.z);
            }
        }
    }

    private void HandleConstraints()
    {
        if (!hasGroundConstraints) return;

        Vector3 pos = transform.position;
        pos.y = Mathf.Min(pos.y, maxHeight);
        pos.x = Mathf.Clamp(pos.x, minConstraintX + 0.2f, maxConstraintX - 0.2f);
        pos.z = Mathf.Clamp(pos.z, minConstraintZ + 0.2f, maxConstraintZ - 0.2f);
        transform.position = pos;
    }

    private void DisplayFPS()
    {
        fpsFrameCount++;
        fpsElapsed += Time.unscaledDeltaTime;
        if (fpsElapsed >= 1f)
        {
            fps = fpsFrameCount / fpsElapsed;
            Debug.Log($"FPS: {fps:F1}");
            fpsFrameCount = 0;
            fpsElapsed = 0f;
        }
    }
}