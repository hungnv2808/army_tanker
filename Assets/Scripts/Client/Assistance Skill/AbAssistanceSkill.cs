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
    protected JoytickState m_joystickState;
    public abstract void Work(Joystick joystickAssistanceSkill);
    protected virtual void Start()
    {
        ArenaUI.Instance.RefreshImage.gameObject.SetActive(false);
    }
    protected void RefreshSkill() {
        Debug.Log("RefreshSkill");
        m_lerpTime = m_timeCountdown;
        m_hasSkillReady = false;
        ArenaUI.Instance.RefreshImage.gameObject.SetActive(true);
        ArenaUI.Instance.RefreshImage.fillAmount = 1;
        ArenaUI.Instance.SecondLabel.text = "" + m_lerpTime;
        StartCoroutine(RefreshSkillLoopCoroutine());
    }
    protected IEnumerator RefreshSkillLoopCoroutine() {
        yield return new WaitForSeconds(1.0f);
        m_lerpTime -= 1;
        ArenaUI.Instance.SecondLabel.text = "" + m_lerpTime;
        ArenaUI.Instance.RefreshImage.fillAmount = m_lerpTime/m_timeCountdown;
        if (m_lerpTime <= 0) {
            m_hasSkillReady = true;
            ArenaUI.Instance.RefreshImage.gameObject.SetActive(false);
            yield break;
        } else {
            StartCoroutine(RefreshSkillLoopCoroutine());
        }
    }
}
