using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSpawner : MonoBehaviour
{
    public string sceneToLoad;
    void Start()
    {
        sceneToLoad = gameDataManager.currentGameData.player.currentScene;
        SceneManager.LoadScene(sceneToLoad);
    }
}
