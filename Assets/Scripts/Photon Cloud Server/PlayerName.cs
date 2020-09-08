using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

namespace Tank3d.PhotonServer
{
    [RequireComponent(typeof(InputField))]
    public class PlayerName : MonoBehaviour
    {
        private const string PLAYER_NAME_PREF_KEY = "PlayerName";
        void Start()
        {
            string defaultName = string.Empty;
            InputField inputField = this.GetComponent<InputField>();
            if (inputField != null)
            {
                if (PlayerPrefs.HasKey(PLAYER_NAME_PREF_KEY))
                {
                    defaultName = PlayerPrefs.GetString(PLAYER_NAME_PREF_KEY, defaultName);
                    inputField.text = defaultName;
                }
            }
            PhotonNetwork.NickName = defaultName;
        }

        public void OnSetPlayerName(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player name is empty or null");
                return;
            }
            PhotonNetwork.NickName = value;
            
            PlayerPrefs.SetString(PLAYER_NAME_PREF_KEY, value);
        }
    }

}
