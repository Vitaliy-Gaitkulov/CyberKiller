using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    public GameObject PlayerPrefab;

    public GameObject skinImage;
    private int selectedSkin = 0;

    public List<GameObject> skinsObject = new List<GameObject>();
    public List<Sprite> skinsImage = new List<Sprite>();

    public void NextOption()
    {
        selectedSkin = selectedSkin + 1;
        if(selectedSkin == skinsImage.Count)
        {
            selectedSkin = 0;
        }
        PlayerPrefab = skinsObject[selectedSkin];
        skinImage.GetComponent<Image>().sprite = skinsImage[selectedSkin];
    }
    
    public void BackOption()
    {
        selectedSkin = selectedSkin - 1;
        if(selectedSkin < 0)
        {
            selectedSkin = skinsImage.Count - 1;
        }
        PlayerPrefab = skinsObject[selectedSkin];
        skinImage.GetComponent<Image>().sprite = skinsImage[selectedSkin];
    }


















    public GameObject GameCanvas;
    public GameObject SceneCamera;

    public static GameMaster Instance;

    public static GameMaster gm;
    AudioSource audioData;

    [SerializeField]
    private int maxLives = 3;
    public static int _remainingLives;
    public static int RemainingLives 
    {
        get { return _remainingLives; }
    }
    [SerializeField]
    private int startingMoney;
    public static int Money;

    [HideInInspector] public GameObject LocalPlayer;
    public Text RespawnTimerText;
    public GameObject RespawnMenu;
    private float TimerAmount = 2f;
    private bool RunSpawnTimer = false;

    public Transform playerPrefab;
    public Transform spawnPoint;
    public float spawnDelay;
    public Transform spawnPrefab;
    public string respawnCountdownSoundName = "RespawnCountdown";
    public string spawnSoundName = "Spawn";
    public Text PingText;
    public string gameOverSoundName = "GameOver";
    public CameraShake cameraShake;
    [SerializeField]
    private GameObject gameOverUI;
    [SerializeField]
    private GameObject upgradeMenu;
    [SerializeField]
    public delegate void UpgradeMenuCallback(bool active);
    public UpgradeMenuCallback onToggleUpgradeMenu;
    private AudioManager audioManager;
    public GameObject disconnectUI;
    private bool Off = false;
    public GameObject PlayerFeed;
    public GameObject FeedGrid;

    void Awake()
    {
        Instance = this;
        GameCanvas.SetActive(true);
        if (gm == null)
        {
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        }
    }

    private void StartRespawn()
    {
        TimerAmount = 0;
        RespawnTimerText.text = "Respawning in " + TimerAmount.ToString();
        Debug.Log(TimerAmount);

        if (TimerAmount <= 0)
        {

            LocalPlayer.GetComponent<PhotonView>().RPC("Respawn", PhotonTargets.AllBuffered);
            LocalPlayer.GetComponent<Health>().EnableInput();
            RespawnLocation();
            RespawnMenu.SetActive(false);
            RunSpawnTimer = false;
            Debug.Log("end respawn");
        }
    }

    void Start(){

        _remainingLives = maxLives;

        Money = startingMoney;

        audioManager = AudioManager.instance;
    }

    private void Update()
    {
        PingText.text = "Ping: " + PhotonNetwork.GetPing();

        if (RunSpawnTimer)
        {
            StartRespawn();
        }
    }

    public void EnableRespawn()
    {
        TimerAmount = 5f;
        RunSpawnTimer = true;
        RespawnMenu.SetActive(true);
    }

    public void RespawnLocation()
    {
        float randomValue = Random.Range(-3, 5f);
        LocalPlayer.transform.localPosition = new Vector2(randomValue, 3f);
    }

    public void CheckInputOff()
    {
        if (Off)
        {
            disconnectUI.SetActive(false);
            Off = false;
        }else if(!Off)
        {
            disconnectUI.SetActive(true);
            Off = true;
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MainMenu");
    }

    private void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        GameObject obj = Instantiate(PlayerFeed, new Vector2(0, 0), Quaternion.identity);
        obj.transform.SetParent(FeedGrid.transform, false);
        obj.GetComponent<Text>().text = player.name + "joined the game";
        obj.GetComponent<Text>().color = Color.green;
    }
    private void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
/*        if (photonView.isMine)
        {
            SceneCamera.SetActive(true);
        }*/
        GameObject obj = Instantiate(PlayerFeed, new Vector2(0, 0), Quaternion.identity);
        obj.transform.SetParent(FeedGrid.transform, false);
        obj.GetComponent<Text>().text = player.name + "left the game";
        obj.GetComponent<Text>().color = Color.red;
    }


    public Transform enemyDeathParticles;

/*    public void ToggleUpgradeMenu()
    {
        upgradeMenu.SetActive(!upgradeMenu.activeSelf);
        waveSpawner.enabled = !upgradeMenu.activeSelf;
        onToggleUpgradeMenu.Invoke(upgradeMenu.activeSelf);
    }

    public void EndGame()
    {
        audioManager.PlaySound(gameOverSoundName);

        Debug.Log("Game Over");
        gameOverUI.SetActive(true);
    }*/

    public void SpawnPlayer()
    {
        PhotonNetwork.Instantiate(PlayerPrefab.name, new Vector3(spawnPoint.position.x, spawnPoint.position.y), Quaternion.identity, 0);
        SceneCamera.SetActive(false);
        GameCanvas.SetActive(false);
    }

    /*    public IEnumerator _RespawnPlayer (){
            audioManager.PlaySound(respawnCountdownSoundName);
            yield return new WaitForSeconds (spawnDelay);

            audioManager.PlaySound(spawnSoundName);
            SpawnPlayer();
           // Instantiate (playerPrefab, spawnPoint.position, spawnPoint.rotation);

            Transform clone = Instantiate(spawnPrefab, spawnPoint.position, spawnPoint.rotation) as Transform;
            Destroy (clone.gameObject, 3f);
        }*/

    /*    [PunRPC]
        public static void KillPlayer(Player player)
        {
            Destroy(player.gameObject);
        }*/
    /*
        public static void KillEnemy (Enemy enemy){
            gm._KillEnemy(enemy);
        }

        public void _KillEnemy (Enemy _enemy){
            audioManager.PlaySound(_enemy.deathSoundName);

            Money += _enemy.moneyDrop;
            audioManager.PlaySound("Money");
            Transform _clone = Instantiate(_enemy.deathParticles, _enemy.transform.position, Quaternion.identity) as Transform;
            Destroy(_clone.gameObject, 5f);
            cameraShake.Shake(_enemy.shakeAmt, _enemy.shakeLength);
            Destroy (_enemy.gameObject);
        }*/

}
