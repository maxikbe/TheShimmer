using UnityEngine;

public class FakeShadowController : MonoBehaviour
{
    public TimeAndLight timeSystem; 
    
    [Header("Shadow Range (0 to 1 scale)")]
    public float sunriseTime = 0.2f; 
    public float sunsetTime = 0.8f;  
    public float noonTime = 0.5f;     

    [Header("Visual Settings")]
    public float maxStretch = 2.5f;

    private GameObject shadowObject;
    private SpriteRenderer shadowRenderer;

    void Start()
    {
        if (timeSystem == null) timeSystem = FindFirstObjectByType<TimeAndLight>(); 
        
        shadowObject = transform.GetChild(0).gameObject;
        shadowRenderer = shadowObject.GetComponent<SpriteRenderer>(); 
    }

    void Update()
    {
        if (timeSystem == null || shadowObject == null || shadowRenderer == null) return;

        float time = timeSystem.currentTime;

        if (time >= sunriseTime && time <= sunsetTime)
        {
            if (!shadowObject.activeSelf) shadowObject.SetActive(true);

            float dayDuration = (sunsetTime - sunriseTime) / 2f;
            float normalizedTime = (time - noonTime) / dayDuration;
            normalizedTime = Mathf.Clamp(normalizedTime, -1f, 1f);

            float currentYScale = -normalizedTime * maxStretch;
            
            shadowObject.transform.localScale = new Vector3(1f, currentYScale, 1f);

            float distFromNoon = Mathf.Abs(normalizedTime); 
            float alpha = Mathf.Lerp(0.4f, 0.1f, distFromNoon);
            shadowRenderer.color = new Color(0, 0, 0, alpha);

            shadowObject.transform.localPosition = Vector3.zero;
        }
        else
        {
            if (shadowObject.activeSelf) shadowObject.SetActive(false);
        }
    }
}