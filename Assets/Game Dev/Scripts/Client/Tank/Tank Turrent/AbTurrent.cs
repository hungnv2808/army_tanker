using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class AbTurrent : MonoBehaviour
{
    public float MaxCooldown = 0.0f;
    protected float m_timerTo = 0;
    protected float m_timerBack = 0;
    protected float m_lerpTime = 0.1f;
    protected float m_lerpRatio;
    protected float m_energyConsumption = 0;
    protected Tank m_TankParentScript;
    protected float m_damage;
    // protected Vector3 m_originalTankTurrentPosition = new Vector3(0, 1.57f, 0); /*vị trí ban đầu của nòng súng trước khi chạy animation*/
    protected Vector3 m_originalTankTurrentPosition;/*vị trí ban đầu của nòng súng trước khi chạy animation*/
    public abstract void ShootAndSync(Transform fireTransform, Transform tankTurren, Vector3 turentDirection, int label, string playerName, int whoViewID); /*bắt buộc class con phải implement*/
    public virtual void Shoot(Transform fireTransform, Transform tankTurren, Vector3 turentDirection, int label, string playerName, int whoViewID) {}// class con tùy chọn để implement
    public virtual void Shoot(Transform fireTransform, Transform tankTurren, Vector3 turrentDirection, bool isShooted, int label, string playerName, int whoViewID) {}// class con tùy chọn để implement
    public virtual void RecoilGun(Transform tankTurren, Vector3 turrentDirection) { // class con tùy chọn để implement
        m_timerTo = 0;
        m_timerBack = 0;
        StartCoroutine(RecoilGunCoroutine(m_originalTankTurrentPosition, tankTurren, turrentDirection));
    }
    protected IEnumerator RecoilGunCoroutine(Vector3 originalPosition, Transform tankTurren, Vector3 turrentDirection)
    {
        m_timerTo += Time.deltaTime;
        if (m_timerTo > m_lerpTime) m_timerTo = m_lerpTime;
        m_lerpRatio = m_timerTo / m_lerpTime;

        tankTurren.localPosition = Vector3.Lerp(originalPosition, tankTurren.localPosition - turrentDirection.normalized * 0.2f, m_lerpRatio);
        if (m_lerpRatio >= 1)
        {
            StartCoroutine(PlayBack(originalPosition, tankTurren));
            yield break;
        }
        else
        {
            yield return null;
            StartCoroutine(RecoilGunCoroutine(originalPosition, tankTurren, turrentDirection));
        }
    }
    protected IEnumerator PlayBack(Vector3 originalPosition, Transform tankTurren)
    {
        m_timerBack += Time.deltaTime;
        if (m_timerBack > m_lerpTime) m_timerBack = m_lerpTime;
        m_lerpRatio = m_timerBack / m_lerpTime;
        tankTurren.localPosition = Vector3.Lerp(tankTurren.localPosition, originalPosition, m_lerpRatio);
        if (m_lerpRatio >= 1) {
            yield break;
        }
        else
        {
            yield return null;
            StartCoroutine(PlayBack(originalPosition, tankTurren));
        }
    }
    protected bool CheckEnergy() {
        if (m_TankParentScript.CurrentEnergy >= m_energyConsumption) {
            m_TankParentScript.CurrentEnergy -= m_energyConsumption;
            // Debug.Log("m_TankParentScript.CurrentEnergy "+ m_TankParentScript.CurrentEnergy);
            m_TankParentScript.EnergyScript.DecreaseEnergy(m_energyConsumption, m_TankParentScript.MaxEnergy);
            return true;
        } else {
            //TODO: run animation run out energy
            m_TankParentScript.EnergyScript.PlayRunOutEnergyAnimation();
            return false;
        }
    }
    public Vector3 OriginalTankTurrentPosition {
        set {
            m_originalTankTurrentPosition = value;
        }
    }
    public float Damage {
        get {
            return m_damage;
        }
        set {
            m_damage = value;
        }
    }
}
