using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class MainMenu : MonoBehaviour
{
    public GameObject saveButtonPrefab; 
    public Transform scrollContent;     
    public AudioSource musicSource;

    void Start()
    {
        string lastSaveFull = PlayerPrefs.GetString("SaveToLoad", "");
        if (string.IsNullOrEmpty(lastSaveFull))
        {
            lastSaveFull = GetLatestSaveFileName(); 
        }

        if (!string.IsNullOrEmpty(lastSaveFull))
        {
            string cleanName = Path.GetFileNameWithoutExtension(lastSaveFull);
            gameDataManager.userDefaultName = cleanName;
            gameDataManager.LoadData();
            if (gameDataManager.currentGameData != null && gameDataManager.currentGameData.settings != null)
            {
                musicSource.volume = gameDataManager.currentGameData.settings.FinalMusicVolume;
                Debug.Log("Nastavení hlasitosti načteno z posledního uloženého souboru: " + cleanName);
            }
        }
        musicSource.Play();
    }

    public void LoadLastGame()
    {
        string lastSave = PlayerPrefs.GetString("SaveToLoad", "");
        if (string.IsNullOrEmpty(lastSave)) lastSave = GetLatestSaveFileName();
        if (!string.IsNullOrEmpty(lastSave))
        {
            LoadGame(lastSave);
        }
    }

    private string GetLatestSaveFileName()
    {
        var directory = new DirectoryInfo(Application.persistentDataPath);
        var lastFile = directory.GetFiles("*.json")
            .Where(f => !f.Name.StartsWith("unity"))
            .OrderByDescending(f => f.LastWriteTime)
            .FirstOrDefault();
        return lastFile != null ? lastFile.Name : "";
    }

    public void NewGame()
    {
        string randomFileName = System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + UnityEngine.Random.Range(100, 999);
        InitializeGameJson.CreateSave(randomFileName);
        
        PlayerPrefs.SetString("SaveToLoad", randomFileName + ".json");
        gameDataManager.userDefaultName = randomFileName;
        

        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void OpenLoadMenu()
    {
        foreach (Transform child in scrollContent) Destroy(child.gameObject);

        string[] files = Directory.GetFiles(Application.persistentDataPath, "*.json");
        foreach (string filePath in files)
        {
            string fileName = Path.GetFileName(filePath);
            fileName = fileName.EndsWith(".json") 
                ? fileName.Substring(0, fileName.Length - 5) 
                : fileName;
            if (fileName.StartsWith("unity")) continue;

            GameObject btn = Instantiate(saveButtonPrefab, scrollContent);
            var textComp = btn.GetComponentInChildren<TMPro.TMP_Text>();
            if (textComp != null) textComp.text = fileName;

            btn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => LoadGame(fileName));
        }
    }

    public void LoadGame(string fileName)
    {
        PlayerPrefs.SetString("SaveToLoad", fileName);
        gameDataManager.userDefaultName = Path.GetFileNameWithoutExtension(fileName);
        gameDataManager.LoadData();
        
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void ExitGame() => Application.Quit();
}