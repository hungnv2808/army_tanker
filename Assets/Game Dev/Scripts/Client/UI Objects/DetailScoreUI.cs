using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class DetailScoreUI : MonoBehaviour
{
    private bool m_isInited = false;

    [SerializeField] private PersonalScoreUI[] m_personalScoreTeam0;
    [SerializeField] private PersonalScoreUI[] m_personalScoreTeam1;

    [SerializeField] private RectTransform m_transform;
    private static DetailScoreUI m_instance;
    [SerializeField] private Button m_closeButton;
    [SerializeField] private GameObject m_detailScorePanel;
    public static DetailScoreUI Instance {
        get {
            return m_instance;
        }
    } 
    private void Awake() {
        if (m_instance != null && m_instance != this) {
            Destroy(m_instance.gameObject);
        }
        m_instance = this;
    }
    private void Start() {
        this.m_closeButton.onClick.AddListener(OnClose);
    }
    public Dictionary<int, PersonalScoreUI> CreatPersonalScoreUI(List<int> tankViewIDs) {
        Dictionary<int, PersonalScoreUI> personalScores = new Dictionary<int, PersonalScoreUI>();
        int indexTeam0 = 0;
        int indexTeam1= 0;
        foreach (var item in tankViewIDs)
        {
            Debug.Log("item: " + item);
            var tank = PhotonView.Find(item);
            if (tank != null) {
                var tankScript = tank.gameObject.GetComponent<Tank>();
                if (tankScript.Team == 0) {
                    personalScores.Add(item, m_personalScoreTeam0[indexTeam0]);
                    m_personalScoreTeam0[indexTeam0].gameObject.SetActive(true);
                    m_personalScoreTeam0[indexTeam0].Tank = tankScript;
                    indexTeam0 += 1;
                    Debug.Log("team0");
                } else {
                    personalScores.Add(item, m_personalScoreTeam1[indexTeam1]);
                    m_personalScoreTeam1[indexTeam1].gameObject.SetActive(true);
                    m_personalScoreTeam1[indexTeam1].Tank = tankScript;
                    indexTeam1 += 1;
                    Debug.Log("team1");

                }
            }
            
        }
        return personalScores;
    }
    private void OnClose() {
        this.m_detailScorePanel.SetActive(false);
    }
}
