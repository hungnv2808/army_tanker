using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankLazerBullet : MonoBehaviour
{
    [SerializeField] private Texture[] m_textures;
    [SerializeField] private LineRenderer m_lineRender;
    private Vector3 m_startPosition;
    private Vector3 m_endPosition;
    private float m_timer;
    private float m_lerpTime = 0.1f;
    // Start is called before the first frame update
    private void Launch() {
        m_timer = 0;
        m_lineRender.SetPosition(1, Vector3.one * 6);
        m_startPosition = m_lineRender.GetPosition(0);
        m_endPosition = m_lineRender.GetPosition(1);
    }
    
    void OnDisable() {
        m_lineRender.SetPosition(1, Vector3.zero);
        CancelInvoke();
    }
}
