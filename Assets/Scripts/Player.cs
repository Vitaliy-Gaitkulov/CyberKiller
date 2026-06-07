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
                this.GetComponent<PhotonView>().RPC("ReduceHealthBar", PhotonTargets.AllBuffered, 999999f);
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
