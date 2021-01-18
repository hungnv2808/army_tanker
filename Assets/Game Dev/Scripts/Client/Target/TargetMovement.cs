using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMovement : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Rigidbody m_rigbody;
    [SerializeField] private Transform m_transform;
    public static int Count = 5;
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
        var targetMovement = GameObject.FindGameObjectWithTag("TargetMovement").transform.GetChild(0).gameObject;
        if (targetMovement != null) targetMovement.SetActive(true);
    }
    public static void Hiden() {
        var targetMovement = GameObject.FindGameObjectWithTag("TargetMovement");
        if (targetMovement != null) targetMovement.SetActive(false);
    }
}
