using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Robot : MonoBehaviour
{
    private Callback ExcuteStateCallback;
    private Callback DectecPlayerCallback;
    [SerializeField] private Animator m_animator;
    private static Animator s_animator;
    private static AnimationEvent s_OnDie;
    [SerializeField] private Transform m_fireTransform;
    [SerializeField] private RobotHealthyBar m_healthyBar;
    private float m_moveSpeed = 10.0f; /*tốc độ di chuyển 15 đơn vị/giây*/
    private float m_maxHeatlhy = 20.0f;
    private float m_currHealthy;
    private float m_distance2TargetTriggerShoot = 30.0f;
    private float m_distance2TargetTriggerMoveShoot = 45.0f;
    private float m_maxCooldown = 1.0f;
    private float m_curCooldown = 0.0f;
    private Vector3 m_appearPosition;
    private string m_bulletPrefabsLink = "Prefabs/Robot Bullet/RobotSingleBullet";
    [SerializeField] private Transform m_transform;
    enum EState {
        None = 0,
        Idle = 1,
        Walk = 2,
        Shoot = 3,
        MoveShoot = 4, /*vừa di chuyển vừa tấn công*/
        Die = 5,
        ComeBack2AppearPosition = 6,
    }
    private EState m_currState;
    private EState m_preState;
    private Vector3 m_movementDirection;
    private Transform m_targetTransform;

    private void OnEnable() {
        m_preState = EState.None;
        m_currState = EState.Idle;
        m_targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        m_appearPosition = m_transform.position;
        m_currHealthy = m_maxHeatlhy;
        // this.AddEventKeyframeAnimation();
    }
    public static void AddEventKeyframeAnimation() {
        // foreach (var item in  m_animator.runtimeAnimatorController.animationClips)
        // {
        //     Debug.Log(item.name);
        // }
        s_animator = GameObject.FindGameObjectWithTag("SmallRobot").GetComponent<Animator>();
        /*thứ tự clip : idle, run, attack, die*/
        s_OnDie = s_OnDie ?? new AnimationEvent();
        s_OnDie.time = 19.0f/23; /*tổng thời gian chạy của animation clip*/
        s_OnDie.functionName = "Die";
        // Debug.Log("###" + m_animator.runtimeAnimatorController.animationClips[3].length);
        s_animator.runtimeAnimatorController.animationClips[3].AddEvent(s_OnDie);
    }
    private void FixedUpdate() {
        if (this.DectecPlayerCallback != null) this.DectecPlayerCallback();
    }
    // Update is called once per frame
    void Update()
    {
        if(!m_currState.Equals(m_preState))
            switch (m_currState) {
                case EState.Idle:
                    m_animator.SetBool("isIdle", true);
                    m_animator.SetBool("isWalked", false);
                    m_animator.SetBool("isShooted", false);
                    m_animator.SetBool("isDied", false);
                    this.ExcuteStateCallback = null;
                    this.DectecPlayerCallback = this.DectectPlayer;
                    break;
                case EState.Walk: 
                    m_animator.SetBool("isIdle", false);
                    m_animator.SetBool("isWalked", true);
                    m_animator.SetBool("isShooted", false);
                    m_animator.SetBool("isDied", false);
                    this.ExcuteStateCallback = this.Walk;
                    break;
                case EState.Shoot:
                    m_animator.SetBool("isIdle", false);
                    m_animator.SetBool("isWalked", false);
                    m_animator.SetBool("isShooted", true);
                    m_animator.SetBool("isDied", false);
                    this.ExcuteStateCallback = this.Shoot;
                    break;
                case EState.MoveShoot:
                    m_animator.SetBool("isIdle", false);
                    m_animator.SetBool("isWalked", false);
                    m_animator.SetBool("isShooted", true);
                    m_animator.SetBool("isDied", false);
                    this.ExcuteStateCallback = this.MoveShoot;
                    break;
                case EState.Die:
                    m_animator.SetBool("isIdle", false);
                    m_animator.SetBool("isWalked", false);
                    m_animator.SetBool("isShooted", false);
                    m_animator.SetBool("isDied", true);
                    this.ExcuteStateCallback = null;
                    this.DectecPlayerCallback = null;
                    break;
                case EState.ComeBack2AppearPosition:
                    m_animator.SetBool("isIdle", false);
                    m_animator.SetBool("isWalked", true);
                    m_animator.SetBool("isShooted", false);
                    m_animator.SetBool("isDied", false);
                    this.ExcuteStateCallback = this.ComeBack2AppearPosition;
                    this.DectecPlayerCallback = this.DectectPlayer;
                    break;

            }
        if (this.ExcuteStateCallback != null)
            this.ExcuteStateCallback();
    }
    private void ChangeState(EState state) {
        m_preState = m_currState;
        m_currState = state;
    }
    private void DectectPlayer() {
        Collider[] hitColliders = Physics.OverlapSphere(m_transform.position, 40.0f);
        for (int i=0; i<hitColliders.Length; i++) {
            if (hitColliders[i].tag.Equals("Player")) {
                this.ChangeState(EState.Walk);
                this.DectecPlayerCallback = null;
            }
        }
    }
    private void RotateGun() {
        m_movementDirection = new Vector3(m_targetTransform.position.x - m_transform.position.x, 0, m_targetTransform.position.z - m_transform.position.z);
        /*góc quay của robot theo trục Oy = góc hợp bởi trục Oz (do nòng súng của robot hợp với trục Oz) và vector hướng dịch chuyển m_movementDirection*/
        m_transform.eulerAngles = new Vector3(m_transform.eulerAngles.x, Mathf.Rad2Deg * Mathf.Atan2(m_movementDirection.x, m_movementDirection.z), m_transform.eulerAngles.z);
    }
    private void CreatBullet() {
        m_curCooldown -= Time.deltaTime;
        if (m_curCooldown <= 0) {
            // Instantiate<RobotSingleBullet>(Resources.Load<RobotSingleBullet>(m_bulletPrefabsLink), m_fireTransform.position, Quaternion.identity).Init(m_transform, m_transform.eulerAngles + Vector3.up * (-90.0f));
            m_curCooldown = m_maxCooldown;
        } 
    }
    private void Walk() {
        this.RotateGun();
        if (m_movementDirection.magnitude > m_distance2TargetTriggerMoveShoot) {
            this.ChangeState(EState.ComeBack2AppearPosition);
        }
        if (m_movementDirection.magnitude >= m_distance2TargetTriggerShoot) {
            m_transform.Translate(m_movementDirection.normalized * m_moveSpeed * Time.deltaTime, Space.World);
        } else {
            this.ChangeState(EState.Shoot);
        }
        
    }
    private void Shoot() {
        this.RotateGun();
        if (m_movementDirection.magnitude <= m_distance2TargetTriggerMoveShoot && m_movementDirection.magnitude > m_distance2TargetTriggerShoot) {
            this.ChangeState(EState.MoveShoot);
        }
        
        //TODO :bắn đạn
        this.CreatBullet();
    }
    private void MoveShoot() {
        this.RotateGun();      
        if (m_movementDirection.magnitude > m_distance2TargetTriggerMoveShoot) {
            this.ChangeState(EState.ComeBack2AppearPosition);
        }
        if (m_movementDirection.magnitude <= m_distance2TargetTriggerShoot) {
            this.ChangeState(EState.Shoot);
        }
        m_transform.Translate(m_movementDirection.normalized * m_moveSpeed * Time.deltaTime, Space.World);
        //TODO :bắn đạn
        this.CreatBullet();
    }
    private void ComeBack2AppearPosition() {
        m_movementDirection = new Vector3(m_appearPosition.x - m_transform.position.x, 0, m_appearPosition.z - m_transform.position.z);
        /*góc quay của robot theo trục Oy = góc hợp bởi trục Oz (do nòng súng của robot hợp với trục Oz) và vector hướng dịch chuyển m_movementDirection*/
        m_transform.eulerAngles = new Vector3(m_transform.eulerAngles.x, Mathf.Rad2Deg * Mathf.Atan2(m_movementDirection.x, m_movementDirection.z), m_transform.eulerAngles.z);
        m_transform.Translate(m_movementDirection.normalized * m_moveSpeed * Time.deltaTime, Space.World);
        // if (Vector3.Distance(m_appearPosition, m_transform.position) <= 0) this.ChangeState(EState.Idle);
        if (m_movementDirection.magnitude <= 1) {
            this.ChangeState(EState.Idle); 
            m_transform.position = m_appearPosition;
        }
    }
    public void ReduceBlood(float damage) {
        m_currHealthy -= damage;
        if (m_currHealthy >= 0) m_healthyBar.ReduceHealthyBar(damage, m_maxHeatlhy);
        if (m_currHealthy <= 0) {
            this.ChangeState(EState.Die);
        }
    }
    private void Die() {
        gameObject.SetActive(false);
    }
    // private void OnCollisionEnter(Collision other) {
    // }
    // private void OnCollisionExit(Collision other) {
    // }
}
