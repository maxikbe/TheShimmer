using UnityEngine;
using UnityEngine. EventSystems;

public class ButtonHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Nastavení zvětšení")]
    [SerializeField] private float hoverScale = 1.2f;        
    [SerializeField] private float animationSpeed = 10f;     

    private Vector3 originalScale;
    private Vector3 targetScale;

    private void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
    }
}