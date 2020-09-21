using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingUI : MonoBehaviour
{
    [SerializeField] private GameObject[] m_displayModels;
    [SerializeField] private SpriteRenderer m_assistanceSpR;
    [SerializeField] private Sprite[] m_assistanceSkillSprites;
    private void Start()
    {
        m_displayModels[PlayFabDatabase.Instance.IndexTankerChampionSelected].SetActive(true);
        m_assistanceSpR.sprite = m_assistanceSkillSprites[PlayFabDatabase.Instance.IndexAssistanceSkillSelected];
    }
}
