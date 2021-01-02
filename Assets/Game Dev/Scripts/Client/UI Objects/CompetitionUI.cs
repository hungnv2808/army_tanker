using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CompetitionUI : MonoBehaviour
{
    public Joystick JoytickMovement;
    public Joystick JoytickCrossHairs1;
    public Joystick JoytickCrossHairs2;
    public Button SpeedUpButton;
    public Button ReduceSpeedButton;
    [SerializeField] private Text m_speedText;
    [SerializeField] private RectTransform m_clockwise; // range euler (0, -270)
    [SerializeField] private GameObject m_mainCamera;
    [SerializeField] private GameObject m_movementPanel;
    [SerializeField] private GameObject m_crosshairsPanel;
    [SerializeField] private Image m_barLaunchForce;
    [SerializeField] private Text m_labelLaunchForce;
    private static CompetitionUI s_instance;
    [SerializeField] private Text m_notiLabel;
    [SerializeField] private Text m_heartLabel;
    [SerializeField] private Text m_ammoLabel;
    [SerializeField] private GameObject m_repairingPanel;
    [SerializeField] private Text m_timerRepairingLabel;
    [SerializeField] private Image m_itemButton;
    private string m_minute;
    private string m_second;
    [SerializeField] private Text m_timeClockLabel;
    private float m_lerpTime = 0; // thời gian theo giây
    public float LerpTime {
        get {
            return m_lerpTime;
        }
    }
    public static CompetitionUI Instance {
        get {
            return s_instance;
        }
    }
    private void Awake() {
        if (s_instance != null && s_instance != this) {
            Destroy(this.gameObject);
            return;
        }
        s_instance = this;
    }
    private void Start() {
        this.TurnClock();
        
        
    }
    public void ModifyHeart(int healthy) {
        m_heartLabel.text = "" + healthy;
    }
    public void ModifyAmmo(int ammo) {
        m_ammoLabel.text = "" + ammo;
    }
    public void OnSpeedUpButtonPointerDown() {
        if (TankCompetition.Instance != null) TankCompetition.Instance.SpeedUp();
    }
    public void OnSpeedUpButtonPointerUp() {
        if (TankCompetition.Instance != null) TankCompetition.Instance.StopSpeedUp();
    }
    public void OnReduceSpeedButtonPointerDown() {
        if (TankCompetition.Instance != null) TankCompetition.Instance.ReduceSpeed();
    }
    public void OnReduceSpeedButtonPointerUp() {
        if (TankCompetition.Instance != null) TankCompetition.Instance.StopReduceSpeed();
    }
    private float m_euler;
    public void UpdateSpeedClock(float speed, float maxSpeed) {
        m_euler = speed * (-270) / maxSpeed;
        m_speedText.text = speed.ToString("0.00");
        m_clockwise.localEulerAngles = new Vector3(0, 0, m_euler);

    }
    public void OnZoomLen() {
        m_mainCamera.SetActive(false);
        TankCompetition.Instance.ZoomLenCamera.SetActive(true);
        TankCompetition.Instance.TargetIcon.SetActive(true);
        m_movementPanel.SetActive(false);
        m_crosshairsPanel.SetActive(true);
    }
    public void OnOffZoomlen() {
        m_mainCamera.SetActive(true);
        TankCompetition.Instance.ZoomLenCamera.SetActive(false);
        TankCompetition.Instance.TargetIcon.SetActive(false);
        m_movementPanel.SetActive(true);
        m_crosshairsPanel.SetActive(false);
    }
    // test
    public void OnHack() {
        TankCompetition.Instance.FinishCompetition();
    }
    public void OnShootButtonPointerDown() {
        TankCompetition.Instance.IncreaseLaunchForce();
    }
    public void OnShootButtonPointerUp() {
        TankCompetition.Instance.StopIncreaseLaunchForce();
    }
    
    public IEnumerator ResetBarLanchForceCoroutine() {
        float _tmp = m_barLaunchForce.fillAmount;
        while (_tmp > 0) {
            _tmp -= Time.deltaTime * 4;
            if (_tmp < 0) _tmp = 0;
            m_barLaunchForce.fillAmount = _tmp;
            m_labelLaunchForce.text = (_tmp * 5).ToString("0.0");
            yield return null;
        }
    }
    public void UpdateBarLaunchForce(float t, float launchForce) {
        m_barLaunchForce.fillAmount = t;
        m_labelLaunchForce.text = launchForce.ToString("0.0");
    }
    public void ChangeTextNotiLabel(bool hasHit,int t) {
        if (hasHit) m_notiLabel.text = "Bạn đã bắn trúng mục tiêu số " + t;
        else m_notiLabel.text = "Bạn đã bắn trượt mục tiêu";
    }
    public void ChangeTextNotiLabel(string text) {
        m_notiLabel.text = text;
    }
    public void ShowRepairingPanel(int time) {
        this.m_repairingPanel.SetActive(true);
        this.m_timerRepairingLabel.text = "" + time;
    }
    public void HideRepairingPanel() {
        this.m_repairingPanel.SetActive(false);
    }
    public void UpdateTimerRepairing(int time) {
        this.m_timerRepairingLabel.text = "" + time;
    }
    public void TurnClock() {
        StartCoroutine(CountdownCoroutine());
    }
    private IEnumerator CountdownCoroutine() {
        yield return new WaitForSeconds(1.0f);
        m_lerpTime += 1.0f;
        if (((int)m_lerpTime / 60) < 10) {
            m_minute = "0" + ( (int)m_lerpTime/60 );
        } else {
            m_minute = "" + ( (int)m_lerpTime/60 );
        }
        if (((int)m_lerpTime % 60) < 10) {
            m_second = "0" + ( (int)m_lerpTime%60 );
        } else {
            m_second = "" + ( (int)m_lerpTime%60 );
        }
        m_timeClockLabel.text = m_minute + ":" + m_second;

        StartCoroutine(CountdownCoroutine());
    }
    public void OnItemClick() {
        TankCompetition.Instance.UseItem();
        m_itemButton.gameObject.SetActive(false);
    }
    public void ShowItemButton(Sprite itemSprite) {
        m_itemButton.gameObject.SetActive(true);
        m_itemButton.sprite = itemSprite;
    }
}
