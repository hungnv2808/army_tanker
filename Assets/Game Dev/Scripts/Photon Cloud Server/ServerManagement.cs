using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
public class ServerManagement : MonoBehaviourPunCallbacks
{
    private static ServerManagement s_instance;
    public static byte MaxPlayersInRoom = 1;
    private int m_timeoutLoadScene = 5;

    private void Awake()
    {
        if (s_instance != null && s_instance != this)
        {
            DestroyImmediate(this.gameObject);
            return;
        }
        s_instance = this;
        DontDestroyOnLoad(this);
    }
    public void CheckTimeoutLoadScene() {
        m_timeoutLoadScene = 5;
        InvokeRepeating("LoadScene", 0.0f, 1.0f);
    }
    private void LoadScene() {
        m_timeoutLoadScene -= 1;
        if (m_timeoutLoadScene <= 0) {
            CancleLoadSceneTimeout();
            PhotonNetwork.LoadLevel("Main Scene");
            StartCoroutine(CheckSceneLoadingCompletelyLoopCoroutine());
        }
    }
    public void CancleLoadSceneTimeout() {
        CancelInvoke("LoadScene");
    }
    public IEnumerator CheckSceneLoadingCompletelyLoopCoroutine() {
        // Temporary disable processing of futher network messages
        // PhotonNetwork.IsMessageQueueRunning = false;
        if (PhotonNetwork.LevelLoadingProgress >= 1) {
            if (PhotonNetwork.IsMasterClient) {
                Debug.Log("load pool is master client");
                PhotonNetwork.Instantiate("Pun Object Pool", Vector3.zero, Quaternion.identity, 0);
                PhotonNetwork.Instantiate("ClientManagement", Vector3.zero, Quaternion.identity, 0);
            }
            // this.StartGameCountDown();
            Debug.Log("####SceneLoadingCompletely");
            // PhotonNetwork.IsMessageQueueRunning = true;
            // CameraFollow.Instance.FindPlayer();
            yield break;
        } 
        yield return null;
        StartCoroutine(CheckSceneLoadingCompletelyLoopCoroutine());
    }
    #region Photon callbacks
    /*khi có 1 người mới tham gia phòng thì callback sẽ được gọi tới những người chơi khác*/
    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        // Debug.LogFormat("OnPlayerEnterRoom() {0}", otherPlayer.NickName); // not seen if you are the player connecting (sẽ không nhìn thấy khi mình đang joining room)
        if (PhotonNetwork.CurrentRoom.PlayerCount >= MaxPlayersInRoom)
        { 
            if (PhotonNetwork.IsMasterClient) { /*thằng master load scene thì tất cả những thằng client khác cũng được đồng bộ scene*/
                Debug.LogFormat("Enough player: {0} player", PhotonNetwork.CurrentRoom.PlayerCount);
                Debug.Log("start game");
                // #Critical
                // Load the Scene.
                PhotonNetwork.LoadLevel("Main Scene");
                Debug.Log("Best Region: " + PhotonNetwork.BestRegionSummaryInPreferences);
                StartCoroutine(CheckSceneLoadingCompletelyLoopCoroutine());
            } else {
                StartCoroutine(CheckSceneLoadingCompletelyLoopCoroutine());
            }
                
        }
        Debug.LogFormat("OnPlayerEnteredRoom: PlayerCount {0}", PhotonNetwork.CurrentRoom.PlayerCount);
    }
    /*khi 1 thằng rời phòng thì pun sẽ gọi callback là hàm này tới tất cả các người chơi khác*/
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("One player left room() {0}", otherPlayer.NickName); // seen when other disconnects
        // ArenaUI.Instance.ShowNotificationPanel();
    }
    // public override void OnLeftRoom() {
    //     /*callback này được gọi khi 
    //     tất cả người chơi rời phòng đều sẽ gọi vào hàm này kể cả khi ta thoát game cũng bị gọi vào hàm này */
    // }
    #endregion

   
    public static ServerManagement Instance
    {
        get
        {
            return s_instance;
        }
    }

}


