using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fast : MonoBehaviour, IItem 
{
    private WaitForEndOfFrame delay = new WaitForEndOfFrame();
    private float m_timer = 5.0f; 
    public void Excute() {
        m_timer = 5.0f;
        StartCoroutine(FastCoroutine());
    }
    public Sprite GetSprite() {
        return gameObject.GetComponent<SpriteRenderer>().sprite;
    }   
    public void Disable() {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<BoxCollider>().enabled = false;
    }
    private IEnumerator FastCoroutine() {
        yield return delay;
        m_timer -= Time.deltaTime; 
        if (m_timer <= 0) {
            yield break;
        } 
        else {
            if (TankCompetition.Instance != null) TankCompetition.Instance.X2Speed();
            StartCoroutine(FastCoroutine());
        } 
        
    }
}
