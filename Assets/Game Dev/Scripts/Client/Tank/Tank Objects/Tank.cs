using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;

[Serializable]
public class InfoTank {
    public string PathAvatar;
    public int RankIndex;
    public int RankPoint;
    public int VioletStar;
    public int GoldStar;
    public int IndexTankerChampionSelected;
    public int IndexAssistanceSkillSelected;

    public InfoTank(string pathAvatar, int rankIndex, int rankPoint, int violetStar, int goldStar, int indexTankerChampionSelected, int indexAssistanceSkillSelected) {
        this.PathAvatar = pathAvatar;
        this.RankIndex = rankIndex;
        this.RankPoint = rankPoint;
        this.VioletStar = violetStar;
        this.GoldStar = goldStar;
        this.IndexTankerChampionSelected = indexTankerChampionSelected;
        this.IndexAssistanceSkillSelected = indexAssistanceSkillSelected;
    }

}
[Serializable]
public class TankerStat {
    public int Index;
    public int Price;
    public bool HasUnlock;
    public float Healthy;
    public float MoveSpeed;
    public float Damage;
    public TankerStat(int index, int price, bool hasUnlock, float healthy, float moveSpeed, float damage) {
        this.Index = index;
        this.Price = price;
        this.HasUnlock = hasUnlock;
        this.Healthy = healthy;
        this.MoveSpeed = moveSpeed;
        this.Damage = damage;
    }
}
[Serializable]
public class AssistanceSkill {
    public int Index;
    public bool HasUnlock;
    public AssistanceSkill(int index, bool hasUnlock) {
        this.Index = index;
        this.HasUnlock = hasUnlock;
    }
}
public class Tank : MonoBehaviourPun, IEvent, IPunObservable
{
    
    [SerializeField] protected Transform m_transform;
    [SerializeField] protected Transform m_tankTurret; /*phần nòng súng xe tăng*/
    [SerializeField] protected Transform m_fireTransform;
    protected LineRenderer m_predictedTrajectoryPathBullet;
    [SerializeField] protected Transform m_tankChassis; /*phần bánh xe tăng*/
    [SerializeField] protected Collider m_boxCollider;
    [SerializeField] protected TankHealthyBar m_healthyBar;
    [SerializeField] protected SpriteRenderer m_healthyBarSpr;
    [SerializeField] protected TextMesh m_heatlthyLabel;
    [SerializeField] protected EnergyBar m_energyScript;
    [SerializeField] protected TextMesh m_displayNameText;
    [SerializeField] protected GameObject m_iconInvisible;
    public Transform BombPowPoint;
    private Transform m_transformParentAutoTargetIcon;
    protected Rigidbody m_rigidbody;
    protected string m_playerName;
    protected string m_pathAvatar;
    protected bool m_isPlayer = false;
    protected float m_maxHealthy;
    protected float m_maxEnergy;
    protected float m_currentEnergy;
    protected float m_currHealthy;
    protected bool isIncreasedBlood = false;
    protected static Tank s_localPlayerInstance;
    protected Vector3 m_revivalPosition;
    protected int m_team = 0; /*0: là team xanh, 1: là team đỏ*/
    protected JoytickState m_joystickCrossHairsState;
    protected List<Tank> m_enemies = new List<Tank>();
    protected Tank m_enemyTarget;
    protected float m_minHPenemy;
    protected Vector3 m_movementDirection = Vector3.zero; /* hướng dịch chuyển của xe tăng*/
    protected Vector3 m_turretDirection = Vector3.zero; /* hướng của nòng súng xe tăng*/
    protected float m_moveSpeed;  /* speed là 15 đơn vị/giây (1 đơn vị = 100px)*/
    protected AbTurrent m_currentTurrent; 
    [SerializeField] protected Transform m_leftTrail;
    [SerializeField] protected Transform m_rightTrail;
    protected TankTrail m_rightTrailEffect;
    protected TankTrail m_leftTrailEffect;
    public Transform PositionEffect;
    public Transform PositionUPDamageEffect;
    protected object[] m_syncData;
    private void Awake()
    {
        /*check xem thằng photonview của xe tăng này có phải là của mình không, tránh việc hủy object khi ta đồng bộ scene */
        if (photonView.IsMine) {
            s_localPlayerInstance = this;
        }
        DontDestroyOnLoad(this.gameObject); /*không xóa object khi người chơi vẫn đang chơi, tránh việc xóa object người chơi khác khi có 1 thằng out game*/
    }
    public virtual void OnEnable() {
        PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
        m_joystickCrossHairsState = JoytickState.None;
        if (photonView.IsMine) {
            this.m_predictedTrajectoryPathBullet = m_predictedTrajectoryPathBullet ?? PunObjectPool.Instance.GetLocalPool("Prefabs/Predicted Trajectory Path Bullet", "Predicted Trajectory Path Bullet", Vector3.zero, Quaternion.identity).GetComponent<LineRenderer>();
            this.m_predictedTrajectoryPathBullet.SetPosition(0, Vector3.zero);
            this.m_predictedTrajectoryPathBullet.SetPosition(1, Vector3.zero);
        }
            
        Invoke("InitAutoTarget", 2.0f);
    }
    
    protected virtual void Start()
    {
        m_rigidbody = gameObject.GetComponent<Rigidbody>();
        // m_playerName = PlayFabDatabase.Instance.DisPlayName;
        
        this.InitTankTurrents();
        m_isPlayer = true;
        this.InitTankerStat();


        // if (photonView.IsMine) {
        //     this.photonView.RPC("PunRPC_GetTeam", RpcTarget.MasterClient);
        //     // var eff = PunObjectPool.Instance.GetLocalPool("Prefabs/Tank Bullet/LightningBodyBlue", "LightningBodyBlue", Vector3.zero, Quaternion.identity);
        //     // eff.transform.SetParent(this.gameObject.transform);
        //     // eff.transform.localPosition = new Vector3(0, 1.8f, -0.2f);
        //     // eff.transform.localScale = new Vector3(4, 4, 3.3f);
        // }
    }
    protected virtual void InitTankerStat() {
        m_maxEnergy = 100.0f;
        m_maxHealthy = PlayFabDatabase.Instance.CurrentTankerStat.Healthy;
        m_moveSpeed = PlayFabDatabase.Instance.CurrentTankerStat.MoveSpeed;
        m_currentTurrent.Damage = PlayFabDatabase.Instance.CurrentTankerStat.Damage;

        m_currentEnergy = m_maxEnergy;
        m_currHealthy = m_maxHealthy;
        m_heatlthyLabel.text = m_currHealthy + "";
    }
    protected void InitAutoTarget() {
        StartCoroutine(LoopDectectPlayerCoroutine());
    }
    protected virtual void CreatTrail() {
        m_leftTrailEffect = PunObjectPool.Instance.GetLocalPool("Prefabs/Effect/FireFloorTrail", "FireFloorTrail", m_leftTrail.position, Quaternion.identity).GetComponent<TankTrail>();
        m_rightTrailEffect = PunObjectPool.Instance.GetLocalPool("Prefabs/Effect/FireFloorTrail", "FireFloorTrail", m_rightTrail.position, Quaternion.identity).GetComponent<TankTrail>();
        m_leftTrailEffect.Init(m_leftTrail);
        m_rightTrailEffect.Init(m_rightTrail);
    }
    protected void DisableTrail() {
        try {
            PunObjectPool.Instance.SetLocalPool(m_leftTrailEffect.gameObject);
            PunObjectPool.Instance.SetLocalPool(m_rightTrailEffect.gameObject);
        } catch (Exception error) {

        }
        
    }
    protected virtual void InitTankTurrents() {
        GameObject lv1Turrent = new GameObject();
        lv1Turrent.name = "Turrent Level 1";
        lv1Turrent.AddComponent<Lv1Turrent>();
        lv1Turrent.transform.SetParent(m_transform);
        this.m_currentTurrent = lv1Turrent.GetComponent<Lv1Turrent>();
        this.m_currentTurrent.OriginalTankTurrentPosition = this.m_tankTurret.localPosition;
    }
    
    public virtual void MoveOnPC(float inputHorizontal, float inputVertical) {
        if (inputHorizontal != 0 || inputVertical != 0)
        {
            m_movementDirection = new Vector3(inputHorizontal, 0, inputVertical);
            m_tankChassis.up = -m_movementDirection.normalized;
            m_tankChassis.eulerAngles = new Vector3(-90, m_tankChassis.eulerAngles.y, m_tankChassis.eulerAngles.z);
            if (m_tankChassis.up == -m_movementDirection.normalized) m_transform.Translate(m_movementDirection.normalized * m_moveSpeed * Time.deltaTime, Space.World); /* mỗi 1 frame dịch chuyển được m_moveSpeed * Time.deltaTime đơn vị unity */
        }
    }
    
    public virtual void Move(Joystick joystickMovement) {
        if (joystickMovement.GetJoystickState())
        {
            m_movementDirection = new Vector3(joystickMovement.Horizontal, 0, joystickMovement.Vertical);
            m_tankChassis.up = Vector3.MoveTowards(m_tankChassis.up, -m_movementDirection.normalized, Time.deltaTime*m_moveSpeed);
            m_tankChassis.eulerAngles = new Vector3(-90, m_tankChassis.eulerAngles.y, m_tankChassis.eulerAngles.z);
            if (m_tankChassis.up == -m_movementDirection.normalized) m_transform.Translate(m_movementDirection.normalized * m_moveSpeed * Time.deltaTime, Space.World);/* mỗi 1 frame dịch chuyển được m_moveSpeed * Time.deltaTime đơn vị unity */
        }
    }
    public virtual void Attack(Joystick joytickCrossHairs) {
        if (joytickCrossHairs.GetJoystickState()) {
                m_joystickCrossHairsState = JoytickState.PointDown;
                this.RotateTurrent(joytickCrossHairs);
                this.m_predictedTrajectoryPathBullet.SetPosition(0, m_fireTransform.position);
                this.m_predictedTrajectoryPathBullet.SetPosition(1, m_fireTransform.position + m_tankTurret.up*-40);
        } else {
            if (m_joystickCrossHairsState.Equals(JoytickState.PointDown)) {
                this.m_predictedTrajectoryPathBullet.SetPosition(0, Vector3.zero);
                this.m_predictedTrajectoryPathBullet.SetPosition(1, Vector3.zero);
                
                this.m_currentTurrent.MaxCooldown = 0;
                this.m_currentTurrent.ShootAndSync(m_fireTransform, m_tankTurret, m_turretDirection, m_team, m_playerName, this.photonView.ViewID);
                m_joystickCrossHairsState = JoytickState.None;
            }
        }
    }

    
    public virtual IEnumerator LoopDectectPlayerCoroutine() {
        yield return new WaitForSeconds(1.0f);
        this.AutoTarget();
        StartCoroutine(LoopDectectPlayerCoroutine());
    }
    protected void AutoTarget() {
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
                m_minHPenemy = m_enemies[0].CurrentHealthy;
                m_enemyTarget = m_enemies[0];
                for (int i = 0; i < m_enemies.Count; i++)
                {
                    if (m_minHPenemy > m_enemies[i].CurrentHealthy) {
                        m_minHPenemy = m_enemies[i].CurrentHealthy;
                        m_enemyTarget = m_enemies[i];
                    }
                }
                

            }
        }
    }
    private void RotateTurrent(Joystick joystick) {
        // m_tankTurret.eulerAngles = new Vector3(m_tankTurret.eulerAngles.x, Mathf.Rad2Deg * Mathf.Atan2(joystick.Horizontal, joystick.Vertical), m_tankTurret.eulerAngles.z);
        // m_turretDirection = new Vector3(joystick.Horizontal, 0, joystick.Vertical);

        // if (m_tankTurret.eulerAngles.y > Mathf.Rad2Deg * Mathf.Atan2(joystick.Horizontal, joystick.Vertical)) {
        //     m_tankTurret.eulerAngles = new Vector3(m_tankTurret.eulerAngles.x, m_tankTurret.eulerAngles.y - Time.deltaTime * 5, m_tankTurret.eulerAngles.z);
        //     m_turretDirection = new Vector3(joystick.Horizontal, 0, joystick.Vertical);
        // } else if (m_tankTurret.eulerAngles.y < Mathf.Rad2Deg * Mathf.Atan2(joystick.Horizontal, joystick.Vertical)) {
        //     m_tankTurret.eulerAngles = new Vector3(m_tankTurret.eulerAngles.x, m_tankTurret.eulerAngles.y + Time.deltaTime * 5, m_tankTurret.eulerAngles.z);
        //     m_turretDirection = new Vector3(joystick.Horizontal, 0, joystick.Vertical);
        // }
        var angle = Mathf.Rad2Deg * Mathf.Atan2(joystick.Horizontal, joystick.Vertical);
        m_tankTurret.eulerAngles = new Vector3(m_tankTurret.eulerAngles.x, Mathf.MoveTowardsAngle(m_tankTurret.eulerAngles.y, angle, Time.deltaTime*180), m_tankTurret.eulerAngles.z);
        m_turretDirection = new Vector3(joystick.Horizontal, 0, joystick.Vertical);
        
        
    }
    public void ReduceBlood(float damage, string whoDamage, int whoViewID)
    {
        m_currHealthy -= damage;
        if (m_currHealthy > 0) {
            m_healthyBar.ReduceHealthyBar(damage, m_maxHealthy);
            m_heatlthyLabel.text = m_currHealthy + "";
        } else {
            this.DeathAndSync(whoDamage, whoViewID);
        }
    }
    public void ChangeBlood(float currentHealthy, float maxHealthy) {
        m_healthyBar.SetCurrentHealthy(currentHealthy, maxHealthy);
        m_heatlthyLabel.text = currentHealthy + "";
    }
    /// <summary>
    /// xe tăng bị giật lùi
    /// </summary>
    public void Recoil(float magnitude) {
        StartCoroutine(RecoilCoroutine(magnitude));
    }
    protected IEnumerator RecoilCoroutine(float magnitude) {
        m_rigidbody.velocity = m_turretDirection.normalized *(-1)* magnitude;
        Debug.Log("m_rigidbody.velocity" + m_rigidbody.velocity);
        yield return new WaitForSeconds(2);
        m_rigidbody.velocity = Vector3.zero;
    }
    protected void DeathAndSync(string whoDamage, int whoViewID) {
        if (this.photonView.IsMine && m_isPlayer) {
            Debug.Log("+++++DeathAndSync");
            CameraFollow.Instance.StopFollowPlayer();
            PunObjectPool.Instance.Allow2RevivalMine(this, whoDamage);
            ClientManagement.Instance.UpdateAndSyncScore(m_team);
        }
        if (!m_isPlayer) {
            PunObjectPool.Instance.Allow2RevivalTankBot(this);
            ClientManagement.Instance.UpdateAndSyncScore(m_team);
        }
        ClientManagement.Instance.PersonalScores[this.photonView.ViewID].UpdateDeathLabel();
        ClientManagement.Instance.PersonalScores[whoViewID].UpdateKillingLabel();
        ArenaUI.Instance.ShowKillingNotificationLabel(whoDamage, m_playerName);
        this.SendDispatchDeath(TankEvent.EVENT_SEND_DISPATCH_DEATH, whoDamage, whoViewID); // dù client hay remote thì cũng đều phải send dispatch khi chết để đồng bộ 2 bên 
        PunObjectPool.Instance.GetLocalPool("Prefabs/Effect/ElectricDeath", "ElectricDeath", m_transform.position + Vector3.up*2 , Quaternion.identity);
        this.DisableTrail();
        PunObjectPool.Instance.SetLocalPool(this.gameObject);
    }
    public void RevivalAndSync() {
        if (this.photonView.IsMine) PunObjectPool.Instance.SendDispatch(TankEvent.EVENT_SEND_DISPATCH_REVIVAL, this.photonView.ViewID);
        m_currHealthy = m_maxHealthy;
        m_currentEnergy = m_maxEnergy;
        m_heatlthyLabel.text = m_currHealthy + "";
        m_energyScript.FillUpMaxEnergyBar();
        m_healthyBar.FillUpMaxHealthyBar();
        // this.Visible();
        this.m_transform.position = m_revivalPosition;
        Invoke("CreatTrail", 1.0f);
        CameraFollow.Instance.InitCameraFollow();
    }
    #region Data Synchonization 
    [PunRPC]
    public void Enable(int viewID) {
        // Debug.Log("Enable RPC");
        PhotonView.Find(viewID).gameObject.SetActive(true);
    }
    [PunRPC] 
    public void Disable(int viewID) {
        PhotonView.Find(viewID).gameObject.SetActive(false);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo messageInfo) {
        if (stream.IsWriting) {
            stream.SendNext(m_currHealthy);
        } else {
            m_currHealthy = (float)stream.ReceiveNext();
            m_healthyBar.SetCurrentHealthy(m_currHealthy, m_maxHealthy);
            m_heatlthyLabel.text = m_currHealthy + "";
        }
    }
    
    [PunRPC]
    public void RPC_UpdateTeam(int viewID, int whichTeam, Vector3 revivalPosition) {
        if (photonView.ViewID == viewID) {
            m_team = whichTeam;
            m_revivalPosition = revivalPosition;
            m_transform.position = revivalPosition;
            this.CreatTrail();
            if (m_team != 0) {
                m_healthyBarSpr.sprite = Resources.Load<Sprite>("Sprites/RedBar");
            } else {
                m_healthyBarSpr.sprite = Resources.Load<Sprite>("Sprites/BlueBar");
            }
            this.photonView.RPC("Enable", RpcTarget.All, viewID);
            this.gameObject.tag = "Team" + m_team;
        }
    }
    
    public void SendDispatchShooted(byte eventCode) {
        object[] eventContent = new object[] { this.photonView.ViewID, m_fireTransform.position, m_tankTurret.eulerAngles, m_turretDirection};
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(eventCode, eventContent, raiseEventOptions, SendOptions.SendUnreliable);
    }
    public void SendDispatchShootedNoCountdown(byte eventCode, bool isShooted) {
        switch (eventCode) {
            case TankEvent.EVENT_SEND_DISPATCH_TURRENT_SHOOTED_NO_COUNTDOWN:
                object[] eventContent_1 = new object[] { this.photonView.ViewID, m_fireTransform.position, m_tankTurret.eulerAngles, m_turretDirection, isShooted};
                RaiseEventOptions raiseEventOptions_1 = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                PhotonNetwork.RaiseEvent(eventCode, eventContent_1, raiseEventOptions_1, SendOptions.SendUnreliable);
            break;
            case TankEvent.EVENT_SEND_DISPATCH_TURRENT_SHOOTED_NO_COUNTDOWN_STOP:
                object[] eventContent_2 = new object[] { this.photonView.ViewID, isShooted};
                RaiseEventOptions raiseEventOptions_2 = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                PhotonNetwork.RaiseEvent(eventCode, eventContent_2, raiseEventOptions_2, SendOptions.SendUnreliable);
            break;
        }
        
    }
    public void SendDispatchDeath(byte eventCode, string whoDamage, int whoViewID) {
        object[] eventContent = new object[] { this.photonView.ViewID, whoDamage, whoViewID };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(eventCode, eventContent, raiseEventOptions, SendOptions.SendUnreliable);
    }
    protected void OnEventReceived(EventData photonEnvent) {
        switch (photonEnvent.Code) {
            case TankEvent.EVENT_SEND_DISPATCH_TURRENT_SHOOTED:
                m_syncData = (object[])photonEnvent.CustomData;
                if (this.photonView.ViewID != (int)m_syncData[0]) return;
                m_fireTransform.position = (Vector3)m_syncData[1];
                m_tankTurret.eulerAngles = (Vector3)m_syncData[2];
                m_turretDirection = (Vector3)m_syncData[3];
                this.m_currentTurrent.Shoot(m_fireTransform, m_tankTurret, m_turretDirection, m_team, m_playerName, this.photonView.ViewID);
                break;
            case TankEvent.EVENT_SEND_DISPATCH_TURRENT_SHOOTED_NO_COUNTDOWN:
                m_syncData = (object[])photonEnvent.CustomData;
                if (this.photonView.ViewID != (int)m_syncData[0]) return;
                m_fireTransform.position = (Vector3)m_syncData[1];
                m_tankTurret.eulerAngles = (Vector3)m_syncData[2];
                m_turretDirection = (Vector3)m_syncData[3];
                this.m_currentTurrent.Shoot(m_fireTransform, m_tankTurret, m_turretDirection, (bool)m_syncData[4], m_team, m_playerName, this.photonView.ViewID);
                break;
            case TankEvent.EVENT_SEND_DISPATCH_TURRENT_SHOOTED_NO_COUNTDOWN_STOP:
                m_syncData = (object[])photonEnvent.CustomData;
                if (this.photonView.ViewID != (int)m_syncData[0]) return;
                this.m_currentTurrent.Shoot(m_fireTransform, m_tankTurret, m_turretDirection, (bool)m_syncData[1], m_team, m_playerName, this.photonView.ViewID);
                break;
            case TankEvent.EVENT_SEND_DISPATCH_DEATH:
                m_syncData = (object[])photonEnvent.CustomData;
                if (this.photonView.ViewID != (int)m_syncData[0]) return;
                // Debug.Log("TankEvent.EVENT_SEND_DISPATCH_DEATH");
                this.DeathAndSync((string)m_syncData[1], (int)m_syncData[2]);
                break;
            default:
                break;
        }
    }
    #endregion
    
    public virtual void OnDisable() {
        
        PhotonNetwork.NetworkingClient.EventReceived -= OnEventReceived;
    }
    protected void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Team0") || other.tag.Equals("Team1")) m_boxCollider.isTrigger = false;
       
    }
    public virtual void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag.Equals("Team0") || other.collider.tag.Equals("Team1"))
        {
            m_boxCollider.isTrigger = true;
        }
    }
    #region  property
    public static Tank LocalPlayerInstance
    {
        get {
            return s_localPlayerInstance;
        }
    }
    public int Team 
    {
        get {
            return m_team;
        }
        set {
            m_team = value;
        }
    }
    public bool IsPlayer {
        get {
            return m_isPlayer;
        }
        set {
            m_isPlayer = value;
        }
    }
    public Vector3 Position {
        get {
            return m_transform.position;
        }
        set {
            m_transform.position = value;
        }
    }
    public Vector3 RevivalPosition 
    {
        get {
            return m_revivalPosition;
        }
        set {
            m_revivalPosition = value;
        }
    }
    public string PlayerName {
        get {
            return m_playerName;
        }
        set {
            m_displayNameText.text = m_playerName;
            m_playerName = value;
        }
    }
    public string PathAvatar {
        get {
            return m_pathAvatar;
        }
        set {
            m_pathAvatar = value;
        }
    }
    public float CurrentHealthy {
        get {
            return m_currHealthy;
        }
        set {
            m_currHealthy = value;
        }
    }
    public float CurrentEnergy {
        get {
            return m_currentEnergy;
        }
        set {
            m_currentEnergy = value;
        }
    }
    public EnergyBar EnergyScript {
        get {
            return m_energyScript;
        }
    }
    public float MaxEnergy {
        get {
            return m_maxEnergy;
        }
    }
    public float MaxHealthy {
        get {
            return m_maxHealthy;
        }
    }
    public TankHealthyBar HealthyBarScript {
        get {
            return m_healthyBar;
        }
    }
    public float MoveSpeed {
        get {
            return m_moveSpeed;
        }
        set {
            m_moveSpeed = value;
        }
    }
    public float Damage {
        get {
            return this.m_currentTurrent.Damage;
        }
        set {
            this.m_currentTurrent.Damage = value;
        }
    }
    #endregion
}
