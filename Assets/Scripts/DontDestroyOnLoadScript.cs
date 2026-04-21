using UnityEngine;

public class DontDestroyOnLoadScript : MonoBehaviour
{
    public static DontDestroyOnLoadScript instance;

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