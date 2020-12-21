using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthyBar : MonoBehaviour
{
    protected float m_timer = 0.0f;
    protected float m_lerpTimer = 0.5f;
    protected float m_lerpRatio;
    [SerializeField] protected Transform m_transform;
    protected Transform m_cameraTransform;
    [SerializeField] protected SpriteRenderer m_barSprite;
    protected float m_originalHeathyBarWidth;
    public virtual void OnEnable() {
        m_originalHeathyBarWidth = m_barSprite.size.x;
        m_cameraTransform = Camera.main.transform;
    }
    protected void Update() {
        m_transform.LookAt(m_cameraTransform);
    }
    // Start is called before the first frame update
    public virtual void FillUpMaxHealthyBar() {
        m_barSprite.size = new Vector2(m_originalHeathyBarWidth, m_barSprite.size.y);
    }
    public virtual void MinHealthy() {
        m_barSprite.size = new Vector2(0, m_barSprite.size.y);
    }
    public virtual void SetCurrentHealthy(float curHealthy, float maxHealthy) {
        m_barSprite.size = new Vector2(m_originalHeathyBarWidth * curHealthy / maxHealthy, m_barSprite.size.y);
    }
    public virtual void ReduceHealthyBar(float damage, float healthy) {
        m_barSprite.size = new Vector2(m_barSprite.size.x - m_originalHeathyBarWidth * damage / healthy, m_barSprite.size.y);
    }
    public virtual void IncreaseHealthyBar(float increaseSum, float healthy) {
        m_barSprite.size = new Vector2(m_barSprite.size.x + m_originalHeathyBarWidth * increaseSum / healthy, m_barSprite.size.y);
    } 
    /// <summary>
    /// hàm tween giảm máu
    /// </summary>
    /// <param name="x"> là phần width của sprite bị giảm đi</param>
    /// <param name="y"> là width của sprite ở hiện tại</param>
    /// <returns></returns>
    protected IEnumerator ReduceHealthyBarCoroutine(float x, float y) {
        Debug.Log("#####ReduceHealthyBarCoroutine");
        m_timer += Time.deltaTime;
        if (m_timer > m_lerpTimer) m_timer = m_lerpTimer;
        m_lerpRatio = m_timer / m_lerpTimer;
        m_barSprite.size = new Vector2(Mathf.Lerp(m_barSprite.size.x, y - x, m_lerpRatio) ,m_barSprite.size.y);
        if (m_lerpRatio >= 1) yield break;
        yield return null;
        StartCoroutine(ReduceHealthyBarCoroutine(x, y));
    }

}
