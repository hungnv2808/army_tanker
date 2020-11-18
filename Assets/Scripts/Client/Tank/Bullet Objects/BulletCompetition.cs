using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCompetition : MonoBehaviour
{
    [SerializeField] private Rigidbody m_rigidbody;
    [SerializeField] private Transform m_transform;

    public static void Spawn(Vector3 position, Vector3 velocity) {
        var _bullet = Instantiate<BulletCompetition>(Resources.Load<BulletCompetition>("Prefabs/Bullet"), position, Quaternion.identity);
        _bullet.m_rigidbody.AddForce(velocity);
    }
    private void OnTriggerEnter(Collider other) {
        if (other.tag.Equals("Target")) {
            var target = other.gameObject.GetComponent<Target>();
            target.Destroy();
            CompetitionUI.Instance.ChangeTextNotiLabel(true, target.m_index);
        } else {
            CompetitionUI.Instance.ChangeTextNotiLabel(false, 0);
        }
        Instantiate(Resources.Load("Prefabs/Effect/ExplosionFireballFire"), m_transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
