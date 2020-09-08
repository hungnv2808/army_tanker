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
    private Joystick m_joytickMovement; /* joystick di chuyển*/
    private Joystick m_joytickCrossHairs; /* joystick tâm ngắm*/
    
    private Joystick m_joystickAssistanceSkill; //kỹ năng tương trợ
    private Tank m_tankPlayer;
    private IAssistanceSkill m_assistanceSkill;
    // Start is called before the first frame update
    void Start()
    {
        m_joytickMovement = ArenaUI.Instance.JoytickMovement;
        m_joytickCrossHairs = ArenaUI.Instance.JoytickCrossHairs;
        m_joystickAssistanceSkill = ArenaUI.Instance.JoytickAssistanceSkill;
        m_assistanceSkill = m_joystickAssistanceSkill.gameObject.GetComponent<BombPow>();

    }

    // Update is called once per frame
    void Update()
    {
         /*photonView.IsMine = true tức là tank này chính là local player và chúng ta được quyền điều khiển nó*/
        if (Tank.LocalPlayerInstance == null  || PhotonNetwork.IsConnected == false) return;
        m_tankPlayer = m_tankPlayer ?? Tank.LocalPlayerInstance.GetComponent<Tank>();
        m_tankPlayer.MoveOnPC(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")); // cai nay de test tren PC
        #if UNITY_STANDALONE_WIN
            m_tankPlayer.MoveOnPC(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        #endif
        m_tankPlayer.Move(m_joytickMovement);
        m_tankPlayer.Attack(m_joytickCrossHairs);
        m_assistanceSkill.Work(m_joystickAssistanceSkill);

    }
    
}
