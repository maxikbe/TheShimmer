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
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.StopPlayback();
        textInfo.text = "Wood "+woodFuel+" / "+maxWoodFuel;
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
                HandleHoldInteraction("Destroying... ", () => Destroy(gameObject));
            }
            else
            {
                holdTimer = 0f;
                if (interactUIHOLDText != null) interactUIHOLDText.text = TextContent;
            }
        }
        if (campFireIsLit && woodFuel < maxWoodFuel)
        {
            interactUITextContent = "Wood [E]";
        }
        else if(!campFireIsLit && woodFuel >= maxWoodFuel)
        {
            interactUITextContent = "LightUp [Hold L]";
        }
        else if(!campFireIsLit && woodFuel < maxWoodFuel)
        {
            interactUITextContent = "Wood [E] / LightUp [Hold L]";
        }
        else
        {
            InteractUI.SetActive(false);
        }
    }

    void HandleHoldInteraction(string progressPrefix, System.Action onComplete)
    {
        holdTimer += Time.deltaTime;
        float progress = (holdTimer / holdDuration) * 100f;

        if (interactUIHOLDText != null)
        {
            interactUIHOLDText.text = progressPrefix + progress.ToString("F0") + "%";
        }

        if (holdTimer >= holdDuration)
        {
            onComplete?.Invoke();
            holdTimer = 0f;
        }
    }

    void LightFire()
    {
        campFireIsLit = true;
        animator.Play("campFireAnimation");
    }

    void AddWood()
    {
        Debug.Log("Added Wood");
        woodFuel +=1;
        textInfo.text = "Wood "+woodFuel+" / "+maxWoodFuel;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            
            Transform canvas = other.transform.Find("PlayerInfoUICanvas");
            if (canvas != null)
            {
                Transform interactTrans = canvas.Find("Interaction");
                Transform holdTrans = canvas.Find("LongInteractionHOLD");

                if (interactTrans != null)
                {
                    InteractUI = interactTrans.gameObject;
                    interactUIText = InteractUI.GetComponentInChildren<TextMeshProUGUI>();
                    interactUIText.text = interactUITextContent;
                    InteractUI.SetActive(true);
                }

                if (holdTrans != null)
                {
                    InteractUIHOLD = holdTrans.gameObject;
                    interactUIHOLDText = InteractUIHOLD.GetComponentInChildren<TextMeshProUGUI>();
                    interactUIHOLDText.text = TextContent;
                    InteractUIHOLD.SetActive(true);
                }
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