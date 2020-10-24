using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCompetition : MonoBehaviour
{
    [SerializeField] private Rigidbody m_rigidbody;

    public static void Spawn(Vector3 position, Vector3 velocity) {
        var _bullet = Instantiate<BulletCompetition>(Resources.Load<BulletCompetition>("Prefabs/Bullet"), position, Quaternion.identity);
        _bullet.m_rigidbody.AddForce(velocity);
    }
    private void OnTriggerEnter(Collider other) {
        Destroy(this.gameObject);
    }
}
