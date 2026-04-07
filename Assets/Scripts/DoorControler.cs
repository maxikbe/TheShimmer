using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorControler : MonoBehaviour
{
    public string sceneToLoad;
    public string spawnPointName;
    private GameObject InteractUI;

    private bool isPlayerInTrigger = false;

    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            Move();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InteractUI = other.transform.Find("PlayerInteractionCanvas/Interaction").gameObject;
            isPlayerInTrigger = true;
            if (InteractUI != null) InteractUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            if (InteractUI != null) InteractUI.SetActive(false);
        }
    }

    void Move()
    {
        PlayerPrefs.SetString("LastSpawnPoint", spawnPointName);
        SceneManager.LoadScene(sceneToLoad);
    }
}