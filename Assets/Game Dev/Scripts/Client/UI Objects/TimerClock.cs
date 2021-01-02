using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class TimerClock : MonoBehaviour
{
    public int LerpTime = 0; // thời gian theo phút
    private float m_lerpTime = 0; // thời gian theo giây
    private static TimerClock s_instance;
    public static TimerClock Instance {
        get {
            return s_instance;
        }
    } 
    private string m_minute;
    private string m_second;
    private Text m_text;
    void Awake()
    {
        if (s_instance != null && s_instance != this) {
            Destroy(s_instance.gameObject);
        }
        s_instance = this;
    }
    void Start()
    {
        LerpTime = 1;
        m_text = GetComponent<Text>();
        m_lerpTime = LerpTime * 60.0f;
    }
    public void TurnClock() {
        StartCoroutine(CountdownCoroutine());
    }
    private IEnumerator CountdownCoroutine() {
        yield return new WaitForSeconds(1.0f);
        if (m_lerpTime > 0) {
            m_lerpTime -= 1.0f;
            if (((int)m_lerpTime / 60) < 10) {
                m_minute = "0" + ( (int)m_lerpTime/60 );
            } else {
                m_minute = "" + ( (int)m_lerpTime/60 );
            }
            if (((int)m_lerpTime % 60) < 10) {
                m_second = "0" + ( (int)m_lerpTime%60 );
            } else {
                m_second = "" + ( (int)m_lerpTime%60 );
            }
            m_text.text = m_minute + ":" + m_second;

            StartCoroutine(CountdownCoroutine());
        } else {
            GetTimeOut();
            yield break;
        }
    }
    public void GetTimeOut() {
        ClientManagement.Instance.PersonalScores[Tank.LocalPlayerInstance.photonView.ViewID].UpdateKillingCount();
        var myTeam = Tank.LocalPlayerInstance.GetComponent<Tank>().Team;
        if (ClientManagement.Instance.Team0Killed > ClientManagement.Instance.Team1Killed) {
            if (myTeam != 1) {
                // win
                ArenaUI.Instance.ShowGameVictoryPanel();
                MissionMangement.Instance.HasWon = true;
            } else {
                //lose
                ArenaUI.Instance.ShowGameDefeatPanel();
                MissionMangement.Instance.HasWon = false;
            }
        } else if (ClientManagement.Instance.Team0Killed < ClientManagement.Instance.Team1Killed) {
            if (myTeam != 1) {
                // lose
                ArenaUI.Instance.ShowGameDefeatPanel();
                MissionMangement.Instance.HasWon = true;
            } else {
                //win
                ArenaUI.Instance.ShowGameVictoryPanel();
                MissionMangement.Instance.HasWon = false;
            }
        } else {
            ArenaUI.Instance.ShowGameDefeatPanel();
            MissionMangement.Instance.HasWon = false;
        }
        
    }
}
