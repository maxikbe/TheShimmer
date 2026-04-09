using UnityEngine;

[ExecuteInEditMode]
public class GlobalSeeThroughController : MonoBehaviour
{
    public Transform playerTransform;
    public float holeRadius = 2.0f;
    public float holeSoftness = 0.5f;
    public float verticalOffset = 1.0f;

    void Update()
    {
        if (playerTransform == null) return;

        Vector3 p = playerTransform.position;

        Vector4 shaderPos = new Vector4(p.x, p.y + verticalOffset, p.z, 0);

        Shader.SetGlobalVector("_GlobalPlayerPos", shaderPos);
        Shader.SetGlobalFloat("_GlobalRadius", holeRadius);
        Shader.SetGlobalFloat("_GlobalSoftness", holeSoftness);
    }
}