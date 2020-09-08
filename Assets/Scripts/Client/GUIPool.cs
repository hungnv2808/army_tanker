using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIPool : MonoBehaviour
{
    private static GUIPool s_instance;
    public static GUIPool Instance {
        get {
            return s_instance;
        }
    }
    [SerializeField] private string[] m_GUIPoolNames;
    private Dictionary<string, HashSet<GameObject>> m_pools;
    [SerializeField] private Transform m_objectParent;
    void Awake() {
        if (s_instance != null && s_instance != this) {
            Destroy(s_instance.gameObject);
        }
        s_instance = this;
    }
    private void Start() {
        m_pools = new Dictionary<string, HashSet<GameObject>>();
        for (int i = 0; i < m_GUIPoolNames.Length; i++)
        {
            m_pools.Add(m_GUIPoolNames[i], new HashSet<GameObject>());
        }
    }
    public GameObject GetLocalPool(string resourcePath, string name) {
        if (m_pools.ContainsKey(name)) {
            foreach (var i in m_pools[name])
            {
                if (!i.activeSelf) {
                    i.SetActive(true);
                    return i;
                }
            }
            var obj = Instantiate(Resources.Load<GameObject>(resourcePath));
            m_pools[name].Add(obj);
            obj.transform.SetParent(m_objectParent);
            return obj;
        } else {
            Debug.LogError("Not exist name object pool :(");
            return null;
        }
    }
    public void SetLocalPool(GameObject obj) {
        obj.SetActive(false);
        obj.transform.position = Vector3.zero;
        obj.transform.localScale = Vector3.one;
    }
   
}
