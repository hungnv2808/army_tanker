using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyMovement : MonoBehaviour
{
    private Vector3 m_target;
    [SerializeField] private bool m_isGold;
    [SerializeField] private bool m_isDiamond;
    private float m_timer;
    private float m_lerpTime = 0.5f;
    private Vector3 m_posStart;
    [SerializeField] private Transform m_transform;
    private void OnEnable() {
        m_timer = 0;
        if (m_isGold && m_isDiamond) {
            m_isDiamond = false;
        }
        if (m_isGold) {
            m_target = MenuUI.Instance.UI_GoldPosition.position;
        }
        if (m_isDiamond) {
            m_target = MenuUI.Instance.UI_DiamondPosition.position;
        }
    }
    public void Init() {
        m_posStart = this.transform.position;
        StartCoroutine(MoveCoroutine());
    }
    private IEnumerator MoveCoroutine() {
        m_timer += Time.deltaTime;
        if (m_timer > m_lerpTime) {
            m_timer = m_lerpTime;
            m_transform.position = Vector3.Lerp(m_posStart, m_target, m_timer/m_lerpTime);
            m_transform.localScale = Vector3.one * Mathf.Lerp(1.0f, 0.3f, m_timer/m_lerpTime);
            GUIPool.Instance.SetLocalPool(this.gameObject);
        } else {
            m_transform.position = Vector3.Lerp(m_posStart, m_target, m_timer/m_lerpTime);
            m_transform.localScale = Vector3.one * Mathf.Lerp(1.0f, 0.3f, m_timer/m_lerpTime);
            yield return null;
            StartCoroutine(MoveCoroutine());
        }
    }
}
