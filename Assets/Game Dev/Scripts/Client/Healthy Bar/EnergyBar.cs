using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public class EnergyBar : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] m_spriteBar;
    [SerializeField] private Tank m_tankScript;
    [SerializeField] private Animator m_animator;
    [SerializeField] private SpriteRenderer m_redBG;
    private bool m_isRunOutEnergy = false;
    private float m_sizeOneBar = 0.8f;
    private float m_tmpSize;
    private float m_maxSize;
    private bool m_isIncreased = false;
    private float m_maxEnergy;
    private void OnEnable() {
        m_isIncreased = false;
        StartCoroutine(IncreaseEnergyCoroutine());
        m_maxSize = m_sizeOneBar * m_spriteBar.Length;
    }
    public void FillUpMaxEnergyBar() {
        m_redBG.color = new Color(m_redBG.color.r, m_redBG.color.g, m_redBG.color.b, 0);
        for (int i = 0; i < m_spriteBar.Length; i++)
        {
            m_spriteBar[i].size = new Vector2(m_sizeOneBar, m_spriteBar[i].size.y);
        }
    }
    public void DecreaseEnergy(float count, float maxCount) {
        this.m_maxEnergy = maxCount;
        this.m_isIncreased = false;
        // Debug.Log("DecreaseEnergy");
        CancelInvoke("IncreaseEnergy");
        m_tmpSize = m_maxSize * count / maxCount; 
        // Debug.Log("count : " + count + ", maxCount : " + maxCount +", m_tmpSize : " + m_tmpSize);
        for (int i = 0; i < m_spriteBar.Length; i++)
        {
            m_tmpSize = m_tmpSize - m_spriteBar[i].size.x;
            // Debug.Log("m_tmpSize" + m_tmpSize);
            if (m_tmpSize > 0) {
                m_spriteBar[i].size = new Vector2(0, m_spriteBar[i].size.y);
            } 
            else {
                m_spriteBar[i].size = new Vector2(-m_tmpSize, m_spriteBar[i].size.y);
                break;
            }
        }
        Invoke("IncreaseEnergy", 2f);
    }
    private void IncreaseEnergy() {
        m_isIncreased = true;
        // Debug.Log("IncreaseEnergy");
    }
    private IEnumerator IncreaseEnergyCoroutine() {
        while (true) {
            if (m_isIncreased) {
                for (int i = m_spriteBar.Length - 1; i >= 0; i--)
                {
                    if (m_spriteBar[i].size.x < m_sizeOneBar) {
                        if (!m_isIncreased) continue;
                        m_tankScript.CurrentEnergy += m_maxEnergy * (m_sizeOneBar-m_spriteBar[i].size.x)/m_maxSize;
                        if (m_tankScript.CurrentEnergy > this.m_maxEnergy) {
                            m_tankScript.CurrentEnergy = this.m_maxEnergy;
                        }
                        m_spriteBar[i].size = new Vector2(m_sizeOneBar, m_spriteBar[i].size.y);
                    }
                    yield return new WaitForSeconds(1f);
                }
                m_isIncreased = false;
            }
            yield return new WaitForSeconds(2.0f);
        }
    }
    public async void PlayRunOutEnergyAnimation() {
        if (m_isRunOutEnergy) return;
        m_isRunOutEnergy = true;
        m_animator.SetBool("isRunOutEnergy", m_isRunOutEnergy);
        while(AnimatorIsPlaying()) {
            await Task.Yield();
            // Debug.Log("loop AnimatorIsPlaying");
        }
        // Debug.Log("finish AnimatorIsPlaying");
        m_isRunOutEnergy = false;
        m_animator.SetBool("isRunOutEnergy", m_isRunOutEnergy);
    }
    private bool AnimatorIsPlaying(){
        return m_animator.GetCurrentAnimatorStateInfo(0).length <= m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}
