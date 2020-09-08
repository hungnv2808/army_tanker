using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BombPow : MonoBehaviour, IAssistanceSkill
{
    private float m_launchPow = 45; // lực ban đầu
    private float m_anglePow; // góc ném ban đầu theo radian
    private Vector3 m_Vo; // vận tốc ban đầu
    private Transform m_iconRange;
    private Transform shootPoint;
    private LineRenderer m_predictedTrajectoryPathBomb;
    private int lineSegment = 20;
    private float m_totalTime = 0.0f;
    private float m_timeCountdown = 60.0f; //30s;
    private float m_lerpTime = 0.0f;
    private JoytickState m_joystickState;
    private Tank m_tankLocalPlayer;
    private bool m_hasSkillReady = true;
    [SerializeField] private Image m_refreshImage;
    [SerializeField] private Text m_secondLabel;
 
    // biết vận tốc ban đầu, biết góc ném (góc giữ Oxz và Oy) => thòi gian, khoảng cách xa nhất...
 
    // Start is called before the first frame update
    private void Start()
    {
        m_predictedTrajectoryPathBomb = PunObjectPool.Instance.GetLocalPool("Prefabs/Predicted Trajectory Path Bomb","Predicted Trajectory Path Bomb", Vector3.zero, Quaternion.identity).GetComponent<LineRenderer>();
        m_iconRange = PunObjectPool.Instance.GetLocalPool("Prefabs/Bomb Range","Bomb Range", Vector3.zero, Quaternion.identity).transform;
        m_iconRange.eulerAngles = new Vector3(90, 0, 0);
        m_iconRange.gameObject.SetActive(false);
        m_predictedTrajectoryPathBomb.positionCount = lineSegment;
        m_hasSkillReady = true;
        m_refreshImage.gameObject.SetActive(false);
        m_joystickState = JoytickState.None;
    }
 
    public void RefreshSkill() {
        Debug.Log("RefreshSkill");
        m_lerpTime = m_timeCountdown;
        m_hasSkillReady = false;
        m_refreshImage.gameObject.SetActive(true);
        m_refreshImage.fillAmount = 1;
        m_secondLabel.text = "" + m_lerpTime;
        StartCoroutine(RefreshSkillLoopCoroutine());
    }
    private IEnumerator RefreshSkillLoopCoroutine() {
        yield return new WaitForSeconds(1.0f);
        m_lerpTime -= 1;
        m_secondLabel.text = "" + m_lerpTime;
        m_refreshImage.fillAmount = m_lerpTime/m_timeCountdown;
        if (m_lerpTime <= 0) {
            m_hasSkillReady = true;
            m_refreshImage.gameObject.SetActive(false);
            yield break;
        } else {
            StartCoroutine(RefreshSkillLoopCoroutine());
        }
    }

    public void Work(Joystick joystickAssistanceSkill)
    {   
        if (!m_hasSkillReady) return;

        if (Tank.LocalPlayerInstance == null) return;
        m_tankLocalPlayer = Tank.LocalPlayerInstance.GetComponent<Tank>();
        if (shootPoint == null) shootPoint = m_tankLocalPlayer.BombPowPoint;

        if (joystickAssistanceSkill.GetJoystickState()) {
            m_joystickState = JoytickState.PointDown;
            Vector2 directionXZ = new Vector2(joystickAssistanceSkill.Horizontal, joystickAssistanceSkill.Vertical);

            m_anglePow = directionXZ.magnitude * 45  * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(joystickAssistanceSkill.Horizontal, directionXZ.magnitude * Mathf.Tan(m_anglePow), joystickAssistanceSkill.Vertical);
            this.m_Vo = direction.normalized * m_launchPow * direction.magnitude;
            m_predictedTrajectoryPathBomb.gameObject.SetActive(true);
            Visualize(m_Vo);
            m_iconRange.gameObject.SetActive(true);

            //tổng thời gian bay  = thời gian vật đạt độ cao cực đại  + thời gian vật từ độ cao cực đại đến mặt đất
            m_totalTime = m_Vo.magnitude * Mathf.Sin(m_anglePow) / Mathf.Abs(Physics.gravity.y) + Mathf.Sqrt( (m_Vo.magnitude*m_Vo.magnitude*Mathf.Sin(m_anglePow)* Mathf.Sin(m_anglePow) + 2*shootPoint.localPosition.y*Mathf.Abs(Physics.gravity.y)) / (Mathf.Abs(Physics.gravity.y)*Mathf.Abs(Physics.gravity.y)) );
            Vector3 range = shootPoint.position + m_Vo * m_totalTime;
            m_iconRange.position = new Vector3(range.x, 0, range.z);
            
        } else {
            if (m_joystickState.Equals(JoytickState.PointDown)) {
                m_joystickState = JoytickState.None;

                m_predictedTrajectoryPathBomb.gameObject.SetActive(false);
                m_iconRange.gameObject.SetActive(false);
                var bombScript = PunObjectPool.Instance.GetLocalPool("Prefabs/Bomb", "Bomb", shootPoint.position, Quaternion.identity).GetComponent<Bomb>();
                bombScript.InfoBomb(m_tankLocalPlayer.PlayerName, m_tankLocalPlayer.Team, m_tankLocalPlayer.photonView.ViewID);
                bombScript.Rigidbody.velocity = m_Vo;
                bombScript.Rigidbody.AddTorque(Vector3.one * 360, ForceMode.Impulse);

                this.RefreshSkill();
            }
            
        }
        
    }
    private void Visualize(Vector3 vo)
    {
        for (int i = 0; i < lineSegment; i++)
        {
            Vector3 pos = CalculatePosInTime(vo, i * m_totalTime / (float)lineSegment);
            m_predictedTrajectoryPathBomb.SetPosition(i, pos);
        }
    }
    
    /*
    * link tham khảo các công thức tính toán https://vatlypt.com/chuyen-de-chuyen-dong-nem-ngang-nem-xien-vat-ly-pho-thong.t26.html,
    * https://www.youtube.com/watch?v=6mJMmF5sLxk&t=21s
    * https://www.youtube.com/watch?v=3DUmpVi82q8
    */
    /// <summary>
    /// lấy toạ độ của vật theo thời gian tính từ thời điểm bắt đầu bay
    /// </summary>
    /// <param name="vo"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    private Vector3 CalculatePosInTime(Vector3 vo, float time)
    {
        Vector3 result = shootPoint.position + (vo* time) + (0.5f * Physics.gravity * (time * time)); // tính tọa độ trục Oxyz: P = Po + Vo * time + (g * t^2)/2
        return result;
    }
}
