using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// tăng 50% máu 
/// </summary>
public class UpHealthy : AbAssistanceSkill
{
    protected override void Start() {
        base.Start();
        m_timeCountdown = 60.0f;
    }
    public override void Work(Joystick joystickAssistanceSkill)
    {
        if (!m_hasSkillReady) return;

        if (joystickAssistanceSkill.GetJoystickState()) {

            if (Tank.LocalPlayerInstance == null) return;
            m_tankLocalPlayer = m_tankLocalPlayer ?? Tank.LocalPlayerInstance.GetComponent<Tank>();

            var effectHealOnce = PunObjectPool.Instance.GetLocalPool("Prefabs/Effect/HealOnce", "HealOnce", m_tankLocalPlayer.BombPowPoint.position, Quaternion.identity).transform;
            effectHealOnce.localEulerAngles = new Vector3(-90, 0, 0);
            m_tankLocalPlayer.CurrentHealthy = m_tankLocalPlayer.CurrentHealthy + 0.5f * m_tankLocalPlayer.MaxHealthy;
            if (m_tankLocalPlayer.CurrentHealthy > m_tankLocalPlayer.MaxHealthy) m_tankLocalPlayer.CurrentHealthy = m_tankLocalPlayer.MaxHealthy;
            m_tankLocalPlayer.ChangeBlood(m_tankLocalPlayer.CurrentHealthy, m_tankLocalPlayer.MaxHealthy);
            this.RefreshSkill();
        }
    }
}
