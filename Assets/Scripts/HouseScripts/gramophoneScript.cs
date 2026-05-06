using UnityEngine;
using TMPro;
using System.Collections;

public class gramophoneScript : MonoBehaviour
{
    public AudioClip musicClip;
    public AudioClip startSound;
    public AudioClip stopSound;

    private string interactUITextContent = "Stop [E]";
    private GameObject InteractUI;
    private TextMeshProUGUI interactUIText;
    private AudioSource audioSource;
    private bool isPlayerInTrigger = false;
    private bool isPlaying = true;
    private float savedTime = 0f;
    private Coroutine activeRoutine;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = musicClip;
        audioSource.playOnAwake = false;
        audioSource.loop = true;
    }

    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyBoardSetting.Interact))
        {
            ToggleMusic();
        }
    }

    private void ToggleMusic()
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);

        if (!isPlaying)
        {
            activeRoutine = StartCoroutine(PlayMusicWithDelay());
        }
        else
        {
            StopMusicWithSave();
        }

        if (interactUIText != null)
        {
            interactUIText.text = interactUITextContent;
        }
    }

    private IEnumerator PlayMusicWithDelay()
    {
        isPlaying = true;
        interactUITextContent = "Stop [E]";

        if (startSound != null)
        {
            audioSource.PlayOneShot(startSound);
            yield return new WaitForSeconds(startSound.length);
        }

        audioSource.time = savedTime;
        audioSource.Play();
        
        activeRoutine = null;
    }

    private void StopMusicWithSave()
    {
        isPlaying = false;
        interactUITextContent = "Play [E]";

        if (audioSource.isPlaying)
        {
            savedTime = audioSource.time;
        }

        audioSource.Stop();

        if (stopSound != null)
        {
            audioSource.PlayOneShot(stopSound);
        }

        activeRoutine = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Transform uiPath = other.transform.Find("PlayerInfoUICanvas/Interaction");
            if (uiPath != null)
            {
                InteractUI = uiPath.gameObject;
                interactUIText = InteractUI.GetComponentInChildren<TextMeshProUGUI>();
                interactUIText.text = interactUITextContent;

                isPlayerInTrigger = true;
                InteractUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            if (InteractUI != null) InteractUI.SetActive(false);
        }
    }
}