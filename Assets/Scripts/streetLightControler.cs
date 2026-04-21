using UnityEngine;
using UnityEngine.Rendering.Universal;

public class streetLightControler : MonoBehaviour
{
    private GameObject lights;
    public Light2D lightIntensity;
    void Start()
    {
        lights = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        if(lightIntensity.intensity > 0.35)
        {
            lights.SetActive(false);
        }
        else
        {
            lights.SetActive(true);
        }
    }
}
