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

        // Первый респавн при старте игры
        /*RespawnPlayer();*/
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

    // Новый метод, вызываемый при нажатии кнопки "Старт"
    public void OnStartButtonPressed()
    {
        runSpawnTimer = false;  // Остановка таймера, если он запущен
        RespawnPlayer();        // Респавн игрока
        RespawnMenu.SetActive(false); // Закрытие окна респавна
    }

    public void RespawnPlayer()
    {
        GameObject playerInstance = PhotonNetwork.Instantiate(PlayerPrefab.name, spawnPoint.position, Quaternion.identity, 0);

        AudioListener[] listeners = GameObject.FindObjectsOfType<AudioListener>();
        if (listeners.Length == 0)
        {
            if (Camera.main != null && Camera.main.GetComponent<AudioListener>() == null)
            {
                Camera.main.gameObject.AddComponent<AudioListener>();
            }
            else if (playerInstance.GetComponent<AudioListener>() == null)
            {
                playerInstance.AddComponent<AudioListener>();
            }
        }

        SceneCamera.SetActive(false);
        GameCanvas.SetActive(false);
        RespawnMenu.SetActive(false);
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
        Destroy(player.gameObject);
        gm.EnableRespawn();
    }
}




/*

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
        selectedSkin = (selectedSkin + 1) % skinsImage.Count;
        UpdateSkin();
    }

    public void BackOption()
    {
        selectedSkin = (selectedSkin - 1 + skinsImage.Count) % skinsImage.Count;
        UpdateSkin();
    }

    private void UpdateSkin()
    {
        PlayerPrefab = skinsObject[selectedSkin];
        skinImage.GetComponent<Image>().sprite = skinsImage[selectedSkin];
    }

    public GameObject GameCanvas;
    public GameObject SceneCamera;

    public static GameMaster Instance;

    public static GameMaster gm;
    private AudioSource audioData;

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
    private float timerAmount = 5f;
    private bool runSpawnTimer = false;

    public Transform spawnPoint;
    public float spawnDelay;
    public Transform spawnPrefab;
    public string respawnCountdownSoundName = "RespawnCountdown";
    public string spawnSoundName = "Spawn";
    public Text PingText;
    public CameraShake cameraShake;
    [SerializeField]
    private GameObject gameOverUI;
    [SerializeField]
    private GameObject upgradeMenu;
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
                RespawnPlayer();
            }
        }
    }

    public void EnableRespawn()
    {
        timerAmount = 5f;
        runSpawnTimer = true;
        RespawnMenu.SetActive(true);
        audioManager.PlaySound(respawnCountdownSoundName);
    }

    public void RespawnPlayer()
    {
        // Создание игрока
        GameObject playerInstance = PhotonNetwork.Instantiate(PlayerPrefab.name, spawnPoint.position, Quaternion.identity, 0);

        // Проверка на наличие активных AudioListener в сцене
        AudioListener[] listeners = GameObject.FindObjectsOfType<AudioListener>();

        // Если нет активного AudioListener, добавляем его к игроку
        if (listeners.Length == 0)
        {
            // Проверяем, есть ли AudioListener у главной камеры
            if (Camera.main != null && Camera.main.GetComponent<AudioListener>() == null)
            {
                Camera.main.gameObject.AddComponent<AudioListener>();
            }
            // Если нет главной камеры, добавляем AudioListener к игроку
            else if (playerInstance.GetComponent<AudioListener>() == null)
            {
                playerInstance.AddComponent<AudioListener>();
            }
        }

        // Отключение камеры сцены
        SceneCamera.SetActive(false);
        GameCanvas.SetActive(false);
    }

    public IEnumerator _RespawnPlayer ()
    {
        audioManager.PlaySound(respawnCountdownSoundName);
        yield return new WaitForSeconds (spawnDelay);

        audioManager.PlaySound(spawnSoundName);
        RespawnPlayer();

        Transform clone = Instantiate(spawnPrefab, spawnPoint.position, spawnPoint.rotation);
        Destroy(clone.gameObject, 3f);
    }

    [PunRPC]
    public static void KillPlayer(Player player)
    {
        Destroy(player.gameObject);
    }
}
*/