using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private Target m_targetNext;
    [SerializeField] private Transform m_transform;
    public int m_index;
    public static List<Target> StaticTargets = new List<Target>();
    public static int Count = 3;
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
    public static void CheckCount() {
        Count -= 1;
    }
    public void Show() {
        m_transform.localEulerAngles = new Vector3(0,90,0);
    }
    public IEnumerator ShowNext(Target targetNext) {
        yield return new WaitForSeconds(2.0f);
        if (targetNext != null) targetNext.Show();
        Destroy(this.gameObject);
    }
    public void Destroy() {
        StartCoroutine(ShowNext(m_targetNext));
    }
    public static void Hiden() {
        var targetStatic = GameObject.FindGameObjectWithTag("TargetStatic");
        if (targetStatic != null) targetStatic.SetActive(false);
    }
}
