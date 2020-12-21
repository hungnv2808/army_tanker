﻿using System.Collections;
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
    [SerializeField] private Button m_CompetitionButton;
    [SerializeField] private Button m_playButton;
    [SerializeField] private GameObject m_shopPanel;
    [SerializeField] private GameObject m_mailBoxPanel;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Animator m_displayModelAnimator;
    [SerializeField] private GameObject m_displayModelCompetition;
    [SerializeField] private GameObject m_competitionPanel;
    [SerializeField] private Text m_goldStarLabel;
    [SerializeField] private Text m_violetStarLabel;
    public RectTransform UI_GoldPosition;
    public RectTransform UI_DiamondPosition;
    private bool m_isOpenMailBox = false;
    private bool isAvatarClicked = false;
    private bool isDailyQuestClicked = false;
    private static MenuUI s_instance;
    [SerializeField] private RectTransform m_listCompetitionRankContent;
    [SerializeField] private GameObject m_celTemplate; 
    [SerializeField] private GameObject m_resultCompetitionPanel;
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
        m_CompetitionButton.onClick.AddListener(OnCompetitionClick);
        m_playButton.onClick.AddListener(OnPlay);
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
    private void OnCompetitionClick() {
        m_competitionPanel.SetActive(true);
        m_displayModelCompetition.SetActive(true);
        m_displayModelAnimator.gameObject.SetActive(false);
    }
    private void OnPlay() {
        // SceneManager.LoadScene("LoadMapCompetition Scene");
        this.ShowListCompetitionRank();
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
    private List<Transform> m_listCompetitionRank;
    public void ShowListCompetitionRank() {
        m_competitionPanel.SetActive(false);
        m_resultCompetitionPanel.SetActive(true);
        PlayFabDatabase.Instance.GetLeaderboard(0);
        m_listCompetitionRank = m_listCompetitionRank ?? new List<Transform>();
        InvokeRepeating("UpdateLeaderboard", 0, 2f);
    }
    public void UpdateLeaderboard() {
        PlayFabDatabase.Instance.GetLeaderboard(1);
    }
    public void OnOkClick() {
        m_resultCompetitionPanel.SetActive(false);
        CancelInvoke("GetLeaderboard");
        // check xem có đc nhận thưởng hay không
    }
    public void UpdateCelLeaderboard(string username, string result, int index) {
        m_listCompetitionRank[index].GetChild(1).GetComponent<Text>().text = username;
        m_listCompetitionRank[index].GetChild(0).GetComponent<Text>().text = result;
    }
    public void CreatCelLeaderboard(string username, string result, int index) {
        var cel = Instantiate(m_celTemplate).transform;
        m_listCompetitionRank.Add(cel);
        cel.gameObject.SetActive(true);
        cel.SetParent(m_listCompetitionRankContent);
        cel.localScale = Vector3.one;
        var rect = cel.GetComponent<RectTransform>();
        rect.localPosition = new Vector3(536.3f, (2*index +1) * -51, 0);
        cel.GetChild(1).GetComponent<Text>().text = username;
        cel.GetChild(0).GetComponent<Text>().text = result;
        m_listCompetitionRankContent.sizeDelta = new Vector2 (m_listCompetitionRankContent.sizeDelta.x, Mathf.Abs((2*index +1) * -51) + 2*51);
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