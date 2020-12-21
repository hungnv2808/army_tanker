using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RobotBullet : TankBullet
{
    protected Tank m_tankTakeDamage;
    public void OnEnable() {
        m_damage = 5.0f;
        m_launchForce = 70.0f;
    }
    // public override void Init(Transform direction, Vector3 eulerAngle) {
    //    base.Init(direction, eulerAngle);
    // }
    public override void OnTriggerEnter(Collider other) {
        this.Explode();
        if (other.tag.Equals("Player")) {
            m_tankTakeDamage = m_tankTakeDamage ?? other.GetComponent<Tank>();
            // m_tankTakeDamage.ReduceBlood(m_damage);
        }
    }
}
