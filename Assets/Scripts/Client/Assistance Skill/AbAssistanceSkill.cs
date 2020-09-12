using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AbAssistanceSkill : MonoBehaviour
{
    protected float m_timeCountdown; //theo giay;
    protected float m_lerpTime = 0.0f;
    protected Tank m_tankLocalPlayer;
    protected bool m_hasSkillReady = true;
    [SerializeField] protected Image m_refreshImage;
    [SerializeField] protected Text m_secondLabel;
    protected JoytickState m_joystickState;
    public abstract void Work(Joystick joystickAssistanceSkill);
    protected void RefreshSkill() {
        Debug.Log("RefreshSkill");
        m_lerpTime = m_timeCountdown;
        m_hasSkillReady = false;
        m_refreshImage.gameObject.SetActive(true);
        m_refreshImage.fillAmount = 1;
        m_secondLabel.text = "" + m_lerpTime;
        StartCoroutine(RefreshSkillLoopCoroutine());
    }
    protected IEnumerator RefreshSkillLoopCoroutine() {
        yield return new WaitForSeconds(1.0f);
        m_lerpTime -= 1;
        m_secondLabel.text = "" + m_lerpTime;
        m_refreshImage.fillAmount = m_lerpTime/m_timeCountdown;
        if (m_lerpTime <= 0) {
            m_hasSkillReady = true;
            m_refreshImage.gameObject.SetActive(false);
            yield break;
        } else {
            StartCoroutine(RefreshSkillLoopCoroutine());
        }
    }
}
