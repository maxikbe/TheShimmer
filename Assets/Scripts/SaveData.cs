using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveData : MonoBehaviour
{
    public GameObject Appearance;
    private GameObject playerObject;
    private GameObject tent;

    void Start()
    {
        playerObject = gameObject.transform.parent.gameObject;
        tent = GameObject.FindWithTag("Tent");
    }
    public void SaveDataFunctiomn()
    {
        gameDataManager.currentGameData.player.playerPos = playerObject.transform.position;
        gameDataManager.currentGameData.player.time = Appearance.GetComponentInChildren<TimeAndLight>().currentTime;
        gameDataManager.currentGameData.player.dayNumber = Appearance.GetComponentInChildren<TimeAndLight>().currectDay;
        gameDataManager.currentGameData.player.currentScene = SceneManager.GetActiveScene().name;
        gameDataManager.SaveData();
    }
}
