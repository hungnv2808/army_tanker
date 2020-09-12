using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Facebook.Unity;
using PlayFab;
using PlayFab.ClientModels;
using LoginResult = PlayFab.ClientModels.LoginResult;
public class Login : MonoBehaviour
{
    [SerializeField] private Transform m_loadingImage;
    [SerializeField] private GameObject m_controlPanel;
    [SerializeField] private Image m_splashImage;
    [SerializeField] private Button m_loginButton;
    [SerializeField] private GameObject m_loginErrorPanel;
    [SerializeField] private GameObject m_settingNamePanel;
    [SerializeField] private InputField m_inputField;
    [SerializeField] private Text m_errorSettingNameLabel;
    private bool isRotateLoadingImageStopped = false;
    private string m_myToken;
    // Start is called before the first frame update
    void Start()
    {
        m_controlPanel.SetActive(false);
        m_loadingImage.gameObject.SetActive(false);
        Invoke("RotateLoadingImage", 1.0f);
        m_loginErrorPanel.SetActive(false);
        // This call is required before any other calls to the Facebook API. We pass in the callback to be invoked once initialization is finished
        
    }
    
    private void RotateLoadingImage() {
        m_splashImage.color = new Color(m_splashImage.color.r, m_splashImage.color.g, m_splashImage.color.b, 0.5f);
        m_loadingImage.gameObject.SetActive(true);
        isRotateLoadingImageStopped = false;
        StartCoroutine(RotateLoadingImageCoroutine());
        this.CheckAutoLoginFB();
    }
    private IEnumerator RotateLoadingImageCoroutine() {
        if (isRotateLoadingImageStopped) {
            m_splashImage.color = new Color(m_splashImage.color.r, m_splashImage.color.g, m_splashImage.color.b, 1);
            m_loadingImage.gameObject.SetActive(false);
            yield break;
        } 
        m_loadingImage.Rotate(0, 0, Time.deltaTime * (-720.0f)); /*tốc độ quay 720 độ 1s*/
        yield return null;
        StartCoroutine(RotateLoadingImageCoroutine());
    }
    public void OnLogin() {
        FB.Init(OnFacebookInitialized, OnHideUnity);  
        m_loginButton.interactable = false;
        m_loginErrorPanel.SetActive(false);
        m_loadingImage.gameObject.SetActive(true);
        isRotateLoadingImageStopped = false;
        StartCoroutine(RotateLoadingImageCoroutine());
    }
    private void OnFacebookInitialized()
    {
        Debug.Log("FB init done!");
        // Once Facebook SDK is initialized, if we are logged in, we log out to demonstrate the entire authentication cycle.
        if (FB.IsLoggedIn) {
            Debug.Log("FB logged in!");
        } else {
            this.FBLogin();
        }
    }
    private void OnHideUnity(bool isGameShow) {
        if (!isGameShow) {
            Time.timeScale = 0;
        } else {
            Time.timeScale = 1;
        }
    }
    private void CheckAutoLoginFB() {
        // PlayerPrefs.DeleteKey("MY_TOKEN"); // cái này để test
        if (!PlayerPrefs.HasKey("MY_TOKEN") || PlayerPrefs.GetString("MY_TOKEN").Equals("")) {
            isRotateLoadingImageStopped = true;
            m_controlPanel.SetActive(true);
        } else {
            PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest { CreateAccount = false, AccessToken = PlayerPrefs.GetString("MY_TOKEN") },
                                                OnPlayfabFacebookAutoAuthSuccess, OnPlayfabFacebookAutoAuthFailed);
        }
    }
    private void FBLogin() {
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, AuthCallback);
    }
    private void AuthCallback(ILoginResult result) {
         // If result has no errors, it means we have authenticated in Facebook successfully
        if (result == null || string.IsNullOrEmpty(result.Error))
        {
            /*
             * We proceed with making a call to PlayFab API. We pass in current Facebook AccessToken and let it create
             * and account using CreateAccount flag set to true. We also pass the callback for Success and Failure results
             */
            PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest { CreateAccount = true, AccessToken = AccessToken.CurrentAccessToken.TokenString },
                                                OnPlayfabFacebookAuthSuccess, OnPlayfabFacebookAuthFailed);
        }
        else
        {
            // If Facebook authentication failed, we stop the cycle with the message
            Debug.Log("FB login fail!");
            m_loginErrorPanel.SetActive(true);
            m_loginButton.interactable = true;
        }
    } 
    // When processing both results, we just set the message, explaining what's going on.
    private void OnPlayfabFacebookAuthSuccess(LoginResult result)
    {
        PlayerPrefs.SetString("MY_TOKEN", AccessToken.CurrentAccessToken.TokenString);
        Debug.Log("Playfab Facebook Auth worked!");
        Debug.Log("Save my token success!");
        PlayFabDatabase.Instance.MyDataID = result.PlayFabId;
        this.CheckDisplayName();
    }
    private void OnPlayfabFacebookAuthFailed(PlayFabError error) {
        Debug.Log("Playfab Facebook Auth Fail!");
        m_loginErrorPanel.SetActive(true);
        m_loginButton.interactable = true;
    }

     private void OnPlayfabFacebookAutoAuthSuccess(LoginResult result)
    {
        Debug.Log("Playfab Facebook Auto Auth worked!");
        PlayFabDatabase.Instance.MyDataID = result.PlayFabId;
        this.CheckDisplayName();
    }
    private void OnPlayfabFacebookAutoAuthFailed(PlayFabError error) {
        Debug.Log("Playfab Facebook Auto Auth Fail!");
        m_loginErrorPanel.SetActive(true);
    }
    // private void CheckUserExisted() {
    //     var result = PlayFabDatabase.Instance.GetUserData(PlayFabDatabase.Instance.MyDataID, null);
    //     if (result != null) {
    //         if (result.Data.ContainsKey("userName")) {
    //             PlayFabDatabase.Instance.MyData = result.Data;
    //             return;
    //         } else {
    //             m_settingNamePanel.SetActive(true);
    //         }
    //     }
    // }
    public void OnCheck() {
        m_errorSettingNameLabel.gameObject.SetActive(false);
        if (m_inputField.text.Length > 15 || m_inputField.text.Length < 5) {
            m_errorSettingNameLabel.gameObject.SetActive(true);
            return;
        }
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest {
            DisplayName = m_inputField.text
        }, resultCallback => {
            PlayFabDatabase.Instance.DisPlayName = m_inputField.text;
            PlayFabDatabase.Instance.InitDatabase();
            Debug.Log("Setting name successly");
            SceneManager.LoadScene("Menu Scene");
        }, errorCallback => {
            Debug.LogError("User name existed");
        });
    }
    private void CheckDisplayName() {
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest {
            PlayFabId = PlayFabDatabase.Instance.MyDataID,
            ProfileConstraints = new PlayerProfileViewConstraints() {
                ShowDisplayName = true
            }
        }, async resultCallback => {
            if (resultCallback.PlayerProfile.DisplayName == null || resultCallback.PlayerProfile.DisplayName.Equals("")) {
                m_settingNamePanel.SetActive(true);
            } else {
                PlayFabDatabase.Instance.DisPlayName = resultCallback.PlayerProfile.DisplayName;
                Debug.Log("GetUserData pathAvatar");
                await PlayFabDatabase.Instance.GetAllData();
                Debug.Log(PlayFabDatabase.Instance.PathAvatar);
                SceneManager.LoadScene("Menu Scene");
            }
            Debug.Log("check display name :" + resultCallback.PlayerProfile.DisplayName);
        }, errorCallback => {
            Debug.LogError("check display name :" + errorCallback.GenerateErrorReport());
        });
        isRotateLoadingImageStopped = true;
    }
}
