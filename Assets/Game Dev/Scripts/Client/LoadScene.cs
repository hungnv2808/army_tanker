using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
public class LoadScene : MonoBehaviour
{
    private static LoadScene m_instance;
    public static LoadScene Instance {
        get {
            return m_instance;
        }
    }
    private void Awake() {
        if (m_instance != null && m_instance != this) {
            Destroy(this.gameObject);
        }
        m_instance = this;
        DontDestroyOnLoad(this);
    }
    private AsyncOperation m_asyncLoad;
    public void Load(string sceneName, Action callback) {
        StartCoroutine(LoadCoroutine(sceneName, callback));
    }   
    public IEnumerator LoadCoroutine(string sceneName, Action callback) {
        m_asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        m_asyncLoad.allowSceneActivation = true;
        while(!m_asyncLoad.isDone) {
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1f);
        callback();
    }
    public void LoadMenuSceneAfterCompetition() {
        StartCoroutine(LoadMenuSceneAfterCompetitionCoroutine());
    }
    public IEnumerator LoadMenuSceneAfterCompetitionCoroutine() {
        m_asyncLoad = SceneManager.LoadSceneAsync("Menu Scene");
        m_asyncLoad.allowSceneActivation = true;
        while(!m_asyncLoad.isDone) {
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1f);
        MenuUI.Instance.ShowListCompetitionRank();
    }
}
