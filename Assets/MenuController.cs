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
    [SerializeField] private Text statusText;

    public int rnd;
    public int value;

    private bool isConnected = false;

    private void Awake()
    {
        value = Random.Range(0, 1000);
        UsernameInput.text = "user" + value.ToString();
        SetStatus("Подключение...");
        PhotonNetwork.ConnectUsingSettings(VersionName);
    }

    private void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    private void OnJoinedLobby()
    {
        isConnected = true;
        SetStatus("Подключено");
    }

    private void OnDisconnectedFromPhoton()
    {
        isConnected = false;
        SetStatus("Нет соединения");
        PhotonNetwork.LoadLevel("MainMenu");
    }

    public void ChangeUserNameInput() { }

    public void SetUserName()
    {
        PhotonNetwork.playerName = UsernameInput.text;
    }

    public void CreateGame()
    {
        if (!isConnected)
        {
            SetStatus("Ожидание подключения...");
            return;
        }

        if (CreateGameInput.text == "")
            CreateGameInput.text = value.ToString();

        if (UsernameInput.text.Length >= 3)
        {
            SetUserName();
            PhotonNetwork.CreateRoom(CreateGameInput.text, new RoomOptions() { maxPlayers = 5 }, null);
        }
    }

    public void JoinGame()
    {
        if (!isConnected)
        {
            SetStatus("Ожидание подключения...");
            return;
        }

        if (JoinGameInput.text == "")
            JoinGameInput.text = "def";

        if (UsernameInput.text.Length >= 3)
        {
            SetUserName();
            RoomOptions roomOptions = new RoomOptions() { maxPlayers = 5 };
            PhotonNetwork.JoinOrCreateRoom(JoinGameInput.text, roomOptions, TypedLobby.Default);
        }
    }

    private void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("MainGame");
    }

    private void SetStatus(string msg)
    {
        if (statusText != null)
            statusText.text = msg;
    }
}
