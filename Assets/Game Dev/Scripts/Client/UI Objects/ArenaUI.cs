using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ArenaUI : MonoBehaviour
{
    private static ArenaUI s_instance;
    public static ArenaUI Instance {
        get {
            return s_instance;
        }
    }
    [SerializeField] private GameObject m_waitingForRevivalPanel;
    [SerializeField] private Text m_countdownRevivalLabel;
    [SerializeField] private GameObject m_controlPanel;
    [SerializeField] private GameObject m_notificationPanel;
    [SerializeField] private GameObject m_countdownPanel;
    [SerializeField] private Text m_countdownLabel;
    [SerializeField] private Text m_fbsLabel;
    [SerializeField] private Text m_team1ScoreLabel;
    [SerializeField] private Text m_team0ScoreLabel;
    [SerializeField] private GameObject m_endGamePanel;
    [SerializeField] private Text m_victoryLabel;
    [SerializeField] private Text m_starGoldLabel;
    [SerializeField] private Text m_starVioletLabel;
    [SerializeField] private Button m_claimRewardButton;
    [SerializeField] private Button m_scorePanel;
    [SerializeField] private GameObject m_detailScoreUI;
    [SerializeField] private RectTransform[] m_killingNotiPositions;
    public Joystick JoytickMovement;
    public Joystick JoytickCrossHairs;
    public Joystick JoytickAssistanceSkill;
    public Image RefreshImage;
    public Text SecondLabel;
    public Image AssistanceSkillImage;
    private bool isRotateLoadingImageStopped = false;
    private bool isStopedCheckingFPS;
    private float m_deltaTime = 0.0f;
    private Queue<RectTransform> m_killingNotifications;
    private float m_fps;
    private void Awake() {
        if (s_instance != null && s_instance != this) {
            Destroy(s_instance);
        }
        s_instance = this;
    }
	private void Start() {
        isStopedCheckingFPS = false;
        m_killingNotifications = new Queue<RectTransform>(3);
        StartCoroutine(CheckFPSCoroutine());
        this.AddEventButton();

        
    }
    private void AddEventButton() {
        m_claimRewardButton.onClick.AddListener(HideEndGamePanel);
        m_scorePanel.onClick.AddListener(ShowDetailScoreUI);
    }
    private IEnumerator CheckFPSCoroutine() {
        if (isStopedCheckingFPS) yield break;
        yield return new WaitForSeconds(3.0f);
        m_deltaTime += (Time.deltaTime - m_deltaTime) * 0.1f;
        m_fps = 1.0f / m_deltaTime;
        m_fbsLabel.text = Mathf.Ceil (m_fps).ToString () + " FPS";
        StartCoroutine(CheckFPSCoroutine());
    }
    public void ShowNotificationPanel() {
        this.m_controlPanel.SetActive(false);
        this.m_notificationPanel.SetActive(true);
    }
     public void HideNotificationPanel() {
        this.m_controlPanel.SetActive(true);
        this.m_notificationPanel.SetActive(false);
    }
    public void OnYes() {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Lobby Scene");
    }
    public void OnNo() {
        this.m_controlPanel.SetActive(true);
        this.m_notificationPanel.SetActive(false);
    }
    public void ShowCountdownPanel() {
        this.m_controlPanel.SetActive(false);
        this.m_countdownPanel.SetActive(true);
    }
    public void HideCountdownPanel() {
        this.m_controlPanel.SetActive(true);
        this.m_countdownPanel.SetActive(false);
    }
    public void ChangeCountdownLabel(string text) {
        this.m_countdownLabel.text = text;
    }
    public void ShowWaitingForRevivalPanel(string playerName) {
        this.ChangeCountdownRevivalLabel("" + 10); 
        m_waitingForRevivalPanel.SetActive(true);
    }
    public void ShowKillingNotificationLabel(string whoDamage, string whoWasKilled) {
        if (m_killingNotifications.Count > 0) {
            var oldKillingNoti = m_killingNotifications.Dequeue();
            GUIPool.Instance.SetLocalPool(oldKillingNoti.gameObject);
        }
        var newKillingNoti = GUIPool.Instance.GetLocalPool("Prefabs/Killing Notification Panel", "Killing Notification Panel").GetComponent<KillingNotification>();
        m_killingNotifications.Enqueue(newKillingNoti.gameObject.GetComponent<RectTransform>());
        newKillingNoti.transform.localPosition = Vector3.zero;
        newKillingNoti.SetText(whoDamage, whoWasKilled);
        int i = 0;
        foreach (var item in m_killingNotifications)
        {
            item.localPosition += m_killingNotiPositions[i].localPosition;
            i += 1;
        }
        Invoke("HideKillingNotificationlLabel", 7.0f);
    }
    private void HideKillingNotificationlLabel() {
        for (int i = 0; i < m_killingNotifications.Count; i++)
        {
            var obj = m_killingNotifications.Dequeue();
            GUIPool.Instance.SetLocalPool(obj.gameObject);

        }
        
    }
    public void HideWaitingForRevivalPanel() {
        m_waitingForRevivalPanel.SetActive(false);
    }
    public void ChangeCountdownRevivalLabel(string text) {
        m_countdownRevivalLabel.text = text;
    }
    public void UpdateScoreLabel(int team0Killed, int team1Killed) {
        m_team1ScoreLabel.text = "" + team1Killed;
        m_team0ScoreLabel.text = "" + team0Killed;
    }
    public void ShowGameVictoryPanel() {
        PhotonNetwork.LeaveRoom();
        m_endGamePanel.SetActive(true);
        m_victoryLabel.text = "VICTORY";
        RandomStarGold(3.0f, 5.0f);
        RandomStarViolet(3, 15);
    }
    public void ShowGameDefeatPanel() {
        PhotonNetwork.LeaveRoom();
        m_endGamePanel.SetActive(true);
        m_victoryLabel.text = "DEFEAT";
        RandomStarGold(0.5f, 2.0f);
    }
    private void RandomStarGold(float min, float max) {
        var value = Random.Range(min, max);
        m_starGoldLabel.text = (int)(value * 100) + "";
        CurrencyManagement.Instance.GoldStar += (int)(value * 100);
    }
    private void RandomStarViolet(int min, int max) {
        var value = Random.Range(min, max);
        m_starVioletLabel.text = value + "";
        CurrencyManagement.Instance.VioletStar += value;
    }
    public void HideEndGamePanel() {
        m_endGamePanel.SetActive(false);
        LoadScene.Instance.Load("Menu Scene", MissionMangement.Instance.CheckMisson);
    }
    
    private void ShowDetailScoreUI() {
        m_detailScoreUI.SetActive(true);
    }
    private void OnDestroy() {
        isStopedCheckingFPS = true;
    }
}
