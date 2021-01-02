using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManagement : MonoBehaviour
{
    private static CurrencyManagement m_instance;
    public static CurrencyManagement Instance {
        get {
            return m_instance;
        }
    }
    public int GoldStar = 0;
    private List<CurrencyMovement> m_golds;
    public int VioletStar = 0;
    private void Awake() {
        if (m_instance != null && m_instance != this) {
            Destroy(this.gameObject);
        }
        m_instance = this;
        DontDestroyOnLoad(this.gameObject);
        m_golds = new List<CurrencyMovement>();
    }
    public void DecreaseGoldStar(int count) {
        this.GoldStar -= count;
        MenuUI.Instance.UpdateCurrencyUI();
    }
    public void DecreaseVioletStar(int count) {
        this.VioletStar -= count;
        MenuUI.Instance.UpdateCurrencyUI();
    }
    public void IncreaseGoldStar(int count) {
        this.GoldStar += count;
        MenuUI.Instance.UpdateCurrencyUI();
    }
    public void IncreaseVioletStar(int count) {
        this.VioletStar += count;
        MenuUI.Instance.UpdateCurrencyUI();
    }
    public IEnumerator EffectGold() {
        m_golds.Clear();
        for (int i = 0; i < 30; i++)
        {
            // var gold = Instantiate(Resources.Load<CurrencyMovement>("Prefabs/gold"));
            var gold = GUIPool.Instance.GetLocalPool("Prefabs/gold", "gold").GetComponent<CurrencyMovement>();
            gold.transform.SetParent(MenuUI.Instance.transform);
            gold.transform.localScale = Vector3.one;
            gold.transform.localPosition = this.RandomCircle(new Vector3(260,-26,0));
            m_golds.Add(gold);
            yield return new WaitForSeconds(0.03f);
        }
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < m_golds.Count; i++)
        {
            m_golds[i].Init();
            yield return new WaitForSeconds(0.02f);
        }
    }
    public IEnumerator EffectGoldAndDiamond() {
        int a = 0;
        m_golds.Clear();
        for (int i = 0; i < 30; i++)
        {
            if (a != 0) {
                a = 0;
                //sinh vang
                var gold = GUIPool.Instance.GetLocalPool("Prefabs/gold", "gold").GetComponent<CurrencyMovement>();
                gold.transform.SetParent(MenuUI.Instance.transform);
                gold.transform.localScale = Vector3.one;
                gold.transform.localPosition = this.RandomCircle(new Vector3(260,-26,0));
                m_golds.Add(gold);
            } else {
                a = 1;
                //sinh kimcuong
                var diamond = GUIPool.Instance.GetLocalPool("Prefabs/diamond", "diamond").GetComponent<CurrencyMovement>();
                diamond.transform.SetParent(MenuUI.Instance.transform);
                diamond.transform.localScale = Vector3.one;
                diamond.transform.localPosition = this.RandomCircle(new Vector3(260,-26,0));
                m_golds.Add(diamond);
            }
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < m_golds.Count; i++)
        {
            m_golds[i].Init();
            yield return new WaitForSeconds(0.01f);
        }
    }
    Vector3 RandomCircle (Vector3 center){
        float ang = Random.value * 360;
        float radius = Random.value * 100;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = 0;
        return pos;
    }
}
