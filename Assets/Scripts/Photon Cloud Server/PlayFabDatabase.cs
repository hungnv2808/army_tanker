using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;
using System.Threading.Tasks;
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
    public void InitUserData() {
        this.SetUserData(new Dictionary<string, string>() {
            {"avatar", "Avatar/1"},
            {"goldStar", "0"},
            {"violetStar", "0"},
            {"rank", "1"},
            {"rankIcon", "Avatar/rank_7"},
            {"pointRank", "0"},
            {"competitorKilledCount", "0"},

            {"achievement1", "false"},
            {"achievement2", "false"},
            {"achievement3", "false"},
            {"achievement4", "false"},
            {"achievement5", "false"},
            {"achievement6", "false"},
            {"achievement7", "false"},
            {"achievement8", "false"},
            {"achievement9", "false"},
            {"achievement10", "false"},
            {"achievement11", "false"},
            {"achievement12", "false"},
            {"achievement13", "false"},
            {"achievement14", "false"},
            {"achievement15", "false"},
            {"achievement16", "false"},
            {"achievement17", "false"},
            {"achievement18", "false"},
            {"achievement19", "false"},
            {"achievement20", "false"},
            {"achievement21", "false"},
            {"achievement22", "false"},
            {"dailyQuest1", "false"},
            {"dailyQuest2", "false"},
            {"dailyQuest3", "false"},
            {"dailyQuest4", "false"},
            {"dailyQuest5", "false"},
            {"dailyQuest6", "false"},
            {"isCompleteAchivementGroup_1", "false"},
            {"isCompleteAchivementGroup_2", "false"},                                  
            {"isCompleteAchivementGroup_3", "false"},                                  
            {"isCompleteAchivementGroup_4", "false"},                                  
            {"isCompleteAchivementGroup_5", "false"},
        });
    }
    public async Task GetData() {
        var data = await PlayFabDatabase.Instance.GetUserData(PlayFabDatabase.Instance.MyDataID, new List<string>() {
                        {"avatar"},
                        {"goldStar"},
                        {"violetStar"},
                        {"rank"},
                        {"rankIcon"},
                        {"pointRank"},
                        {"competitorKilledCount"},

                        {"achievement1"},
                        // nhóm phần thưởng sau khi hoàn thành số trận đầu tiên nhất định (nhóm 1)
                        {"achievement2"},
                        {"achievement3"},
                        // nhóm phần thưởng khi thắng số trận liên tục (nhóm 2)
                        {"achievement4"},
                        {"achievement5"},
                        {"achievement6"},
                        // nhóm phần thưởng khi hạ gục số đối thủ nhất định trong 1 trận (nhóm 3)
                        {"achievement7"},
                        {"achievement8"},
                        {"achievement9"},
                        {"achievement10"},
                        {"achievement11"},
                        // nhóm phần thưởng khi tổng số hạ gục đối thủ nhất định (nhóm 4)
                        {"achievement12"},
                        {"achievement13"},
                        {"achievement14"},
                        {"achievement15"},
                        {"achievement16"},
                        // nhóm phần thưởng khi lên rank (nhóm 5)
                        {"achievement17"},
                        {"achievement18"},
                        {"achievement19"},
                        {"achievement20"},
                        {"achievement21"},
                        {"achievement22"},
                        // nhóm phần thưởng cho nhiệm vụ hàng ngày
                        {"dailyQuest1"},
                        {"dailyQuest2"},
                        {"dailyQuest3"},
                        {"dailyQuest4"},
                        {"dailyQuest5"},
                        {"dailyQuest6"},
                        // biến này để check xem nhóm mission đã hoàn thành chưa, trong DB cũng phải lưu giá trị này để, mỗi lần check mission thì check biến này trước 
                        {"isCompleteAchivementGroup_1"},
                        {"isCompleteAchivementGroup_2"},                                  
                        {"isCompleteAchivementGroup_3"},                                  
                        {"isCompleteAchivementGroup_4"},                                  
                        {"isCompleteAchivementGroup_5"},
                    });
        if (data.ContainsKey("goldStar")) CurrencyManagement.Instance.GoldStar = int.Parse(data["goldStar"].Value);
        if (data.ContainsKey("violetStar")) CurrencyManagement.Instance.VioletStar = int.Parse(data["violetStar"].Value);
        if (data.ContainsKey("competitorKilledCount")) MissionMangement.Instance.CompetitorKilledCount = int.Parse(data["competitorKilledCount"].Value);
        for (int i = 1; i <= 22; i++)
        {
            if (data.ContainsKey("achievement"+i)) {
                MissionMangement.Instance.Misstions["achievement"+i] = bool.Parse(data["achievement"+i].Value);
            }
        }
        for (int i = 1; i <= 6; i++)
        {
            if (data.ContainsKey("dailyQuest"+i)) {
                MissionMangement.Instance.Misstions["dailyQuest"+i] = bool.Parse(data["dailyQuest"+i].Value);
            }
        }
        for (int i = 1; i <= 5; i++)
        {
            if (data.ContainsKey("isCompleteAchivementGroup_"+i)) {
                MissionMangement.Instance.Misstions["isCompleteAchivementGroup_"+i] = bool.Parse(data["isCompleteAchivementGroup_"+i].Value);
            }
        }
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
}
