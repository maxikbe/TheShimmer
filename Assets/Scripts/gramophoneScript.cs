using UnityEngine;
using TMPro;
using System.Collections;

public class gramophoneScript : MonoBehaviour
{
    public bool isFunctional = false;
    public string popUpTextContent = "It´s blocked...";
    public string interactUITextContent = "Use [E]"; 
    public float transitionWaitTime = 1f;

    private GameObject InteractUI;
    private TextMeshProUGUI interactUIText;
    private GameObject PopUpUI;
    private TextMeshProUGUI popUpText;
    private Animator popUpAnimator;

    private bool isPlayerInTrigger = false;

    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            if (PopUpUI != null && popUpText != null && popUpAnimator != null)
            {
                popUpText.text = popUpTextContent;
                PopUpUI.SetActive(true);
                popUpAnimator.Play("PopUpUI", -1, 0f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InteractUI = other.transform.Find("PlayerInfoUICanvas/Interaction").gameObject;
            interactUIText = InteractUI.GetComponentInChildren<TextMeshProUGUI>();
            interactUIText.text = interactUITextContent;

            if (!isFunctional)
            {
                PopUpUI = other.transform.Find("PlayerInfoUICanvas/PopUp").gameObject;
                popUpText = PopUpUI.GetComponentInChildren<TextMeshProUGUI>();
                popUpAnimator = PopUpUI.GetComponent<Animator>();
            }
            
            isPlayerInTrigger = true;
            if (InteractUI != null) InteractUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            if (InteractUI != null) InteractUI.SetActive(false);
            if (PopUpUI != null) PopUpUI.SetActive(false); 
        }
    }
}