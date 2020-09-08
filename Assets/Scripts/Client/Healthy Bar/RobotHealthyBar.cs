using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotHealthyBar : HealthyBar
{
    private Transform m_cameraTransform;
    [SerializeField] private Transform m_transform;
    //TODO : xử lý text máu ở đây và các effect khi bị mất máu
    public override void OnEnable() {
        base.OnEnable();
        m_cameraTransform = Camera.main.transform;
    }
    private void LateUpdate() {
        m_transform.LookAt(m_cameraTransform.position);
    }
}
