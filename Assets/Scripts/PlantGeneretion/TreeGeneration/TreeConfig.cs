using UnityEngine;

public class TreeConfig : MonoBehaviour
{
    [Header("General")]
    public int resolution = 64; 
    public float scale = 0.08f; 
    public int seed = 0; 

    [Header("Trunk")]
    [Range(0.0f, 0.5f)]
    public float trunkHeight = 0.3f; 
    [Range(0.01f, 0.3f)]
    public float trunkWidth = 0.05f; 
    [Range(0.0f, 1.0f)]
    public float trunkNoiseThreshold = 0.4f;
    public Color trunkColor = new Color(0.5f, 0.3f, 0.1f); 

    [Header("Crown Shape and Noise")]
    [Range(0.5f, 3.0f)]
    public float crownRatio = 1.5f; 
    [Range(0.5f, 5.0f)]
    public float falloffStrength = 2.0f; 
    [Range(0.0f, 1.0f)]
    public float leafThreshold = 0.5f;
    
    [Header("Colors")]
    public Color baseLeafColor = new Color(0.2f, 0.6f, 0.3f); 
    public Color highlightColor = new Color(0.4f, 0.8f, 0.5f); 

    [Header("Features (Fruit/Flowers)")]
    public bool addFruitOrFlowers = true;
    public Color fruitFlowerColor = Color.red;
    [Range(0.0f, 0.1f)]
    public float fruitFlowerDensity = 0.01f;
}
