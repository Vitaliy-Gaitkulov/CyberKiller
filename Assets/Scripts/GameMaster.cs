using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject GameCanvas;
    public GameObject SceneCamera;

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

    void Awake(){
        GameCanvas.SetActive(true);
        if (gm == null){
            gm = GameObject.FindGameObjectWithTag ("GM").GetComponent<GameMaster>();
        }
    }

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
    private WaveSpawner waveSpawner;

    public delegate void UpgradeMenuCallback(bool active);
    public UpgradeMenuCallback onToggleUpgradeMenu;

    private AudioManager audioManager;

    public GameObject disconnectUI;
    private bool Off = false;

    public GameObject PlayerFeed;
    public GameObject FeedGrid;

    void Start(){
        _remainingLives = maxLives;

        Money = startingMoney;

        audioManager = AudioManager.instance;
    }

    private void Update()
    {
        PingText.text = "Ping: " + PhotonNetwork.GetPing();
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
        GameObject obj = Instantiate(PlayerFeed, new Vector2(0, 0), Quaternion.identity);
        obj.transform.SetParent(FeedGrid.transform, false);
        obj.GetComponent<Text>().text = player.name + "left the game";
        obj.GetComponent<Text>().color = Color.red;
    }


    public Transform enemyDeathParticles;

    public void ToggleUpgradeMenu()
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
    }

    public void SpawnPlayer()
    {
        PhotonNetwork.Instantiate(PlayerPrefab.name, new Vector3(spawnPoint.position.x, spawnPoint.position.y), Quaternion.identity, 0);
        GameCanvas.SetActive(false);
    }

    public IEnumerator _RespawnPlayer (){
        audioManager.PlaySound(respawnCountdownSoundName);
        yield return new WaitForSeconds (spawnDelay);

        audioManager.PlaySound(spawnSoundName);
       // Instantiate (playerPrefab, spawnPoint.position, spawnPoint.rotation);

        Transform clone = Instantiate(spawnPrefab, spawnPoint.position, spawnPoint.rotation) as Transform;
        Destroy (clone.gameObject, 3f);
    }

    public static void KillPlayer (Player player){
        Destroy (player.gameObject);
        _remainingLives -= 1;
        if(_remainingLives <= 0)
        {
            gm.EndGame();
        }else
        {
            gm.StartCoroutine (gm._RespawnPlayer());
        }
    }

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
    }

}
