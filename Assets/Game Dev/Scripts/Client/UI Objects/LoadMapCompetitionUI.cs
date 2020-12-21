using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LoadMapCompetitionUI : MonoBehaviour
{
    [SerializeField] private Text m_progressText;
    private async void Start() {
        await PlayFabDatabase.Instance.UpdateResultCompetition(-1);
        StartCoroutine(LoadMapCompetition());
        Debug.Log("Competition Scene");
    }
    private IEnumerator LoadMapCompetition() {
        yield return new WaitForSeconds(2f);
        AsyncOperation async = SceneManager.LoadSceneAsync("Competition Scene");
        while (!async.isDone) {
            Debug.Log("async.progress" + async.progress);
            m_progressText.text = (async.progress/0.9f)*100f + "%";
            yield return null;
        }
        
    }
}
