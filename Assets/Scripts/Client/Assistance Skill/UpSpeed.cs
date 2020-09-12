using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// tăng 30% tốc độ dịch chuyển trong 30s
/// </summary>
public class UpSpeed : AbAssistanceSkill
{
    private float m_currentMoveSpeed;
    private float m_timerUpSpeed;
    private void Start() {
        m_timeCountdown = 60.0f;
    }
    public override void Work(Joystick joystickAssistanceSkill)
    {
        if (!m_hasSkillReady) return;

        if (joystickAssistanceSkill.GetJoystickState()) {

            if (Tank.LocalPlayerInstance == null) return;
            m_tankLocalPlayer = m_tankLocalPlayer ?? Tank.LocalPlayerInstance.GetComponent<Tank>();

            m_currentMoveSpeed = m_tankLocalPlayer.MoveSpeed;
            m_tankLocalPlayer.MoveSpeed = m_currentMoveSpeed * 1.3f;
            StartCoroutine(LoopUpSpeedCoroutine());
        }
    }
    private IEnumerator LoopUpSpeedCoroutine() {
        m_timerUpSpeed = 30.0f;
        while(m_timerUpSpeed > 0) {
            yield return new WaitForSeconds(1.0f);
            m_timerUpSpeed -= 1;
        }
        m_tankLocalPlayer.MoveSpeed = m_currentMoveSpeed;
    }

}
