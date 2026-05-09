using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls; // Indispensable pour le nouveau système

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

    void Awake()
    {
        cam = GetComponent<Camera>();
        Vector3 angles = transform.eulerAngles;
        pitch = angles.x;
        yaw = angles.y;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
        HandleGroundCollision();
        HandleConstraints();
    }

    private void HandleMovement()
    {
        Vector3 move = Vector3.zero;
        var keyboard = Keyboard.current;

        if (keyboard == null) return;

        if (keyboard.wKey.isPressed) move += transform.forward;
        if (keyboard.sKey.isPressed) move += -transform.forward;
        if (keyboard.aKey.isPressed) move += -transform.right;
        if (keyboard.dKey.isPressed) move += transform.right;

        if (keyboard.leftCtrlKey.isPressed) move.y -= 1f;
        if (keyboard.spaceKey.isPressed) move.y += 1f;

        if (keyboard.leftShiftKey.isPressed) speedMultiplier = 2f;
        else speedMultiplier = 1f;
        
        transform.position += move.normalized * moveSpeed * speedMultiplier * Time.deltaTime;
    }

    private void HandleRotation()
    {
        var mouse = Mouse.current;
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
        var mouse = Mouse.current;
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
        Vector3 pos = transform.position;
        pos.y = Mathf.Min(pos.y, maxHeight);

        float minX = float.MaxValue, maxX = float.MinValue, minZ = float.MaxValue, maxZ = float.MinValue;
        bool foundGround = false;

        for (int i = 0; i < ground.Length; i++)
        {
            Renderer groundRenderer = ground[i].GetComponent<Renderer>();
            if (groundRenderer != null)
            {
                foundGround = true;
                Bounds bounds = groundRenderer.bounds;
                minX = Mathf.Min(minX, bounds.min.x);
                maxX = Mathf.Max(maxX, bounds.max.x);
                minZ = Mathf.Min(minZ, bounds.min.z);
                maxZ = Mathf.Max(maxZ, bounds.max.z);
            }
        }

        if (foundGround)
        {            
            pos.x = Mathf.Clamp(pos.x, minX+0.2f, maxX-0.2f);
            pos.z = Mathf.Clamp(pos.z, minZ+0.2f, maxZ-0.2f);
        }

        transform.position = pos;
    }

}