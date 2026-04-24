using UnityEngine;

public class playerLoader : MonoBehaviour
{
    void Start()
    {
        if (gameDataManager.currentGameData != null && gameDataManager.currentGameData.player != null)
        {
            Vector2 savedPos = gameDataManager.currentGameData.player.playerPos;
            transform.position = new Vector3(savedPos.x, savedPos.y, transform.position.z);
        }
    }
}