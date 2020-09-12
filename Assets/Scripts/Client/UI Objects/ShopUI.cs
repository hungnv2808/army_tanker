using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShopUI : MonoBehaviour
{
    [SerializeField] private Button[] m_itemsShop;
    [SerializeField] private Button[] m_itemsUpdate;
    [SerializeField] private Button[] m_itemsSkin;
    [SerializeField] private GameObject[] m_tankModels;
    [SerializeField] private GameObject m_comingSoonLabel;
    private int m_indexModelShow;
    private static ShopUI s_instance;
    public static ShopUI Instance {
        get {
            return s_instance;
        }
    }
    private void Awake() {
        if (s_instance != null && s_instance != this) {
            Destroy(s_instance);
        }
        s_instance = this;
    }
    private void Start() {
        this.AddEventItemsShop();
        m_tankModels[PlayFabDatabase.Instance.IndexTankerChampionSelected].SetActive(true);
        
        for (int i = 0; i < PlayFabDatabase.Instance.TankerChampions.Count; i++)
        {
            if (PlayFabDatabase.Instance.TankerChampions[i].HasUnlock)
                this.m_itemsShop[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        for (int i = 0; i < PlayFabDatabase.Instance.AssistanceSkills.Count; i++)
        {
            if (PlayFabDatabase.Instance.AssistanceSkills[i].HasUnlock)
                this.m_itemsSkin[i].transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    private void AddEventItemsShop() {
        m_itemsShop[0].onClick.AddListener(() => {
            if (m_indexModelShow != 0) {
                HandleShowCurrentModel(0);
            }
        });
        m_itemsShop[1].onClick.AddListener(() => {
            if (m_indexModelShow != 1) {
                HandleShowCurrentModel(1);
            }
        });
        m_itemsShop[2].onClick.AddListener(() => {
            if (m_indexModelShow != 2) {
                HandleShowCurrentModel(2);
            }
        });
        for (int i = 3; i < m_itemsShop.Length; i++)
        {
            m_itemsShop[i].onClick.AddListener(() => {
                if (m_indexModelShow != 3) {
                    for (int j = 0; j < m_tankModels.Length; j++)
                    {
                        m_tankModels[j].SetActive(false);
                    }
                    m_comingSoonLabel.SetActive(true);
                    m_indexModelShow = 3;
                }
            });
        }
    }

    private void HandleShowCurrentModel(int index) {
        m_indexModelShow = index;
        if (PlayFabDatabase.Instance.TankerChampions[index].HasUnlock) {
            PlayFabDatabase.Instance.IndexTankerChampionSelected = index;
        }
        
        for (int j = 0; j < m_tankModels.Length; j++)
        {
            m_tankModels[j].SetActive(false);
        }
        m_comingSoonLabel.SetActive(false);
        m_tankModels[index].SetActive(true);
    }
    public void CloseClickHandle() {
        for (int j = 0; j < m_tankModels.Length; j++)
        {
            m_tankModels[j].SetActive(false);
        }
        m_comingSoonLabel.SetActive(false);
        m_tankModels[PlayFabDatabase.Instance.IndexTankerChampionSelected].SetActive(true);
    }
}
