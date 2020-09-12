using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button m_avatar;
    [SerializeField] private Button m_dailyQuest;
    [SerializeField] private Button m_fightButton;
    [SerializeField] private Button m_achievementButton;
    [SerializeField] private Text m_playerNameLabel;
    [SerializeField] private GameObject m_avatarInventoryPanel;
    [SerializeField] private GameObject m_dailyQuestPanel;
    [SerializeField] private DailyQuestUI m_dailyQuestUI;
    [SerializeField] private GameObject m_achievementPanel;
    [SerializeField] private Button m_mailBoxButton;
    [SerializeField] private Button m_shopButton;
    [SerializeField] private Button m_closeShopButton;
    [SerializeField] private GameObject m_shopPanel;
    [SerializeField] private GameObject m_mailBoxPanel;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Animator m_displayModelAnimator;
    [SerializeField] private Text m_goldStarLabel;
    [SerializeField] private Text m_violetStarLabel;
    public RectTransform UI_GoldPosition;
    public RectTransform UI_DiamondPosition;
    private bool m_isOpenMailBox = false;
    private bool isAvatarClicked = false;
    private bool isDailyQuestClicked = false;
    private static MenuUI s_instance;
    public static MenuUI Instance {
        get {
            return s_instance;
        }
    }
    private void Awake() {
        if (s_instance != null && s_instance != this) {
            Destroy(this.gameObject);
        }
        s_instance = this;
    }
    private void OnEnable() {
        this.UpdateCurrencyUI();
    }
    private void Start() {
        m_avatar.onClick.AddListener(OnAvatarClick);
        m_dailyQuest.onClick.AddListener(OnDailyQuestClick);
        m_fightButton.onClick.AddListener(OnFightClick);
        m_achievementButton.onClick.AddListener(OnAchievementClick);
        m_mailBoxButton.onClick.AddListener(OnMailBoxClick);
        m_shopButton.onClick.AddListener(OnShopClick);
        m_closeShopButton.onClick.AddListener(OnCloseShopClick);
        this.ShowPlayerNameLabel(PlayFabDatabase.Instance.DisPlayName, PlayFabDatabase.Instance.PathAvatar);
        m_animator.SetBool("isOpenShop", false);
        m_animator.SetBool("isOpenSelectMap", false);
        m_displayModelAnimator.SetBool("isDisplayRight", false);
    }
    private void OnAvatarClick() {
        isAvatarClicked = !isAvatarClicked;
        if (isAvatarClicked) {
            m_avatarInventoryPanel.SetActive(true);
        } else {
            m_avatarInventoryPanel.SetActive(false);
        }
    }
    public void HideAvatarInventory() {
        this.OnAvatarClick();
    }
    public void OnDailyQuestClick() {
        isDailyQuestClicked = !isDailyQuestClicked;
        if (isDailyQuestClicked) {
            this.ShowDailyQuestPanel();
        } else {
            m_dailyQuestUI.HideDailyQuestPanel();
        }
    }
    private void OnAchievementClick() {
        m_achievementPanel.SetActive(true);
    }
    public void ShowPlayerNameLabel(string playerName, string pathAvatar) {
        m_playerNameLabel.text = playerName;
        this.AvatarLabel.sprite = Resources.Load<Sprite>(pathAvatar);
    }
    private void ShowDailyQuestPanel() {
        m_dailyQuestPanel.SetActive(true);
        m_dailyQuestUI.ShowDailyQuestPanel();
    }
    private void OnMailBoxClick() {
        m_isOpenMailBox = !m_isOpenMailBox;
        m_mailBoxPanel.SetActive(m_isOpenMailBox);
    }
    private void OnFightClick() {
        m_animator.SetBool("isOpenSelectMap", true);
        Invoke("LoadSelectMapScene", 0.7f);
    }
    private void LoadSelectMapScene() {
        SceneManager.LoadScene("Lobby Scene");
    }
    private void OnShopClick() {
        m_displayModelAnimator.SetBool("isDisplayRight", true);
        m_animator.SetBool("isOpenShop", true);
        // AnimatorHelper.RunActionSequence(m_animator, ShowShop);
    }
    private void OnCloseShopClick() {
        ShopUI.Instance.CloseClickHandle();
        m_animator.SetBool("isOpenShop", false);
        AnimatorHelper.RunActionSequence(m_animator, () => {
            m_displayModelAnimator.SetBool("isDisplayRight", false);
        });
    }
    private void ShowShop() {
        m_shopPanel.SetActive(true);
    }
    private void HiddenShop() {
        m_shopPanel.SetActive(false);
    }
    public void UpdateCurrencyUI() {
        m_goldStarLabel.text = CurrencyManagement.Instance.GoldStar + "";
        m_violetStarLabel.text = CurrencyManagement.Instance.VioletStar + "";
    }
    public GameObject DailyQuestPanel {
        get {
            return m_dailyQuestPanel;
        }
    }
    public Image AvatarLabel {
        get {
            return m_avatar.GetComponent<Image>();
        }
    }
}
