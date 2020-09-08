using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
public delegate void Callback();
public class NPC : Tank
{
    public enum State {
        None = 0,
        Attack,
        Wander,
    }
    public class StateScore {
        public float Score;
        public State State;
        public StateScore(State state, float score) {
            this.Score = score;
            this.State = state;
        }
    }
    private MovementNode m_currentNode;
    private MovementNode m_nextNode;
    private MovementNode m_root;
    private int m_indexNode;
    private float m_timer = 0.0f;
    private Callback DectecPlayerCallback;
    private bool m_hasEnemy = false;
    private AbTurrent m_currentTurrent;
    private int m_turrentLevel = 0;
    private bool m_hasInited = false;
    private void Awake() {
        
    }
    public override void OnEnable() {
        PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
        if (m_hasInited) {
            this.Init();
        } else {
            Invoke("Init", ClientManagement.Instance.StartGameCountdowm + 1);
        }
        this.m_hasEnemy = false;
    }
    private void Init() {
        this.m_hasInited= true;
        this.DectecPlayerCallback = DectectPlayer;
        m_displayNameText.text = m_playerName;
        if (!PhotonNetwork.IsMasterClient) return;
        StartCoroutine(LoopDectectPlayerCoroutine());
        if (m_team != 0) {
            m_root = GenMap.Instance.Team1Roots[UnityEngine.Random.Range(0,3)];
            m_revivalPosition = m_root.Position;
        } else {
            m_root = GenMap.Instance.Team0Roots[UnityEngine.Random.Range(0,3)];
            m_revivalPosition = m_root.Position;
        }
        m_currentNode = m_root;
        Debug.Log("Root" + m_root);
        GetNextNode();
    }
    private void Start() {
        this.InitTankTurrents();
        m_rigidbody = gameObject.GetComponent<Rigidbody>();
        this.m_turrentLevel = UnityEngine.Random.Range(1, 5);
        m_currentTurrent = m_turrents["Turrent Level " + m_turrentLevel];
        m_currHealthy = m_maxHealthy;
        m_currentEnergy = m_maxEnergy; 
    }

    
    private void Update() {
        if (PhotonNetwork.IsMasterClient) {
            if (this.m_hasEnemy) {
            this.Attack();
            } else {
                this.Wander();
            }
        }
    }   
    public override IEnumerator LoopDectectPlayerCoroutine() {
        yield return new WaitForSeconds(1.0f);
        if (DectecPlayerCallback != null) {
            this.DectecPlayerCallback();
        }
        StartCoroutine(LoopDectectPlayerCoroutine());
    }
    
    private void DectectPlayer() {
        var enemies = Physics.OverlapSphere(m_transform.position, 40.0f);
        this.m_enemies.Clear();
        m_enemyTarget = null;
        if (enemies != null && enemies.Length != 0) {
            for (int i=0; i<enemies.Length; i++) {
                if (enemies[i].tag.Equals("Team" + (1-m_team))) {
                    this.m_enemies.Add(enemies[i].GetComponent<Tank>());
                }
            }
            if (m_enemies.Count != 0) {
                this.m_hasEnemy = true;
                m_minHPenemy = m_enemies[0].CurrentHealthy;
                m_enemyTarget = m_enemies[0];
                for (int i = 0; i < m_enemies.Count; i++)
                {
                    if (m_minHPenemy > m_enemies[i].CurrentHealthy) {
                        m_minHPenemy = m_enemies[i].CurrentHealthy;
                        m_enemyTarget = m_enemies[i];
                    }
                }
            } else {
                this.m_hasEnemy = false;
                
            }
        }
        
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(m_transform.position, 40.0f);   
    }
    
    private float m_distanceBetweenTowNode;
    private float m_lerpTime = 0;
    private void GetNextNode() {
        m_nextNode = m_currentNode.NextNodes[UnityEngine.Random.Range(0, m_currentNode.NextNodes.Length)];
        m_distanceBetweenTowNode = Vector3.Distance(m_currentNode.Position, m_nextNode.Position);
        m_lerpTime = m_distanceBetweenTowNode / m_moveSpeed;
        m_timer = 0;
    }
    private void Wander() {
        m_timer += Time.deltaTime;
        if (m_timer > m_lerpTime) {
            m_timer = m_lerpTime;
        }
        try {
            m_transform.position = Vector3.Lerp(m_currentNode.Position, m_nextNode.Position, m_timer/m_lerpTime);
            m_tankChassis.up = -(m_nextNode.Position - m_transform.position).normalized; // quay mặt theo hướng di chuyển
            m_tankChassis.eulerAngles = new Vector3(-90, m_tankChassis.eulerAngles.y, m_tankChassis.eulerAngles.z);
            if (m_timer == m_lerpTime) {
                m_currentNode = m_nextNode;
                GetNextNode();
            }
        } catch (Exception err) {

        }
    }
    private void Attack() {
        if (m_enemyTarget != null) {
            m_tankTurret.LookAt(this.m_enemyTarget.transform);
            m_tankTurret.eulerAngles = new Vector3(-90, m_tankTurret.eulerAngles.y, m_tankTurret.eulerAngles.z);
            this.m_currentTurrent.ShootAndSync(this.m_turrentLevel, m_fireTransform,m_tankTurret, (m_enemyTarget.Position - m_transform.position).normalized, m_team, m_playerName, this.photonView.ViewID);
        }
        
    }
    
    public override void OnCollisionEnter(Collision other) {
        
        if (other.collider.tag.Equals("Team0") || other.collider.tag.Equals("Team1"))
        {
            m_boxCollider.isTrigger = true;
        }
    }
    private void RotateTurrent() {
        m_tankTurret.eulerAngles = new Vector3(m_tankTurret.eulerAngles.x, UnityEngine.Random.Range(0, 360), m_tankTurret.eulerAngles.z);

    }
    

    public override void OnDisable() {
        StopCoroutine(LoopDectectPlayerCoroutine());
    }
}
