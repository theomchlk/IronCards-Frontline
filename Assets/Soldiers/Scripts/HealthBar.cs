using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Camera targetCamera;

    private void Start()
    {
        if (targetCamera != null) return;

        foreach (Camera cam in Camera.allCameras)
        {
            if (cam.gameObject.scene == gameObject.scene)
            {
                targetCamera = cam;
                return;
            }
        }

        targetCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (targetCamera != null)
            transform.rotation = targetCamera.transform.rotation;
        else 
            Debug.LogWarning("HealthBar: No target camera assigned.");
    }

    public void SetHealth(float current, float max)
    {
        if (max <= 0f) return;
        float ratio = Mathf.Clamp01(current / max);
        fillImage.fillAmount = ratio;
        fillImage.color = Color.Lerp(Color.red, Color.green, ratio);
    }
}
