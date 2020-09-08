using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PersonalScoreUI : MonoBehaviour
{
    [SerializeField] private Text m_nameLabel;
    [SerializeField] private Text m_killingLabel;
    [SerializeField] private Text m_deathLabel;
    [SerializeField] private Image m_avatar;
    private string m_name;
    private Tank m_tank;
    private int m_killingCount = 0;
    private int m_deathCount = 0;
    [SerializeField] private RectTransform m_tranform;
    private void Start() {
        this.UpdateName();
        this.UpdateAvatar();
    }
    public void UpdateName() {
        this.m_name = m_tank.PlayerName;
        m_nameLabel.text = m_tank.PlayerName;
    }
    public void UpdateKillingLabel() {
        m_killingCount += 1;
        m_killingLabel.text = "" + m_killingCount;
    }
    public void UpdateDeathLabel() {
        m_deathCount += 1;
        m_deathLabel.text = "" + m_deathCount;
    }
    /// <summary>
    /// </summary>
    /// <param name="pathAvatar">"Avatar/" + index</param>
    public void UpdateAvatar() {
        this.m_avatar.sprite = Resources.Load<Sprite>(m_tank.PathAvatar);
    }
    public void UpdateKillingCount() {
        MissionMangement.Instance.CompetitorKilledInArenaCount = m_killingCount;
        MissionMangement.Instance.CompetitorKilledCount += m_killingCount;
    }
    public Tank Tank {
        get {
            return m_tank;
        }
        set {
            m_tank = value;
        }
    }
    public Vector3 Position {
        get {
            return m_tranform.localPosition;
        }
        set {
            m_tranform.localPosition = value;
        }
    }
}
