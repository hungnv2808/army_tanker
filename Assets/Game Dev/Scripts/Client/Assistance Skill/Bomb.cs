using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private MeshRenderer m_meshRender;
    private GameObject m_effectExploision;
    private int m_collisionCount;
    private Transform m_transform;
    private float m_damage = 50.0f;
    private int m_whichTeam;
    private string m_whoseBomb;
    private int m_viewID;
    private Rigidbody m_rig;
    private void OnEnable() {
        m_meshRender.enabled = true;
        m_transform = m_transform ?? transform;
        m_rig = m_rig ?? gameObject.GetComponent<Rigidbody>();
        m_collisionCount = 1;
        m_damage = 1.5f * Tank.LocalPlayerInstance.GetComponent<Tank>().Damage;
    }
    public void InfoBomb(string whoseBomb, int whichTeam, int viewID) {
        this.m_whoseBomb = whoseBomb;
        this.m_whichTeam = whichTeam;
        this.m_viewID = viewID;
    }
    private void OnCollisionEnter(Collision other) {
        
        m_collisionCount -= 1;
        if (m_collisionCount < 0) return;
        m_rig.velocity = Vector3.zero;
        m_meshRender.enabled = false;
        m_effectExploision = PunObjectPool.Instance.GetLocalPool("Prefabs/Effect/Bomb Explosion", "Bomb Explosion", m_transform.position, Quaternion.identity);
        CameraFollow.Instance.Shake(0.3f, 1f);
        this.ReduceEnemyBlood();
            //TODO: xử lý mất máu người chơi
        
        Invoke("Disable", 1.2f);
    }
    // private void OnDrawGizmos() {
    //     Gizmos.DrawWireSphere(m_transform.position, 9.0f);
    // }
    private void ReduceEnemyBlood() {
        var enemies = Physics.OverlapSphere(m_transform.position, 9.0f);
        for (int i=0; i<enemies.Length; i++) 
            if (enemies[i].tag.Equals("Team" + (1-m_whichTeam))) {
                enemies[i].GetComponent<Tank>().ReduceBlood(m_damage, m_whoseBomb, m_viewID);
            }
    }
    private void Disable() {
        PunObjectPool.Instance.SetLocalPool(m_effectExploision);
        gameObject.SetActive(false);
    }
    public Rigidbody Rigidbody {
        set {
            m_rig = value;
        }
        get {
            return m_rig;
        }
    }
}
