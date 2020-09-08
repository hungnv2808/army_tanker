using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Mail : MonoBehaviour
{
    private bool m_isOpened = false;
    [SerializeField] private Text m_contentLabel;
    [SerializeField] private Button m_mailButton; 
    [SerializeField] private GameObject m_notificationImage;
    private string m_content;
    private int m_gold;
    private int m_diamond;
    private int m_index;

    private void Start() {
        m_mailButton.onClick.AddListener(OnOpen);
    }
    public void Init(string content, int gold, int diamond, int index, bool isOpened) {
        m_contentLabel.text = "     Thư mới\n"+content;
        this.m_content = content;
        this.m_gold = gold;
        this.m_diamond = diamond;
        this.m_isOpened = isOpened;
        if (m_isOpened) this.m_notificationImage.SetActive(false); else this.m_notificationImage.SetActive(true);
        this.m_index = index;
    }
    public void OnOpen() {
        if (!this.m_isOpened) {
            this.m_isOpened = true;
            MailManagement.Instance.Mails[this.m_index].IsOpened = true;
            MailManagement.Instance.DecreaseUnreadMailCount();
            m_notificationImage.SetActive(false);
        }
        MailBoxUI.Instance.ShowMailDetail("     Đã đọc\n"+this.m_content, this.m_gold, this.m_diamond);
        m_contentLabel.text = "     Đã đọc\n"+this.m_content;
    }
}
