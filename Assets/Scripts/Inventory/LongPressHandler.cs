using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class LongPressHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private float requiredHoldTime = 0.6f;
    public UnityEvent onLongPress = new UnityEvent();

    [Header("Visual Progress")]
    [SerializeField] private Image fillImage; 
    [SerializeField] private Color successColor = Color.green;

    [Header("Selection Status")]
    [SerializeField] private Image backgroundToColor; 
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color pickedColor = new Color(0.7f, 1f, 0.7f); 

    private bool isPointerDown;
    private float pointerDownTimer;
    private Color originalFillColor;

    void Start()
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = 0;
            originalFillColor = fillImage.color;
        }
    }

    public void SetPickedStatus(bool isPicked)
    {
        if (backgroundToColor != null)
        {
            backgroundToColor.color = isPicked ? pickedColor : normalColor;
        }
    }

    void Update()
    {
        if (isPointerDown)
        {
            pointerDownTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(pointerDownTimer / requiredHoldTime);

            if (fillImage != null)
            {
                fillImage.fillAmount = progress;
                Debug.Log($"[LongPress] Progress: {progress * 100}% na {gameObject.name}");
            }

            if (pointerDownTimer >= requiredHoldTime)
            {
                Debug.Log($"[LongPress] DOSAŽENO LIMITU na {gameObject.name}");
                onLongPress?.Invoke();
                StartCoroutine(SuccessFlash());
                ResetPointer();
            }
        }
    }

    private IEnumerator SuccessFlash()
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = 1;
            fillImage.color = successColor;
            yield return new WaitForSeconds(0.2f);
            fillImage.color = originalFillColor;
            fillImage.fillAmount = 0;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"[LongPress] Pointer DOWN na {gameObject.name}");
        isPointerDown = true;
        pointerDownTimer = 0;
    }

    public void OnPointerUp(PointerEventData eventData) => ResetPointer();
    public void OnPointerExit(PointerEventData eventData) => ResetPointer();

    private void ResetPointer()
    {
        isPointerDown = false;
        pointerDownTimer = 0;
        if (fillImage != null && !LeanTweenIsActive()) 
        {
            fillImage.fillAmount = 0;
        }
    }

    private bool LeanTweenIsActive() => pointerDownTimer >= requiredHoldTime;
}