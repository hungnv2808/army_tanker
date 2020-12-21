using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MailBoxUI : MonoBehaviour 
{
    private static MailBoxUI m_instance;
    [SerializeField] private Text m_contentDetailLabel;
    [SerializeField] private Text m_goldLabel;
    [SerializeField] private Text m_diamondLabel;
    [SerializeField] private GameObject m_notificationImage; 
    [SerializeField] private Button m_okButton;
    [SerializeField] private GameObject m_recivedLabel;
    [SerializeField] private Transform m_contentScrollView;
    private int m_mailCount = 0;
    private float m_offsetY = 8.5f;
    private float m_mailSizeY = 80.0f;
    private void Awake() {
        if (m_instance != null && m_instance != this) {
            Destroy(m_instance.gameObject);
        }
        m_instance = this;
    }
    private void Start() {
        m_okButton.onClick.AddListener(OnOk);
        this.RefreshMailBox();
        InvokeRepeating("CheckRefreshMailBox", 4.0f,4.0f);
    }
    public void CheckRefreshMailBox() {
        if (MailManagement.Instance.IsRefresh) {
            this.RefreshMailBox();
        }
    }
    private void RefreshMailBox() {
        MailManagement.Instance.IsRefresh = false;
        this.DestroyOldMail();
        for (int i = MailManagement.Instance.Mails.Count-1; i >= 0; i--)
        {
            this.CreatMail(MailManagement.Instance.Mails[i].Content,MailManagement.Instance.Mails[i].Gold,
                            MailManagement.Instance.Mails[i].Diamond, 
                            i,
                            MailManagement.Instance.Mails[i].IsOpened);
        }
        if (MailManagement.Instance.UnreadMailCount > 0) {
            m_notificationImage.SetActive(true);
        } else {
            this.TurnOffNotification();
        }
    }
    public void TurnOffNotification() {
        m_notificationImage.SetActive(false);
    }
    private void DestroyOldMail() {
        for (int i = 0; i < m_contentScrollView.childCount; i++)
        {
            Destroy(this.m_contentScrollView.GetChild(i).gameObject);
        }
    }
    private void ShowOkButton() {
        m_recivedLabel.SetActive(false);
        m_okButton.gameObject.SetActive(true);
    }
    private void HideOkButton() {
        m_recivedLabel.SetActive(true);
        m_okButton.gameObject.SetActive(false);
    }
    private void OnOk() {
        StartCoroutine(CurrencyManagement.Instance.EffectGoldAndDiamond());
    }
    private void CreatMail(string text, int gold, int diamond, int index, bool isOpened) {
        //TODO
        m_mailCount += 1;
        var mail = Instantiate<Mail>(Resources.Load<Mail>("Prefabs/Mail"));
        mail.transform.SetParent(m_contentScrollView);
        mail.GetComponent<RectTransform>().localPosition = new Vector3(145, -(m_mailCount * m_offsetY + (m_mailCount - 1)* m_mailSizeY), 0);
        mail.Init(text, gold, diamond, index, isOpened);
        m_contentScrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(m_contentScrollView.GetComponent<RectTransform>().sizeDelta.x, (m_mailCount * m_offsetY + m_mailCount * m_mailSizeY));
    }
    public void ShowMailDetail(string text, int gold, int diamond) {
        this.m_contentDetailLabel.text = text;
        this.m_goldLabel.text = "" + gold;
        this.m_diamondLabel.text = "" + diamond;
    }
    public static MailBoxUI Instance {
        get {
            return m_instance;
        }
    }
}
