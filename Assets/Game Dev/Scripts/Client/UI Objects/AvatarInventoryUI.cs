using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AvatarInventoryUI : MonoBehaviour
{
    [SerializeField] private  Button[] m_avatars;    
    private void Start() {
        m_avatars[0].onClick.AddListener(delegate() {
                this.SelectAvatar(1);
                MenuUI.Instance.HideAvatarInventory();
        });
        m_avatars[1].onClick.AddListener(delegate() {
                this.SelectAvatar(2);
                MenuUI.Instance.HideAvatarInventory();
        });
        m_avatars[2].onClick.AddListener(delegate() {
                this.SelectAvatar(3);
                MenuUI.Instance.HideAvatarInventory();
        });
        m_avatars[3].onClick.AddListener(delegate() {
                this.SelectAvatar(4);
                MenuUI.Instance.HideAvatarInventory();
        });
        m_avatars[4].onClick.AddListener(delegate() {
                this.SelectAvatar(5);
                MenuUI.Instance.HideAvatarInventory();
        });
        m_avatars[5].onClick.AddListener(delegate() {
                this.SelectAvatar(6);
                MenuUI.Instance.HideAvatarInventory();
        });
        m_avatars[6].onClick.AddListener(delegate() {
                this.SelectAvatar(7);
                MenuUI.Instance.HideAvatarInventory();
        });
        m_avatars[7].onClick.AddListener(delegate() {
                this.SelectAvatar(8);
                MenuUI.Instance.HideAvatarInventory();
        });
        m_avatars[8].onClick.AddListener(delegate() {
                this.SelectAvatar(9);
                MenuUI.Instance.HideAvatarInventory();
        });
    }
    public void SelectAvatar(int index) {
        Debug.Log("Avatar/" + index);
        MenuUI.Instance.AvatarLabel.sprite = Resources.Load<Sprite>("Avatar/" + index);
        this.UpdateAvatar("Avatar/" + index);
    }
    public void UpdateAvatar(string value) {
        PlayFabDatabase.Instance.InfoTank.PathAvatar = value;
    }
}
