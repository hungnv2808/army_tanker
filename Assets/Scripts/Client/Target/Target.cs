using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private Target m_targetNext;
    [SerializeField] private Transform m_transform;
    public int m_index;
    public static List<Target> StaticTargets = new List<Target>();
    private Callback m_moveHandle;
    // Start is called before the first frame update
    private void Start() {
        StaticTargets.Add(this);
        Debug.Log(StaticTargets.Count);
    }
    public static void ShowFirstStaticTarget() {
        for (int i = 0; i < StaticTargets.Count; i++)
        {
            if (StaticTargets[i].m_index == 1) StaticTargets[i].Show();
        }
        
    }
    public void Show() {
        m_transform.localEulerAngles = new Vector3(0,90,0);
    }
    public IEnumerator ShowNext(Target targetNext) {
        yield return new WaitForSeconds(2.0f);
        if (targetNext != null) targetNext.Show();
        Destroy(this.gameObject);
    }
    public void BeginMoving()
    {
        m_moveHandle = Move;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_moveHandle != null) m_moveHandle();
    }
    private void Move() {

    }
    public void Destroy() {
        StartCoroutine(ShowNext(m_targetNext));
    }
}
