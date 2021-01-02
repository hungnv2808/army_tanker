using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
    private static SceneLoader s_instance;
    public static SceneLoader Instance {
        get {
            return s_instance;
        }
    }
     private void Awake() {
        if (s_instance != null && s_instance != this) {
            Destroy(this.gameObject);
            return;
        }
        s_instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    public IEnumerator LoadMenuSceneAfterFinishedCompetition() {
        yield return new WaitForSeconds(3.0f);
        AsyncOperation asyn = SceneManager.LoadSceneAsync("Menu Scene");
        while (!asyn.isDone) {
            yield return null;
        }
        MenuUI.Instance.ShowListCompetitionRank();
    }

}
