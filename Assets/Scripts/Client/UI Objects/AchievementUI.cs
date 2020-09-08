using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour
{
    [SerializeField] private Transform m_contentScrollView;
    private int m_achievementCount;
    [SerializeField] private Achievement m_achievementScriptObjectable;
    private List<Text> m_achievementTexts;
    [SerializeField] private Sprite[] m_sprites;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Button m_closeButton;
    private int m_groupAchievement_1;
    private static AchievementUI m_instance;
    private List<GameObject> m_iconMissionCompletions;
    public static AchievementUI Instance {
        get {
            return m_instance;
        }
    }
    // Start is called before the first frame update
    private bool m_init = false;
    private void Awake() {
        if (m_instance != null && m_instance != this) {
            Destroy(m_instance.gameObject);
        }
        m_instance = this;
    }
    private void OnEnable()
    {
        if (!m_init) this.Init();
        this.HideCloseButton();
        m_animator.SetBool("isOpened", true);
        Invoke("ShowCloseButton", 0.5f);
        this.RefreshIconMissionCompletion();
    }
    private void Init()
    {
        m_closeButton.onClick.AddListener(OnClose);
        m_init = true;
        m_achievementTexts = new List<Text>();
        m_iconMissionCompletions = new List<GameObject>();
        m_achievementCount = m_contentScrollView.childCount;
        for (int i = 0; i < m_achievementCount; i++)
        {
            m_achievementTexts.Add(m_contentScrollView.GetChild(i).GetChild(0).GetComponent<Text>());
            m_achievementTexts[i].text = m_achievementScriptObjectable.AchievementNames[i].Name;

            GameObject goldImage = new GameObject("Gold Image");
            var goldTransform = goldImage.AddComponent<RectTransform>();
            goldImage.transform.SetParent(m_contentScrollView.GetChild(i));
            var img = goldImage.gameObject.AddComponent<Image>();
            img.sprite = m_sprites[0];
            goldTransform.sizeDelta = new Vector2(30, 30);
            goldTransform.localPosition = new Vector2(192.0f, 14.6f);
            goldTransform.localScale = Vector3.one;

            GameObject goldLabel = new GameObject("Gold Label");
            var goldLabelTransform = goldLabel.AddComponent<RectTransform>();
            goldLabel.transform.SetParent(m_contentScrollView.GetChild(i));
            var label = goldLabel.gameObject.AddComponent<Text>();
            label.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            label.fontSize = 25;
            label.color = Color.white;
            goldLabelTransform.sizeDelta = new Vector2(136.9f, 32.2f);
            goldLabelTransform.localPosition = new Vector2(281.5f, 13.5f);
            goldLabelTransform.localScale = Vector3.one;
            label.text = "" + m_achievementScriptObjectable.AchievementNames[i].GoldAward;
            
            GameObject iconMissionCompletion = new GameObject("Icon Mission Completion");
            iconMissionCompletion.SetActive(false);
            m_iconMissionCompletions.Add(iconMissionCompletion);
            var iconMissionTransform = iconMissionCompletion.AddComponent<RectTransform>();
            iconMissionCompletion.transform.SetParent(m_contentScrollView.GetChild(i));
            var icon = iconMissionCompletion.AddComponent<Image>();
            icon.sprite = this.m_sprites[2];
            iconMissionTransform.sizeDelta = new Vector2(36, 54);
            iconMissionTransform.localPosition = new Vector2(320.0f, 0f);
            iconMissionTransform.localScale = Vector3.one;

            if (m_achievementScriptObjectable.AchievementNames[i].DiamondAward != 0) {
                GameObject diamondImage = new GameObject("Diamond Image");
                var diamondImageTransform = diamondImage.AddComponent<RectTransform>();
                diamondImage.transform.SetParent(m_contentScrollView.GetChild(i));
                var _img = diamondImage.gameObject.AddComponent<Image>();
                _img.sprite = m_sprites[1];
                diamondImageTransform.sizeDelta = new Vector2(30, 30);
                diamondImageTransform.localPosition = new Vector2(192.0f, -17.6f);
                diamondImageTransform.localScale = Vector3.one;

                GameObject diamondLabel = new GameObject("Diamond Label");
                var diamondLabelTransform = diamondLabel.AddComponent<RectTransform>();
                diamondLabel.transform.SetParent(m_contentScrollView.GetChild(i));
                var _label = diamondLabel.gameObject.AddComponent<Text>();
                _label.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                _label.fontSize = 25;
                _label.color = Color.white;
                diamondLabelTransform.sizeDelta = new Vector2(136.9f, 32.2f);
                diamondLabelTransform.localPosition = new Vector2(281.5f, -18.0f);
                diamondLabelTransform.localScale = Vector3.one;
                _label.text = "" + m_achievementScriptObjectable.AchievementNames[i].DiamondAward;
            }
        }
    }
    public void RefreshIconMissionCompletion() {
        for (int i = 1; i <= 22; i++)
        {
            this.m_iconMissionCompletions[i-1].SetActive((bool)MissionMangement.Instance.Misstions["achievement"+i]);
        }
    }

    private void OnClose() {
        this.HideCloseButton();
        m_animator.SetBool("isOpened", false);
        Invoke("Disable", 0.5f);
    }
    private void ShowCloseButton() {
        m_closeButton.gameObject.SetActive(true);
    }
    private void HideCloseButton() {
        m_closeButton.gameObject.SetActive(false);
    }
    public void Disable() {
        gameObject.SetActive(false);
    }
    public Achievement AchievementScriptObjectable {
        get {
            return m_achievementScriptObjectable;
        }
    }
}
