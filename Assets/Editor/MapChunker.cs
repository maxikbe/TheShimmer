using UnityEngine;
using UnityEditor;

public class MapChunker : EditorWindow
{
    [MenuItem("Window/Custom/Map Chunker Tool")]
    public static void ShowWindow()
    {
        GetWindow<MapChunker>("Map Chunker");
    }

    public Transform treeContainer;
    public float chunkSize = 50f;

    void OnGUI()
    {
        GUILayout.Label("Map Chunking Utility", EditorStyles.boldLabel);
        
        treeContainer = (Transform)EditorGUILayout.ObjectField("Tree Container", treeContainer, typeof(Transform), true);
        chunkSize = EditorGUILayout.FloatField("Chunk Size", chunkSize);

        if (GUILayout.Button("Chunk My Map Now"))
        {
            if (treeContainer == null) {
                Debug.LogError("Please drag the TREE_CONTAINER object into the slot!");
                return;
            }
            ExecuteChunking();
        }
    }

    void ExecuteChunking()
    {
        int totalObjects = treeContainer.childCount;
        for (int i = totalObjects - 1; i >= 0; i--)
        {
            Transform t = treeContainer.GetChild(i);
            
            int x = Mathf.FloorToInt(t.position.x / chunkSize);
            int y = Mathf.FloorToInt(t.position.y / chunkSize);
            string chunkName = "Chunk_" + x + "_" + y;

            Transform chunkFolder = treeContainer.Find(chunkName);
            if (chunkFolder == null)
            {
                chunkFolder = new GameObject(chunkName).transform;
                chunkFolder.parent = treeContainer;
            }

            t.parent = chunkFolder;
        }
        Debug.Log("Chunking Complete!");
    }
}