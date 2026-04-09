using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;

public class TimeAndLight : MonoBehaviour
{
    public Light2D globalLight;
    public Gradient nightDayColor;
    public TextMeshProUGUI clockText;
    public float secondsInDay = 1200f;
    public float minIntensity = 0.01f;
    public float maxIntensity = 1.0f;
    
    [Range(0, 1)]
    public float currentTime = 0.5f;

    void Start()
    {
        if (globalLight == null) globalLight = GetComponent<Light2D>();
    }

    void Update()
    {
        currentTime += (Time.deltaTime / secondsInDay);
        if (currentTime >= 1) currentTime = 0;

        float intensity = CalculateIntensity();
        globalLight.intensity = intensity;
        globalLight.color = nightDayColor.Evaluate(currentTime);

        UpdateClock();
    }

    float CalculateIntensity()
    {
        if (currentTime >= 0.208f && currentTime < 0.375f)
        {
            float t = (currentTime - 0.208f) / (0.375f - 0.208f);
            return Mathf.Lerp(minIntensity, maxIntensity, t);
        }
        else if (currentTime >= 0.375f && currentTime < 0.666f)
        {
            return maxIntensity;
        }
        else if (currentTime >= 0.666f && currentTime < 0.875f)
        {
            float t = (currentTime - 0.666f) / (0.875f - 0.666f);
            return Mathf.Lerp(maxIntensity, minIntensity, t);
        }
        else
        {
            return minIntensity;
        }
    }

    void UpdateClock()
    {
        if (clockText == null) return;

        float totalMinutes = currentTime * 1440f; 
        int hours = Mathf.FloorToInt(totalMinutes / 60f);
        int minutes = Mathf.FloorToInt(totalMinutes % 60f);

        string suffix = hours >= 12 ? "PM" : "AM";
        int displayHour = hours % 12;
        if (displayHour == 0) displayHour = 12;

        clockText.text = string.Format("{0:0}:{1:00} {2}", displayHour, minutes, suffix);
    }
}