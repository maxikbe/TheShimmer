
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChangeMesh : MonoBehaviour
{
    [SerializeField] private Sprite replacementSprite;
    [SerializeField] private bool useWorldSpaceCanvas = false;
    [SerializeField] private float imageScale = 1f;
    
    private List<GameObject> replacedObjects = new List<GameObject>();
    private bool isReplaced = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!isReplaced)
            {
                ReplaceAllObjectsWithImage();
            }
            else
            {
                RestoreOriginalObjects();
            }
        }
    }

    void ReplaceAllObjectsWithImage()
    {
        if (replacementSprite == null)
        {
            Debug.LogError("Replacement Sprite není nastaven!");
            return;
        }

        // Najde VŠECHNY GameObjecty ve scéně
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int replacedCount = 0;

        foreach (GameObject obj in allObjects)
        {
            // Přeskočí kameru, tento skript a již vytvořené image objekty
            if (obj == this.gameObject || 
                obj.name.EndsWith("_Image") || 
                obj.name == "Image" ||
                obj.GetComponent<Camera>() != null)
            {
                continue;
            }

            // Vytvoří nový GameObject pro obrázek
            GameObject imageObject = new GameObject(obj.name + "_Image");
            imageObject.transform.position = obj.transform.position;
            imageObject.transform.rotation = obj.transform.rotation;
            imageObject.transform.localScale = obj.transform.localScale;
            imageObject.transform.parent = obj.transform.parent;

            if (useWorldSpaceCanvas)
            {
                // World Space Canvas
                Canvas canvas = imageObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace;
                
                CanvasScaler scaler = imageObject.AddComponent<CanvasScaler>();
                scaler.dynamicPixelsPerUnit = 10;
                
                RectTransform canvasRect = imageObject.GetComponent<RectTransform>();
                canvasRect.sizeDelta = new Vector2(100, 100) * imageScale;

                // Přidá Image komponentu
                GameObject imageChild = new GameObject("Image");
                imageChild.transform.SetParent(imageObject.transform, false);
                
                Image image = imageChild.AddComponent<Image>();
                image.sprite = replacementSprite;
                image.preserveAspect = true;
                
                RectTransform imageRect = imageChild.GetComponent<RectTransform>();
                imageRect.anchorMin = Vector2.zero;
                imageRect.anchorMax = Vector2.one;
                imageRect.sizeDelta = Vector2.zero;
            }
            else
            {
                // Sprite Renderer (jednodušší varianta)
                SpriteRenderer spriteRenderer = imageObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = replacementSprite;
                spriteRenderer.transform.localScale *= imageScale;
            }

            // Deaktivuje původní objekt
            obj.SetActive(false);
            
            // Uloží reference pro možné obnovení
            replacedObjects.Add(obj);
            replacedObjects.Add(imageObject);
            
            replacedCount++;
        }

        isReplaced = true;
        Debug.Log($"Nahrazeno {replacedCount} objektů obrázkem.");
    }

    void RestoreOriginalObjects()
    {
        // Obnoví původní objekty a smaže dočasné
        for (int i = 0; i < replacedObjects.Count; i += 2)
        {
            GameObject originalObject = replacedObjects[i];
            GameObject imageObject = replacedObjects[i + 1];
            
            if (originalObject != null)
            {
                originalObject.SetActive(true);
            }
            
            if (imageObject != null)
            {
                Destroy(imageObject);
            }
        }

        replacedObjects.Clear();
        isReplaced = false;
        Debug.Log("Obnoveny všechny původní objekty.");
    }
}
