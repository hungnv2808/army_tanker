using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// tăng 50% sát thương trong 45s
/// </summary>
public class UpDamage : AbAssistanceSkill
{
    private float m_currentDamage;
    private float m_timerUpDamage;
    protected override void Start() {
        base.Start();
        m_timeCountdown = 100.0f;
    }
    public override void Work(Joystick joystickAssistanceSkill)
    {
        if (!m_hasSkillReady) return;

        if (joystickAssistanceSkill.GetJoystickState()) {

            if (Tank.LocalPlayerInstance == null) return;
            m_tankLocalPlayer = m_tankLocalPlayer ?? Tank.LocalPlayerInstance.GetComponent<Tank>();

            m_currentDamage = m_tankLocalPlayer.Damage;
            m_tankLocalPlayer.Damage = m_currentDamage * 1.5f;
            this.RefreshSkill();
            StartCoroutine(LoopUpSpeedCoroutine());
        }
    }
    private IEnumerator LoopUpSpeedCoroutine() {
        m_timerUpDamage = 45.0f;
        while(m_timerUpDamage > 0) {
            yield return new WaitForSeconds(1.0f);
            m_timerUpDamage -= 1;
        }
        m_tankLocalPlayer.Damage = m_currentDamage;
    }
}
