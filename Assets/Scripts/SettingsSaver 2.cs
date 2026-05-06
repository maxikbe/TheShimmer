using UnityEngine;
[System.Serializable]
public class SettingsSaver2
{
    // GameSettings
    public bool autoSave;
    public int autoSaveTime;
    public GameDifficulty currentDifficulty;
    public bool needToEat;
    public bool needToDrink;
    public bool needToSleep;
    public bool staminaEnabled;
    public bool inventoryKapacityEnabled;
    public int inventoryKapacity;
    public int masterVolume;
    public int musicVolume;
    public int sfxVolume;
    public int ambientVolume;
    public bool ambientVolumeEnabled;
    public bool sfxVolumeEnabled;
    public bool musicVolumeEnabled;
    public Language currentLanguage;
    public bool fpsShown;
    public bool pingShown;

    // KeyBoardSetting
    public KeyCode keyUp;
    public KeyCode keyDown;
    public KeyCode keyLeft;
    public KeyCode keyRight;
    public KeyCode keyRun;
    public KeyCode swapUp;
    public KeyCode swapDown;
    public KeyCode swapLeft;
    public KeyCode swapRight;
    public KeyCode swapAliveUp;
    public KeyCode swapAliveDown;
    public KeyCode chooseSpecialSpell;
    public KeyCode chooseNormalSpell;
    public KeyCode chooseItem;
    public KeyCode doAccept;
    public KeyCode doBack;
    public KeyCode jump;
    public KeyCode dodge;
    public KeyCode parry;
}
