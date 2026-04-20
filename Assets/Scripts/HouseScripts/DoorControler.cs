using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem.Interactions;

public class DoorControler : MonoBehaviour
{
    public string sceneToLoad;
    public bool isFunctional = false;
    public string spawnPointName;
    public string popUpTextContent = "It´s blocked...";
    public string interactUITextContent = "Enter [E]"; 
    public float transitionWaitTime = 1f;

    private GameObject InteractUI;
    private GameObject PopUpUI;
    private TextMeshProUGUI popUpText;
    private TextMeshProUGUI interactUIText;
    private Animator popUpAnimator;
    private Animator transitionAnimator;
    private GameObject TransitionPanel;
    

    private bool isPlayerInTrigger = false;

    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            if (isFunctional)
            {
                StartCoroutine(ProcessMove());
            }
            else
            {
                if (PopUpUI != null && popUpText != null && popUpAnimator != null)
                {
                    popUpText.text = popUpTextContent;
                    PopUpUI.SetActive(true);
                    popUpAnimator.Play("PopUpUI", -1, 0f);
                }
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
            else
            {
                TransitionPanel = other.transform.Find("PlayerInfoUICanvas/TransitionPanel").gameObject;
                transitionAnimator = TransitionPanel.GetComponent<Animator>();
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

    IEnumerator ProcessMove()
    {
        if (InteractUI != null) InteractUI.SetActive(false);

        if (TransitionPanel != null && transitionAnimator != null)
        {
            TransitionPanel.SetActive(true);
            transitionAnimator.Play("TransitionAnimationReversed", -1, 0f); 
        }

        yield return new WaitForSeconds(transitionWaitTime);

        PlayerPrefs.SetString("LastSpawnPoint", spawnPointName);
        SceneManager.LoadScene(sceneToLoad);
    }
}