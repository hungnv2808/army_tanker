using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailManagement : MonoBehaviour
{
    public class MailData 
    {
        public string Content;
        public int Gold;
        public int Diamond; 
        public bool IsOpened;
        public MailData(string content, int gold, int diamond, bool isOpened) {
            this.Content = content;
            this.Gold = gold;
            this.Diamond = diamond;
            this.IsOpened = isOpened;
        }
    }
    private static MailManagement m_instance;
    private int m_unreadMailCount = 0;
    private List<MailData> m_mails;
    public bool IsRefresh = false;
    public static MailManagement Instance {
        get {
            return m_instance;
        }
    }
    private void Awake() {
        if (m_instance != null && m_instance != this) {
            Destroy(this.gameObject);
            return;
        }
        m_instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    private void Start() {
        m_mails = new List<MailData>();
    }
    public void PushNotification(string content, int gold, int diamond, bool isOpened) {
        Debug.Log("Mail content : " + content);
        this.m_mails.Add(new MailData(content, gold, diamond, isOpened));
        this.m_unreadMailCount += 1;
        this.IsRefresh = true;
    }
    public void DecreaseUnreadMailCount() {
        m_unreadMailCount -= 1;
        if (m_unreadMailCount <= 0) {
            m_unreadMailCount = 0;
            MailBoxUI.Instance.TurnOffNotification();
        }
    }
    
    public void GetMailData() {

    }
    public List<MailData> Mails {
        get {
            return m_mails;
        }
    }
    public int UnreadMailCount {
        get {
            return m_unreadMailCount;
        }
    }
}
