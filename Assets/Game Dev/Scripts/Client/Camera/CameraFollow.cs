using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public enum PlayMode {
    None = 0,
    ModeMoba = 1,
    ModeCompetition = 2,

}
public class CameraFollow : MonoBehaviour
{
    private Callback m_methodExcute;
    public PlayMode PlayMode;
    private float elapsed = 0.0f;
    private float m_randomX;
    private float m_randomY;
    [SerializeField] private Transform m_target;
    [SerializeField] private Transform m_transform;
    [SerializeField] private Vector3 m_offset;
    public Vector3 AxisCoordinateVertical;
    public Vector3 AxisCoordinateHorizontal;
    public Vector3 AxisCoordinateVerticalJoystickCrossHairs;
    public Vector3 AxisCoordinateHorizontalJoystickCrossHairs;
    private float m_minX = -102.0f;
    private float m_maxX = 102.0f;
    private float m_minZ = -86.5f;
    private float m_maxZ = 30.5f;
    private float m_clampX;
    private float m_clampZ;
    private float m_movementSpeed = 2f; /*tốc độ di chuyển camera lúc start game là 10 đơn vị 1s*/
    private Vector3 m_stopMovementOnStartGamePosition;
    private bool isStopedFollowing = false;
    private float m_startTime;
    public static CameraFollow Instance {
        get {
            return s_instance;
        }
    }
    private static CameraFollow s_instance;
    // Start is called before the first frame update
    private void Awake() {
         if (s_instance != null && s_instance != this) {
            DestroyImmediate(s_instance);
            return;
        }
        s_instance = this;
    }
    // void Start()
    // {
       
    //     // this.FindPlayer();
    //     // this.InitCameraFollow(); //test
        
    // }
    public void FindPlayer() {
        isStopedFollowing = true;
        StartCoroutine(FindPlayerLoopCorountine());
    }
    public void StopFollowPlayer() {
        isStopedFollowing = true;
    }
    public void InitCameraFollow() {
        isStopedFollowing = false;
        this.ChangeFollowing(PlayMode);
        
        // m_stopMovementOnStartGamePosition = m_target.position + m_offset;
        // m_startTime = Time.time;
        // StartCoroutine(CameraMovementOnStartGameCoroutine());
    }
    private IEnumerator FindPlayerLoopCorountine() {
        if (Tank.LocalPlayerInstance != null || TankCompetition.Instance != null) {
            this.InitCameraFollow();
            yield break;
        }
        yield return null;
        StartCoroutine(FindPlayerLoopCorountine());
    }
    void Update() {
        if (this.m_methodExcute != null) this.m_methodExcute();
    }
    public void FollowFixed() {
        if (!isStopedFollowing) {
            try {
                m_transform.localPosition = m_target.position + m_offset;
                m_clampX = m_transform.localPosition.x > m_maxX ? m_maxX : m_transform.localPosition.x < m_minX ? m_minX : m_transform.localPosition.x;
                m_clampZ = m_transform.localPosition.z > m_maxZ ? m_maxZ : m_transform.localPosition.z < m_minZ ? m_minZ : m_transform.localPosition.z;
                m_transform.localPosition = new Vector3(
                                            m_clampX,
                                            m_transform.localPosition.y,
                                            m_clampZ);
            }
            catch (Exception error) {
                return;
            }
            
        }
    }
    public void FollowModeCompetition() {
            try {
                // m_target = m_target ?? TankCompetition.Instance.m_transform;
                m_transform.position = m_target.position + m_offset;
                // m_transform.localEulerAngles = new Vector3(m_transform.localEulerAngles.x, TankCompetition.Instance.m_tankChassis.localEulerAngles.y, m_transform.localEulerAngles.z);
            }
            catch (Exception error) {
                return;
            }
    }
    public void ChangeFollowing(PlayMode playMode) {
        switch((int)playMode) {
            case 1:
                m_target = Tank.LocalPlayerInstance.transform;
                m_offset = new Vector3(0, 45.0f, -29.0f);
                this.m_methodExcute = FollowFixed;
                break;
            case 2:
                m_target = TankCompetition.Instance.m_transform;
                m_offset = m_transform.position - m_target.position; // di chuyển trong chế độ thi đấu
                // m_offset = new Vector3(0, 6, -5);// khi bắn mục tiêu trong chế độ thi đấu
                AxisCoordinateVertical = -m_target.GetComponent<TankCompetition>().m_tankChassis.up;
                AxisCoordinateHorizontal = m_target.GetComponent<TankCompetition>().m_tankChassis.right;
                this.m_methodExcute = FollowModeCompetition;
                this.ChangeCornerCamera();
                break;
        }
        
    }
    public void ChangeCornerCamera() {
        m_transform.forward = -m_target.GetComponent<TankCompetition>().m_tankChassis.up;
        m_transform.eulerAngles = new Vector3(15, m_transform.eulerAngles.y, m_transform.eulerAngles.z);
        AxisCoordinateVertical = -m_target.GetComponent<TankCompetition>().m_tankChassis.up;
        AxisCoordinateHorizontal = m_target.GetComponent<TankCompetition>().m_tankChassis.right;
        m_transform.position = m_target.GetComponent<TankCompetition>().PosCamera.position;
        m_offset = m_transform.position - m_target.position;
    }
    private IEnumerator CameraMovementOnStartGameCoroutine() {
        if (Vector3.Distance(m_transform.position, m_stopMovementOnStartGamePosition) > 0.0f) {
            float distCovered = (Time.time - m_startTime) * m_movementSpeed; /*khoảng cách đã dịch chuyển được*/
            float fractionOfJourney = distCovered / (Vector3.Distance(m_transform.position, m_stopMovementOnStartGamePosition)); /*phân số quãng đường đi được chia cho quãng đường ban đầu*/
            m_transform.position = Vector3.Lerp(m_transform.position, m_stopMovementOnStartGamePosition, fractionOfJourney);
        } else {
            isStopedFollowing = true;
            yield break;
        } 
        yield return null;
        StartCoroutine(CameraMovementOnStartGameCoroutine());
    }
    public void Shake(float duration, float magnitude) {
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }
    private IEnumerator ShakeCoroutine(float duration, float magnitude) {   
        Vector3 originalPos = m_transform.localPosition;
        elapsed = 0.0f;
        while (elapsed < duration) {
            m_randomX = originalPos.x + UnityEngine.Random.Range(-1f, 1f) * magnitude;
            m_randomY = originalPos.y + UnityEngine.Random.Range(-1f, 1f) * magnitude;
            m_transform.localPosition = new Vector3(m_randomX, m_randomY, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }
        m_transform.localPosition = originalPos;
    }
}
