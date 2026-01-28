using UnityEngine;

[ExecuteInEditMode]
public class SeeThroughManager : MonoBehaviour
{
    public Transform playerTransform;
    [Range(0.1f, 10f)] public float holeRadius = 2.0f;
    [Range(0.01f, 5f)] public float holeSoftness = 0.5f;

    // Use property IDs for better performance
    private static readonly int PlayerPosID = Shader.PropertyToID("_GlobalPlayerPos");
    private static readonly int RadiusID = Shader.PropertyToID("_GlobalRadius");
    private static readonly int SoftnessID = Shader.PropertyToID("_GlobalSoftness");

    private void Update()
    {
        if (playerTransform == null) return;

        // Send the player's world position to all shaders
        Vector3 p = playerTransform.position;
        p.y += 1;
        Shader.SetGlobalVector(PlayerPosID, new Vector4(p.x, p.y, p.z, 0));
        Shader.SetGlobalFloat(RadiusID, holeRadius);
        Shader.SetGlobalFloat(SoftnessID, holeSoftness);
    }
}