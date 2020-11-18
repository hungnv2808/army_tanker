using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankCompetition : MonoBehaviour
{
    private Vector3 m_movementDirection = Vector3.zero; /* hướng dịch chuyển của xe tăng*/
    public Transform m_tankChassis; /*phần bánh xe tăng*/
    public Transform m_tankTurret; /*phần nòng súng xe tăng*/
    public Transform m_directionShoot;
    public Transform m_tankTurretParent;
    public Vector3 m_turretDirection = Vector3.zero; /* hướng của nòng súng xe tăng*/
    [SerializeField] private Transform m_fireTransform;
    public Transform m_transform;
    public Transform PosCamera;
    private float m_moveSpeed = 0;  /* speed là 15 đơn vị/giây (1 đơn vị = 100px)*/
    private float m_maxSpeed = 30;
    public GameObject ZoomLenCamera;
    public GameObject TargetIcon;
    private int m_healthy;
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
        m_healthy = 3;
        CompetitionUI.Instance.ModifyHeart(m_healthy);
        m_movementDirection = -m_tankChassis.up;
        m_turretDirection = -m_tankTurret.up;
        m_joystickCrossHairsState = JoytickState.None;
        this.RefreshAxisJoytickCrossHairs();
    }
    private void RefreshHealthy() {
        m_healthy = 3;
        CompetitionUI.Instance.ModifyHeart(m_healthy);
    }
    private void ReduceHealthy() {
        m_healthy -= 1;
        if (m_healthy <= 0) {
            this.Repair();
        }
        CompetitionUI.Instance.ModifyHeart(m_healthy);
    }
    private void Repair() {
        StartCoroutine(RepairCoroutine());
    }
    private IEnumerator RepairCoroutine() {
        int timer = 10;
        CompetitionUI.Instance.ShowRepairingPanel(timer);
        var timeWaiting = new WaitForSeconds(1);
        while(timer > 0) {
            yield return timeWaiting;
            timer -= 1;
            CompetitionUI.Instance.UpdateTimerRepairing(timer);
        }
        this.RefreshHealthy();
        CompetitionUI.Instance.HideRepairingPanel();
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
                Debug.Log("joytickCrossHairs1.GetJoystickState()");
        } else {
            if (m_joystickCrossHairsState.Equals(JoytickState.PointDown)) {
                this.RefreshAxisJoytickCrossHairs();
                Debug.Log("m_joystickCrossHairsState.Equals(JoytickState.PointDown");
                m_joystickCrossHairsState = JoytickState.None;
            }
        }
        if (joytickCrossHairs2.GetJoystickState()) {
            Debug.Log("joytickCrossHairs2.GetJoystickState()");
            this.RotateVerticalTurrent(joytickCrossHairs2);
        }
    }
    public float LaunchForce;
    private void ShootHandle(float launchForce) {
        this.LaunchForce = launchForce;
        BulletCompetition.Spawn(m_fireTransform.position, -m_directionShoot.up * launchForce * 4000);
        // StartCoroutine(RecoilGunCoroutine(m_tankTurret.localPosition, m_tankTurret, -m_tankTurret.up));
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
    private float m_eulerXTurrent = 70;
    private float m_minEulerXTurrent = 50;
    private float m_maxEulerXTurrent = 90;
 
    private void RotateVerticalTurrent(Joystick joystick) {
        // goc x: -80 to -100
        m_eulerXTurrent = 0.5f * (m_maxEulerXTurrent+m_minEulerXTurrent + joystick.Vertical*(m_maxEulerXTurrent-m_minEulerXTurrent));
        m_tankTurretParent.localRotation = Quaternion.Euler(-m_eulerXTurrent, m_tankTurretParent.localEulerAngles.y, m_tankTurretParent.localEulerAngles.z);
        // Debug.Log("m_axisY"+m_axisY);
        // Debug.Log("m_axisZ" + m_axisZ);
        // m_tankTurret.up = (joystick.Vertical* m_axisZ + m_axisY);

        // Debug.Log("m_tankTurret.up"+ m_tankTurret.up);

    }
    private Callback m_excuteChangeSpeed;
    private void Update() {
        if (m_excuteChangeSpeed != null) m_excuteChangeSpeed();
    }
    public void SpeedUp() {
        m_excuteChangeSpeed = SpeedUpHandle;
    }
    public void StopSpeedUp() {
        m_excuteChangeSpeed = StopChangeSpeedHandle;
    }
    public void ReduceSpeed() {
        m_excuteChangeSpeed = ReduceSpeedHandle;
    }
    public void StopReduceSpeed() {
        m_excuteChangeSpeed = StopChangeSpeedHandle;
    }
    private void SpeedUpHandle() {
        // Debug.Log("tang toc");
        if (m_moveSpeed >= m_maxSpeed) m_moveSpeed = m_maxSpeed;
        else m_moveSpeed += Time.deltaTime*10; // mất 3s để tăng đến vận tốc tối đa
        CompetitionUI.Instance.UpdateSpeedClock(m_moveSpeed, m_maxSpeed);
    }
    private void ReduceSpeedHandle() {
        // Debug.Log("giam toc");
        if (m_moveSpeed <= 0) m_moveSpeed = 0;
        else m_moveSpeed -= Time.deltaTime*20;// mất 1,5s để giảm vận tốc từ 30 về 0
        CompetitionUI.Instance.UpdateSpeedClock(m_moveSpeed, m_maxSpeed);
    }
    private void StopChangeSpeedHandle() {
        // Debug.Log("giam dan toc");
        if (m_moveSpeed <= 0) m_moveSpeed = 0;
        else m_moveSpeed -= Time.deltaTime*1;// 1s thì vận tốc giảm 1
        CompetitionUI.Instance.UpdateSpeedClock(m_moveSpeed, m_maxSpeed);
    }
    private bool m_hasShootButtonPressed = false;
    public void IncreaseLaunchForce() {
        m_hasShootButtonPressed = true;
        StartCoroutine(LoopIncreaseLaunchForceCoroutine());
    }
    public void StopIncreaseLaunchForce() {
        m_hasShootButtonPressed = false;
    }
    private IEnumerator LoopIncreaseLaunchForceCoroutine() {
        yield return StartCoroutine(CompetitionUI.Instance.ResetBarLanchForceCoroutine());
        float _tmpLanchForce = 0.0f;
        while (m_hasShootButtonPressed) {
            _tmpLanchForce += Time.deltaTime * 1.5f; // 1 giây tăng 1.5
            if (_tmpLanchForce > 5) _tmpLanchForce = 5;
            CompetitionUI.Instance.UpdateBarLaunchForce(_tmpLanchForce / 5, _tmpLanchForce);
            yield return null;
        }
        ShootHandle(_tmpLanchForce);
    }
    private float m_timerTo = 0;
    private float m_timerBack = 0;
    private float m_lerpTime = 0.1f;
    private float m_lerpRatio;
    private IEnumerator RecoilGunCoroutine(Vector3 originalPosition, Transform tankTurren, Vector3 turrentDirection)
    {
        m_timerTo += Time.deltaTime;
        if (m_timerTo > m_lerpTime) m_timerTo = m_lerpTime;
        m_lerpRatio = m_timerTo / m_lerpTime;

        tankTurren.localPosition = Vector3.Lerp(originalPosition, tankTurren.localPosition - turrentDirection.normalized * 0.2f, m_lerpRatio);
        if (m_lerpRatio >= 1)
        {
            StartCoroutine(PlayBack(originalPosition, tankTurren));
            yield break;
        }
        else
        {
            yield return null;
            StartCoroutine(RecoilGunCoroutine(originalPosition, tankTurren, turrentDirection));
        }
    }
    private IEnumerator PlayBack(Vector3 originalPosition, Transform tankTurren)
    {
        m_timerBack += Time.deltaTime;
        if (m_timerBack > m_lerpTime) m_timerBack = m_lerpTime;
        m_lerpRatio = m_timerBack / m_lerpTime;
        tankTurren.localPosition = Vector3.Lerp(tankTurren.localPosition, originalPosition, m_lerpRatio);
        if (m_lerpRatio >= 1) {
            yield break;
        }
        else
        {
            yield return null;
            StartCoroutine(PlayBack(originalPosition, tankTurren));
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.tag.Equals("Door")) {
            Target.ShowFirstStaticTarget();
        }
    }
    private void OnCollisionEnter(Collision other) {
        if (other.collider.tag.Equals("Wall")) {
            m_excuteChangeSpeed = null;
            Debug.Log("dam vao tuong");
            this.ReduceHealthy();
            m_moveSpeed = 0; 
            CompetitionUI.Instance.UpdateSpeedClock(m_moveSpeed, m_maxSpeed);
        }
    }
}
