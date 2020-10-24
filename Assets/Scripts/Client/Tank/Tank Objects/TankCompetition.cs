using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankCompetition : MonoBehaviour
{
    private Vector3 m_movementDirection = Vector3.zero; /* hướng dịch chuyển của xe tăng*/
    public Transform m_tankChassis; /*phần bánh xe tăng*/
    public Transform m_tankTurret; /*phần nòng súng xe tăng*/
    public Vector3 m_turretDirection = Vector3.zero; /* hướng của nòng súng xe tăng*/
    [SerializeField] private Transform m_fireTransform;
    public Transform m_transform;
    public Transform PosCamera;
    private float m_moveSpeed = 0;  /* speed là 15 đơn vị/giây (1 đơn vị = 100px)*/
    private float m_maxSpeed = 30;
    public GameObject ZoomLenCamera;
    public GameObject TargetIcon;
    private JoytickState m_joystickCrossHairsState;
    private static TankCompetition s_instance;
    public static TankCompetition Instance {
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
        m_movementDirection = -m_tankChassis.up;
        m_turretDirection = -m_tankTurret.up;
        m_joystickCrossHairsState = JoytickState.None;
        this.RefreshAxisJoytickCrossHairs();
    }
    // Update is called once per frame
    public void MoveOnPC(float inputHorizontal, float inputVertical) {
        if (inputHorizontal != 0 || inputVertical != 0)
        {
            m_movementDirection = new Vector3(inputHorizontal, 0, inputVertical);
            m_tankChassis.up = -m_movementDirection.normalized;
            m_tankChassis.eulerAngles = new Vector3(-90, m_tankChassis.eulerAngles.y, m_tankChassis.eulerAngles.z);
        }
        if (m_tankChassis.up == -m_movementDirection.normalized) m_transform.Translate(m_movementDirection.normalized * m_moveSpeed * Time.deltaTime, Space.World); /* mỗi 1 frame dịch chuyển được m_moveSpeed * Time.deltaTime đơn vị unity */

    }
    
    public void Move(Joystick joystickMovement) {
        if (joystickMovement.GetJoystickState())
        {
            m_movementDirection = Vector3.MoveTowards(m_movementDirection, joystickMovement.Horizontal * CameraFollow.Instance.AxisCoordinateHorizontal + joystickMovement.Vertical * CameraFollow.Instance.AxisCoordinateVertical,Time.deltaTime*10); // lấy trục z (CameraFollow.Instance.m_transform.forward) của camera ứng với trục vertical của joytick
            m_tankChassis.up = Vector3.MoveTowards(m_tankChassis.up, -m_movementDirection.normalized, Time.deltaTime*10);
            m_tankChassis.eulerAngles = new Vector3(-90, m_tankChassis.eulerAngles.y, m_tankChassis.eulerAngles.z);
        }
        if (m_movementDirection == Vector3.zero && m_moveSpeed > 0) {
            m_movementDirection = CameraFollow.Instance.AxisCoordinateVertical;
        }
        m_transform.Translate(m_movementDirection.normalized * m_moveSpeed * Time.deltaTime, Space.World);/* mỗi 1 frame dịch chuyển được m_moveSpeed * Time.deltaTime đơn vị unity */
    }
    public void Attack(Joystick joytickCrossHairs1, Joystick joytickCrossHairs2) {
        if (joytickCrossHairs1.GetJoystickState()) {
                m_joystickCrossHairsState = JoytickState.PointDown;
                this.RotateRoundTurrent(joytickCrossHairs1);
        } else {
            if (m_joystickCrossHairsState.Equals(JoytickState.PointDown)) {
                this.RefreshAxisJoytickCrossHairs();
            
                m_joystickCrossHairsState = JoytickState.None;
            }
        }
        if (joytickCrossHairs2.GetJoystickState()) {
            this.RotateVerticalTurrent(joytickCrossHairs2);
        }
    }
    public float launchForce;
    public void Shoot() {
        BulletCompetition.Spawn(m_fireTransform.position, -m_tankTurret.up * launchForce * 15000);
    }
    private void RefreshAxisJoytickCrossHairs() {
        Debug.Log("RefreshAxisJoytickCrossHairs");
        CameraFollow.Instance.AxisCoordinateHorizontalJoystickCrossHairs = m_tankTurret.right;
        CameraFollow.Instance.AxisCoordinateVerticalJoystickCrossHairs  = -m_tankTurret.up;
    }
    private void RotateRoundTurrent(Joystick joystick) {
        m_turretDirection = joystick.Horizontal * CameraFollow.Instance.AxisCoordinateHorizontalJoystickCrossHairs + joystick.Vertical * CameraFollow.Instance.AxisCoordinateVerticalJoystickCrossHairs;
        m_tankTurret.up = Vector3.MoveTowards(m_tankTurret.up, -m_turretDirection.normalized, Time.deltaTime*7);
        m_tankTurret.eulerAngles = new Vector3(-90, m_tankTurret.eulerAngles.y, m_tankTurret.eulerAngles.z);
    }
    private float m_eulerXTurrent = 90;
    private float m_minEulerXTurrent = 80;
    private float m_maxEulerXTurrent = 100;
    private void RotateVerticalTurrent(Joystick joystick) {
        // goc x: -80 to -100
        float tmp = m_eulerXTurrent;
        m_eulerXTurrent = 0.5f * (m_maxEulerXTurrent+m_minEulerXTurrent + joystick.Vertical*(m_maxEulerXTurrent-m_minEulerXTurrent));
        if (m_eulerXTurrent != tmp) {
            m_tankTurret.localEulerAngles = new Vector3(-m_eulerXTurrent, m_tankTurret.localEulerAngles.y, m_tankTurret.localEulerAngles.z);
        }
        Debug.Log(m_tankTurret.localEulerAngles);
    }
    private Callback m_excuteChangeSpeed;
    private void Update() {
        if (m_excuteChangeSpeed != null) m_excuteChangeSpeed();
    }
    public void SpeedUp() {
        m_excuteChangeSpeed = LoopSpeedUpCoroutine;
    }
    public void StopSpeedUp() {
        m_excuteChangeSpeed = LoopStopChangeSpeedCoroutine;
    }
    public void ReduceSpeed() {
        m_excuteChangeSpeed = LoopReduceSpeedCoroutine;
    }
    public void StopReduceSpeed() {
        m_excuteChangeSpeed = LoopStopChangeSpeedCoroutine;
    }
    private void LoopSpeedUpCoroutine() {
        // Debug.Log("tang toc");
        if (m_moveSpeed >= m_maxSpeed) m_moveSpeed = m_maxSpeed;
        else m_moveSpeed += Time.deltaTime*10; // mất 3s để tăng đến vận tốc tối đa
        CompetitionUI.Instance.UpdateSpeedClock(m_moveSpeed, m_maxSpeed);
    }
    private void LoopReduceSpeedCoroutine() {
        // Debug.Log("giam toc");
        if (m_moveSpeed <= 0) m_moveSpeed = 0;
        else m_moveSpeed -= Time.deltaTime*20;// mất 1,5s để giảm vận tốc từ 30 về 0
        CompetitionUI.Instance.UpdateSpeedClock(m_moveSpeed, m_maxSpeed);
    }
    private void LoopStopChangeSpeedCoroutine() {
        // Debug.Log("giam dan toc");
        if (m_moveSpeed <= 0) m_moveSpeed = 0;
        else m_moveSpeed -= Time.deltaTime*1;// 1s thì vận tốc giảm 1
        CompetitionUI.Instance.UpdateSpeedClock(m_moveSpeed, m_maxSpeed);
    }
}
