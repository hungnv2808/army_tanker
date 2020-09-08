using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public enum Rank {
    None = 0,
    Bronze,
    Silver,
    Gold,
    Platinum,
    Diamond,
    Master,
    GrandMaster,
} 
public class MissionMangement : MonoBehaviour
{
    private Hashtable m_misstions;
    private const int ACHIVEMENT_COUNT = 23;
    private const int DAILY_QUEST_COUNT = 6;
    private static MissionMangement s_instance;
    private int m_victoryContinueCount = 0;
    public int CompetitorKilledCount = 0; // tổng số kẻ địch đã hạ gục
    public int CompetitorKilledInArenaCount;
    private int m_firstArenaCount = 0;
    private Rank m_rank;
    [SerializeField] private Achievement m_achievementScriptObjectable;
    [HideInInspector] public bool HasWon = false; 
    public static MissionMangement Instance {
        get {
            return s_instance;
        }
    }
    private void Awake() {
        if (s_instance != null && s_instance != this) {
            Destroy(this.gameObject);
        }
        s_instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    private void Start() {
        this.Init();
    }
    private void Init() {
        m_misstions = new Hashtable()
        {
            {"achievement1", false},
            // nhóm phần thưởng sau khi hoàn thành số trận đầu tiên nhất định (nhóm 1)
            {"achievement2", false},
            {"achievement3", false},
            // nhóm phần thưởng khi thắng số trận liên tục (nhóm 2)
            {"achievement4", false},
            {"achievement5", false},
            {"achievement6", false},
            // nhóm phần thưởng khi hạ gục số đối thủ nhất định trong 1 trận (nhóm 3)
            {"achievement7", false},
            {"achievement8", false},
            {"achievement9", false},
            {"achievement10", false},
            {"achievement11", false},
            // nhóm phần thưởng khi tổng số hạ gục đối thủ nhất định (nhóm 4)
            {"achievement12", false},
            {"achievement13", false},
            {"achievement14", false},
            {"achievement15", false},
            {"achievement16", false},
            // nhóm phần thưởng khi lên rank (nhóm 5)
            {"achievement17", false},
            {"achievement18", false},
            {"achievement19", false},
            {"achievement20", false},
            {"achievement21", false},
            {"achievement22", false},
            // nhóm phần thưởng cho nhiệm vụ hàng ngày
            {"dailyQuest1", false},
            {"dailyQuest2", false},
            {"dailyQuest3", false},
            {"dailyQuest4", false},
            {"dailyQuest5", false},
            {"dailyQuest6", false},
            // biến này để check xem nhóm mission đã hoàn thành chưa, trong DB cũng phải lưu giá trị này để, mỗi lần check mission thì check biến này trước 
            {"isCompleteAchivementGroup_1", false},
            {"isCompleteAchivementGroup_2", false},                                  
            {"isCompleteAchivementGroup_3", false},                                  
            {"isCompleteAchivementGroup_4", false},                                  
            {"isCompleteAchivementGroup_5", false},                                  

        };
    }
    public void CheckMisson() {
        this.CheckAchievementGroup_1();
        this.CheckAchievementGroup_2();
        this.CheckAchievementGroup_3();
        this.CheckAchievementGroup_4();

        this.CheckDailyQuest();
    }
    public void CheckDailyQuest() {
        
    }
    public void LevelupRank(){
        this.CheckAchievementGroup_5();
    }
    public void CheckAchievementGroup_1() {
        if ((bool)m_misstions["isCompleteAchivementGroup_1"]) return;
        
        m_firstArenaCount += 1;
        switch (m_firstArenaCount) {
            case 1:
                if (!(bool)m_misstions["achievement2"]) {
                    m_misstions["achievement2"] = true; 
                    PlayFabDatabase.Instance.SetUserData(new Dictionary<string, string>() {
                        {"achievement2", "true"}
                    });
                    MailManagement.Instance.PushNotification(m_achievementScriptObjectable.AchievementNames[1].Name, 
                                                            m_achievementScriptObjectable.AchievementNames[1].GoldAward,
                                                            m_achievementScriptObjectable.AchievementNames[1].DiamondAward,
                                                            false);
                }
                break;
            case 3:
                if (!(bool)m_misstions["achievement3"]) {
                    m_misstions["achievement3"] = true; 
                    PlayFabDatabase.Instance.SetUserData(new Dictionary<string, string>() {
                        {"achievement3", "true"},
                        {"isCompleteAchivementGroup_1", "true"}
                    });
                    MailManagement.Instance.PushNotification(m_achievementScriptObjectable.AchievementNames[2].Name, 
                                                            m_achievementScriptObjectable.AchievementNames[2].GoldAward,
                                                            m_achievementScriptObjectable.AchievementNames[2].DiamondAward,
                                                            false);
                }
                break;
        }
    }
    public void CheckAchievementGroup_2() {
        if ((bool)m_misstions["isCompleteAchivementGroup_2"]) return;

        if (HasWon) m_victoryContinueCount += 1;
        else m_victoryContinueCount = 0;
        switch (m_victoryContinueCount) {
            case 5:
                if (!(bool)m_misstions["achievement4"]) {
                    m_misstions["achievement4"] = true; 
                    PlayFabDatabase.Instance.SetUserData(new Dictionary<string, string>() {
                        {"achievement4", "true"}
                    });
                    MailManagement.Instance.PushNotification(m_achievementScriptObjectable.AchievementNames[3].Name, 
                                                            m_achievementScriptObjectable.AchievementNames[3].GoldAward,
                                                            m_achievementScriptObjectable.AchievementNames[3].DiamondAward,
                                                            false);
                }
                break;
            case 10:
                if (!(bool)m_misstions["achievement5"]) {
                    m_misstions["achievement5"] = true; 
                    PlayFabDatabase.Instance.SetUserData(new Dictionary<string, string>() {
                        {"achievement5", "true"}
                    });
                    MailManagement.Instance.PushNotification(m_achievementScriptObjectable.AchievementNames[4].Name, 
                                                            m_achievementScriptObjectable.AchievementNames[4].GoldAward,
                                                            m_achievementScriptObjectable.AchievementNames[4].DiamondAward,
                                                            false);
                }
                break;
            case 15:
                if (!(bool)m_misstions["achievement6"]) {
                    m_misstions["achievement6"] = true; 
                    PlayFabDatabase.Instance.SetUserData(new Dictionary<string, string>() {
                        {"achievement6", "true"},
                        {"isCompleteAchivementGroup_2", "true"}
                    });
                    MailManagement.Instance.PushNotification(m_achievementScriptObjectable.AchievementNames[5].Name, 
                                                            m_achievementScriptObjectable.AchievementNames[5].GoldAward,
                                                            m_achievementScriptObjectable.AchievementNames[5].DiamondAward,
                                                            false);
                }
                break;
        }
    }
    public void CheckAchievementGroup_3() {
        if ((bool)m_misstions["isCompleteAchivementGroup_3"]) return;
        switch (this.CompetitorKilledInArenaCount) {
            case 1:
                if (!(bool)m_misstions["achievement7"]) {
                    m_misstions["achievement7"] = true;
                    PlayFabDatabase.Instance.SetUserData(new Dictionary<string, string>() {
                        {"achievement7", "true"},
                    });
                    MailManagement.Instance.PushNotification(m_achievementScriptObjectable.AchievementNames[6].Name, 
                                                            m_achievementScriptObjectable.AchievementNames[6].GoldAward,
                                                            m_achievementScriptObjectable.AchievementNames[6].DiamondAward,
                                                            false);
                }
                break;
            case 3:
                if (!(bool)m_misstions["achievement8"]) {
                    m_misstions["achievement8"] = true;
                    PlayFabDatabase.Instance.SetUserData(new Dictionary<string, string>() {
                        {"achievement8", "true"},
                    });
                    MailManagement.Instance.PushNotification(m_achievementScriptObjectable.AchievementNames[7].Name, 
                                                            m_achievementScriptObjectable.AchievementNames[7].GoldAward,
                                                            m_achievementScriptObjectable.AchievementNames[7].DiamondAward,
                                                            false);
                }
                break;
            case 5:
                if (!(bool)m_misstions["achievement9"]) {
                    m_misstions["achievement9"] = true;
                    PlayFabDatabase.Instance.SetUserData(new Dictionary<string, string>() {
                        {"achievement9", "true"},
                    });
                    MailManagement.Instance.PushNotification(m_achievementScriptObjectable.AchievementNames[8].Name, 
                                                            m_achievementScriptObjectable.AchievementNames[8].GoldAward,
                                                            m_achievementScriptObjectable.AchievementNames[8].DiamondAward,
                                                            false);
                }
                break;
            case 10:
                if (!(bool)m_misstions["achievement10"]) {
                    m_misstions["achievement10"] = true;
                    PlayFabDatabase.Instance.SetUserData(new Dictionary<string, string>() {
                        {"achievement10", "true"},
                    });
                    MailManagement.Instance.PushNotification(m_achievementScriptObjectable.AchievementNames[9].Name, 
                                                            m_achievementScriptObjectable.AchievementNames[9].GoldAward,
                                                            m_achievementScriptObjectable.AchievementNames[9].DiamondAward,
                                                            false);
                }
                break;
            case 15:
                if (!(bool)m_misstions["achievement11"]) {
                    m_misstions["achievement11"] = true;
                    PlayFabDatabase.Instance.SetUserData(new Dictionary<string, string>() {
                        {"achievement11", "true"},
                        {"isCompleteAchivementGroup_3", "true"}
                    });
                    MailManagement.Instance.PushNotification(m_achievementScriptObjectable.AchievementNames[10].Name, 
                                                            m_achievementScriptObjectable.AchievementNames[10].GoldAward,
                                                            m_achievementScriptObjectable.AchievementNames[10].DiamondAward,
                                                            false);
                }
                break;
        }
    }
    public void CheckAchievementGroup_4() {
        if ((bool)m_misstions["isCompleteAchivementGroup_4"]) return;
        switch (this.CompetitorKilledInArenaCount) {
            case 10:
                if (!(bool)m_misstions["achievement12"]) {
                    m_misstions["achievement12"] = true;
                    PlayFabDatabase.Instance.SetUserData(new Dictionary<string, string>() {
                        {"achievement12", "true"},
                    });
                    MailManagement.Instance.PushNotification(m_achievementScriptObjectable.AchievementNames[11].Name, 
                                                            m_achievementScriptObjectable.AchievementNames[11].GoldAward,
                                                            m_achievementScriptObjectable.AchievementNames[11].DiamondAward,
                                                            false);
                }
                break;
            case 15:
                if (!(bool)m_misstions["achievement13"]) {
                    m_misstions["achievement13"] = true;
                    PlayFabDatabase.Instance.SetUserData(new Dictionary<string, string>() {
                        {"achievement13", "true"},
                    });
                    MailManagement.Instance.PushNotification(m_achievementScriptObjectable.AchievementNames[12].Name, 
                                                            m_achievementScriptObjectable.AchievementNames[12].GoldAward,
                                                            m_achievementScriptObjectable.AchievementNames[12].DiamondAward,
                                                            false);
                }
                break;
            case 30:
                if (!(bool)m_misstions["achievement14"]) {
                    m_misstions["achievement14"] = true;
                    PlayFabDatabase.Instance.SetUserData(new Dictionary<string, string>() {
                        {"achievement14", "true"},
                    });
                    MailManagement.Instance.PushNotification(m_achievementScriptObjectable.AchievementNames[13].Name, 
                                                            m_achievementScriptObjectable.AchievementNames[13].GoldAward,
                                                            m_achievementScriptObjectable.AchievementNames[13].DiamondAward,
                                                            false);
                }
                break;
            case 50:
                if (!(bool)m_misstions["achievement15"]) {
                    m_misstions["achievement15"] = true;
                    PlayFabDatabase.Instance.SetUserData(new Dictionary<string, string>() {
                        {"achievement15", "true"},
                    });
                    MailManagement.Instance.PushNotification(m_achievementScriptObjectable.AchievementNames[14].Name, 
                                                            m_achievementScriptObjectable.AchievementNames[14].GoldAward,
                                                            m_achievementScriptObjectable.AchievementNames[14].DiamondAward,
                                                            false);
                }
                break;
            case 100:
                if (!(bool)m_misstions["achievement16"]) {
                    m_misstions["achievement16"] = true;
                    PlayFabDatabase.Instance.SetUserData(new Dictionary<string, string>() {
                        {"achievement16", "true"},
                        {"isCompleteAchivementGroup_4", "true"}
                    });
                    MailManagement.Instance.PushNotification(m_achievementScriptObjectable.AchievementNames[15].Name, 
                                                            m_achievementScriptObjectable.AchievementNames[15].GoldAward,
                                                            m_achievementScriptObjectable.AchievementNames[15].DiamondAward,
                                                            false);
                }
                break;
        }
    }
    public void CheckAchievementGroup_5() {
        switch(m_rank) {                    
            case Rank.Silver:
                m_misstions["achievement17"] = true;
                PlayFabDatabase.Instance.SetUserData(new Dictionary<string, string>() {
                    {"achievement17", "true"},
                });
                MailManagement.Instance.PushNotification(m_achievementScriptObjectable.AchievementNames[16].Name, 
                                                            m_achievementScriptObjectable.AchievementNames[16].GoldAward,
                                                            m_achievementScriptObjectable.AchievementNames[16].DiamondAward,
                                                            false);
                break;
            case Rank.Gold:
                m_misstions["achievement18"] = true;
                PlayFabDatabase.Instance.SetUserData(new Dictionary<string, string>() {
                    {"achievement18", "true"},
                });
                MailManagement.Instance.PushNotification(m_achievementScriptObjectable.AchievementNames[17].Name, 
                                                            m_achievementScriptObjectable.AchievementNames[17].GoldAward,
                                                            m_achievementScriptObjectable.AchievementNames[17].DiamondAward,
                                                            false);
                break;
            case Rank.Platinum:
                m_misstions["achievement19"] = true;
                PlayFabDatabase.Instance.SetUserData(new Dictionary<string, string>() {
                    {"achievement19", "true"},
                });
                MailManagement.Instance.PushNotification(m_achievementScriptObjectable.AchievementNames[18].Name, 
                                                            m_achievementScriptObjectable.AchievementNames[18].GoldAward,
                                                            m_achievementScriptObjectable.AchievementNames[18].DiamondAward,
                                                            false);
                break;
            case Rank.Diamond:
                m_misstions["achievement20"] = true;
                PlayFabDatabase.Instance.SetUserData(new Dictionary<string, string>() {
                    {"achievement20", "true"},
                });
                MailManagement.Instance.PushNotification(m_achievementScriptObjectable.AchievementNames[19].Name, 
                                                            m_achievementScriptObjectable.AchievementNames[19].GoldAward,
                                                            m_achievementScriptObjectable.AchievementNames[19].DiamondAward,
                                                            false);
                break;
            case Rank.Master:
                m_misstions["achievement21"] = true;
                PlayFabDatabase.Instance.SetUserData(new Dictionary<string, string>() {
                    {"achievement21", "true"},
                });
                MailManagement.Instance.PushNotification(m_achievementScriptObjectable.AchievementNames[20].Name, 
                                                            m_achievementScriptObjectable.AchievementNames[20].GoldAward,
                                                            m_achievementScriptObjectable.AchievementNames[20].DiamondAward,
                                                            false);
                break;
            case Rank.GrandMaster:
                m_misstions["achievement22"] = true;
                PlayFabDatabase.Instance.SetUserData(new Dictionary<string, string>() {
                    {"achievement22", "true"},
                    {"isCompleteAchivementGroup_5", "true"}
                });
                MailManagement.Instance.PushNotification(m_achievementScriptObjectable.AchievementNames[21].Name, 
                                                            m_achievementScriptObjectable.AchievementNames[21].GoldAward,
                                                            m_achievementScriptObjectable.AchievementNames[21].DiamondAward,
                                                            false);
                break;
        }
    }
    public Rank Rank {
        get {
            return m_rank;
        }
        set {
            m_rank = value;
        }
    }
    public Hashtable Misstions {
        get {
            return m_misstions;
        }
    }
}
