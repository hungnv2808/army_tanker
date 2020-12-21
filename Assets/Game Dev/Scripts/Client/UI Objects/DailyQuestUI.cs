using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyQuestUI : MonoBehaviour
{
    [SerializeField] private Animator m_dailyQuestAnimator;
    public void ShowDailyQuestPanel() {
        m_dailyQuestAnimator.SetBool("isOpened", true);
    }
    public void HideDailyQuestPanel() {
        m_dailyQuestAnimator.SetBool("isOpened", false);
        Invoke("Disable", 0.5f);
    }
    private void Disable() {
        MenuUI.Instance.DailyQuestPanel.SetActive(false);
    }
    private void OnDisable() {
        CancelInvoke();
    }

}
