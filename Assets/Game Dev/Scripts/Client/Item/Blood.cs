using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour,IItem
{
    public void Excute() {
        if (TankCompetition.Instance != null) TankCompetition.Instance.RefreshHealthy();
    }
    public Sprite GetSprite() {
        return gameObject.GetComponent<SpriteRenderer>().sprite;
    }  
    public void Disable() {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<BoxCollider>().enabled = false;
    }
}
