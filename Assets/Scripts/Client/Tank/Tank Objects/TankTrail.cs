using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankTrail : MonoBehaviour
{
    private Transform m_target;
    [SerializeField] private Transform m_transform;
    // Update is called once per frame
    void Update()
    {
        if (m_target != null)  m_transform.position = m_target.position;
    }
    public void Init(Transform transform) {
        this.m_target = transform;
    }
}
