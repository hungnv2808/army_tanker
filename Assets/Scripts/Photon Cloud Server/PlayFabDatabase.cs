using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;
using System.Threading.Tasks;
using System;
public class PlayFabDatabase : MonoBehaviour
{
    private static PlayFabDatabase s_instance;
    private string m_myDataID;
    private string m_disPlayName;
    private string m_pathAvatar;
    public string PathAvatar {
        get {
            return m_pathAvatar;
        }
        set {
            m_pathAvatar = value;
        }
    }
    public string DisPlayName {
        get {
            return m_disPlayName;
        }
        set {
            m_disPlayName = value;
        }
    }
    private Dictionary<string, UserDataRecord> m_myData;
    public Dictionary<string, UserDataRecord> MyData {
        get {
            return m_myData;
        }
        set {
            m_myData = value;
        }
    }
    public string MyDataID {
        get {
            return m_myDataID;
        }
        set {
            m_myDataID = value;
        }
    }
    public List<AchievementData> m_achievementDatas;
    public List<AchievementData> m_dailyQuestDatas;
    private List<TankerStat> m_tankerChampions;
    private List<AssistanceSkill> m_assistanceSkills;
    private int m_indexTankerChampionSelected;
    private int m_indexAssistanceSkillSelected;
    private InfoTank m_infoTank;
    public InfoTank InfoTank {
        get {
            return m_infoTank;
        }
    }
    public List<TankerStat> TankerChampions {
        get {
            return m_tankerChampions;
        }
    }
    public List<AssistanceSkill> AssistanceSkills {
        get {
            return m_assistanceSkills;
        }
    }
    public int IndexTankerChampionSelected {
        get {
            return m_indexTankerChampionSelected;
        }
        set {
            m_indexTankerChampionSelected = value;
            m_infoTank.IndexTankerChampionSelected = value;
        }
    }
    public int IndexAssistanceSkillSelected {
        get {
            return m_indexAssistanceSkillSelected;
        }
        set {
            m_indexAssistanceSkillSelected = value;
            m_infoTank.IndexAssistanceSkillSelected = value;
        }
    }
    public static PlayFabDatabase Instance {
        get {
            return s_instance;
        }
    }
    private void Awake() {
        if (s_instance != null && s_instance != this) {
            Destroy(s_instance.gameObject);
        }
        s_instance = this;
        DontDestroyOnLoad(s_instance);
    }
    public void InitDatabase() {
        Debug.Log("Start creat database...");
        this.InitData();
        this.UpdateDataServer();
        this.UpdateDataClient();
    }
    public void InitData() {
        m_infoTank = new InfoTank("Avatar/1", 1, 0, 0, 0, 0, 0);
        this.m_pathAvatar = m_infoTank.PathAvatar;
        m_tankerChampions = new List<TankerStat>();
        m_tankerChampions.Add(new TankerStat(0, 0, true, 1000, 7, 250));//tank1
        m_tankerChampions.Add(new TankerStat(1, 300, false, 1000, 9, 500));//tank2
        m_tankerChampions.Add(new TankerStat(2, 500, false, 1000, 10, 700));//drone

        m_assistanceSkills = new List<AssistanceSkill>();
        m_assistanceSkills.Add(new AssistanceSkill(0, true));// phép bổ trợ tăng máu,
        m_assistanceSkills.Add(new AssistanceSkill(1, false));// phép bổ tăng tốc,
        m_assistanceSkills.Add(new AssistanceSkill(2, false));// phép bổ tăng sát thương,
        m_assistanceSkills.Add(new AssistanceSkill(3, false));// phép bổ trợ ném bom,
        
        m_achievementDatas = new List<AchievementData>();
        m_achievementDatas.Add(new AchievementData(1, false, 0));
        m_achievementDatas.Add(new AchievementData(2, false, 0));
        m_achievementDatas.Add(new AchievementData(3, false, 0));
        m_achievementDatas.Add(new AchievementData(4, false, 0));
        m_achievementDatas.Add(new AchievementData(5, false, 0));
        m_achievementDatas.Add(new AchievementData(6, false, 0));
        m_achievementDatas.Add(new AchievementData(7, false, 0));
        m_achievementDatas.Add(new AchievementData(8, false, 0));
        m_achievementDatas.Add(new AchievementData(9, false, 0));
        m_achievementDatas.Add(new AchievementData(10, false, 0));
        m_achievementDatas.Add(new AchievementData(11, false, 0));
        m_achievementDatas.Add(new AchievementData(12, false, 0));
        m_achievementDatas.Add(new AchievementData(13, false, 0));
        m_achievementDatas.Add(new AchievementData(14, false, 0));
        m_achievementDatas.Add(new AchievementData(15, false, 0));
        m_achievementDatas.Add(new AchievementData(16, false, 0));
        m_achievementDatas.Add(new AchievementData(17, false, 0));
        m_achievementDatas.Add(new AchievementData(18, false, 0));
        m_achievementDatas.Add(new AchievementData(19, false, 0));
        m_achievementDatas.Add(new AchievementData(20, false, 0));
        m_achievementDatas.Add(new AchievementData(21, false, 0));
        m_achievementDatas.Add(new AchievementData(22, false, 0));

        m_dailyQuestDatas = new List<AchievementData>();
        m_dailyQuestDatas.Add(new AchievementData(1, false, 0));
        m_dailyQuestDatas.Add(new AchievementData(2, false, 0));
        m_dailyQuestDatas.Add(new AchievementData(3, false, 0));
        m_dailyQuestDatas.Add(new AchievementData(4, false, 0));
        m_dailyQuestDatas.Add(new AchievementData(5, false, 0));
        m_dailyQuestDatas.Add(new AchievementData(6, false, 0));
    }
    public async Task GetAllData() {
        var data = await PlayFabDatabase.Instance.GetUserData(PlayFabDatabase.Instance.MyDataID, new List<string>() {
                        {"date"},
                        {"infoTank"},
                        // {"competitorKilledCount", "0"},
                        {"tankerChampion"},
                        {"assistanceSkill"},
                        {"achievement"},
                        {"dailyQuest"},
                    });
        
        DateTime timeDataServer = DateTime.Parse(data["date"].Value);
        if (!PlayerPrefs.HasKey("date")) {
            Debug.Log("load dữ liệu từ server");
            this.HandleDataServer(data);
            return;
        }
        DateTime timeDataLocal = DateTime.Parse(PlayerPrefs.GetString("date"));
        int resultDateTimeCompare = DateTime.Compare(timeDataServer, timeDataLocal);
        // if (resultDateTimeCompare < 0) { // time save server is earlier than time save local
        //     //load dữ liệu từ local và đồng bộ dữ liệu từ local lên server
        //     Debug.Log("load dữ liệu từ local");
        //     this.HandleDataLocal(data);
        // } else {
        //     // load dữ liệu từ server
        //     Debug.Log("load dữ liệu từ server");
        //     Debug.Log(data["tankerChampion"].Value);
        //     this.HandleDataServer(data);
            
        // }
        //load dữ liệu từ server
        Debug.Log("load dữ liệu từ server");
        Debug.Log(data["tankerChampion"].Value);
        this.HandleDataServer(data);
        
        // if (data.ContainsKey("competitorKilledCount")) MissionMangement.Instance.CompetitorKilledCount = int.Parse(data["competitorKilledCount"].Value);
        // for (int i = 1; i <= 22; i++)
        // {
        //     if (data.ContainsKey("achievement"+i)) {
        //         MissionMangement.Instance.Misstions["achievement"+i] = bool.Parse(data["achievement"+i].Value);
        //     }
        // }
        // for (int i = 1; i <= 6; i++)
        // {
        //     if (data.ContainsKey("dailyQuest"+i)) {
        //         MissionMangement.Instance.Misstions["dailyQuest"+i] = bool.Parse(data["dailyQuest"+i].Value);
        //     }
        // }
        // for (int i = 1; i <= 5; i++)
        // {
        //     if (data.ContainsKey("isCompleteAchivementGroup_"+i)) {
        //         MissionMangement.Instance.Misstions["isCompleteAchivementGroup_"+i] = bool.Parse(data["isCompleteAchivementGroup_"+i].Value);
        //     }
        // }
        
    }
    private void HandleDataServer(Dictionary<string, UserDataRecord> data) {
        if (data.ContainsKey("infoTank")) { 
            this.m_infoTank = JsonHelper.FormJon<InfoTank>(data["infoTank"].Value);
            this.m_pathAvatar = m_infoTank.PathAvatar;
            this.m_indexTankerChampionSelected = m_infoTank.IndexTankerChampionSelected;
            this.m_indexAssistanceSkillSelected = m_infoTank.IndexAssistanceSkillSelected;
            CurrencyManagement.Instance.VioletStar = m_infoTank.VioletStar;
            CurrencyManagement.Instance.GoldStar = m_infoTank.GoldStar;
        }
        if (data.ContainsKey("tankerChampion")) m_tankerChampions = JsonHelper.FormJon<List<TankerStat>>(data["tankerChampion"].Value);
        if (data.ContainsKey("assistanceSkill")) m_assistanceSkills = JsonHelper.FormJon<List<AssistanceSkill>>(data["assistanceSkill"].Value);
        //TODO : load dữ liệu của achievement và daily quest
    }
    private void HandleDataLocal(Dictionary<string, UserDataRecord> data) {
        if (!PlayerPrefs.HasKey("infoTank") || !PlayerPrefs.HasKey("tankerChampion") || !PlayerPrefs.HasKey("assistanceSkill")) {
            Debug.Log("Dữ liệu local đã bị mất, đang load dữ liệu từ server");
            this.HandleDataServer(data);
        }
        if (PlayerPrefs.HasKey("infoTank")) { 
            this.m_infoTank = JsonHelper.FormJon<InfoTank>(PlayerPrefs.GetString("infoTank"));
            this.m_pathAvatar = m_infoTank.PathAvatar;
            this.m_indexTankerChampionSelected = m_infoTank.IndexTankerChampionSelected;
            this.m_indexAssistanceSkillSelected = m_infoTank.IndexAssistanceSkillSelected;
            CurrencyManagement.Instance.VioletStar = m_infoTank.VioletStar;
            CurrencyManagement.Instance.GoldStar = m_infoTank.GoldStar;
        }
        if (PlayerPrefs.HasKey("tankerChampion")) m_tankerChampions = JsonHelper.FormJon<List<TankerStat>>(PlayerPrefs.GetString("tankerChampion"));
        if (PlayerPrefs.HasKey("assistanceSkill")) m_assistanceSkills = JsonHelper.FormJon<List<AssistanceSkill>>(PlayerPrefs.GetString("assistanceSkill"));
        //TODO : load dữ liệu của achievement và daily quest
    }
    public void UpdateDataServer() {
        string infoTankJson = JsonHelper.ToJson<InfoTank>(m_infoTank);
        string tankerChampionJson = JsonHelper.ToJson<List<TankerStat>>(m_tankerChampions);
        string assistanceSkillJson = JsonHelper.ToJson<List<AssistanceSkill>>(m_assistanceSkills);
        string achievementDataJson = JsonHelper.ToJson<List<AchievementData>>(m_achievementDatas);
        string dailyQuestDataJson = JsonHelper.ToJson<List<AchievementData>>(m_dailyQuestDatas);
        this.SetUserData(new Dictionary<string, string>() {
            //giới hạn update chỉ update đc 10 key, get data tối đa đc 12
            {"date", DateTime.Now.ToString()},
            {"infoTank", infoTankJson},
            // {"competitorKilledCount", "0"},
            {"tankerChampion", tankerChampionJson},
            {"assistanceSkill", assistanceSkillJson},
            {"achievement", achievementDataJson},
            {"dailyQuest", dailyQuestDataJson},
        });
        Debug.Log("Update success data server!");
    }
    public void UpdateDataClient() {
        string infoTankJson = JsonHelper.ToJson<InfoTank>(m_infoTank);
        string tankerChampionJson = JsonHelper.ToJson<List<TankerStat>>(m_tankerChampions);
        string assistanceSkillJson = JsonHelper.ToJson<List<AssistanceSkill>>(m_assistanceSkills);
        string achievementDataJson = JsonHelper.ToJson<List<AchievementData>>(m_achievementDatas);
        string dailyQuestDataJson = JsonHelper.ToJson<List<AchievementData>>(m_dailyQuestDatas);
        PlayerPrefs.SetString("date", DateTime.Now.ToString());
        PlayerPrefs.SetString("infoTank", infoTankJson);
        PlayerPrefs.SetString("tankerChampion", tankerChampionJson);
        PlayerPrefs.SetString("assistanceSkill", assistanceSkillJson);
        PlayerPrefs.SetString("achievement", achievementDataJson);
        PlayerPrefs.SetString("dailyQuest", dailyQuestDataJson);
        Debug.Log("Update success data local!");

    }
    public void SetUserData(Dictionary<string, string> data) {
        // return;
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest() {
            Data = data
        },
        resultCallback => Debug.Log("Successfully update user data"),
        errorCallback => {
            Debug.Log("Error update user data");
            Debug.LogError(errorCallback.GenerateErrorReport());
        });
    }
    // public async Task<Dictionary<string, UserDataRecord>> GetUserData(string playerDataID, List<string> keys) {
    //     return await ExcuteTask(playerDataID, keys);
    // }
    public async Task<Dictionary<string, UserDataRecord>> GetUserData(string playerDataID, List<string> keys) {
        int isComplete = 0;
        Dictionary<string, UserDataRecord> userDataResult = null;
        PlayFabClientAPI.GetUserData(new GetUserDataRequest() {
            PlayFabId = playerDataID,
            Keys = keys // các trường mà mình muốn lấy từ db về 
        }, result => {
            Debug.Log("Got user data:");
            if (result.Data == null) {
                Debug.Log("No data");
            } 
            else {
                userDataResult = result.Data;
                isComplete = 400;
                Debug.Log("1 + " + userDataResult);
                Debug.Log("Get user data success :)");
            }
        }, error => {
            isComplete = 404;
            Debug.Log("Got error retrieving user data:");
            Debug.LogError(error.GenerateErrorReport());
        });
        while(isComplete == 0) {
            await Task.Yield();
        }
        return userDataResult;
    }
    private async Task UpdatePlayerStatistics(List<StatisticUpdate> statistics) {
        int isComplete = 0;
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest {
            Statistics = statistics
        },
            result => { isComplete = 400; Debug.Log("User statistics updated"); },
            error => { isComplete = 404; Debug.LogError(error.GenerateErrorReport()); }
        );
        while(isComplete == 0) {
            await Task.Yield();
        }
    }
    public async void UpdateResultRound1() {
        await PlayFabDatabase.Instance.UpdatePlayerStatistics(new List<StatisticUpdate> {
                    new StatisticUpdate {StatisticName = "ResultRound1", Value = (int)CompetitionUI.Instance.LerpTime}
                });
        // update xong chuyển đến màn hình xếp hạng
        /*
        màn thi đấu diễn ra trong 30 phút cả 2 vòng: thí sinh nào không hoàn thành các vòng chơi trong 30 phút sẽ bị loại khỏi cuộc chơi (thua cuộc)
        bảng xếp hạng sẽ có list những player hoàn thành vòng loại
        và có label hiển thị số người chơi hoàn thành/tổng số người tham gia
        */
        
    }
    public string GetPlayerProfile(string id) {
        string displayerName = null;
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest {
            PlayFabId = id,
            ProfileConstraints = new PlayerProfileViewConstraints() {
                ShowDisplayName = true
            }
        }, resultCallback => {
            displayerName = resultCallback.PlayerProfile.DisplayName;
            Debug.Log("" + displayerName);
        }, errorCallback => {
            displayerName = null;
            Debug.LogError("lỗi get player profile: " + errorCallback.GenerateErrorReport());
        });
        return displayerName;
    }
    // private void OnApplicationFocus(bool focusStatus) {
    //     Debug.Log("App focus");
    //     this.UpdateDataClient();
    //     this.UpdateDataServer();
    // }
    // private void OnApplicationPause(bool pauseStatus) {
        
    // }
    private void OnApplicationQuit() {
        Debug.Log("App Quit");
        this.UpdateDataClient();
        this.UpdateDataServer();
    }
    public TankerStat CurrentTankerStat {
        get {
            return this.m_tankerChampions[this.m_indexTankerChampionSelected];
        }
    }
}
