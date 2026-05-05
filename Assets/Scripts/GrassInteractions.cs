using UnityEngine;

public class GrassInteractions : MonoBehaviour
{
    [SerializeField] public Material grassMaterial;

    void Update()
    {
        grassMaterial.SetVector("_PlayerPos", transform.position);
    }
}