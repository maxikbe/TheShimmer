using UnityEngine;
using System;

[RequireComponent(typeof(SpriteRenderer))]
public class LeafGenerationScript : MonoBehaviour
{
    public enum LeafShapeType
    {
        Elliptical,
        Spear,
        Lobbed
    }

    public LeafShapeType leafShape = LeafShapeType.Elliptical;
    public int textureSize = 32;
    public float widthOfLeaf = 0.45f;
    public float lobeFrequency = 10f;
    public float lobeDepth = 0.1f;
    
    public float noiseScale = 10f;
    public float colorVariationStrength = 0.2f;
    public Color leafColor = new Color(100/255f, 160/255f, 60/255f, 1.0f);
    // private float greenValue = 160f;

    // private float leafChangeTimer = 0;
    private SpriteRenderer sr;
    private Vector2 noiseOffset;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        
        //Leaf Setting
        textureSize = 256;
        widthOfLeaf = 0.45f;
        // leafChangeTimer = UnityEngine.Random.Range(1f,3f);
        // Array values = Enum.GetValues(typeof(LeafShapeType));
        // leafShape = (LeafShapeType)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        
        noiseOffset = new Vector2(UnityEngine.Random.Range(0f, 100f), UnityEngine.Random.Range(0f, 100f));
        
        //Generations
        GenerateLeafSprite();
    }

    void Update()
    {
        // leafChangeTimer -= Time.deltaTime;
        // if(leafChangeTimer <= 0)
        // {
        //     leafChangeTimer = UnityEngine.Random.Range(1f,3f);
        //     //Debug.Log("Updated");
        //     GenerateLeafSprite();
        // }
    }

    private float CalculateMaxLeafWidth(float v_norm)
    {
        float width = 0f;

        switch (leafShape)
        {
            case LeafShapeType.Elliptical:
                width = Mathf.Sin(v_norm * Mathf.PI);
                break;

            case LeafShapeType.Spear:
                width = Mathf.Pow(v_norm, 0.5f) * (1f - v_norm);
                break;
            case LeafShapeType.Lobbed:
                width = Mathf.Sin(v_norm * Mathf.PI);
                float lobes = Mathf.Sin(v_norm * lobeFrequency * Mathf.PI) * lobeDepth;
                width = width * (1f + lobes);
                width = Mathf.Max(0, width);
                break;
        }

        if (v_norm < 0.1f)
        {
            width *= v_norm * 10f;
        }

        return width;
    }

    void GenerateLeafSprite()
    {
        // if(greenValue > 80)
        // {
        //     greenValue -= 2;
        //     leafColor = new Color(100/255f, greenValue/255f, 60/255f, 1.0f);
        // }
        
        Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        Color[] clearPixels = new Color[textureSize * textureSize];
        for (int i = 0; i < clearPixels.Length; i++)
        {
            clearPixels[i] = Color.clear;
        }
        texture.SetPixels(clearPixels);

        float centerX = textureSize / 2f;
        
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float v_norm = (float)y / textureSize; 
                float u = (x - centerX) / (textureSize / 2f);

                float maxLeafWidth = CalculateMaxLeafWidth(v_norm);
                
                float u_scaled = u / maxLeafWidth;

                if (Mathf.Abs(u_scaled) < widthOfLeaf) 
                {
                    float sampleX = (float)x / textureSize * noiseScale + noiseOffset.x;
                    float sampleY = (float)y / textureSize * noiseScale + noiseOffset.y;
                    
                    float noise = Mathf.PerlinNoise(sampleX, sampleY); 
                    
                    float colorAdjustment = (noise - 0.5f) * colorVariationStrength;
                    Color finalColor = leafColor * (1.0f + colorAdjustment);

                    texture.SetPixel(x, y, finalColor);
                    
                    if (Mathf.Abs(u) < 0.02f) 
                    {
                        texture.SetPixel(x, y, finalColor * 0.7f);
                    }
                }
            }
        }
        
        texture.Apply();
        
        Rect rect = new Rect(0, 0, textureSize, textureSize);
        Vector2 pivot = new Vector2(0.5f, 0f); 
        
        Sprite newSprite = Sprite.Create(texture, rect, pivot, 300f);
        sr.sprite = newSprite;
    }
}
