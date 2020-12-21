using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillingNotification : MonoBehaviour
{
    [SerializeField] private Text m_whoKillLabel;
    [SerializeField] private Text m_whoDieLabel;

    public void SetText(string whoKillLabel, string whoDieLabel) {
        m_whoKillLabel.text = whoKillLabel;
        m_whoDieLabel.text = whoDieLabel;
    }
}
