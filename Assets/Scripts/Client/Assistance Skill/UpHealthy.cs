using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// tăng 50% máu 
/// </summary>
public class UpHealthy : AbAssistanceSkill
{
    private void Start() {
        m_timeCountdown = 60.0f;
    }
    public override void Work(Joystick joystickAssistanceSkill)
    {
        if (!m_hasSkillReady) return;

        if (joystickAssistanceSkill.GetJoystickState()) {

            if (Tank.LocalPlayerInstance == null) return;
            m_tankLocalPlayer = m_tankLocalPlayer ?? Tank.LocalPlayerInstance.GetComponent<Tank>();

            m_tankLocalPlayer.CurrentHealthy = m_tankLocalPlayer.CurrentHealthy + 0.5f * m_tankLocalPlayer.MaxHealthy;
            m_tankLocalPlayer.HealthyBarScript.SetCurrentHealthy(m_tankLocalPlayer.CurrentHealthy, m_tankLocalPlayer.MaxHealthy);
            this.RefreshSkill();
        }
    }
}
