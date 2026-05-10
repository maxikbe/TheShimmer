using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;

public class TimeAndLight : MonoBehaviour
{
    public Light2D globalLight;
    public Gradient nightDayColor;
    public TextMeshProUGUI WatchText;
    public GameObject WatchUILongArm;
    public GameObject WatchUIShortArm;
    public float secondsInDay = 1200f;
    public float minIntensity = 0.01f;
    public float maxIntensity = 1.0f;
    
    [Range(0, 1)]
    public float currentTime = 0.5f;
    public int currectDay = 1;
    public TextMeshProUGUI dayTextUI;

    void Awake()
    {
        if (globalLight == null) globalLight = GetComponent<Light2D>();
    }

    void Start()
    {
        currentTime = gameDataManager.currentGameData.player.time;
        currectDay = gameDataManager.currentGameData.player.dayNumber;
        dayTextUI.text = currectDay.ToString();
    }

    void Update()
    {
        currentTime += (Time.deltaTime / secondsInDay);
        if (currentTime >= 1) {currentTime = 0; currectDay +=1;}
        dayTextUI.text = currectDay.ToString();

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
        float totalMinutesInDay = currentTime * 1440f; 
        float hours = totalMinutesInDay / 60f;
        float minutes = totalMinutesInDay % 60f;

        if (WatchText != null)
        {
            string suffix = hours >= 12 ? "PM" : "AM";
            WatchText.text = suffix;
        }

        if (WatchUIShortArm != null)
        {
            float hourRotation = (currentTime * 720f);
            WatchUIShortArm.transform.localRotation = Quaternion.Euler(0, 0, -hourRotation);
        }

        if (WatchUILongArm != null)
        {
            float minuteRotation = (hours * 360f);
            WatchUILongArm.transform.localRotation = Quaternion.Euler(0, 0, -minuteRotation+90);
        }
    }
}