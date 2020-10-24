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
    private static CompetitionUI s_instance;
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
    public void OnShoot() {
        TankCompetition.Instance.Shoot();
    }
}
