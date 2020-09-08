using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
public class Lv5Turrent : AbTurrent
{
    /*
    layerMask = 1 << LayerMask.NameToLayer ("layerX"); // only check for collisions with layerX
 
    layerMask = ~(1 << LayerMask.NameToLayer ("layerX")); // ignore collisions with layerX
 
    LayerMask layerMask = ~(1 << LayerMask.NameToLayer ("layerX") | 1 << LayerMask.NameToLayer ("layerY")); // ignore both layerX and layerY
    */
    
    
    private float m_curCooldown = 0.0f;
    RaycastHit hit;
    private Transform m_fireTransform;
    private Transform m_tankTurrent;
    private TankLightingBullet m_lightingObject;
    private Transform m_lightingTransform;
    private Transform m_lightingEffectTransform;
    private Transform m_lightingChildEffectTransform;
    public bool m_isShooted = false;
    private int m_layerMask;
    private string m_whoDamage;
    private int m_whoViewID;
    private Collider m_hitCollider;

    
    private void Start() {
        MaxCooldown = 0.2f;
        m_TankParentScript = GetComponentInParent<Tank>();
        m_lightingObject = PunObjectPool.Instance.GetLocalPool("Prefabs/Tank Bullet/Tank Lightning Bullet", "Tank Lightning Bullet",Vector3.zero, Quaternion.identity).GetComponent<TankLightingBullet>();
        m_lightingTransform = m_lightingObject.gameObject.transform;
        m_lightingEffectTransform = PunObjectPool.Instance.GetLocalPool("Prefabs/Tank Bullet/LightningOrbBlue", "LightningOrbBlue", Vector3.zero, Quaternion.identity).transform;
        m_lightingChildEffectTransform = m_lightingEffectTransform.GetChild(0);
        m_energyConsumption = 50.0f;
        this.HideLazer();
        
        m_layerMask = ~(1 << LayerMask.NameToLayer("HealField") | 1 << LayerMask.NameToLayer("Player Bullet") | 1 << LayerMask.NameToLayer("Grass")); // bỏ qua thằng layer heal field
    }
    public override void ShootAndSync(int lvTurrent, Transform fireTransform, Transform tankTurren, Vector3 turrentDirection, int label, string whoDamage, int whoViewID) {
        if (!this.CheckEnergy()) return;
        m_whoDamage = whoDamage;
        m_whoViewID = whoViewID;
        m_fireTransform = fireTransform;
        m_tankTurrent = tankTurren;
        if (!m_isShooted) {
            this.ShowLazer(label);
        }
        m_curCooldown -= Time.deltaTime;
        if (m_curCooldown <= 0) {
            m_TankParentScript.SendDispatchShootedNoCountdown(TankEvent.EVENT_SEND_DISPATCH_TURRENT_SHOOTED_NO_COUNTDOWN, lvTurrent, true);
            m_curCooldown = MaxCooldown;
        }
    }
    public override void Shoot(Transform fireTransform, Transform tankTurren, Vector3 turrentDirection, bool isShooted, int label, string whoDamage, int whoViewID) {
        m_whoDamage = whoDamage;
        m_whoViewID = whoViewID;
        m_fireTransform = fireTransform;
        m_tankTurrent = tankTurren;
        if (isShooted)
            this.ShowLazer(label);
        else {
            this.HideLazer();
        }

    }
    public void ShowLazer(int label) {
        m_isShooted = true;
        m_lightingObject.Label = label;
        m_lightingObject.gameObject.SetActive(true);
        m_lightingObject.StartTransform.localPosition = Vector3.zero;
        m_lightingEffectTransform.gameObject.SetActive(true);
        Invoke("StopShooted", 0.5f);
    }
    public void StopShooted() {
        if (m_isShooted) {
            this.HideLazer();
            m_TankParentScript.SendDispatchShootedNoCountdown(TankEvent.EVENT_SEND_DISPATCH_TURRENT_SHOOTED_NO_COUNTDOWN_STOP, 5, false);
        }
    }
    public void HideLazer() {
        m_lightingObject.gameObject.SetActive(false);
        m_lightingEffectTransform.gameObject.SetActive(false);
        m_isShooted = false;
    }
    private void Update() {
        if (!m_isShooted) {
            return;
        }
        try {
            m_lightingTransform.position = m_fireTransform.position;
            if (Physics.Raycast(m_fireTransform.position, m_tankTurrent.up * (-1.0f), out hit, 40.0f, m_layerMask)) {
                m_lightingObject.EndTransform.position = hit.point;
                m_lightingEffectTransform.position = hit.point;
                // if (m_LazerEffect.isStopped) {
                m_lightingChildEffectTransform.gameObject.SetActive(true);
                m_lightingChildEffectTransform.GetComponent<ParticleSystem>().Play();
                m_lightingEffectTransform.localScale = Vector3.one * 5;

                m_hitCollider = hit.collider;
                Debug.Log("collider.tag" + m_hitCollider.tag);
                Debug.Log("Team" + (1 - m_lightingObject.Label));
                if (m_hitCollider.tag.Equals("Team" + (1 - m_lightingObject.Label))) {
                    Debug.Log("Tru mau");
                    m_hitCollider.GetComponent<Tank>().ReduceBlood(m_lightingObject.Damage, m_whoDamage, m_whoViewID);
                }
                // }
            } else {
                m_lightingObject.EndTransform.position = m_tankTurrent.up * (-1.0f) * 30.0f + m_lightingTransform.position;
                m_lightingEffectTransform.position = m_lightingObject.EndTransform.position;
                m_lightingEffectTransform.localScale = Vector3.one * 3;
                // if (m_LazerEffect.isPlaying) {
                    m_lightingChildEffectTransform.GetComponent<ParticleSystem>().Stop();
                    m_lightingChildEffectTransform.gameObject.SetActive(false);
                // }
            }
        } catch (Exception error) {
            
        }
        

    }

}
