using UnityEngine;

public class SeeThroughController : MonoBehaviour
{
    public Transform player;
    public Material seeThroughMaterial;

    void Update()
    {
        if (player != null && seeThroughMaterial != null)
        {
            seeThroughMaterial.SetVector("_PlayerPosition", player.position);
        }
    }
}