using UnityEngine;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour
{
    public static PlayerGUI Instance { get; private set; }

    [Header("UI Progress Bars")]
    [SerializeField] private Image thirstBar;
    [SerializeField] private Image hungerBar;
    [SerializeField] private Image staminaBar;
    [SerializeField] private Image sleepBar;

    private float currentThirst, maxThirst;
    private float currentHunger, maxHunger;
    private float currentStamina, maxStamina;
    private float currentSleep, maxSleep;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        LoadInitialData();
        InvokeRepeating(nameof(UpdateLevelsPerMinute), 60f, 60f);
        InvokeRepeating(nameof(UpdateStaminaLevel), 0.1f, 0.1f);
    }

    private void LoadInitialData()
    {
        var p = gameDataManager.currentGameData.player;

        currentThirst = p.thirstLevel;
        maxThirst = p.maxThirstLevel;

        currentHunger = p.hungerLevel;
        maxHunger = p.maxHungerLevel;

        currentStamina = p.staminaLevel;
        maxStamina = p.maxStaminaLevel;

        currentSleep = p.sleepLevel;
        maxSleep = p.maxSleepLevel;

        RefreshAllVisuals();
    }

    private void UpdateLevelsPerMinute()
    {
        UpdateThirst(-2f);
        UpdateHunger(-2f);
        UpdateSleep(-0.5f);
    }

    private void UpdateStaminaLevel()
    {
        UpdateStamina(+0.5f);
    }

    public void UpdateThirst(float amount)
    {
        currentThirst = Mathf.Clamp(currentThirst + amount, 0, maxThirst);
        if (thirstBar != null) thirstBar.fillAmount = currentThirst / maxThirst;
    }

    public void UpdateHunger(float amount)
    {
        currentHunger = Mathf.Clamp(currentHunger + amount, 0, maxHunger);
        if (hungerBar != null) hungerBar.fillAmount = currentHunger / maxHunger;
    }

    public void UpdateStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina + amount, 0, maxStamina);
        if (staminaBar != null) staminaBar.fillAmount = currentStamina / maxStamina;
    }

    public void SetStamina(float current, float max)
    {
        currentStamina = Mathf.Clamp(current, 0, max);
        maxStamina = max;
        if (staminaBar != null) staminaBar.fillAmount = currentStamina / maxStamina;
    }

    public void UpdateSleep(float amount)
    {
        currentSleep = Mathf.Clamp(currentSleep + amount, 0, maxSleep);
        if (sleepBar != null) sleepBar.fillAmount = currentSleep / maxSleep;
    }

    private void RefreshAllVisuals()
    {
        UpdateThirst(0);
        UpdateHunger(0);
        UpdateStamina(0);
        UpdateSleep(0);
    }
}