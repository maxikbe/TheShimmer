using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    void Start()
    {
        string targetName = PlayerPrefs.GetString("LastSpawnPoint");
        GameObject spawnPoint = GameObject.Find(targetName);

        if (spawnPoint != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = spawnPoint.transform.position;
            }
        }
    }
}