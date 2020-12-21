using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum JoytickState
{
    None = 0,
    PointDown,
    PointUp,
}
public class GameControler : MonoBehaviour
{
    [SerializeField] private PlayMode m_playMode; 
    private Joystick m_joytickMovement; /* joystick di chuyển*/
    private Joystick m_joytickCrossHairs; /* joystick tâm ngắm*/
    private Joystick m_joytickCrossHairs2;
    private Joystick m_joystickAssistanceSkill; //kỹ năng tương trợ
    private Tank m_tankPlayer;
    private AbAssistanceSkill m_currentAssistanceSkillScript;
    private Callback m_updateHandle;
    [SerializeField] private Sprite[] m_assistanceSkillImage;
    // Start is called before the first frame update
    void Start()
    {
        switch ((int)m_playMode) {
            case 1:
                StartModeMoba();
                m_updateHandle = UpdateModeMoba;
                break;
            case 2:
                StartModeCompetition();
                m_updateHandle = UpdateModeCompetition;
                break;

        }
        // Instantiate(Resources.Load("Prefabs/Tank Competition"), Vector3.zero, Quaternion.identity);//test
        
    }
    private void StartModeMoba() {
        m_joytickMovement = ArenaUI.Instance.JoytickMovement;
        m_joytickCrossHairs = ArenaUI.Instance.JoytickCrossHairs;
        m_joystickAssistanceSkill = ArenaUI.Instance.JoytickAssistanceSkill;
        this.AddAssistanceSkillScript();
    }
    private void StartModeCompetition() {
        m_joytickMovement = CompetitionUI.Instance.JoytickMovement;
        m_joytickCrossHairs = CompetitionUI.Instance.JoytickCrossHairs1;
        m_joytickCrossHairs2 = CompetitionUI.Instance.JoytickCrossHairs2;
    }
    private void AddAssistanceSkillScript() {
        switch (PlayFabDatabase.Instance.IndexAssistanceSkillSelected) {
            case 0:
                m_currentAssistanceSkillScript = m_joystickAssistanceSkill.gameObject.AddComponent<UpHealthy>();
                break;
            case 1:
                m_currentAssistanceSkillScript = m_joystickAssistanceSkill.gameObject.AddComponent<UpSpeed>();
                break;
            case 2:
                m_currentAssistanceSkillScript = m_joystickAssistanceSkill.gameObject.AddComponent<UpDamage>();
                break;
            case 3:
                m_currentAssistanceSkillScript = m_joystickAssistanceSkill.gameObject.AddComponent<BombPow>();
                break;
                
        }
        ArenaUI.Instance.AssistanceSkillImage.sprite = m_assistanceSkillImage[PlayFabDatabase.Instance.IndexAssistanceSkillSelected];
    }

    // Update is called once per frame
    // void Update()
    // {
    //     if (m_updateHandle != null) m_updateHandle(); 

    // }
    private void FixedUpdate() {
        if (m_updateHandle != null) m_updateHandle(); 
    }
    private void UpdateModeMoba() {
         /*photonView.IsMine = true tức là tank này chính là local player và chúng ta được quyền điều khiển nó*/
        if (Tank.LocalPlayerInstance == null  || PhotonNetwork.IsConnected == false) return;
        m_tankPlayer = m_tankPlayer ?? Tank.LocalPlayerInstance.GetComponent<Tank>();
        m_tankPlayer.MoveOnPC(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")); // cai nay de test tren PC
        #if UNITY_STANDALONE_WIN
            m_tankPlayer.MoveOnPC(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        #endif
        m_tankPlayer.Move(m_joytickMovement);
        m_tankPlayer.Attack(m_joytickCrossHairs);
        m_currentAssistanceSkillScript.Work(m_joystickAssistanceSkill);
    }
    private void UpdateModeCompetition() {
        if (TankCompetition.Instance != null) {
            TankCompetition.Instance.MoveOnPC(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            #if UNITY_STANDALONE_WIN
            TankCompetition.Instance.MoveOnPC(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            #endif
            TankCompetition.Instance.Move(m_joytickMovement);
            TankCompetition.Instance.Attack(m_joytickCrossHairs, m_joytickCrossHairs2);
        }
    }
}
