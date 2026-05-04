using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public struct LineData {
    public float time;
    public int voiceId;
    public int controllerId;
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
public struct Speech {
    public int voiceId;
    public AudioClip voice;
}

[System.Serializable]
public struct CutSceneController {
    public int controllerId;
    public RuntimeAnimatorController controller;
    public TimelineAsset timeline;               
}

[System.Serializable]
public struct CutScene {
    public int id;
    public List<Speech> speeches;
    public List<CutSceneController> controllers;
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
    [SerializeField] private Image blackOverlay;
    [SerializeField] private float transitionDuration = 0.5f;
    [SerializeField] private PlayableDirector playableDirector;
    public string targetChildName = "CutSceneImg";
    public static int currentSceneIndex = 0;
    private bool isPlaying;
    private DialogueList allDialogues;
    void Start() {
        LoadDialoguesForCurrentLanguage();

        if (blackOverlay != null) {
            Color c = blackOverlay.color;
            c.a = 0f;
            blackOverlay.color = c;
        }

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

        speechSource.volume = gameDataManager.currentGameData.settings.FinalSpeechVolume;
        musicSource.volume = gameDataManager.currentGameData.settings.FinalMusicVolume * 0.2f;

        if (scene.music != null) {
            musicSource.clip = scene.music;
            musicSource.Play();
        }

        int lastVoiceId = -1;
        int lastControllerId = -1;
        float timer = 0f;
        int lineIndex = 0;

        float totalDuration = 10f; 
        foreach (var s in scene.speeches)
            if (s.voice != null) totalDuration = Mathf.Max(totalDuration, s.voice.length);

        foreach (var cc in scene.controllers)
            if (cc.timeline != null && cc.controller == null) totalDuration = Mathf.Max(totalDuration, (float)cc.timeline.duration);

        while (timer < totalDuration || speechSource.isPlaying || 
              (playableDirector != null && playableDirector.state == PlayState.Playing) || 
              (data.lines != null && lineIndex < data.lines.Count)) {

            while (data.lines != null && lineIndex < data.lines.Count && timer >= data.lines[lineIndex].time) {
                LineData line = data.lines[lineIndex];
                textDisplay.text = line.text;

                bool voiceChanged = line.voiceId != lastVoiceId;
                bool controllerChanged = line.controllerId != lastControllerId;

                if (voiceChanged || controllerChanged) {
                    CutSceneController cc = scene.controllers.Find(c => c.controllerId == line.controllerId);
                    if (lastVoiceId == -1) {
                        ApplyController(cc);
                        if (voiceChanged) ApplyVoice(scene, line.voiceId);
                    } else {
                        yield return StartCoroutine(SwapWithTransition(
                            cc,
                            voiceChanged ? scene.speeches.Find(s => s.voiceId == line.voiceId) : (Speech?)null
                        ));
                    }

                    lastVoiceId = line.voiceId;
                    lastControllerId = line.controllerId;
                }

                lineIndex++;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        textDisplay.text = "";
        isPlaying = false;
        currentSceneIndex++;

        if (currentSceneIndex < cutScenes.Length)
            PlayScene();
    }

    private void ApplyController(CutSceneController cc) {
        Transform childTransform = transform.Find(targetChildName);
        Animator anim = childTransform != null ? childTransform.GetComponent<Animator>() : null;

        if (anim != null) {
            anim.runtimeAnimatorController = cc.controller;
            if (cc.controller != null) {
                anim.Play(0);
            }
        }

        if (cc.timeline != null) 
        {
            if (playableDirector != null) 
            {
                playableDirector.gameObject.SetActive(true);
                playableDirector.enabled = true;
                playableDirector.timeUpdateMode = DirectorUpdateMode.UnscaledGameTime;

                if (playableDirector.playableAsset != cc.timeline) playableDirector.playableAsset = cc.timeline;
                
                playableDirector.Stop();
                playableDirector.time = 0;
                playableDirector.Evaluate(); 
                playableDirector.Play();
            }
        } else if (playableDirector != null && playableDirector.state == PlayState.Playing) 
        {
            playableDirector.Stop();
        } else
        {
            playableDirector.gameObject.SetActive(false);
        }
    }

    private void ApplyVoice(CutScene scene, int voiceId) {
        Speech speech = scene.speeches.Find(s => s.voiceId == voiceId);
        if (speech.voice != null) {
            speechSource.clip = speech.voice;
            speechSource.Play();
        }
    }

    private IEnumerator SwapWithTransition(CutSceneController cc, Speech? newSpeech) {
        yield return StartCoroutine(FadeOverlay(0f, 1f));

        ApplyController(cc);
        if (newSpeech.HasValue && newSpeech.Value.voice != null) {
            speechSource.clip = newSpeech.Value.voice;
            speechSource.Play();
        }

        yield return StartCoroutine(FadeOverlay(1f, 0f));
    }

    private IEnumerator FadeOverlay(float startAlpha, float endAlpha) {
        if (blackOverlay == null) yield break;

        float elapsed = 0f;
        Color c = blackOverlay.color;

        while (elapsed < transitionDuration) {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / transitionDuration);
            blackOverlay.color = c;
            yield return null;
        }

        c.a = endAlpha;
        blackOverlay.color = c;
    }
}