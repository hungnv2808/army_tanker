using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
public class PunObjectPool : MonoBehaviourPun
{
    [SerializeField] private string[] m_punObjectPoolNames;// tên giống với các object chứa trong file resource của photon
    [SerializeField] private string[] m_localObjectPoolNames;
    private Transform m_objectParent;
    public Transform ObjecParent {
        get {
            return m_objectParent;
        }
    }
    private object[] m_syncData;
    private float m_revivalMaxTime = 10.0f;
    private float m_revivalTimer = 0;
    private static PunObjectPool s_instance;
    public static PunObjectPool Instance {
        get {
            return s_instance;
        }
        set {
            s_instance = value;
        }
    }
    private Dictionary<string, HashSet<GameObject>> m_punPools;
    private Dictionary<string, HashSet<GameObject>> m_localPools;
    void Awake() {
        if (s_instance != null && s_instance != this) {
            Destroy(s_instance.gameObject);
        }
        s_instance = this;
        DontDestroyOnLoad(this.gameObject);
        m_objectParent = new GameObject("Objecg Pool").transform;
        m_objectParent.position = Vector3.zero;
    }
    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
    }
    private void Init() {
        Debug.Log("Khoi tao pool");
        m_punPools = new Dictionary<string, HashSet<GameObject>>();
        for (int i = 0; i < m_punObjectPoolNames.Length; i++)
        {
            m_punPools.Add(m_punObjectPoolNames[i], new HashSet<GameObject>());
        }

        m_localPools = new Dictionary<string, HashSet<GameObject>>();
        for (int i = 0; i < m_localObjectPoolNames.Length; i++)
        {
            m_localPools.Add(m_localObjectPoolNames[i], new HashSet<GameObject>());
        }
    }
    public GameObject GetPunPool(string name, Vector3 position, Quaternion quaternion) {
        if (m_punPools == null) {
            this.Init();
        }
        if (m_punPools.ContainsKey(name)) {
            foreach (var i in m_punPools[name])
            {
                if (!i.activeSelf) {
                    i.transform.position = position;
                    i.GetComponent<PhotonView>().RPC("Enable", RpcTarget.All, i.GetComponent<PhotonView>().ViewID);
                    return i;
                }
            }
            var obj = PhotonNetwork.Instantiate(name, position, quaternion, 0);
            m_punPools[name].Add(obj);
            obj.transform.SetParent(m_objectParent);
            return obj;
        } else {
            Debug.LogError("Not exist name object pool :(");
            return null;
        }
    }
    public void SetPunPool(GameObject obj) {
        obj.GetComponent<PhotonView>().RPC("Disable", RpcTarget.All, obj.GetComponent<PhotonView>().ViewID);
        obj.transform.position = Vector3.zero;
    }
    public GameObject GetLocalPool(string resourcePath, string name, Vector3 position, Quaternion quaternion) {
        if (m_localPools == null) {
            this.Init();
        }
        if (m_localPools.ContainsKey(name)) {
            foreach (var i in m_localPools[name])
            {
                if (!i.activeSelf) {
                    i.transform.position = position;
                    i.SetActive(true);
                    return i;
                }
            }
            var obj = Instantiate(Resources.Load<GameObject>(resourcePath), position, quaternion);
            m_localPools[name].Add(obj);
            obj.transform.SetParent(m_objectParent);
            return obj;
        } else {
            Debug.LogError("Not exist name object pool :(");
            return null;
        }
    }
    public void SetLocalPool(GameObject obj) {
        obj.SetActive(false);
        obj.transform.position = Vector3.zero;
    }
    public void Allow2RevivalMine(Tank tankObj, string whoDamage) {
        m_revivalTimer = m_revivalMaxTime;
        ArenaUI.Instance.ShowWaitingForRevivalPanel(whoDamage);
        StartCoroutine(Allow2RevivalCoroutine(tankObj));
    }
    public void Allow2RevivalTankBot(Tank bot) {
        m_revivalTimer = m_revivalMaxTime;
        StartCoroutine(Allow2RevivalTankBotCoroutine(bot));
    }
    private IEnumerator Allow2RevivalTankBotCoroutine(Tank bot) {
        yield return new WaitForSeconds(1.0f);
        m_revivalTimer -= 1;
        if (m_revivalTimer <= 0) {
            bot.RevivalAndSync();
            bot.gameObject.SetActive(true);
            yield break;
        }
        StartCoroutine(Allow2RevivalTankBotCoroutine(bot));
    }
    private IEnumerator Allow2RevivalCoroutine(Tank tankObj) {
        yield return new WaitForSeconds(1.0f);
        m_revivalTimer -= 1;
        ArenaUI.Instance.ChangeCountdownRevivalLabel("" + m_revivalTimer);
        if (m_revivalTimer <= 0) {
            tankObj.RevivalAndSync();
            ArenaUI.Instance.HideWaitingForRevivalPanel();
            tankObj.gameObject.SetActive(true);
            yield break;
        }
        StartCoroutine(Allow2RevivalCoroutine(tankObj));
    }
    #region Received Event from server
    public void SendDispatch(byte eventCode, int photonViewID) {
        object[] eventContent = new object[] { photonViewID };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(eventCode, eventContent, raiseEventOptions, SendOptions.SendUnreliable);
        Debug.Log("Pun object pool SendDispatch");
    }
    private void OnEventReceived (EventData photonEvent) {
        if (photonEvent.Code == TankEvent.EVENT_SEND_DISPATCH_REVIVAL) {
            m_syncData = (object[])photonEvent.CustomData;
            var obj = PhotonView.Find((int)m_syncData[0]);
            obj.gameObject.GetComponent<Tank>().RevivalAndSync();
            obj.gameObject.SetActive(true);
            Debug.Log("Pun object pool OnEventReceived");
        }
        // tankObj.RevivalAndSync();
        // tankObj.gameObject.SetActive(true);
        // if (this.photonView.ViewID != (int)m_syncData[0]) return;
    }
    #endregion
    private void OnDisable() {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEventReceived;
    }
}
