using UnityEngine;
using TMPro;

public class campFireScript : MonoBehaviour
{
    private TextMeshProUGUI interactUIText;
    public TextMeshProUGUI textInfo;
    private GameObject InteractUI;
    public string interactUITextContent = "Wood [E] / LightUp [Hold L]"; 
    public float holdDuration = 3f; 
    private float holdTimer = 0f;
    private bool campFireIsLit = false;
    private bool isPlayerNearby = false;

    public string TextContent = "Destroy [Hold F]";
    private TextMeshProUGUI interactUIHOLDText;
    private GameObject InteractUIHOLD;
    private Animator animator;
    private int woodFuel = 1;
    private int maxWoodFuel = 3;
    private CampFire myData;

    public void Initialize(CampFire data)
    {
        myData = data;
        campFireIsLit = data.isLit;
        woodFuel = data.woodFuelAmount;

        if (animator == null) animator = GetComponent<Animator>();

        if (textInfo != null)
        {
            textInfo.text = "Wood " + woodFuel + " / " + maxWoodFuel;
        }

        if (campFireIsLit && animator != null) 
        {
            animator.Play("campFireAnimation");
        }
    }

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        animator.StopPlayback();
    }

    void Update()
    {
        if (isPlayerNearby)
        {
            if (woodFuel < maxWoodFuel && Input.GetKeyDown(KeyCode.E))
            {
                AddWood();
            }

            if (!campFireIsLit && Input.GetKey(KeyCode.L))
            {
                HandleHoldInteraction("Lighting... ", LightFire);
            }
            else if (Input.GetKey(KeyCode.F))
            {
                HandleHoldInteraction("Destroying... ", DestroyFire);
            }
            
            if (Input.GetKeyUp(KeyCode.L) || Input.GetKeyUp(KeyCode.F))
            {
                holdTimer = 0f;
                if (interactUIHOLDText != null) interactUIHOLDText.text = TextContent;
            }
        }
    }

    void AddWood()
    {
        woodFuel++;
        if (myData != null) myData.woodFuelAmount = woodFuel;
        textInfo.text = "Wood " + woodFuel + " / " + maxWoodFuel;
        gameDataManager.SaveData();
    }

    void LightFire()
    {
        campFireIsLit = true;
        if (myData != null) myData.isLit = true;
        animator.Play("campFireAnimation");
        gameDataManager.SaveData();
    }

    void DestroyFire()
    {
        gameDataManager.currentGameData.player.campFires.Remove(myData);
        gameDataManager.SaveData();
        Destroy(gameObject);
    }

    void HandleHoldInteraction(string progressPrefix, System.Action onComplete)
    {
        holdTimer += Time.deltaTime;
        if (interactUIHOLDText != null)
            interactUIHOLDText.text = progressPrefix + (holdTimer / holdDuration * 100f).ToString("F0") + "%";

        if (holdTimer >= holdDuration)
        {
            onComplete?.Invoke();
            holdTimer = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Transform canvas = other.transform.Find("PlayerInfoUICanvas");
            if (canvas != null)
            {
                InteractUI = canvas.Find("Interaction").gameObject;
                InteractUIHOLD = canvas.Find("LongInteractionHOLD").gameObject;
                interactUIText = InteractUI.GetComponentInChildren<TextMeshProUGUI>();
                interactUIText.text = interactUITextContent;
                InteractUI.SetActive(true);
                interactUIHOLDText = InteractUIHOLD.GetComponentInChildren<TextMeshProUGUI>();
                interactUIHOLDText.text = TextContent;
                InteractUIHOLD.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            holdTimer = 0f;
            if (InteractUI != null) InteractUI.SetActive(false);
            if (InteractUIHOLD != null) InteractUIHOLD.SetActive(false);
        }
    }
}