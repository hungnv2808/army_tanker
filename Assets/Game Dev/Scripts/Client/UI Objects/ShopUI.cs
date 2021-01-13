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
    [SerializeField] private DataTankChampion m_dataTankChampion;
    public DataTankChampion DataTankChampion {
        get {
            return this.m_dataTankChampion;
        }
    }
    [SerializeField] private Text m_currSpeedStatLabel;
    [SerializeField] private Text m_nextSpeedPriceLabel;
    [SerializeField] private Animator m_speedUpdateButtonAnimator;
    [SerializeField] private Text m_currHealthyStatLabel;
    [SerializeField] private Text m_nextHealthyPriceLabel;
    [SerializeField] private Animator m_healthyUpdateButtonAnimator;
    [SerializeField] private Text m_currDamageStatLabel;
    [SerializeField] private Text m_nextDamagePriceLabel;
    [SerializeField] private Animator m_damageUpdateButtonAnimator;
    private int m_currSpeedUpdatePrice = -1;
    private int m_currHealthyUpdatePrice = -1;
    private int m_currDamageUpdatePrice = -1;
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

        m_speedUpdateButtonAnimator.GetComponent<Button>().onClick.AddListener(() => {
            SoundManagement.Instance.PlaySoundClick();
            this.UpdateTankChampionStatHandle(0);
        });
        m_healthyUpdateButtonAnimator.GetComponent<Button>().onClick.AddListener(() => {
            SoundManagement.Instance.PlaySoundClick();
            this.UpdateTankChampionStatHandle(1);
        });
        m_damageUpdateButtonAnimator.GetComponent<Button>().onClick.AddListener(() => {
            SoundManagement.Instance.PlaySoundClick();
            this.UpdateTankChampionStatHandle(2);
        });
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
    public void HandleShowCurrentModel(int index) {
        if (m_indexModelShow >= 0) m_tankModels[m_indexModelShow].SetActive(false);
        m_indexModelShow = index;
        this.LoadDataTankChampion(index);
        if (PlayFabDatabase.Instance.TankerChampions[index].HasUnlock) {
            m_priceButton.gameObject.SetActive(false);
            PlayFabDatabase.Instance.IndexTankerChampionSelected = index;
            this.CheckTankChampionUpdateStat(index);
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
    private void LoadDataTankChampion(int index) {
        Debug.Log("index" + index);
        m_currSpeedStatLabel.text = m_dataTankChampion.Champions[index].MoveSpeed[PlayFabDatabase.Instance.TankerChampions[index].MoveSpeed].Stat + "";
        m_currHealthyStatLabel.text = m_dataTankChampion.Champions[index].Healthy[PlayFabDatabase.Instance.TankerChampions[index].Healthy].Stat + "";
        m_currDamageStatLabel.text = m_dataTankChampion.Champions[index].Damage[PlayFabDatabase.Instance.TankerChampions[index].Damage].Stat + "";

        if (PlayFabDatabase.Instance.TankerChampions[index].MoveSpeed + 1 < 3) {
            if (PlayFabDatabase.Instance.TankerChampions[index].HasUnlock) m_currSpeedUpdatePrice = m_dataTankChampion.Champions[index].MoveSpeed[PlayFabDatabase.Instance.TankerChampions[index].MoveSpeed + 1].Price;
            else m_currSpeedUpdatePrice = -1;
            m_nextSpeedPriceLabel.text = m_dataTankChampion.Champions[index].MoveSpeed[PlayFabDatabase.Instance.TankerChampions[index].MoveSpeed + 1].Price + "";
        }
        else {
            m_speedUpdateButtonAnimator.gameObject.SetActive(false);
            m_nextSpeedPriceLabel.text = "max";
            m_currSpeedUpdatePrice = -1;
        }

        if (PlayFabDatabase.Instance.TankerChampions[index].Healthy + 1 < 3) {
            if (PlayFabDatabase.Instance.TankerChampions[index].HasUnlock) m_currHealthyUpdatePrice = m_dataTankChampion.Champions[index].Healthy[PlayFabDatabase.Instance.TankerChampions[index].Healthy + 1].Price;
            else m_currHealthyUpdatePrice = -1;
            m_nextHealthyPriceLabel.text = m_dataTankChampion.Champions[index].Healthy[PlayFabDatabase.Instance.TankerChampions[index].Healthy + 1].Price + "";
        } 
        else {
            m_healthyUpdateButtonAnimator.gameObject.SetActive(false);
            m_nextHealthyPriceLabel.text = "max";
            m_currHealthyUpdatePrice = -1;
        }

        if (PlayFabDatabase.Instance.TankerChampions[index].Damage + 1 < 3) {
            if (PlayFabDatabase.Instance.TankerChampions[index].HasUnlock) m_currDamageUpdatePrice = m_dataTankChampion.Champions[index].Damage[PlayFabDatabase.Instance.TankerChampions[index].Damage + 1].Price;
            else m_currDamageUpdatePrice = -1;
            m_nextDamagePriceLabel.text = m_dataTankChampion.Champions[index].Damage[PlayFabDatabase.Instance.TankerChampions[index].Damage + 1].Price + "";
        } 
        else {
            m_damageUpdateButtonAnimator.gameObject.SetActive(false);
            m_nextDamagePriceLabel.text = "max";
            m_currDamageUpdatePrice = -1;
        }
    }
    private void CheckTankChampionUpdateStat(int index) {
        if (m_currSpeedUpdatePrice > 0 && CurrencyManagement.Instance.GoldStar >= m_currSpeedUpdatePrice) m_speedUpdateButtonAnimator.SetBool("HasEnoughCondition", true);
        else m_speedUpdateButtonAnimator.SetBool("HasEnoughCondition", false);

        if (m_currHealthyUpdatePrice > 0 && CurrencyManagement.Instance.GoldStar >= m_currHealthyUpdatePrice) m_healthyUpdateButtonAnimator.SetBool("HasEnoughCondition", true);
        else m_healthyUpdateButtonAnimator.SetBool("HasEnoughCondition", false);
        
        if (m_currDamageUpdatePrice > 0 && CurrencyManagement.Instance.GoldStar >= m_currDamageUpdatePrice) m_damageUpdateButtonAnimator.SetBool("HasEnoughCondition", true);
        else m_damageUpdateButtonAnimator.SetBool("HasEnoughCondition", false);
    }
    private void UpdateTankChampionStatHandle(int type) {
        switch (type) {
            case 0 :
                if (m_currSpeedUpdatePrice > 0 && CurrencyManagement.Instance.GoldStar >= m_currSpeedUpdatePrice ) {
                    PlayFabDatabase.Instance.TankerChampions[PlayFabDatabase.Instance.IndexTankerChampionSelected].MoveSpeed += 1;
                    CurrencyManagement.Instance.DecreaseGoldStar(m_currSpeedUpdatePrice);
                    this.UpdateCurrencyUI();
                    this.LoadDataTankChampion(PlayFabDatabase.Instance.IndexTankerChampionSelected);
                    this.CheckTankChampionUpdateStat(PlayFabDatabase.Instance.IndexTankerChampionSelected);
                }
                break;
            case 1:
                if (m_currHealthyUpdatePrice > 0 && CurrencyManagement.Instance.GoldStar >= m_currHealthyUpdatePrice ) {
                    PlayFabDatabase.Instance.TankerChampions[PlayFabDatabase.Instance.IndexTankerChampionSelected].Healthy += 1;
                    CurrencyManagement.Instance.DecreaseGoldStar(m_currHealthyUpdatePrice);
                    this.UpdateCurrencyUI();
                    this.LoadDataTankChampion(PlayFabDatabase.Instance.IndexTankerChampionSelected);
                    this.CheckTankChampionUpdateStat(PlayFabDatabase.Instance.IndexTankerChampionSelected);
                }
                    
                break;
            case 2:
                if (m_currDamageUpdatePrice > 0 && CurrencyManagement.Instance.GoldStar >= m_currDamageUpdatePrice ) {
                    PlayFabDatabase.Instance.TankerChampions[PlayFabDatabase.Instance.IndexTankerChampionSelected].Damage += 1;
                    CurrencyManagement.Instance.DecreaseGoldStar(m_currDamageUpdatePrice);
                    this.UpdateCurrencyUI();
                    this.LoadDataTankChampion(PlayFabDatabase.Instance.IndexTankerChampionSelected);
                    this.CheckTankChampionUpdateStat(PlayFabDatabase.Instance.IndexTankerChampionSelected);
                }
                    
                break;
        }
    }
}
