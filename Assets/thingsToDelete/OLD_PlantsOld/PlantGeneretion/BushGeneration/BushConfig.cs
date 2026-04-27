using UnityEngine;

public class BushConfig : MonoBehaviour
{
    public int resolution = 32;
    public float scale = 0.15f; 

    [Header("Leaf Noise")]
    [Range(0.0f, 1.0f)]
    public float leafThreshold = 0.5f;
    
    [Tooltip("Other Settings")]
    [Range(0.0f, 1.0f)]
    public float trunkSize = 0.1f;
    [Range(0.0f, 1.0f)]
    public float trunkWidth = 0.5f;
    public float falloffStrength = 1.0f; 

    [Header("Colors")]
    public Color baseLeafColor = new Color(0.2f, 0.6f, 0.3f); // Dark Green
    public Color highlightColor = new Color(0.4f, 0.8f, 0.5f); // Light Green
    public Color trunkColor = new Color(0.5f, 0.3f, 0.1f);      // Brown

    [Header("Features")]
    public bool addFlowers = true;
    public Color flowerColor = Color.red;
    [Range(0.0f, 1.0f)]
    public float flowerDensity = 0.05f;

    public int seed = 0;
}
