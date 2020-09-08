﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Lv3Turrent : AbTurrent
{
    
    private float m_curCooldown = 0.0f;
    
    private void Start() {
        m_TankParentScript = GetComponentInParent<Tank>();
        m_energyConsumption = 50.0f;
        MaxCooldown = 0.9f;
    }
    public override void ShootAndSync(int lvTurrent, Transform fireTransform, Transform tankTurren, Vector3 turrentDirection, int label, string playerName, int whoViewID) {
        m_curCooldown -= Time.deltaTime;
        if (m_curCooldown <= 0)
        {
            if (!this.CheckEnergy()) return;
            this.CreatTrippleBullet(fireTransform, tankTurren, turrentDirection, label, playerName, whoViewID);
            m_TankParentScript.SendDispatchShooted(TankEvent.EVENT_SEND_DISPATCH_TURRENT_SHOOTED, lvTurrent);
            this.RecoilGun(tankTurren, turrentDirection);
            m_TankParentScript.Recoil(8);
            m_curCooldown = MaxCooldown;
            if (m_TankParentScript.photonView.IsMine && m_TankParentScript.IsPlayer) {
                CameraFollow.Instance.Shake(0.15f, 0.35f);
            }
        }
    }
    public override void Shoot(Transform fireTransform, Transform tankTurren, Vector3 turrentDirection, int label, string playerName, int whoViewID) {
        this.CreatTrippleBullet(fireTransform, tankTurren, turrentDirection, label, playerName, whoViewID);
        this.RecoilGun(tankTurren, turrentDirection);
        m_curCooldown = MaxCooldown;
    }

    private void CreatTrippleBullet(Transform fireTransform, Transform tankTurren, Vector3 turrentDirection, int label, string playerName, int whoViewID) {
        tankTurren.eulerAngles = new Vector3(tankTurren.eulerAngles.x, tankTurren.eulerAngles.y + 10, tankTurren.eulerAngles.z);
        // Instantiate<TankBullet>(Resources.Load<TankBullet>("Prefabs/Tank Bullet/Tank Radiating Bullet"), fireTransform.position, Quaternion.identity).Init(tankTurren, tankTurren.eulerAngles + new Vector3(-90.0f, 0, 0), 10.0f, 60.0f, 0.5f);
        PunObjectPool.Instance.GetLocalPool("Prefabs/Tank Bullet/Tank Radiating Bullet", "Tank Radiating Bullet", fireTransform.position, Quaternion.identity).GetComponent<TankBullet>().Init(label, playerName, whoViewID, tankTurren, tankTurren.eulerAngles + new Vector3(-90.0f, 0, 0), 10.0f, 40.0f, 1f);/*phải cộng -90 độ là do thằng xe thăng bị quay 1 góc -90 độ*/

        tankTurren.eulerAngles = new Vector3(tankTurren.eulerAngles.x, tankTurren.eulerAngles.y - 10, tankTurren.eulerAngles.z);
        // Instantiate<TankBullet>(Resources.Load<TankBullet>("Prefabs/Tank Bullet/Tank Radiating Bullet"), fireTransform.position, Quaternion.identity).Init(tankTurren, tankTurren.eulerAngles + new Vector3(-90.0f, 0, 0), 10.0f, 60.0f, 0.5f);
        PunObjectPool.Instance.GetLocalPool("Prefabs/Tank Bullet/Tank Radiating Bullet", "Tank Radiating Bullet", fireTransform.position, Quaternion.identity).GetComponent<TankBullet>().Init(label, playerName, whoViewID, tankTurren, tankTurren.eulerAngles + new Vector3(-90.0f, 0, 0), 10.0f, 40.0f, 1f);/*phải cộng -90 độ là do thằng xe thăng bị quay 1 góc -90 độ*/
        
        tankTurren.eulerAngles = new Vector3(tankTurren.eulerAngles.x, tankTurren.eulerAngles.y - 10, tankTurren.eulerAngles.z);
        // Instantiate<TankBullet>(Resources.Load<TankBullet>("Prefabs/Tank Bullet/Tank Radiating Bullet"), fireTransform.position, Quaternion.identity).Init(tankTurren, tankTurren.eulerAngles + new Vector3(-90.0f, 0, 0), 10.0f, 60.0f, 0.5f);
        PunObjectPool.Instance.GetLocalPool("Prefabs/Tank Bullet/Tank Radiating Bullet", "Tank Radiating Bullet", fireTransform.position, Quaternion.identity).GetComponent<TankBullet>().Init(label, playerName, whoViewID, tankTurren, tankTurren.eulerAngles + new Vector3(-90.0f, 0, 0), 10.0f, 40.0f, 1f);/*phải cộng -90 độ là do thằng xe thăng bị quay 1 góc -90 độ*/

    }
}
