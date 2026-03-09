using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class LongPressHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private float delayBeforeStart = 0.5f;
    [SerializeField] private float requiredHoldTime = 0.6f;
    public UnityEvent onLongPress = new UnityEvent();

    [Header("Visual Progress")]
    [SerializeField] private Image fillImage; 
    [SerializeField] private Color successColor = Color.green;

    [Header("Selection Status")]
    [SerializeField] private Image backgroundToColor; 
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color pickedColor = new Color(0.9f, 0.9f, 0.9f); 

    private bool isPointerDown;
    private float pointerDownTimer;
    private Color originalFillColor;
    private bool longPressTriggered;

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
        if (backgroundToColor != null) backgroundToColor.color = isPicked ? pickedColor : normalColor;
    }

    void Update()
    {
        if (isPointerDown && !longPressTriggered)
        {
            pointerDownTimer += Time.deltaTime;

            if (pointerDownTimer > delayBeforeStart)
            {
                float progress = Mathf.Clamp01((pointerDownTimer - delayBeforeStart) / requiredHoldTime);

                if (fillImage != null)
                {
                    fillImage.fillAmount = progress;
                }

                if (pointerDownTimer >= delayBeforeStart + requiredHoldTime)
                {
                    longPressTriggered = true;
                    onLongPress?.Invoke();
                    StartCoroutine(SuccessFlash());
                }
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
        isPointerDown = true;
        pointerDownTimer = 0;
        longPressTriggered = false;
    }

    public void OnPointerUp(PointerEventData eventData) => ResetPointer();
    public void OnPointerExit(PointerEventData eventData) => ResetPointer();

    private void ResetPointer()
    {
        isPointerDown = false;
        if (!longPressTriggered && fillImage != null)
        {
            fillImage.fillAmount = 0;
        }
        pointerDownTimer = 0;
    }
}