using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class TankBullet : MonoBehaviour, IBullet
{
    protected delegate void Callback();
    [SerializeField] protected Rigidbody m_rigidbody;
    [SerializeField] protected Transform m_transform;
    protected float m_launchForce;
    protected float m_damage;
    protected float m_lifeTime;
    protected Vector3 m_velocity;
    protected Robot m_RobotTakeDamage;
    protected int m_label;
    protected string m_whoDamage;
    protected int m_whoViewID;
    
    public virtual void Init(int label, string whoDamage, int whoViewID, Transform tankTurrent, Vector3 eulerAngle, float damage, float launchForce, float lifeTime) {
        m_whoDamage = whoDamage;
        m_whoViewID = whoViewID;
        m_label = label;
        m_transform.eulerAngles = eulerAngle;
        m_launchForce = launchForce;
        m_damage = damage;
        m_lifeTime = lifeTime;
        this.Move(tankTurrent);
        Invoke("Destroy", m_lifeTime);
    }
    public virtual void Move(Transform tankTurrent) {
        /* công thức tính vận tốc ném ngang : Vo = L x Sqrt( g / (2*H) )
        ** L: tầm ném xa của viên đạn
        ** H: chiều cao ném
        ** g: gia tốc rơi tự do 9,8m/s^2
        */
        m_velocity = tankTurrent.up * m_launchForce * (-1.0f) ;
        m_rigidbody.velocity = m_velocity;
    }
    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    public virtual void Destroy()
    {
        DestroyHandler(null);
    }
    protected virtual void DestroyHandler(Callback callback) {
        if (callback != null) callback();
        m_rigidbody.velocity = Vector3.zero;
        PunObjectPool.Instance.SetLocalPool(this.gameObject);
    } 
    public virtual void Explode() {
        GetEffect("Prefabs/Effect/BulletFatExplosionBlue", "BulletFatExplosionBlue");
    }
    protected void GetEffect(string resourcePath, string name) {
        CancelInvoke("Destroy");
        var eff = PunObjectPool.Instance.GetLocalPool(resourcePath, name, m_transform.position, Quaternion.identity);
        m_rigidbody.velocity = Vector3.zero;
        PunObjectPool.Instance.SetLocalPool(this.gameObject);
    }
    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    public virtual void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals("Team" + m_label)) {
            this.Explode();
        }
        if (other.tag.Equals("Team" + (1 - m_label))) {
            other.GetComponent<Tank>().ReduceBlood(this.m_damage, this.m_whoDamage, this.m_whoViewID);
        }
        //chạy effect
        // if (other.tag.Equals("SmallRobot")) {
        //     m_RobotTakeDamage = m_RobotTakeDamage ?? other.GetComponent<Robot>();
        //     m_RobotTakeDamage.ReduceBlood(this.m_damage);
        // } 
    }
}
