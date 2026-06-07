using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : Photon.MonoBehaviour
{

    public Image FillImage;

    public float HealthAmount;

    public PlayerMove plMove;
    public Rigidbody2D rb;
    public BoxCollider2D bc;
    public CircleCollider2D cc;
    public SpriteRenderer sr;
    public GameObject PlayerCanvas;

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = AudioManager.instance;

        if(photonView.isMine)
        {
            GameMaster.Instance.LocalPlayer = this.gameObject;
        }
    }

    [PunRPC]
    public void ReduceHealthBar(float amount)
    {
        audioManager.PlaySound("Grunt");
        ModifyHealth(amount);
    }

    [PunRPC]
    private void CheckHealth()
    {
        FillImage.fillAmount = HealthAmount / 100f;
        if(photonView.isMine && HealthAmount <= 0)
        {;
            GameMaster.Instance.EnableRespawn();
            plMove.DisableInput = true;
            this.GetComponent<PhotonView>().RPC("Dead", PhotonTargets.AllBuffered);
        }
    }

    public void EnableInput()
    {
        plMove.DisableInput = false;
    }



    [PunRPC]
    private void Dead()
    {
        rb.gravityScale = 1;
        bc.enabled = false;
        cc.enabled = false;
        sr.enabled = false;
        PlayerCanvas.SetActive(false);
    }

    [PunRPC]
    private void Respawn()
    {
        rb.gravityScale = 3;
        bc.enabled = true;
        cc.enabled = true;
        sr.enabled = true;
        PlayerCanvas.SetActive(true);
        FillImage.fillAmount = 1f;
        HealthAmount = 100f;
        EnableInput();

    }

    private void ModifyHealth(float amount)
    {
        
        if (photonView.isMine)
        {
            HealthAmount -= amount;
            FillImage.fillAmount -= amount;
        }
        else
        {
        HealthAmount -= amount;
        FillImage.fillAmount -= amount;
        }

        CheckHealth();
    }
}
