using UnityEngine;
public enum GameDifficulty
{
    Easy,
    Normal,
    Hard
}   
public enum Language
{
    English,
    Czech
}
public class GameSettings : MonoBehaviour
{
    public static bool autoSave = true;
    public static int autoSaveTime = 6000; // in seconds 
    public static GameDifficulty currentDifficulty = GameDifficulty.Normal;
    public static bool needToEat = true;
    public static bool needToDrink = true;
    public static bool needToSleep = true;
    public static bool staminaEnabled = true;
    public static bool inventoryKapacityEnabled = true;
    public static int inventoryKapacity = 20;
    public static int masterVolume = 100;
    public static int musicVolume = 100;
    public static int sfxVolume = 100;
    public static int ambientVolume = 100;
    public static bool ambientVolumeEnabled = true;
    public static bool sfxVolumeEnabled = true;
    public static bool musicVolumeEnabled = true;
    public static Language currentLanguage = Language.English;
    public static bool fpsShown = false;
    public static bool pingShown = false;
}
