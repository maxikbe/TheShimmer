using UnityEngine;

[ExecuteAlways]
public class IndividualTreeLogic : MonoBehaviour
{
    [Header("Color Settings")]
    public Color newColor = Color.blue; 
    [Range(0f, 1f)]
    public float transparency = 1.0f; // 1 is solid, 0 is clear

    [Header("Detection Settings")]
    public Color colorToReplace = new Color(0.1f, 0.5f, 0.1f);
    [Range(0f, 1f)]
    public float tolerance = 0;

    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock propBlock;

    void OnValidate() => UpdateTreeColor();
    void Start() => UpdateTreeColor();

    public void UpdateTreeColor()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (propBlock == null) propBlock = new MaterialPropertyBlock();

        spriteRenderer.GetPropertyBlock(propBlock);

        // We pass the newColor but override its Alpha with our slider
        Color finalColor = newColor;
        finalColor.a = transparency;

        propBlock.SetColor("_NewColor", finalColor);
        propBlock.SetColor("_TargetColor", colorToReplace);
        propBlock.SetFloat("_Tolerance", tolerance);

        spriteRenderer.SetPropertyBlock(propBlock);
    }
}