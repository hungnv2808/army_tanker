using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank2 : Tank
{
    protected override void CreatTrail()
    {
        m_leftTrailEffect = PunObjectPool.Instance.GetLocalPool("Prefabs/Effect/IceFloorTrail", "IceFloorTrail", m_leftTrail.position, Quaternion.identity).GetComponent<TankTrail>();
        m_rightTrailEffect = PunObjectPool.Instance.GetLocalPool("Prefabs/Effect/IceFloorTrail", "IceFloorTrail", m_rightTrail.position, Quaternion.identity).GetComponent<TankTrail>();
        m_leftTrailEffect.Init(m_leftTrail);
        m_rightTrailEffect.Init(m_rightTrail);
    }
    protected override void InitTankTurrents()
    {
        GameObject lv1Turrent = new GameObject();
        lv1Turrent.name = "Turrent Level 2";
        lv1Turrent.AddComponent<Lv2Turrent>();
        lv1Turrent.transform.SetParent(m_transform);
        this.m_currentTurrent = lv1Turrent.GetComponent<Lv2Turrent>();
        this.m_currentTurrent.OriginalTankTurrentPosition = this.m_tankTurret.localPosition;
    }
}
