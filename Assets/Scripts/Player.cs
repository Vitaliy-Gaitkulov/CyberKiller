using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Photon.MonoBehaviour
{
    public int fallBoundary = -20;

    public string deathSoundName = "DeathVoice";
    public string damageSoundName = "Grunt";

    private AudioManager audioManager;

    [SerializeField]
    private StatusIndicator statusIndicator;

    private PlayerStats stats;

	void Start()
	{
        if (photonView.isMine)
        {
            stats = PlayerStats.instance;

            stats.curHealth = stats.maxHealth;

		    if (statusIndicator == null)
		    {
			    Debug.LogError("No status indicator referenced on Player");
		    }
		    else
		    {
			    statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
		    }

            GameMaster.gm.onToggleUpgradeMenu += OnUpgradeMenuToggle;

            audioManager = AudioManager.instance;
            if(audioManager == null)
            {
                Debug.LogError("no audimanager found");
            }

            InvokeRepeating("RegenHealth", 1f/stats.healthRegenRate, 1f/stats.healthRegenRate);
        }

	}

    void RegenHealthHelper()
    {
        //photonView.RPC("RegenHealth", PhotonTargets.AllBuffered);

    }

    void RegenHealth()
    {
        stats.curHealth += 1;
        statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
    }

    void Update (){
        if(photonView.isMine)
        {
            if (transform.position.y <= fallBoundary){
                DamagePlayer (999999);
            }
        }
    }

    void OnUpgradeMenuToggle (bool active)
    {
        if(this != null){
            Weapon _weapon = GetComponentInChildren<Weapon>();
            if(_weapon != null)
            {
                _weapon.enabled = !active;
            }
        }

    }

    void OnDestroy() 
    {
        GameMaster.gm.onToggleUpgradeMenu -= OnUpgradeMenuToggle;
    }

    [PunRPC]
    public void DamagePlayer (int damage){
        stats.curHealth -= damage;
        if (stats.curHealth <= 0){
            audioManager.PlaySound(deathSoundName);
            GameMaster.KillPlayer(this);
        }else
        {
            audioManager.PlaySound(damageSoundName);
        }
        statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
    }

}
