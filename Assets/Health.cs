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

    private bool isDead = false;
    AudioManager audioManager;

    private void Awake()
    {
        audioManager = AudioManager.instance;

        if (photonView.isMine)
        {
            GameMaster.Instance.LocalPlayer = this.gameObject;
        }
    }

    [PunRPC]
    public void ReduceHealthBar(float amount)
    {
        if (isDead) return;

        audioManager.PlaySound("Grunt");
        ModifyHealth(amount);
    }

    private void CheckHealth()
    {
        FillImage.fillAmount = HealthAmount / 100f;
        if (photonView.isMine && HealthAmount <= 0 && !isDead)
        {
            isDead = true;
            plMove.DisableInput = true;
            photonView.RPC("Dead", PhotonTargets.AllBuffered);
            GameMaster.Instance.EnableRespawn();
        }
    }

    public void EnableInput()
    {
        plMove.DisableInput = false;
    }

    [PunRPC]
    private void Dead()
    {
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        bc.enabled = false;
        cc.enabled = false;
        sr.enabled = false;
        PlayerCanvas.SetActive(false);
    }

    // Вызывается из GameMaster.RespawnPlayer() по RPC
    [PunRPC]
    public void Respawn(Vector3 spawnPos)
    {
        isDead = false;
        HealthAmount = 100f;
        FillImage.fillAmount = 1f;
        rb.gravityScale = 3;
        rb.velocity = Vector2.zero;
        bc.enabled = true;
        cc.enabled = true;
        sr.enabled = true;
        PlayerCanvas.SetActive(true);
        transform.position = spawnPos;
        GetComponent<Player>()?.ResetFallFlag();
        EnableInput();
    }

    private void ModifyHealth(float amount)
    {
        HealthAmount -= amount;
        FillImage.fillAmount = HealthAmount / 100f;
        CheckHealth();
    }
}
