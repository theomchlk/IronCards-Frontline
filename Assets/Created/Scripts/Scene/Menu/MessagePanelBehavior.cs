using UnityEngine;
using TMPro;

public class MessagePanelBehavior : MonoBehaviour
{
    public static MessagePanelBehavior Local;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text messageText;
    private float _timeStay;
    private float _timeFade;
    
    public void Awake()
    {
        Local = this;
    }

    public void Update()
    {
        if (_timeStay > 0)
        {
            _timeStay -= Time.deltaTime;
            return;
        }

        if (_timeFade > 0)
        {
            canvasGroup.alpha = canvasGroup.alpha * (_timeFade - Time.deltaTime) / _timeFade; ;
            _timeFade -= Time.deltaTime;
        }
    }

    public void SetMessage(string message, float timeStay, float timeFade)
    {
        canvasGroup.alpha = 1;
        messageText.text = message;
        _timeStay = timeStay;
        _timeFade = timeFade;
    }
    
    
}
