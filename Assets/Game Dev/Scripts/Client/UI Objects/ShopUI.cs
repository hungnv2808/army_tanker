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
    [SerializeField] private SpriteRenderer m_currentAssistanceSkill;
    [SerializeField] private GameObject m_comingSoonLabel;
    [SerializeField] private Button m_priceButton;
    [SerializeField] private Text m_priceLabel;
    [SerializeField] private Text m_goldStarLabel;
    [SerializeField] private Text m_violetStarLabel;
    private int m_indexModelShow = -1;
    private int m_indexAssistanceSkillShow = -1;
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
    private void OnEnable() {
        this.UpdateCurrencyUI();
    }
    private void Start() {
        this.AddEventItemsShop();
        this.AddEventItemsAssistanceSkill();
        m_indexAssistanceSkillShow = PlayFabDatabase.Instance.IndexAssistanceSkillSelected;
        m_indexModelShow = PlayFabDatabase.Instance.IndexTankerChampionSelected;
        m_tankModels[PlayFabDatabase.Instance.IndexTankerChampionSelected].SetActive(true);
        this.HandleShowCurrentAssistanceSkill(PlayFabDatabase.Instance.IndexAssistanceSkillSelected);

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
                SoundManagement.Instance.PlaySoundClick();
                HandleShowCurrentModel(0);
            }
        });
        m_itemsShop[1].onClick.AddListener(() => {
            if (m_indexModelShow != 1) {
                SoundManagement.Instance.PlaySoundClick();
                HandleShowCurrentModel(1);
            }
        });
        m_itemsShop[2].onClick.AddListener(() => {
            if (m_indexModelShow != 2) {
                SoundManagement.Instance.PlaySoundClick();
                HandleShowCurrentModel(2);
            }
        });
        for (int i = 3; i < m_itemsShop.Length; i++)
        {
            m_itemsShop[i].onClick.AddListener(() => {
                if (m_indexModelShow != -1) {
                    SoundManagement.Instance.PlaySoundClick();
                    for (int j = 0; j < m_tankModels.Length; j++)
                    {
                        m_tankModels[j].SetActive(false);
                    }
                    m_priceButton.gameObject.SetActive(false);
                    m_comingSoonLabel.SetActive(true);
                    m_indexModelShow = -1;
                }
            });
        }
    }
    private void AddEventItemsAssistanceSkill() {
        m_itemsSkin[0].onClick.AddListener(() => {
            if (m_indexAssistanceSkillShow != 0) {
                SoundManagement.Instance.PlaySoundClick();
                this.HandleShowCurrentAssistanceSkill(0);
            }
        });
        m_itemsSkin[1].onClick.AddListener(() => {
            if (m_indexAssistanceSkillShow != 1) {
                SoundManagement.Instance.PlaySoundClick();
                this.HandleShowCurrentAssistanceSkill(1);
            }
        });
        m_itemsSkin[2].onClick.AddListener(() => {
            if (m_indexAssistanceSkillShow != 2) {
                SoundManagement.Instance.PlaySoundClick();
                this.HandleShowCurrentAssistanceSkill(2);
            }
        });
        m_itemsSkin[3].onClick.AddListener(() => {
            if (m_indexAssistanceSkillShow != 3) {
                SoundManagement.Instance.PlaySoundClick();
                this.HandleShowCurrentAssistanceSkill(3);
            }
        });
    }
    private void HandleShowCurrentAssistanceSkill(int index) {
        if (PlayFabDatabase.Instance.AssistanceSkills[index].HasUnlock) {
            m_indexAssistanceSkillShow = index;
            PlayFabDatabase.Instance.IndexAssistanceSkillSelected = index;
            m_currentAssistanceSkill.sprite = m_itemsSkin[index].image.sprite;
        }
        
    }
    private void HandleShowCurrentModel(int index) {
        if (m_indexModelShow >= 0) m_tankModels[m_indexModelShow].SetActive(false);
        m_indexModelShow = index;
        if (PlayFabDatabase.Instance.TankerChampions[index].HasUnlock) {
            m_priceButton.gameObject.SetActive(false);
            PlayFabDatabase.Instance.IndexTankerChampionSelected = index;
        } else {
            m_priceButton.gameObject.SetActive(true);
            m_priceLabel.text = "" + PlayFabDatabase.Instance.TankerChampions[index].Price;
        }
        m_comingSoonLabel.SetActive(false);
        m_tankModels[index].SetActive(true);
        
    }
    public void CloseClickHandle() {
        if (m_indexModelShow != PlayFabDatabase.Instance.IndexTankerChampionSelected) {
            if (m_indexModelShow >= 0) m_tankModels[m_indexModelShow].SetActive(false);
            m_comingSoonLabel.SetActive(false);
            m_priceButton.gameObject.SetActive(false);
            m_tankModels[PlayFabDatabase.Instance.IndexTankerChampionSelected].SetActive(true);
            m_indexModelShow = PlayFabDatabase.Instance.IndexTankerChampionSelected;
        }
    }
    public void UpdateCurrencyUI() {
        m_goldStarLabel.text = CurrencyManagement.Instance.GoldStar + "";
        m_violetStarLabel.text = CurrencyManagement.Instance.VioletStar + "";
    }
}
