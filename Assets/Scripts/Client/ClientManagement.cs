using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
public class ClientManagement : MonoBehaviourPun
{
    private static ClientManagement s_instance;
    private int m_team1Killed = 0;
    private int m_team0Killed = 0;
    private Vector3[] m_team0RevivalPositions;
    private Vector3[] m_team1RevivalPositions;
    [SerializeField] private string[] m_listDisplayNameBot;
    private int m_startGameCountdown = 3;
    public int StartGameCountdowm {
        get {
            return m_startGameCountdown;
        }
    }
    private int m_nextPlayersTeam = 0;
    private List<int> m_tankViewIDs;
    private Dictionary<int, PersonalScoreUI> m_personalScores;
    void Awake() {
        if (s_instance != null && s_instance != this)
        {
            DestroyImmediate(this.gameObject);
            return;
        }
        s_instance = this;
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        Debug.Log("######Start client management");
        this.InitRevivalPositions();
        // if (PhotonNetwork.IsMasterClient) {
        //     m_tankViewIDs = new List<int>();
        // }
        m_tankViewIDs = new List<int>();
        this.StartGameCountDown();
    }
    private void InitBotTank(int botCount) {
        List<string> botNames = GetDisplayNameRandom(botCount);
        for (int i = 0; i < botCount; i++)
        {
            var tankClone = PunObjectPool.Instance.GetPunPool("Tank1 Bot", Vector3.left * 150, Quaternion.identity).GetComponent<Tank>();
            tankClone.IsPlayer = false;
            tankClone.PlayerName = botNames[i];
            tankClone.PathAvatar = "Avatar/1";
            m_tankViewIDs.Add(tankClone.photonView.ViewID);
            this.photonView.RPC("RPC_AsyncInfoTank", RpcTarget.Others, tankClone.photonView.ViewID, tankClone.PlayerName, tankClone.PathAvatar);
        }
    }
    public List<string> GetDisplayNameRandom(int nameCount) {
        int rd = UnityEngine.Random.Range(0, 21);
        var names = new List<string>();
        int i = 0;
        while(i < nameCount) {
            if (rd + i >= m_listDisplayNameBot.Length) {
                names.Add(m_listDisplayNameBot[rd + i - m_listDisplayNameBot.Length]);
            } else {
                names.Add(m_listDisplayNameBot[rd + i]);
            }
            i += 1;
        }
        return names;
    }
    
    private void InitRevivalPositions() {
        m_team0RevivalPositions = new Vector3[3];
        m_team1RevivalPositions = new Vector3[3];
        //Vị trí team xanh
        // m_team0RevivalPositions[0] = GenMap.Instance.GridMap[6,2];
        // m_team0RevivalPositions[1] = GenMap.Instance.GridMap[7,2];
        // m_team0RevivalPositions[2] = GenMap.Instance.GridMap[8,2];

        m_team0RevivalPositions[0] = new Vector3(-78.5f, 0, 12);
        m_team0RevivalPositions[1] = new Vector3(-78.5f, 0, -12);
        m_team0RevivalPositions[2] = new Vector3(-67.5f, 0, 0);
        //Vị trí team đỏ
        // m_team1RevivalPositions[0] = GenMap.Instance.GridMap[6,17];
        // m_team1RevivalPositions[1] = GenMap.Instance.GridMap[7,16];
        // m_team1RevivalPositions[2] = GenMap.Instance.GridMap[8,17];

        m_team1RevivalPositions[0] = new Vector3(78.5f, 0, 12);
        m_team1RevivalPositions[1] = new Vector3(78.5f, 0, -12);
        m_team1RevivalPositions[2] = new Vector3(67.5f, 0, 0);
    }
    private bool CheckPunObjectPool() {
        if (PunObjectPool.Instance == null) {
            try {
                PunObjectPool.Instance = GameObject.FindGameObjectWithTag("Pun Object Pool").GetComponent<PunObjectPool>();
            } catch (Exception error) {}
            if (PunObjectPool.Instance == null) {
                Debug.Log("load pool is NOT master client"); // load pool is not master client
                PhotonNetwork.Instantiate("Pun Object Pool", Vector3.zero, Quaternion.identity, 0);
                return true;
            }
            if (PunObjectPool.Instance == null) {
                Debug.LogError("Pool is NUll :(");
                return false;
            }
            return true;
        }
        return true;
    }
    public void CreatPlayer() {
        if (!CheckPunObjectPool()) {
            return;
        }
        if (Tank.LocalPlayerInstance == null) {
            Debug.LogFormat("We are Instantiating LocalPlayer ");
            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            ArenaUI.Instance.HideNotificationPanel();
            var tankClone = PunObjectPool.Instance.GetPunPool("Tank" + (PlayFabDatabase.Instance.IndexTankerChampionSelected+1), Vector3.zero, Quaternion.identity);
            var script = tankClone.GetComponent<Tank>();
            script.photonView.RPC("Disable", RpcTarget.All, script.photonView.ViewID);
            script.IsPlayer = true;
            script.PlayerName = PlayFabDatabase.Instance.DisPlayName;
            script.PathAvatar = PlayFabDatabase.Instance.PathAvatar;
            this.photonView.RPC("RPC_SendPlayerViewID", RpcTarget.MasterClient, script.photonView.ViewID);
            this.photonView.RPC("RPC_AsyncInfoTank", RpcTarget.Others, script.photonView.ViewID, script.PlayerName, script.PathAvatar);
        }
    }
    public IEnumerator CreatTeam() {
        for (int i = 0; i < m_tankViewIDs.Count; i++)
        {
            var tank = PhotonView.Find(m_tankViewIDs[i]).gameObject.GetComponent<Tank>();
            var whichTeam = this.GetTeam();
            tank.Team = whichTeam;
            tank.photonView.RPC("RPC_UpdateTeam", RpcTarget.All, m_tankViewIDs[i], whichTeam, this.GetRevivalPosition(whichTeam, tank.IsPlayer));
            Debug.Log("Creat Team for : " + i);
        }
        yield return new WaitForSeconds(1.0f);
    }
    public void StartGameCountDown() {
        m_startGameCountdown = 3;
        ArenaUI.Instance.ShowCountdownPanel();
        ArenaUI.Instance.ChangeCountdownLabel("" + m_startGameCountdown);
        StartCoroutine(StartGameCountDownCoroutine());
        
        
    }
    private void InitPersonalScore() {
        //TODO: convert mảng m_tankViewIDs sang json rồi gửi
        this.photonView.RPC("PunRPC_AsyncInitPersonalScores", RpcTarget.All, JsonHelper.ToJson<List<int>>(this.m_tankViewIDs));
    }
    public int GetTeam() {
        if (m_nextPlayersTeam == 0) {
            m_nextPlayersTeam = 1;
        } 
        else {
            m_nextPlayersTeam = 0;
        }
        return m_nextPlayersTeam;
    } 
    public Vector3 GetRevivalPosition(int whichTeam, bool isPlayer) {
        if (whichTeam == 0) {
            if (isPlayer) {
                return m_team0RevivalPositions[UnityEngine.Random.Range(0, 3)];
            } else {
                return GenMap.Instance.Team0Roots[UnityEngine.Random.Range(0, 3)].Position;
            }
            
        } 
        else {
            if (isPlayer) {
                return m_team1RevivalPositions[UnityEngine.Random.Range(0, 3)];
            } else {
                return GenMap.Instance.Team1Roots[UnityEngine.Random.Range(0, 3)].Position;
            }
        }
    }
    
    private IEnumerator StartGameCountDownCoroutine() {
        Debug.Log("######StartGameCountDown");
        ArenaUI.Instance.HideLoadingArenaPanel();
        yield return new WaitForSeconds(1.0f);
        m_startGameCountdown -= 1;
        ArenaUI.Instance.ChangeCountdownLabel("" + m_startGameCountdown);
        if (m_startGameCountdown <= 0) {
            yield return new WaitForSeconds(1.0f);
            ArenaUI.Instance.ChangeCountdownLabel("Go");
            yield return new WaitForSeconds(0.9f);
            ArenaUI.Instance.HideCountdownPanel();

            this.CreatPlayer();
            if (PhotonNetwork.IsMasterClient) {
                this.InitBotTank(ServerManagement.MaxPlayersInRoom - PhotonNetwork.CurrentRoom.PlayerCount);
                yield return StartCoroutine(this.CreatTeam());  
                this.InitPersonalScore();
            }
            Tank.LocalPlayerInstance.GetComponent<PhotonView>().RPC("Enable", RpcTarget.All, Tank.LocalPlayerInstance.GetComponent<PhotonView>().ViewID);
            TimerClock.Instance.TurnClock();
            yield break;
        }
        StartCoroutine(StartGameCountDownCoroutine());
    }
    public void UpdateAndSyncScore(int whoDeath) {
        this.photonView.RPC("PunRPC_UpdateScore", RpcTarget.All, whoDeath);
    }
    [PunRPC]
    public void RPC_SendPlayerViewID(int viewID) {
        m_tankViewIDs.Add(viewID);
    }
    [PunRPC]
    public void RPC_AsyncInfoTank(int viewID, string playerName, string pathAvatar) {
        var tank = PhotonView.Find(viewID);
        if (tank != null) {
            var script = tank.gameObject.GetComponent<Tank>();
            script.PlayerName = playerName;
            script.PathAvatar = pathAvatar;
        }
    }
    [PunRPC]
    private void PunRPC_UpdateScore(int whoDeath) {
        if (whoDeath != 1) {
            m_team1Killed += 1;
        } else {
            m_team0Killed += 1;
        }
        ArenaUI.Instance.UpdateScoreLabel(m_team0Killed, m_team1Killed);
    }
    [PunRPC]
    private void PunRPC_AsyncInitPersonalScores(string json) {
        this.m_tankViewIDs = JsonHelper.FormJon<List<int>>(json);
        if (this.m_tankViewIDs != null){
            m_personalScores = DetailScoreUI.Instance.CreatPersonalScoreUI(m_tankViewIDs);
        }
    }


    public int Team1Killed {
        get {
            return m_team1Killed;
        }
        set {
            m_team1Killed = value;
        }
    }
    public int Team0Killed {
        get {
            return m_team0Killed;
        }
        set {
            m_team0Killed = value;
        }
    }
    public int NextPlayerTeam {
        get {
            return m_nextPlayersTeam;
        }
    }
    public Dictionary<int, PersonalScoreUI> PersonalScores {
        get {
            return m_personalScores;
        }
    }
    public static ClientManagement Instance
    {
        get
        {
            return s_instance;
        }
    }
}
