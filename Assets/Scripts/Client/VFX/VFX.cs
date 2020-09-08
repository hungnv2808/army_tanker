using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX : MonoBehaviour
{
    [SerializeField] private float m_lifeTime;
    private void OnEnable() {
        Invoke("Disable", m_lifeTime);
    }
    public void Disable() {
        PunObjectPool.Instance.SetLocalPool(this.gameObject);
    }
}
