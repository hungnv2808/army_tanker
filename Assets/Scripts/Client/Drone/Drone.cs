using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Drone : Tank
{
    private JoytickState m_joystickMovementState;
    [SerializeField] private Transform m_crosshairIcon;
    [SerializeField] private GameObject m_crosshairRange;
    [SerializeField] private Animator m_animator;
    private float m_crosshairRangeX = 3.0f;
    private float m_crosshairRangeZ = 4.5f;

    
    public override void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
        if (photonView.IsMine) {
            this.m_predictedTrajectoryPathBullet = m_predictedTrajectoryPathBullet ?? PunObjectPool.Instance.GetLocalPool("Prefabs/Predicted Trajectory Path Bullet", "Predicted Trajectory Path Bullet", Vector3.zero, Quaternion.identity).GetComponent<LineRenderer>();
            this.m_predictedTrajectoryPathBullet.SetPosition(0, Vector3.zero);
            this.m_predictedTrajectoryPathBullet.SetPosition(1, Vector3.zero);
        }
            
        m_animator.SetBool("isMoved", false);
        m_joystickCrossHairsState = JoytickState.None;
        m_joystickMovementState = JoytickState.None;
        m_crosshairIcon.gameObject.SetActive(false);
        m_crosshairRange.SetActive(false);

        m_moveSpeed = 10.0f;
        m_leftTrail.localPosition = Vector3.zero;
        m_rightTrail.localPosition = Vector3.zero;
        Invoke("InitAutoTarget", 2.0f);
    }
    protected override void CreatTrail()
    {
        m_leftTrailEffect = PunObjectPool.Instance.GetLocalPool("Prefabs/Effect/DroneTrail", "DroneTrail", m_leftTrail.position, Quaternion.identity).GetComponent<TankTrail>();
        m_rightTrailEffect = PunObjectPool.Instance.GetLocalPool("Prefabs/Effect/DroneTrail", "DroneTrail", m_rightTrail.position, Quaternion.identity).GetComponent<TankTrail>();
        m_leftTrailEffect.Init(m_leftTrail);
        m_rightTrailEffect.Init(m_rightTrail);
    }
    public override void MoveOnPC(float inputHorizontal, float inputVertical) {
        if (inputHorizontal != 0 || inputVertical != 0)
        {
            m_movementDirection = new Vector3(inputHorizontal, 0, inputVertical);
            m_tankChassis.forward = Vector3.MoveTowards(m_tankChassis.forward, m_movementDirection.normalized, Time.deltaTime*7);
            if (!m_joystickMovementState.Equals(JoytickState.PointDown)) {
                m_joystickMovementState = JoytickState.PointDown;
                m_animator.SetBool("isMoved", true);
            } 
            if (m_tankChassis.forward == m_movementDirection.normalized) {
                m_transform.Translate(m_movementDirection.normalized * m_moveSpeed * Time.deltaTime, Space.World);/* mỗi 1 frame dịch chuyển được m_moveSpeed * Time.deltaTime đơn vị unity */
            }
        } else {
            if (m_joystickMovementState.Equals(JoytickState.PointDown)) {
                m_joystickMovementState = JoytickState.None;
                m_animator.SetBool("isMoved", false);
            }
        }
    }
    public override void Move(Joystick joystick) {
        if (joystick.GetJoystickState())
        {
           

            m_movementDirection = new Vector3(joystick.Horizontal, 0, joystick.Vertical);
            m_tankChassis.forward = Vector3.MoveTowards(m_tankChassis.forward, m_movementDirection.normalized, Time.deltaTime*7);
            if (!m_joystickMovementState.Equals(JoytickState.PointDown)) {
                m_joystickMovementState = JoytickState.PointDown;
                m_animator.SetBool("isMoved", true);
            } 
            if (m_tankChassis.forward == m_movementDirection.normalized) {
                m_transform.Translate(m_movementDirection.normalized * m_moveSpeed * Time.deltaTime, Space.World);/* mỗi 1 frame dịch chuyển được m_moveSpeed * Time.deltaTime đơn vị unity */
            }
        } else {
            if (m_joystickMovementState.Equals(JoytickState.PointDown)) {
                m_joystickMovementState = JoytickState.None;
                m_animator.SetBool("isMoved", false);
            } 
            
        }
    }
    public override void Attack(Joystick joytick)
    {
        if (joytick.GetJoystickState()) {
                m_joystickCrossHairsState = JoytickState.PointDown;
                m_crosshairIcon.gameObject.SetActive(true);
                m_crosshairRange.SetActive(true);
                m_crosshairIcon.localPosition = new Vector3(joytick.Horizontal * m_crosshairRangeX, 0, joytick.Vertical * m_crosshairRangeZ);
                this.m_predictedTrajectoryPathBullet.SetPosition(0, m_fireTransform.position);
                this.m_predictedTrajectoryPathBullet.SetPosition(1, m_crosshairIcon.position);
        } else {
            if (m_joystickCrossHairsState.Equals(JoytickState.PointDown)) {
                m_crosshairIcon.gameObject.SetActive(false);
                m_crosshairRange.SetActive(false);
                this.m_predictedTrajectoryPathBullet.SetPosition(0, Vector3.zero);
                this.m_predictedTrajectoryPathBullet.SetPosition(1, Vector3.zero);
                // this.m_turrents["Turrent Level 4"].MaxCooldown = 0;
                // this.m_turrents["Turrent Level 4"].ShootAndSync(4, m_fireTransform, m_tankTurret, m_turretDirection, m_team, m_playerName, this.photonView.ViewID);
                m_joystickCrossHairsState = JoytickState.None;
            }
        }
    }
    public override void Invisible()
    {
    }
    public override void Visible()
    {
    }
}
