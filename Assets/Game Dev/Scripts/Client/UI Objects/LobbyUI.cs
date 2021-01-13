using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Tank3d.PhotonServer;
using UnityEngine.SceneManagement;
public class LobbyUI : MonoBehaviour
{
    private static LobbyUI s_instance;
    public static LobbyUI Instance {
        get {
            return s_instance;
        }
    }
    [SerializeField] private GameObject m_3vs3Button;
    [SerializeField] private GameObject m_2vs2Button;
    [SerializeField] private GameObject m_1vs1Button;
    [SerializeField] private GameObject m_testButton;
    [SerializeField] private GameObject m_backButton;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Text m_goldStarLabel;
    [SerializeField] private Text m_violetStarLabel;

    private void Awake() {
        if (s_instance != null && s_instance != this) {
            DestroyImmediate(s_instance);
        }
        s_instance = this;
        m_3vs3Button.GetComponent<Button>().onClick.AddListener(On3vs3Click);
        m_2vs2Button.GetComponent<Button>().onClick.AddListener(On2vs2Click);
        m_1vs1Button.GetComponent<Button>().onClick.AddListener(On1vs1Click);
        m_testButton.GetComponent<Button>().onClick.AddListener(OnTestRoomClick);
        m_backButton.GetComponent<Button>().onClick.AddListener(OnBackClick);
    }
    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        this.m_animator.SetBool("isOpenedSelectMap", true);
        this.UpdateCurrencyUI();
    }
    public void On3vs3Click() {
        SoundManagement.Instance.PlaySoundOpenPanel();
        ServerManagement.MaxPlayersInRoom = 6;
        ServerConnection.Instance.JoinRoom();

    }
    public void On2vs2Click() {
        SoundManagement.Instance.PlaySoundOpenPanel();
        ServerManagement.MaxPlayersInRoom = 4;
        ServerConnection.Instance.JoinRoom();
    }
    public void On1vs1Click() {
        SoundManagement.Instance.PlaySoundOpenPanel();
        ServerManagement.MaxPlayersInRoom = 2;
        ServerConnection.Instance.JoinRoom();
    }
    public void OnTestRoomClick() {
        ServerManagement.MaxPlayersInRoom = 1;
        ServerConnection.Instance.JoinRoom();
    }
    public void OnBackClick() {
        SoundManagement.Instance.PlaySoundClick();
        this.m_animator.SetBool("isOpenedSelectMap", false);
        Invoke("LoadMenuScene", 0.7f);
    }
    private void LoadMenuScene() {
        SceneManager.LoadScene("Menu Scene");
    }
    public void UpdateCurrencyUI() {
        m_goldStarLabel.text = CurrencyManagement.Instance.GoldStar + "";
        m_violetStarLabel.text = CurrencyManagement.Instance.VioletStar + "";
    }
}
