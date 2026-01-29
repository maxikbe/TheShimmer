using UnityEngine;
using UnityEngine.UI;

public class BackgroundScript : MonoBehaviour
{
    [Header("Nastavení Spawneru")]
    [SerializeField] private GameObject flowerPrefab; // Tvůj Image prefab
    [SerializeField] private float spawnRate = 0.2f;
    [SerializeField] private Color[] colors = { Color.white, Color.red, Color.magenta };

    void Start()
    {
        // Spouští generování lístků v pravidelných intervalech
        InvokeRepeating(nameof(SpawnFlower), 0, spawnRate);
    }

    void SpawnFlower()
    {
        if (flowerPrefab == null) return;

        // Vytvoří kopii prefabu jako potomka tohoto objektu (Canvasu/Panelu)
        GameObject go = Instantiate(flowerPrefab, transform);
        
        // Nastavení náhodné pozice nad horním okrajem (v rámci Canvasu)
        RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        float width = canvasRect.rect.width;
        float height = canvasRect.rect.height;

        float randomX = Random.Range(-width / 2f, width / 2f);
        go.transform.localPosition = new Vector3(randomX, height / 2f + 50, 0);
        go.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360));

        // Přidání komponenty pro pohyb, kterou definujeme níže
        FlowerMovement movement = go.AddComponent<FlowerMovement>();
        
        // Výběr náhodné barvy
        Color randomColor = colors[Random.Range(0, colors.Length)];
        
        // Inicializace pohybu
        movement.Setup(
            randomColor, 
            Random.Range(40f, 100f), // Rychlost pádu
            Random.Range(1f, 2.5f),  // Rychlost pohupování
            Random.Range(15f, 40f)   // Šířka pohupování
        );
    }
}

public class FlowerMovement : MonoBehaviour
{
    private float speed;
    private float swaySpeed;
    private float swayWidth;
    private float timer;
    private Vector3 startPos;
    private float killY;

    public void Setup(Color col, float s, float swS, float swW)
    {
        if (TryGetComponent<Image>(out Image img))
        {
            img.color = col;
            // Aby lístky neblokovaly klikání myší (Raycast)
            img.raycastTarget = false;
        }

        speed = s;
        swaySpeed = swS;
        swayWidth = swW;
        timer = Random.Range(0f, 10f);
        startPos = transform.localPosition;
        
        // Určíme, kde má lístek zmizet (pod spodním okrajem)
        killY = -transform.localPosition.y; 
    }

    void Update()
    {
        timer += Time.deltaTime * swaySpeed;
        
        // Plynulý pohyb dolů
        startPos.y -= speed * Time.deltaTime;
        
        // Výpočet "vlnění" do stran
        float xOffset = Mathf.Sin(timer) * swayWidth;
        
        transform.localPosition = new Vector3(startPos.x + xOffset, startPos.y, 0);

        // Pokud je lístek příliš nízko, zničíme ho
        if (transform.localPosition.y < killY)
        {
            Destroy(gameObject);
        }
    }
}