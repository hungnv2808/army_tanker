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
    private GameObject m_effectSpeedy;
    protected override void Start() {
        base.Start();
        m_timeCountdown = 90.0f;
    }
    public override void Work(Joystick joystickAssistanceSkill)
    {
        if (!m_hasSkillReady) return;

        if (joystickAssistanceSkill.GetJoystickState()) {

            if (Tank.LocalPlayerInstance == null) return;
            m_tankLocalPlayer = m_tankLocalPlayer ?? Tank.LocalPlayerInstance.GetComponent<Tank>();

            m_effectSpeedy = PunObjectPool.Instance.GetLocalPool("Prefabs/Effect/WindlinesSpeedy","WindlinesSpeedy", Vector3.zero, Quaternion.identity);
            var effectSpeedyTransform = m_effectSpeedy.transform;
            effectSpeedyTransform.SetParent(m_tankLocalPlayer.PositionEffect);
            effectSpeedyTransform.localScale = Vector3.one;
            effectSpeedyTransform.localEulerAngles = Vector3.zero;
            effectSpeedyTransform.localPosition = Vector3.zero;

            m_currentMoveSpeed = m_tankLocalPlayer.MoveSpeed;
            m_tankLocalPlayer.MoveSpeed = m_currentMoveSpeed * 1.5f;
            this.RefreshSkill();
            StartCoroutine(LoopUpSpeedCoroutine());
        }
    }
    private IEnumerator LoopUpSpeedCoroutine() {
        m_timerUpSpeed = 20.0f;
        while(m_timerUpSpeed > 0) {
            yield return new WaitForSeconds(1.0f);
            m_timerUpSpeed -= 1;
        }
        m_effectSpeedy.SetActive(false);
        var effectSpeedyTransform = m_effectSpeedy.transform;
        effectSpeedyTransform.SetParent(PunObjectPool.Instance.ObjecParent);
        effectSpeedyTransform.localEulerAngles = Vector3.zero;
        effectSpeedyTransform.localScale = Vector3.one;
        PunObjectPool.Instance.SetLocalPool(m_effectSpeedy);

        m_tankLocalPlayer.MoveSpeed = m_currentMoveSpeed;
    }
    
}
