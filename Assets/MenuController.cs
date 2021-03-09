using System.Collections;
using System.Collections.Generic;
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

    public Random rnd = new Random();
    public int value;

    //Получить случайное число (в диапазоне от 0 до 10)



    private void Awake()
    {
        value = Random.Range(0, 1000);

        PhotonNetwork.ConnectUsingSettings(VersionName);
        UsernameInput.text = "user" + value.ToString();

    }

    private void Start()
    {

    }

    private void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("Connected");
    }

    public void ChangeUserNameInput()
    {
        if(UsernameInput.text.Length >= 3)
        {
            //StartButton.SetActive(true);
        }
        else
        {
            //StartButton.SetActive(false);
        }
    }

    public void SetUserName()
    {
        //usernameMenu.SetActive(false);
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
        if(JoinGameInput.text == "")
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


}
