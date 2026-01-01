using System;
using UnityEngine;

public class BushGeneration : MonoBehaviour
{
    public BushConfig config;
    public SpriteRenderer spriteRenderer;

    private Texture2D texture;
    private Color[] pixels;

    private float Timer = 0;
    public int refreshRateTime = 1;
    private System.Random localRandom;

    void Start()
    {
        if (config != null && config.seed == 0)
        {
            config.seed = UnityEngine.Random.Range(1, 100000); 
        }
        
        localRandom = new System.Random(config.seed);
        GenerateBush();
        Timer = refreshRateTime;
    }

    void FixedUpdate()
    {
        Timer -= Time.deltaTime;
        if(Timer <= 0)
        {
            Timer = refreshRateTime;
            GenerateBush();
        }
    }

    public void GenerateBush()
    {
        if (config == null || spriteRenderer == null) return;

        float offsetX = config.seed * 10f; 
        float offsetY = config.seed * 15f; 

        localRandom = new System.Random(config.seed);

        texture = new Texture2D(config.resolution, config.resolution)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        pixels = new Color[config.resolution * config.resolution];

        float center = config.resolution / 2f;
        float radiusSq = center * center;

        float trunkWidthPixels = config.resolution * config.trunkWidth;
        float trunkStart = center - (trunkWidthPixels / 2f);
        float trunkEnd = center + (trunkWidthPixels / 2f);

        for (int y = 0; y < config.resolution; y++)
        {
            for (int x = 0; x < config.resolution; x++)
            {
                int index = y * config.resolution + x;

                float noiseX = x * config.scale + transform.position.x;
                float noiseY = y * config.scale + transform.position.y;
                float noiseValue = Mathf.PerlinNoise(noiseX, noiseY);

                float distX = x - center;
                float distY = y - center;
                float distanceSq = distX * distX + distY * distY;

                float falloff = 1f - Mathf.Pow(distanceSq / radiusSq, config.falloffStrength);

                Color pixelColor = Color.clear;

                if (y < config.resolution * config.trunkSize && pixelColor == Color.clear && x >= trunkStart && x <= trunkEnd)
                {

                    if (noiseValue > 0.4f) 
                        pixelColor = config.trunkColor;
                    else
                        pixelColor = Color.clear;
                }
                else
                {

                    float finalNoise = noiseValue * falloff;

                    if (finalNoise > config.leafThreshold)
                    {

                        float shadeMix = Mathf.InverseLerp(config.leafThreshold, 1f, finalNoise);
                        pixelColor = Color.Lerp(config.baseLeafColor, config.highlightColor, shadeMix);

                        if (config.addFlowers && localRandom.NextDouble() < config.flowerDensity)
                        {
                            pixelColor = config.flowerColor; 
                        }
                    }
                    else
                    {
                        pixelColor = Color.clear; 
                    }
                }

                pixels[index] = pixelColor;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        Sprite newSprite = Sprite.Create(
            texture, 
            new Rect(0, 0, config.resolution, config.resolution), 
            Vector2.one * 0.5f,
            config.resolution
        );

        spriteRenderer.sprite = newSprite;
    }
}
