using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Lv2Turrent : AbTurrent
{
    
    private float m_curCooldown = 0.0f;
    private int m_simpleDamage;
    private void Start() {
        m_TankParentScript = GetComponentInParent<Tank>();
        m_energyConsumption = 40.0f;
        MaxCooldown = 0.8f;
        
    }
    public override void ShootAndSync(Transform fireTransform, Transform tankTurren, Vector3 turrentDirection, int label, string playerName, int whoViewID) {
        m_curCooldown -= Time.deltaTime;
        if (m_curCooldown <= 0)
        {
            if (!this.CheckEnergy()) return;
            StartCoroutine(CreatTrippleBulletCoroutine(fireTransform, tankTurren, turrentDirection, label, playerName, whoViewID));
            m_TankParentScript.SendDispatchShooted(TankEvent.EVENT_SEND_DISPATCH_TURRENT_SHOOTED);
            this.RecoilGun(tankTurren, turrentDirection);
            m_TankParentScript.Recoil(10);
            m_curCooldown = MaxCooldown;
            if (m_TankParentScript.photonView.IsMine && m_TankParentScript.IsPlayer) {
                CameraFollow.Instance.Shake(0.15f, 0.35f);
            }
        }
    }
    public override void Shoot(Transform fireTransform, Transform tankTurren, Vector3 turrentDirection, int label, string playerName, int whoViewID) {
        StartCoroutine(CreatTrippleBulletCoroutine(fireTransform, tankTurren, turrentDirection, label, playerName, whoViewID));
        this.RecoilGun(tankTurren, turrentDirection);
        m_curCooldown = MaxCooldown;
    }
    private IEnumerator CreatTrippleBulletCoroutine(Transform fireTransform, Transform tankTurren, Vector3 turrentDirection, int label, string playerName, int whoViewID) {
        m_simpleDamage = (int)this.m_damage/3;
        // Instantiate<TankBullet>(Resources.Load<TankBullet>("Prefabs/Tank Bullet/Tank Normal Bullet"), fireTransform.position, Quaternion.identity).Init(tankTurren, tankTurren.eulerAngles + new Vector3(-90.0f, 0, 0), 5.0f, 100.0f, 0.4f);
        PunObjectPool.Instance.GetLocalPool("Prefabs/Tank Bullet/Tank Normal Bullet", "Tank Normal Bullet", fireTransform.position, Quaternion.identity).GetComponent<TankBullet>().Init(label, playerName, whoViewID, tankTurren, tankTurren.eulerAngles + new Vector3(-90.0f, 0, 0), m_simpleDamage, 40.0f, 1f);/*phải cộng -90 độ là do thằng xe thăng bị quay 1 góc -90 độ*/

        yield return new WaitForSeconds(0.1f);
        // Instantiate<TankBullet>(Resources.Load<TankBullet>("Prefabs/Tank Bullet/Tank Normal Bullet"), fireTransform.position, Quaternion.identity).Init(tankTurren, tankTurren.eulerAngles + new Vector3(-90.0f, 0, 0), 5.0f, 100.0f, 0.4f);
        PunObjectPool.Instance.GetLocalPool("Prefabs/Tank Bullet/Tank Normal Bullet", "Tank Normal Bullet", fireTransform.position, Quaternion.identity).GetComponent<TankBullet>().Init(label, playerName, whoViewID, tankTurren, tankTurren.eulerAngles + new Vector3(-90.0f, 0, 0), m_simpleDamage, 40.0f, 1f);/*phải cộng -90 độ là do thằng xe thăng bị quay 1 góc -90 độ*/
        
        yield return new WaitForSeconds(0.1f);
        // Instantiate<TankBullet>(Resources.Load<TankBullet>("Prefabs/Tank Bullet/Tank Normal Bullet"), fireTransform.position, Quaternion.identity).Init(tankTurren, tankTurren.eulerAngles + new Vector3(-90.0f, 0, 0), 5.0f, 100.0f, 0.4f);
        PunObjectPool.Instance.GetLocalPool("Prefabs/Tank Bullet/Tank Normal Bullet", "Tank Normal Bullet", fireTransform.position, Quaternion.identity).GetComponent<TankBullet>().Init(label, playerName, whoViewID, tankTurren, tankTurren.eulerAngles + new Vector3(-90.0f, 0, 0), m_simpleDamage, 40.0f, 1f);/*phải cộng -90 độ là do thằng xe thăng bị quay 1 góc -90 độ*/

    }
}
