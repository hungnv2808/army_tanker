using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFlight : MonoBehaviour
{
    [SerializeField] private Rigidbody m_rigbody;
    [SerializeField] private Transform m_transform;
    public static int Count = 4;
    void Start()
    {
        this.Reciprocate();
    }
    // xoay chiều đi
    private void Reciprocate() {
        m_transform.localEulerAngles = new Vector3(0, m_transform.localEulerAngles.y + 180, 0);
        m_rigbody.velocity =  m_transform.forward * 10.0f;
    }
    private void OnTriggerEnter(Collider other) {
        if (other.tag.Equals("Bound")) {
            this.Reciprocate();
        }
    }
    public void Destroy() {
        Destroy(this.gameObject);
    }
    public static void CheckCount() {
        Count -= 1;
    }
    public static void Show() {
        var targetFlight = GameObject.FindGameObjectWithTag("TargetFlight");
        if (targetFlight != null) targetFlight.SetActive(true);
    }
    public static void Hiden() {
        var targetFlight = GameObject.FindGameObjectWithTag("TargetFlight");
        if (targetFlight != null) targetFlight.SetActive(false);
    }
}
