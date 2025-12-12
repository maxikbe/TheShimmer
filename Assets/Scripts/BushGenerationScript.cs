using UnityEngine;

using System;



[RequireComponent(typeof(SpriteRenderer))]

public class BushGenerationScript : MonoBehaviour
{
    [Header("Deterministic Seed")]
    public int seed = 12345;
    private System.Random randomGenerator;

    [Header("Generation Parameters")]
    public int maxDepth = 5;
    public float baseLength = 2.5f;
    public float baseWidth = 0.2f;
    public int branchCount = 2;

    [Header("Branching Rules")]
    public float lengthScaleFactor = 0.8f;
    public float widthScaleFactor = 0.8f;
    public float minAngle = 15f;
    public float maxAngle = 45f;

    [Header("Appearance")]
    public Material lineMaterial;
    public Color baseColor = new Color(0.5f, 0.3f, 0.1f);
    
    [Header("Leaf Prefab")]
    public GameObject leafPrefab;

    [Header("Leaf Spawning")]
    [Range(0f, 1f)]
    public float leafSpawnChance = 0.5f;

    void Start()
    {
        seed = UnityEngine.Random.Range(100000000, 999999999);
        GenerateBush();
    }

    private float RandomRange(float min, float max)
    {
        double range = max - min;
        double sample = randomGenerator.NextDouble();
        return (float)(min + (sample * range));
    }

    public void GenerateBush()
    {
        randomGenerator = new System.Random(seed); 

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        
        if (lineMaterial == null)
        {
            Debug.LogError("Line Material is missing! Please assign one to the script in the Inspector.");
            return;
        }

        GenerateBranch(this.transform, baseLength, baseWidth, 0);
    }

    private void GenerateBranch(Transform parentTransform, float length, float width, int depth)
    {
        if (depth >= maxDepth)
        {
            return;
        }

        GameObject stickObject = new GameObject($"Branch_D{depth}_{parentTransform.childCount}");
        stickObject.transform.SetParent(parentTransform);

        if (depth == 0)
        {
            stickObject.transform.localPosition = Vector3.zero;
            stickObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            stickObject.transform.localPosition = Vector3.zero;
            stickObject.transform.localRotation = Quaternion.identity;
        }

        float colorVariation = RandomRange(0f, 0.1f);
        Color stickColor = Color.Lerp(baseColor, Color.black, colorVariation);

        DrawStick(stickObject, length, width, stickColor);

        if (leafPrefab != null && UnityEngine.Random.value < leafSpawnChance)
        {
            SpawnLeaf(stickObject.transform); 
        }

        float newLength = length * lengthScaleFactor;
        float newWidth = width * widthScaleFactor;

        for (int i = 0; i < branchCount; i++)
        {
            float angle = RandomRange(minAngle, maxAngle) * (i % 2 == 0 ? 1f : -1f);
            Quaternion branchRotation = Quaternion.Euler(0, 0, angle);

            GameObject branchHolder = new GameObject($"Holder_D{depth+1}_{i}");
            branchHolder.transform.SetParent(stickObject.transform); 
            branchHolder.transform.localPosition = Vector3.zero; 
            branchHolder.transform.localRotation = branchRotation;

            GenerateBranch(branchHolder.transform, newLength, newWidth, depth + 1);
        }
    }

    private void DrawStick(GameObject stickObject, float length, float width, Color color)
    {
        LineRenderer lr = stickObject.GetComponent<LineRenderer>();
        if (lr == null)
        {
            lr = stickObject.AddComponent<LineRenderer>();
        }

        lr.positionCount = 2;
        lr.useWorldSpace = false;

        lr.material = lineMaterial;
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = width;
        lr.endWidth = width * 0.5f;

        lr.SetPosition(0, Vector3.zero);
        lr.SetPosition(1, Vector3.up * length);

        lr.sortingLayerName = "Default";
        lr.sortingOrder = -stickObject.transform.GetSiblingIndex();
    }
    
    private void SpawnLeaf(Transform stickTransform)
    {
        LineRenderer stickLr = stickTransform.GetComponent<LineRenderer>();
        
        if (stickLr == null) return;
        
        Vector3 spawnPosition = stickLr.GetPosition(1);
        
        GameObject leafInstance = Instantiate(leafPrefab, stickTransform);
        
        leafInstance.name = "Leaf";
        leafInstance.transform.localPosition = spawnPosition;
    }
}