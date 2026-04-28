using UnityEngine;
using UnityEngine.Rendering.Universal;

public class streetLightControler : MonoBehaviour
{
    private GameObject lights;
    private Light2D globalLight;

    void Start()
    {
        lights = transform.GetChild(0).gameObject;
        
        if (AppearencePersistance.instance != null)
        {
            globalLight = AppearencePersistance.instance.transform.gameObject.GetComponentInChildren<Light2D>();
        }
    }

   void Update()
    {
        if (globalLight.intensity > 0.35f)
        {
            if (lights.activeSelf) lights.SetActive(false);
        }
        else
        {
            if (!lights.activeSelf) lights.SetActive(true);
        }
    }
}