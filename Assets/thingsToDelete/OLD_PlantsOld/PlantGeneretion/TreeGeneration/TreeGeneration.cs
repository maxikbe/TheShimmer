using UnityEngine;
using System;
using static TreeConfig;

public class TreeGeneration : MonoBehaviour
{
    public TreeConfig config;
    public SpriteRenderer spriteRenderer;

    Texture2D texture;
    Color[] pixels;

    float timer;
    public float refreshRateTime = 1f;

    System.Random localRandom;

    void Start()
    {
        if (config.seed == 0)
            config.seed = UnityEngine.Random.Range(1, 100000);

        localRandom = new System.Random(config.seed);
        GenerateTree();
    }

    void FixedUpdate()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = refreshRateTime;
            GenerateTree();
        }
    }

    float GetRadiusAtHeight(float h)
    {
        switch (config.type)
        {
            case TreeType.Oak:
                return Mathf.Lerp(0.3f, 1f, Mathf.Sin(h * Mathf.PI));
            case TreeType.Birch:
                return Mathf.Lerp(0.15f, 0.4f, 1f - h);
            case TreeType.Willow:
                return Mathf.Lerp(0.8f, 0.4f, h);
            case TreeType.Pine:
                return Mathf.Lerp(1f, 0.05f, h);
            default:
                return Mathf.Sin(h * Mathf.PI);
        }
    }

    public void GenerateTree()
    {
        if (config == null || spriteRenderer == null) return;

        float seedX = config.seed * 10f;
        float seedY = config.seed * 15f;

        localRandom = new System.Random(config.seed);

        texture = new Texture2D(config.resolution, config.resolution);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        pixels = new Color[config.resolution * config.resolution];

        float center = config.resolution / 2f;
        float trunkH = config.resolution * config.trunkHeight;
        float trunkW = config.resolution * config.trunkWidth;
        float crownH = config.resolution - trunkH;
        float crownRadiusX = config.resolution * 0.5f;

        for (int y = 0; y < config.resolution; y++)
        {
            for (int x = 0; x < config.resolution; x++)
            {
                Color c = Color.clear;

                if (y < trunkH)
                {
                    if (Mathf.Abs(x - center) < trunkW * 0.5f)
                    {
                        float n = Mathf.PerlinNoise(x * config.scale + seedX, y * config.scale + seedY);
                        if (n > config.trunkNoiseThreshold)
                            c = config.trunkColor;
                    }
                }
                else
                {
                    float h01 = (y - trunkH) / crownH;
                    if (h01 < 0f || h01 > 1f) goto SetPixel;

                    float radius = GetRadiusAtHeight(h01);
                    float nx = Mathf.Abs(x - center) / (crownRadiusX * radius);

                    float n = Mathf.PerlinNoise(x * config.scale + seedX, y * config.scale + seedY);
                    float edgeNoise = (n - 0.5f) * 0.4f;

                    float silhouette = nx + edgeNoise;

                    if (config.type == TreeType.Willow)
                        silhouette += Mathf.Pow(h01, 2f) * 0.3f;

                    float falloff = 1f - Mathf.Pow(silhouette, config.falloffStrength);

                    if (falloff > 0f && n > config.leafThreshold)
                    {
                        float shade = Mathf.InverseLerp(config.leafThreshold, 1f, n);
                        c = Color.Lerp(config.baseLeafColor, config.highlightColor, shade);

                        if (config.addFruitOrFlowers && localRandom.NextDouble() < config.fruitFlowerDensity)
                            c = config.fruitFlowerColor;
                    }
                }

            SetPixel:
                pixels[y * config.resolution + x] = c;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        spriteRenderer.sprite = Sprite.Create(
            texture,
            new Rect(0, 0, config.resolution, config.resolution),
            Vector2.one * 0.5f,
            config.resolution
        );
    }
}
