using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSettingsUIManager : MonoBehaviour
{
    [Header("Hlavní okno nastavení")]
    // Sem v Inspectoru přetáhni ten samotný panel s nastavením
    public GameObject settingsPanel;
    
    
    [Header("Toggles (On/Off)")]
    public Toggle autoSaveToggle;
    public Toggle needToEatToggle;
    public Toggle staminaToggle;
    public Toggle fpsToggle;

    [Header("Sliders (0 - 100)")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    [Header("Dropdowns")]
    public TMP_Dropdown difficultyDropdown;
    public TMP_Dropdown languageDropdown;

    void Start()
    {
        // 1. Nejprve synchronizujeme UI s tím, co už je načteno v GameSettings
        SyncUIWithSettings();

        // 2. Navěsíme "posluchače". Když s něčím hneš, hned se to propíše a uloží.
        autoSaveToggle.onValueChanged.AddListener(val => { GameSettings.autoSave = val; SaveAndApply(); });
        needToEatToggle.onValueChanged.AddListener(val => { GameSettings.needToEat = val; SaveAndApply(); });
        staminaToggle.onValueChanged.AddListener(val => { GameSettings.staminaEnabled = val; SaveAndApply(); });
        fpsToggle.onValueChanged.AddListener(val => { GameSettings.fpsShown = val; SaveAndApply(); });

        masterVolumeSlider.onValueChanged.AddListener(val => { GameSettings.masterVolume = (int)val; SaveAndApply(); });
        musicVolumeSlider.onValueChanged.AddListener(val => { GameSettings.musicVolume = (int)val; SaveAndApply(); });
        sfxVolumeSlider.onValueChanged.AddListener(val => { GameSettings.sfxVolume = (int)val; SaveAndApply(); });

        difficultyDropdown.onValueChanged.AddListener(val => { GameSettings.currentDifficulty = (GameDifficulty)val; SaveAndApply(); });
        languageDropdown.onValueChanged.AddListener(val => { GameSettings.currentLanguage = (Language)val; SaveAndApply(); });
    }

    // Načte aktuální statické hodnoty do UI prvků, aby to odpovídalo savu
    private void SyncUIWithSettings()
    {
        autoSaveToggle.isOn = GameSettings.autoSave;
        needToEatToggle.isOn = GameSettings.needToEat;
        staminaToggle.isOn = GameSettings.staminaEnabled;
        fpsToggle.isOn = GameSettings.fpsShown;

        masterVolumeSlider.value = GameSettings.masterVolume;
        musicVolumeSlider.value = GameSettings.musicVolume;
        sfxVolumeSlider.value = GameSettings.sfxVolume;

        // Enumy jdou do Dropdownů hezky přecastoavt na (int), protože defaultně jdou od 0, 1, 2...
        difficultyDropdown.value = (int)GameSettings.currentDifficulty;
        languageDropdown.value = (int)GameSettings.currentLanguage;
    }

    // Zavolá tvoji existující logiku pro uložení do JSONu
    private void SaveAndApply()
    {
        // Posbírá aktuální stav GameSettings a kláves a hodí to do paměti GameData
        gameDataManager.CaptureCurrentSettings(); 
        
        // Fyzicky to zapíše do Data.json na disk
        gameDataManager.SaveData(null, false); 
    }
    
    // Zavoláš tlačítkem "Settings" v menu
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        
        // Pro-gamer move: Pokaždé když to otevřeš, znovu načti hodnoty.
        // Kdyby se něco změnilo na pozadí, ať nemáš v UI starý data.
        SyncUIWithSettings(); 
    }

    // Zavoláš křížkem nebo tlačítkem "Zpět"
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    // Pokud bys to chtěl zavírat i otevírat jedním tlačítkem/klávesou
    public void ToggleSettings()
    {
        bool isCurrentlyOpen = settingsPanel.activeSelf;
        
        // Přepne stav na opačný (když je to zapnuté, tak to vypne a naopak)
        settingsPanel.SetActive(!isCurrentlyOpen);

        // Pokud to zrovna otevíráme, sesynchronizujeme UI
        if (!isCurrentlyOpen)
        {
            SyncUIWithSettings();
        }
    }
}