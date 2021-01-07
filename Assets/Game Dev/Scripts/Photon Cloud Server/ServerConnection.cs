using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace Tank3d.PhotonServer
{
    /// <summary>
    /// khi đã login game và tìm kiếm trận đấu thì launcher xử lý phần tìm kiếm trận đấu 
    /// </summary>
    public class ServerConnection : MonoBehaviourPunCallbacks
    {
        public static ServerConnection Instance {
            get {
                return s_instance;
            }
        }
        #region private serializable fields
        private static ServerConnection s_instance;
        
        #endregion

        #region  private fields
        /*version của client. Người chơi sẽ được chia ra từ những người khác theo thông số này*/
        private string m_gameVersion = "1";
        #endregion


        #region MonoBehaviour Callbacks 
        private void Awake()
        {
            
            if (s_instance != null && s_instance != this)
            {
                DestroyImmediate(this.gameObject);
                return;
            }
            s_instance = this;
            PhotonNetwork.AutomaticallySyncScene = true; /*đảm bảo rằng khi sử dụng PhotonNetwork.LoadLevel() trên 1 thằng client thì tất cả những client khác trong cùng phòng sẽ tự động đồng bộ level
            của thằng kia*/
            DontDestroyOnLoad(this);
            Connect2MasterServer();
        }
        #endregion
        
        #region public method
        
        /*xử lý khi người chơi tìm trận đấu thì call method OnConnect*/
        public void Connect2MasterServer()
        {
            /*IsConnect trả về false là chúng ta chưa kết nối được và ngược lại, nếu không kết nối được thì khởi tạo lại connect tới server*/
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("Connected MasterServer successly :)");
                Debug.Log("Count Of Rooms: "+ PhotonNetwork.CountOfRooms);
                /*we need at this point to attempt joining a random room. If it fails, we will get notified in OnJoinRandomFailed() and we will creat one*/
            }
            else
            {
                Debug.Log("Creating Connection MasterServer...");
                /*we must first and foremost connect to Photon Online server*/
                PhotonNetwork.ConnectUsingSettings(); /* kết nối game master server, khi connect thành công sẽ gọi đến callback OnConnectedToMaster() phía dưới*/
                PhotonNetwork.GameVersion = m_gameVersion;
                
            }
        }
        public void JoinRoom() {
            if (PhotonNetwork.IsConnected) {
                JoinRandomRoom();
            } else {
                // handle when player disconected
            }
            // this.Connect2MasterServer();// connect to server and join room
        }
        #endregion 
        private void JoinRandomRoom() {
            PhotonNetwork.JoinRandomRoom(); 
        }
        #region MonobehaviourPunCallbacks Callbacks
        public override void OnConnectedToMaster()
        {
            /*khi mình rời phòng thì thằng callback OnLeftRoom() sẽ chạy sau khi mà thằng PhotonNetwork.LeaveRoom() chạy xong, tiếp đến thì nó sẽ gọi vào hàm này*/
            Debug.Log("Connected MasterServer successly :)");
            Debug.Log("Count Of Rooms: "+ PhotonNetwork.CountOfRooms);

        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogErrorFormat("PUN basic tutorial/launcher: OnDisconnected() was called by PUN with reason {0}", cause);
            SceneManager.LoadScene("Lobby Scene");
        }
        /*hàm này là 1 callback và nó sẽ được gọi sau khi PhotonNetwork.JoinRandomRoom() trả về thất bại*/
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.LogFormat("You JoinRandoomFailed {0}", message);
            Debug.LogFormat("Creating room...");
            /*nếu join randoom lỗi có thể là do phòng không tồn tại hoặc đã đầy, bây giờ tạo 1 phòng mới */
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = ServerManagement.MaxPlayersInRoom });
        }
        /*hàm này là 1 callback và nó sẽ được gọi sau khi PhotonNetwork.JoinRandomRoom() trả về thành công*/
        public override void OnJoinedRoom()
        {
            Debug.Log("You joined room successly :)");  
            Debug.Log("PhotonNetwork.CurrentRoom.Name " + PhotonNetwork.CurrentRoom.Name);
            // #Critical: We only load if we are the first player, else we rely on (dựa vào) `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
            if(PhotonNetwork.CurrentRoom.PlayerCount < ServerManagement.MaxPlayersInRoom) {
                Debug.Log("Wating for other player");
                if (PhotonNetwork.IsMasterClient) {
                    ServerManagement.Instance.CheckTimeoutLoadScene();
                }
            } else {
                Debug.Log("start game");
                //TODO: NOTE
                // if (ServerManagement.MaxPlayersInRoom == 1) {
                //     PhotonNetwork.LoadLevel("Main Scene"); // dòng này là để test offline
                //     ServerManagement.Instance.CancleLoadSceneTimeout();
                // } 
                StartCoroutine(ServerManagement.Instance.CheckSceneLoadingCompletelyLoopCoroutine());
            }
        }
        #endregion
    }
}

