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

    public float HealtAmount;

    void Awake()
    {
        if (photonView.isMine)
        {
            PlayerCamera.SetActive(true);
            audioManager = AudioManager.instance;

        }
    }

  /*  void RegenHealth()
    {
        stats.curHealth += 1;
        statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);

    }*/

    void Update (){
        if(photonView.isMine)
        {
            if (transform.position.y <= fallBoundary){
                //this.GetComponent<PhotonView>().RPC("ReduceHealth", PhotonTargets.AllBuffered, 999999f);
            }
        }
    }

/*    void OnUpgradeMenuToggle (bool active)
    {
        if(this != null){
            Weapon _weapon = GetComponentInChildren<Weapon>();
            if(_weapon != null)
            {
                _weapon.enabled = !active;
            }
        }

    }*/

    void OnDestroy() 
    {
        //GameMaster.gm.onToggleUpgradeMenu -= OnUpgradeMenuToggle;
        //GameMaster.gm.SceneCamera.SetActive(true);

        //GameMaster.gm.GameCanvas.SetActive(true);
    }

/*    [PunRPC]
    public void DamagePlayer (int damage){
        if(!photonView.isMine)
        {
            return;
        }
        stats.curHealth -=damage;
        if (stats.curHealth <= 0){
            audioManager.PlaySound(deathSoundName);
            GameMaster.KillPlayer(this);
            //photonView.RPC("KillPlayer", PhotonTargets.AllBuffered, this);
        }
        else
        {
            audioManager.PlaySound(damageSoundName);
        }
        statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
    }*/

}
