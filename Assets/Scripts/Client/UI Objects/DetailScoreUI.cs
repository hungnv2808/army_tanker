using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class DetailScoreUI : MonoBehaviour
{
    private bool m_isInited = false;
    [SerializeField] private Vector3[] m_positionPersonalScoreTeam0;
    [SerializeField] private Vector3[] m_positionPersonalScoreTeam1;
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
            var personalScoreClone = Instantiate(Resources.Load("Prefabs/Personal Score")) as GameObject;
            if (tank != null) {
                personalScores.Add(item, personalScoreClone.GetComponent<PersonalScoreUI>());
                personalScoreClone.GetComponent<PersonalScoreUI>().Tank = tank.gameObject.GetComponent<Tank>();
                if (personalScores[item].Tank.Team == 0) {
                    personalScoreClone.transform.SetParent(m_transform);
                    personalScoreClone.GetComponent<PersonalScoreUI>().Position = m_positionPersonalScoreTeam0[indexTeam0];
                    indexTeam0 += 1;
                    Debug.Log("team0");
                } else {
                    personalScoreClone.transform.SetParent(m_transform);
                    personalScoreClone.GetComponent<PersonalScoreUI>().Position = m_positionPersonalScoreTeam1[indexTeam1];
                    indexTeam1 += 1;
                    Debug.Log("team1");

                }
            } else {
                Destroy(personalScoreClone);
            }
            
        }
        return personalScores;
    }
    private void OnClose() {
        this.m_detailScorePanel.SetActive(false);
    }
}
