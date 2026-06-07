using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private string VersionName = "0.1";
    [SerializeField] private GameObject usernameMenu;
    [SerializeField] private GameObject ConnectPanell;

    [SerializeField] private InputField UsernameInput;
    [SerializeField] private InputField CreateGameInput;
    [SerializeField] private InputField JoinGameInput;

    [SerializeField] private GameObject StartButton;

    public int rnd;
    public int value;

    private void Awake()
    {
        value = Random.Range(0, 1000);
        PhotonNetwork.ConnectUsingSettings(VersionName);
        UsernameInput.text = "user" + value.ToString();
    }

    private void Start() { }

    private void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public void ChangeUserNameInput() { }

    public void SetUserName()
    {
        PhotonNetwork.playerName = UsernameInput.text;
    }

    public void CreateGame()
    {
        if (CreateGameInput.text == "")
        {
            CreateGameInput.text = value.ToString();
        }

        if (UsernameInput.text.Length >= 3)
        {
            SetUserName();
            PhotonNetwork.CreateRoom(CreateGameInput.text, new RoomOptions() { maxPlayers = 5 }, null);
        }
    }

    public void JoinGame()
    {
        if (JoinGameInput.text == "")
        {
            JoinGameInput.text = "def";
        }

        if (UsernameInput.text.Length >= 3)
        {
            SetUserName();
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.maxPlayers = 5;
            PhotonNetwork.JoinOrCreateRoom(JoinGameInput.text, roomOptions, TypedLobby.Default);
        }
    }

    private void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("MainGame");
    }

    private void OnDisconnectedFromPhoton()
    {
        PhotonNetwork.LoadLevel("MainMenu");
    }
}
