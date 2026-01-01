using UnityEngine;
using System;

public class TreeGeneration : MonoBehaviour
{
    public TreeConfig config;
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
        
        GenerateTree();
    }

    void FixedUpdate()
    {
        Timer -= Time.deltaTime;
        if(Timer <= 0)
        {
            Timer = refreshRateTime;
            GenerateTree();
        }
    }

    public void GenerateTree()
    {
        if (config == null || spriteRenderer == null) return;

        float seedOffsetX = config.seed * 10f; 
        float seedOffsetY = config.seed * 15f; 

        localRandom = new System.Random(config.seed);
        
        texture = new Texture2D(config.resolution, config.resolution)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        pixels = new Color[config.resolution * config.resolution];

        float center = config.resolution / 2f;
        float halfResolution = config.resolution / 2f;

        float trunkWidth = config.resolution * config.trunkWidth;
        float trunkHeight = config.resolution * config.trunkHeight;
        float crownStartHeight = config.resolution * config.trunkHeight;
        float crownHeight = config.resolution * (1f - config.trunkHeight);
        float crownRadiusX = config.resolution * 0.5f; 
        float crownRadiusY = crownHeight * 0.5f;

        for (int y = 0; y < config.resolution; y++)
        {
            for (int x = 0; x < config.resolution; x++)
            {
                int index = y * config.resolution + x;
                Color pixelColor = Color.clear;

                if (y < trunkHeight)
                {
                    if (x > center - trunkWidth / 2f && x < center + trunkWidth / 2f)
                    {
                        float noiseX = (x * config.scale) + transform.position.x + seedOffsetX;
                        float noiseY = (y * config.scale) + transform.position.y + seedOffsetY;
                        float noiseValue = Mathf.PerlinNoise(noiseX, noiseY);

                        if (noiseValue > config.trunkNoiseThreshold) 
                            pixelColor = config.trunkColor;
                    }
                }
                else 
                {
                    float normalizedX = (x - center) / crownRadiusX;
                    float normalizedY = (y - (crownStartHeight + crownRadiusY)) / crownRadiusY;

                    float distanceFactor = (normalizedX * normalizedX) + (normalizedY * normalizedY / config.crownRatio);
                    float falloff = 1f - Mathf.Pow(distanceFactor, config.falloffStrength);

                    if (falloff > 0f) 
                    {
                        float noiseX = (x * config.scale) + transform.position.x + seedOffsetX;
                        float noiseY = (y * config.scale) + transform.position.y + seedOffsetY;
                        float noiseValue = Mathf.PerlinNoise(noiseX, noiseY);

                        float finalNoise = noiseValue * falloff;

                        if (finalNoise > config.leafThreshold)
                        {
                            float shadeMix = Mathf.InverseLerp(config.leafThreshold, 1f, finalNoise);
                            pixelColor = Color.Lerp(config.baseLeafColor, config.highlightColor, shadeMix);

                            if (config.addFruitOrFlowers && localRandom.NextDouble() < config.fruitFlowerDensity)
                            {
                                pixelColor = config.fruitFlowerColor; 
                            }
                        }
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
