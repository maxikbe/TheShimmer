using UnityEngine;
using UnityEngine.Rendering.Universal;

[DefaultExecutionOrder(-100)] 
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
            Light2D[] lights = GetComponentsInChildren<Light2D>();
            foreach (Light2D l in lights)
            {
                l.enabled = false;
            }

            Destroy(gameObject);
        }
    }
}