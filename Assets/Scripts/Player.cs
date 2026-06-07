using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Photon.MonoBehaviour
{
    public int fallBoundary = -20;
    public string deathSoundName = "DeathVoice";
    public string damageSoundName = "Grunt";
    private AudioManager audioManager;
    public GameObject PlayerCamera;

    public PlayerStats stats = new PlayerStats();
    public GameObject statusIndicator;

    void Start()
    {
        EnsureSingleAudioListener();
    }

    private void EnsureSingleAudioListener()
    {
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        if (listeners.Length > 1)
        {
            for (int i = 1; i < listeners.Length; i++)
            {
                Destroy(listeners[i]);
            }
        }
        else if (listeners.Length == 0)
        {
            gameObject.AddComponent<AudioListener>();
        }
    }

    void Awake()
    {
        if (photonView.isMine)
        {
            PlayerCamera.SetActive(true);
            audioManager = AudioManager.instance;
        }

        stats.curHealth = stats.maxHealth;
    }

    void Update()
    {
        if (photonView.isMine)
        {
            if (transform.position.y <= fallBoundary)
            {
                this.GetComponent<PhotonView>().RPC("ReduceHealth", PhotonTargets.AllBuffered, 999999f);
            }
        }
    }

    [PunRPC]
    public void ReduceHealth(float damage)
    {
        stats.TakeDamage((int)damage);

        if (stats.curHealth <= 0)
        {
            audioManager.PlaySound(deathSoundName);
            GameMaster.KillPlayer(this);
        }
    }

    [PunRPC]
    public void DamagePlayer(int damage)
    {
        if (!photonView.isMine)
        {
            return;
        }

        stats.TakeDamage(damage);

        if (stats.curHealth <= 0)
        {
            audioManager.PlaySound(deathSoundName);
            GameMaster.KillPlayer(this);
        }
        else
        {
            audioManager.PlaySound(damageSoundName);
        }

        if (statusIndicator != null)
        {
            statusIndicator.GetComponent<StatusIndicator>().SetHealth(stats.curHealth, stats.maxHealth);
        }
    }

    public class PlayerStats
    {
        public int maxHealth = 100;
        public int curHealth;

        public PlayerStats()
        {
            curHealth = maxHealth;
        }

        public float GetHealthPercentage()
        {
            return (float)curHealth / maxHealth;
        }

        public void RegenerateHealth(int amount)
        {
            curHealth = Mathf.Clamp(curHealth + amount, 0, maxHealth);
        }

        public void TakeDamage(int damage)
        {
            curHealth = Mathf.Clamp(curHealth - damage, 0, maxHealth);
        }
    }
}




/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Photon.MonoBehaviour
{
    public int fallBoundary = -20;
    public string deathSoundName = "DeathVoice";
    public string damageSoundName = "Grunt";

    private AudioManager audioManager;
    public GameObject PlayerCamera;

    // Добавляем PlayerStats
    public PlayerStats stats = new PlayerStats();

    public GameObject statusIndicator;  // UI элемент для отображения здоровья (если имеется)

    void Start()
    {
        EnsureSingleAudioListener();
    }

    private void EnsureSingleAudioListener()
    {
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        if (listeners.Length > 1)
        {
            for (int i = 1; i < listeners.Length; i++)
            {
                Destroy(listeners[i]);
            }
        }
        else if (listeners.Length == 0)
        {
            gameObject.AddComponent<AudioListener>();
        }
    }

    void Awake()
    {
        if (photonView.isMine)
        {
            PlayerCamera.SetActive(true);
            audioManager = AudioManager.instance;
        }

        // Инициализация текущего здоровья на основе максимального
        stats.curHealth = stats.maxHealth;
    }

    void Update()
    {
        if (photonView.isMine)
        {
            // Проверка на падение ниже границы
            if (transform.position.y <= fallBoundary)
            {
                this.GetComponent<PhotonView>().RPC("ReduceHealth", PhotonTargets.AllBuffered, 999999f);
            }
        }
    }

    // Метод для получения урона
    [PunRPC]
    public void DamagePlayer(int damage)
    {
        if (!photonView.isMine)
        {
            return;
        }

        stats.TakeDamage(damage);

        if (stats.curHealth <= 0)
        {
            audioManager.PlaySound(deathSoundName);
            GameMaster.KillPlayer(this);
        }
        else
        {
            audioManager.PlaySound(damageSoundName);
        }

        // Обновление отображения здоровья
        if (statusIndicator != null)
        {
            statusIndicator.GetComponent<StatusIndicator>().SetHealth(stats.curHealth, stats.maxHealth);
        }
    }

    // Метод для уменьшения здоровья (например, при падении)
    [PunRPC]
    public void ReduceHealth(float damage)
    {
        stats.TakeDamage((int)damage);

        if (stats.curHealth <= 0)
        {
            audioManager.PlaySound(deathSoundName);
            GameMaster.KillPlayer(this);
        }
    }

    public class PlayerStats
    {
        public int maxHealth = 100;  // Максимальное здоровье игрока
        public int curHealth;        // Текущее здоровье игрока

        // Конструктор для инициализации текущего здоровья
        public PlayerStats()
        {
            curHealth = maxHealth;
        }

        // Метод для получения процента здоровья
        public float GetHealthPercentage()
        {
            return (float)curHealth / maxHealth;
        }

        // Метод для восстановления здоровья
        public void RegenerateHealth(int amount)
        {
            curHealth = Mathf.Clamp(curHealth + amount, 0, maxHealth);
        }

        // Метод для получения урона
        public void TakeDamage(int damage)
        {
            curHealth = Mathf.Clamp(curHealth - damage, 0, maxHealth);
        }
    }
}





using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Photon.MonoBehaviour
{
    public int fallBoundary = -20;
    public string deathSoundName = "DeathVoice";
    public string damageSoundName = "Grunt";
    private AudioManager audioManager;
    public GameObject PlayerCamera;
    public StatusIndicator statusIndicator;

    public float HealtAmount;

    void Awake()
    {
        if (photonView.isMine)
        {
            PlayerCamera.SetActive(true);
            audioManager = AudioManager.instance;

        }
    }

    void RegenHealth()
    {
        stats.curHealth += 1;
        statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);

    }

    void Update()
    {
        if (photonView.isMine)
        {
            if (transform.position.y <= fallBoundary)
            {
                this.GetComponent<PhotonView>().RPC("ReduceHealth", PhotonTargets.AllBuffered, 999999f);
            }
        }
    }

    [PunRPC]
    public void ReduceHealth(float damage)
    {
        stats.curHealth -= damage;
        if (stats.curHealth <= 0)
        {
            audioManager.PlaySound(deathSoundName);
            GameMaster.KillPlayer(this);
        }
    }

    void OnUpgradeMenuToggle(bool active)
    {
        if (this != null)
        {
            Weapon _weapon = GetComponentInChildren<Weapon>();
            if (_weapon != null)
            {
                _weapon.enabled = !active;
            }
        }

    }
    [PunRPC]
    public void DamagePlayer(int damage)
    {
        if (!photonView.isMine)
        {
            return;
        }
        stats.curHealth -= damage;
        if (stats.curHealth <= 0)
        {
            audioManager.PlaySound(deathSoundName);
            GameMaster.KillPlayer(this);
        }
        else
        {
            audioManager.PlaySound(damageSoundName);
        }
        statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
    }

    void OnDestroy()
    {
        //GameMaster.gm.onToggleUpgradeMenu -= OnUpgradeMenuToggle;
        //GameMaster.gm.SceneCamera.SetActive(true);

        //GameMaster.gm.GameCanvas.SetActive(true);
    }

    [PunRPC]
    public void DamagePlayer(int damage)
    {
        if (!photonView.isMine)
        {
            return;
        }
        stats.curHealth -= damage;
        if (stats.curHealth <= 0)
        {
            audioManager.PlaySound(deathSoundName);
            GameMaster.KillPlayer(this);
            //photonView.RPC("KillPlayer", PhotonTargets.AllBuffered, this);
        }
        else
        {
            audioManager.PlaySound(damageSoundName);
        }
        statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
    }

}
*/