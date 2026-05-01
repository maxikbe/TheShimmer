using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public struct LineData {
    public float time;
    public string text;
}

[System.Serializable]
public struct DialogueData {
    public int id;
    public string name;
    public List<LineData> lines;
}

[System.Serializable]
public struct DialogueList {
    public List<DialogueData> dialogues;
}

[System.Serializable]
public struct CutScene {
    public int id;
    public RuntimeAnimatorController controller;
    public AudioClip speech;
    public AudioClip music;
}

public class CutSceneManager : MonoBehaviour {
    [SerializeField] private TextAsset jsonFile;
    [SerializeField] private CutScene[] cutScenes;
    [SerializeField] private TMP_Text textDisplay;
    [SerializeField] private AudioSource speechSource;
    public string targetChildName = "CutSceneImg";
    private int currentSceneIndex = 0;
    private bool isPlaying;
    private DialogueList allDialogues;

    void Start() {
        allDialogues = JsonUtility.FromJson<DialogueList>(jsonFile.text);
        PlayScene();
    }

    public void PlayScene() {
        if (isPlaying || currentSceneIndex >= cutScenes.Length) return;
        
        DialogueData currentData = allDialogues.dialogues.Find(d => d.id == cutScenes[currentSceneIndex].id);
        StartCoroutine(PlaySceneRoutine(cutScenes[currentSceneIndex], currentData));
    }

    private IEnumerator PlaySceneRoutine(CutScene scene, DialogueData data) {
        isPlaying = true;
        float timer = 0;
        int lineIndex = 0;

        Transform childTransform = transform.Find(targetChildName);
        if (childTransform != null) {
            Animator anim = childTransform.GetComponent<Animator>();
            if (anim != null && scene.controller != null) {
                anim.runtimeAnimatorController = scene.controller;
                anim.Play(0); 
            }
        }

        if (scene.speech != null) {
            speechSource.clip = scene.speech;
            speechSource.Play();
        }

        float duration = scene.speech != null ? scene.speech.length : 5f; 

        while (timer < duration) {
            timer += Time.deltaTime;

            if (data.lines != null && lineIndex < data.lines.Count) {
                if (timer >= data.lines[lineIndex].time) {
                    textDisplay.text = data.lines[lineIndex].text;
                    lineIndex++;
                }
            }
            yield return null;
        }

        textDisplay.text = "";
        isPlaying = false;
        currentSceneIndex++;

        if (currentSceneIndex < cutScenes.Length) {
            PlayScene();
        }
    }
}