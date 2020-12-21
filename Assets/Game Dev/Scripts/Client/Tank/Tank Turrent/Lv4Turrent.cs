using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Lv4Turrent : AbTurrent
{
    
    private float m_curCooldown = 0.0f;
    
    private void Start() {
        m_TankParentScript = GetComponentInParent<Tank>();
        m_energyConsumption = 50.0f;
        MaxCooldown = 0.5f;
    }
    public override void ShootAndSync(Transform fireTransform, Transform tankTurren, Vector3 turrentDirection, int label, string whoDamage, int whoViewID) {
        m_curCooldown -= Time.deltaTime;
        if (m_curCooldown <= 0)
        {
            if (!this.CheckEnergy()) return;
            PunObjectPool.Instance.GetLocalPool("Prefabs/Tank Bullet/Tank Rocket Bullet", "Tank Rocket Bullet", fireTransform.position, Quaternion.identity).GetComponent<TankBullet>().Init(label, whoDamage, whoViewID, tankTurren, tankTurren.eulerAngles + new Vector3(-90.0f, 0, 0), 20.0f, 45.0f, 1f);/*phải cộng -90 độ là do thằng xe thăng bị quay 1 góc -90 độ*/
            m_TankParentScript.SendDispatchShooted(TankEvent.EVENT_SEND_DISPATCH_TURRENT_SHOOTED);
            this.RecoilGun(tankTurren, turrentDirection);
            m_TankParentScript.Recoil(10);
            m_curCooldown = MaxCooldown;
            if (m_TankParentScript.photonView.IsMine && m_TankParentScript.IsPlayer) {
                CameraFollow.Instance.Shake(0.15f, 0.35f);
            }
        }
    }
    public override void Shoot(Transform fireTransform, Transform tankTurren, Vector3 turrentDirection, int label, string whoDamage, int whoViewID) {
        PunObjectPool.Instance.GetLocalPool("Prefabs/Tank Bullet/Tank Rocket Bullet", "Tank Rocket Bullet", fireTransform.position, Quaternion.identity).GetComponent<TankBullet>().Init(label, whoDamage, whoViewID, tankTurren, tankTurren.eulerAngles + new Vector3(-90.0f, 0, 0), 20.0f, 45.0f, 1f);/*phải cộng -90 độ là do thằng xe thăng bị quay 1 góc -90 độ*/
        this.RecoilGun(tankTurren, turrentDirection);
        m_curCooldown = MaxCooldown;
    }
}
