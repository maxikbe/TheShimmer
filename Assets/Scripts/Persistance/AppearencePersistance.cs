using UnityEngine;

public class AppearencePersistance : MonoBehaviour
{
    public static AppearencePersistance instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
