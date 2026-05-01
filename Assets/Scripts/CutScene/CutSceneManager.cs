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

[System.Serializable]
public struct LanguageJson {
    public Language language;
    public TextAsset jsonFile;
}

public class CutSceneManager : MonoBehaviour {
    [SerializeField] private List<LanguageJson> languageFiles;
    [SerializeField] private CutScene[] cutScenes;
    [SerializeField] private TMP_Text textDisplay;
    [SerializeField] private AudioSource speechSource;
    [SerializeField] private AudioSource musicSource;
    public string targetChildName = "CutSceneImg";
    private int currentSceneIndex = 0;
    private bool isPlaying;
    private DialogueList allDialogues;

    void Start() {
        LoadDialoguesForCurrentLanguage();
        PlayScene();
    }

    private void LoadDialoguesForCurrentLanguage() {
        Language currentLanguage = gameDataManager.currentGameData != null
            ? gameDataManager.currentGameData.settings.currentLanguage
            : GameSettings.currentLanguage;

        LanguageJson match = languageFiles.Find(l => l.language == currentLanguage);

        TextAsset file = match.jsonFile != null ? match.jsonFile : languageFiles[0].jsonFile;
        allDialogues = JsonUtility.FromJson<DialogueList>(file.text);
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

        speechSource.volume = gameDataManager.currentGameData.settings.FinalSpeechVolume;
        musicSource.volume  = gameDataManager.currentGameData.settings.FinalMusicVolume - (gameDataManager.currentGameData.settings.FinalMusicVolume * 0.8f);
        Debug.Log(musicSource.volume + " " + speechSource.volume);
        

        if (scene.speech != null) {
            speechSource.clip = scene.speech;
            speechSource.Play();
        }
        if (scene.music != null) {
            musicSource.clip = scene.music;
            musicSource.Play();
        }

        float duration = scene.speech != null ? scene.speech.length : 10f; 

        while (timer < duration) {
            timer += Time.deltaTime;

            while (data.lines != null && lineIndex < data.lines.Count && timer >= data.lines[lineIndex].time) {
                textDisplay.text = data.lines[lineIndex].text;
                lineIndex++;
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