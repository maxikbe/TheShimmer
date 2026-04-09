using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public float fadeOutDelay = 1f;

    void Start()
    {
        GameObject canvas = GameObject.Find("PlayerInfoUICanvas");
        if (canvas != null)
        {
            Transform panelTransform = canvas.transform.Find("TransitionPanel");
            if (panelTransform != null)
            {
                GameObject transitionPanel = panelTransform.gameObject;
                Animator transitionAnimator = transitionPanel.GetComponent<Animator>();

                if (transitionAnimator != null)
                {
                    transitionPanel.SetActive(true);
                    transitionAnimator.Play("TransitionAnimation", -1, 0f);
                }
            }
        }

        if (PlayerPrefs.HasKey("LastSpawnPoint"))
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

            PlayerPrefs.DeleteKey("LastSpawnPoint");
        }
    }
}