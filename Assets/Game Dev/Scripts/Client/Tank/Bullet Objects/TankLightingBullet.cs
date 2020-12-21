using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankLightingBullet : MonoBehaviour
{
    private string m_playerName;
    public Transform StartTransform;
    [HideInInspector] public int Label;
    public Transform EndTransform;
    [HideInInspector] public float Damage = 20;
    // Start is called before the first frame update
    void OnDisable() {
        StartTransform.position = Vector3.zero;
        EndTransform.position = Vector3.zero;
    }
    public string PlayerName {
        get {
            return m_playerName;
        }
        set {
            m_playerName = value;
        }
    }
}
