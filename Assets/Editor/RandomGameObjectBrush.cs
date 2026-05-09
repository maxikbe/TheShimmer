using UnityEngine;
using UnityEditor;
using UnityEditor.Tilemaps;

[CreateAssetMenu(fileName = "New Random Power Brush", menuName = "2D/Brushes/Random Power Brush")]
[CustomGridBrush(false, true, false, "Random Power Brush")]
public class RandomGameObjectBrush : GameObjectBrush
{
    public GameObject[] prefabs;
    public int radius = 0;
    [Range(0, 1)] public float density = 0.5f;
    public float minScale = 1f;
    public float maxScale = 1f;
    public bool randomRotation = false;

    public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
    {
        if (prefabs == null || prefabs.Length == 0) return;

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (Random.value > density) continue;

                Vector3Int paintPos = new Vector3Int(position.x + x, position.y + y, position.z);
                GameObject randomPrefab = prefabs[Random.Range(0, prefabs.Length)];

                this.Init(Vector3Int.one, Vector3Int.zero);
                this.SetGameObject(Vector3Int.zero, randomPrefab);

                base.Paint(gridLayout, brushTarget, paintPos);

                GameObject placedObject = GetObjectAtPosition(gridLayout, brushTarget, paintPos);
                if (placedObject != null)
                {
                    ApplyTransform(placedObject);
                }
            }
        }
    }

    private void ApplyTransform(GameObject obj)
    {
        float uniformScale = Random.Range(minScale, maxScale);
        obj.transform.localScale = new Vector3(uniformScale, uniformScale, uniformScale);

        if (randomRotation)
        {
            obj.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        }
    }

    private GameObject GetObjectAtPosition(GridLayout grid, GameObject target, Vector3Int pos)
    {
        foreach (Transform child in target.transform)
        {
            if (grid.WorldToCell(child.position) == pos) return child.gameObject;
        }
        return null;
    }
}