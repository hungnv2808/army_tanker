using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Facebook.Unity;
using PlayFab;
using PlayFab.ClientModels;
using LoginResult = PlayFab.ClientModels.LoginResult;
using DG.Tweening;
public class Login : MonoBehaviour
{
    [SerializeField] private GameObject m_controlPanel, m_settingNamePanel, m_errorLoginPanel;
    [SerializeField] private Button m_loginFacebookButton;
    [SerializeField] private InputField m_nameInGameLabel, m_userNameLabel, m_passwordLabel, m_registerUserNameLabel, m_registerPasswordLabel, m_registerConfirmPasswordLabel;
    [SerializeField] private Text m_errorSettingNameLabel, m_errorLoginLabel;
    [SerializeField] private Transform m_loginPanel, m_registerPanel;
    [SerializeField] private LoadingIcon m_loadingIcon;
    private string m_myToken;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        m_controlPanel.SetActive(false);
        m_loadingIcon.gameObject.SetActive(true);
        m_loadingIcon.Init();
        // This call is required before any other calls to the Facebook API. We pass in the callback to be invoked once initialization is finished
        yield return new WaitForSeconds(2.0f);
        m_loadingIcon.gameObject.SetActive(false);
        m_loginPanel.DOScale(Vector3.one, 0.2f).SetLoops(1).SetEase(Ease.OutBack);
    }
    
    // login facebook event button
    public void OnLoginFacebook() {
        FB.Init(OnFacebookInitialized, OnHideUnity);  
        m_loginFacebookButton.interactable = false;
        m_errorLoginPanel.SetActive(false);
    }
    public void OnLogin() {
        m_errorLoginPanel.SetActive(false);
        m_userNameLabel.text.Trim();
        m_passwordLabel.text.Trim();
        if (m_userNameLabel.text.Length == 0 || m_passwordLabel.text.Length == 0) {
            Debug.Log("tài khoản hoặc mật khẩu không để trống");
            m_errorLoginPanel.SetActive(true);
            m_errorLoginLabel.text = "Email hoặc mật khẩu không để trống";
        } else {
            m_errorLoginPanel.SetActive(false);
            m_loginPanel.DOScale(Vector3.zero, 0.2f).SetLoops(1).SetEase(Ease.InBack).OnComplete(() => {
                m_loadingIcon.gameObject.SetActive(true);
                m_loadingIcon.Init();
                var request = new LoginWithEmailAddressRequest{Email = m_userNameLabel.text, Password = m_passwordLabel.text};
                PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
            });
        }
    }
    private void OnLoginSuccess(LoginResult result) {
        Debug.Log("Đăng nhập thành công");
        PlayFabDatabase.Instance.MyDataID = result.PlayFabId;
        this.CheckDisplayName();
    }
    private void OnLoginFailure(PlayFabError error) {
        m_errorLoginPanel.SetActive(true);
        m_loginPanel.DOScale(Vector3.one, 0.2f).SetLoops(1).SetEase(Ease.OutBack);
        if ((int)error.Error == 1001) {
            m_errorLoginLabel.text = "Tài khoản chưa đăng ký";
        } else {
            m_errorLoginLabel.text = "Email hoặc mật khẩu không chính xác";
            Debug.Log("tài khoản hoặc mật khẩu không chính xác");
        }
        
        Debug.LogError((int)error.Error);
    }
    public void OnRegister() {
        // m_loginPanel.DOPlayBackwards();
        if (m_registerUserNameLabel.text.Length == 0 || m_registerPasswordLabel.text.Length == 0) {
            m_errorLoginPanel.SetActive(true);
            m_errorLoginLabel.text = "Email hoặc mật khẩu không để trống";
        } 
        else if (m_registerPasswordLabel.text.Length < 6 && m_registerPasswordLabel.text.Length > 0) {
            m_errorLoginPanel.SetActive(true);
            m_errorLoginLabel.text = "Mật khẩu phải dài hơn 6 kí tự";
        }
        else if (!m_registerConfirmPasswordLabel.text.Equals(m_registerPasswordLabel.text)) {
            m_errorLoginPanel.SetActive(true);
            m_errorLoginLabel.text = "Xác nhận mật khẩu không chính xác";
        } else {
            m_errorLoginPanel.SetActive(false);
            var registerRequest = new RegisterPlayFabUserRequest {Email = m_registerUserNameLabel.text, Password = m_registerPasswordLabel.text, RequireBothUsernameAndEmail = false};
            PlayFabClientAPI.RegisterPlayFabUser(registerRequest, RegisterSuccess, RegisterFailure);
        }
        
    }
    private void RegisterSuccess(RegisterPlayFabUserResult result) {
        Debug.Log("Đăng ký tài khoản thành công");
        m_settingNamePanel.SetActive(true);
        m_registerPanel.gameObject.SetActive(false);
    }
    private void RegisterFailure(PlayFabError error) {
        Debug.LogError((int)error.Error);
        if ((int)error.Error == 1000) {
            m_errorLoginPanel.SetActive(true);
            m_errorLoginLabel.text = "Email không hợp lệ";
        }
    }
    public void OnRegisterLabel() {
        m_loginPanel.DOScale(Vector3.zero, 0.2f).SetLoops(1).SetEase(Ease.InBack).OnComplete(() => {
            m_registerPanel.DOScale(Vector3.one, 0.2f).SetLoops(1).SetEase(Ease.OutBack);
        });
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
    /// <summary>
    // check user logined onto game ? 
    /// </summary>
    private void CheckAutoLoginFB() {
        // PlayerPrefs.DeleteKey("MY_TOKEN"); // cái này để test
        if (!PlayerPrefs.HasKey("MY_TOKEN") || PlayerPrefs.GetString("MY_TOKEN").Equals("")) {
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
            m_errorLoginPanel.SetActive(true);
            m_loginFacebookButton.interactable = true;
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
        m_errorLoginPanel.SetActive(true);
        m_loginFacebookButton.interactable = true;
    }

     private void OnPlayfabFacebookAutoAuthSuccess(LoginResult result)
    {
        Debug.Log("Playfab Facebook Auto Auth worked!");
        PlayFabDatabase.Instance.MyDataID = result.PlayFabId;
        this.CheckDisplayName();
    }
    private void OnPlayfabFacebookAutoAuthFailed(PlayFabError error) {
        Debug.Log("Playfab Facebook Auto Auth Fail!");
        m_errorLoginPanel.SetActive(true);
    }
    /// <summary>
    ///event button check name ingame 
    /// </summary>
    public void OnCheck() {
        m_errorLoginPanel.SetActive(false);
        m_errorSettingNameLabel.gameObject.SetActive(false);
        if (m_nameInGameLabel.text.Length > 15 || m_nameInGameLabel.text.Length < 5) {
            m_errorSettingNameLabel.gameObject.SetActive(true);
            return;
        }
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest {
            DisplayName = m_nameInGameLabel.text
        }, resultCallback => {
            PlayFabDatabase.Instance.DisPlayName = m_nameInGameLabel.text;
            PlayFabDatabase.Instance.InitDatabase();
            Debug.Log("Setting name successly");
            Invoke("LoadMenuSecen", 5f);
        }, errorCallback => {
            m_errorLoginPanel.SetActive(true);
            m_errorLoginLabel.text = "Tên đã tồn tại";
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
                Invoke("LoadMenuSecen", 5f);
            }
            Debug.Log("check display name :" + resultCallback.PlayerProfile.DisplayName);
        }, errorCallback => {
            Debug.LogError("check display name :" + errorCallback.GenerateErrorReport());
        });
    }
    private void LoadMenuSecen() {
        SceneManager.LoadScene("Menu Scene");
    }
}
