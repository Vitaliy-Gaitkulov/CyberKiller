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

    public GameObject GameCanvas;
    public GameObject SceneCamera;

    public static GameMaster Instance;
    public static GameMaster gm;

    private AudioSource audioData;
    [SerializeField] private int maxLives = 3;
    public static int _remainingLives;
    public static int RemainingLives => _remainingLives;

    [SerializeField] private int startingMoney;
    public static int Money;
    [HideInInspector] public GameObject LocalPlayer;

    public Text RespawnTimerText;
    public GameObject RespawnMenu;
    private float timerAmount = 5f;
    private bool runSpawnTimer = false;

    public Transform spawnPoint;
    public float spawnDelay = 3f;
    public Transform spawnPrefab;
    public string respawnCountdownSoundName = "RespawnCountdown";
    public string spawnSoundName = "Spawn";
    public Text PingText;
    public CameraShake cameraShake;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject upgradeMenu;
    private AudioManager audioManager;
    public GameObject disconnectUI;
    private bool off = false;
    public GameObject PlayerFeed;
    public GameObject FeedGrid;

    void Awake()
    {
        Instance = this;
        GameCanvas.SetActive(true);
        if (gm == null)
        {
            gm = this;
        }
    }

    void Start()
    {
        _remainingLives = maxLives;
        Money = startingMoney;
        audioManager = AudioManager.instance;
    }

    private void Update()
    {
        PingText.text = "Ping: " + PhotonNetwork.GetPing();

        if (runSpawnTimer)
        {
            timerAmount -= Time.deltaTime;
            RespawnTimerText.text = "Respawning in " + Mathf.Ceil(timerAmount).ToString();
            if (timerAmount <= 0)
            {
                runSpawnTimer = false;
                StartCoroutine(_RespawnPlayer());
            }
        }
    }

    public void EnableRespawn()
    {
        timerAmount = spawnDelay;
        runSpawnTimer = true;
        RespawnMenu.SetActive(true);
        audioManager.PlaySound(respawnCountdownSoundName);
    }

    public void OnStartButtonPressed()
    {
        runSpawnTimer = false;
        RespawnPlayer();
        RespawnMenu.SetActive(false);
    }

    public void RespawnPlayer()
    {
        RespawnMenu.SetActive(false);

        if (LocalPlayer != null)
        {
            // Возрождаем существующего игрока на spawn point
            LocalPlayer.GetComponent<PhotonView>().RPC("Respawn", PhotonTargets.AllBuffered, spawnPoint.position);
        }
        else
        {
            // Первый спавн — создаём нового игрока
            PhotonNetwork.Instantiate(PlayerPrefab.name, spawnPoint.position, Quaternion.identity, 0);
            SceneCamera.SetActive(false);
            GameCanvas.SetActive(false);
        }
    }

    public IEnumerator _RespawnPlayer()
    {
        yield return new WaitForSeconds(spawnDelay);

        audioManager.PlaySound(spawnSoundName);
        RespawnPlayer();

        Transform clone = Instantiate(spawnPrefab, spawnPoint.position, spawnPoint.rotation);
        Destroy(clone.gameObject, 3f);
    }

    [PunRPC]
    public static void KillPlayer(Player player)
    {
        // Не уничтожаем объект — Health.Dead() уже скрыл игрока.
        // EnableRespawn вызывается из Health.CheckHealth напрямую.
    }
}
