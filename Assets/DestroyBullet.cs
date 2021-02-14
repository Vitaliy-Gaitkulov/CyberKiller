using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBullet : Photon.MonoBehaviour
{

    public Transform HitPrefab;
    public int BulletDamage;
    // Start is called before the first frame update
    void Start()
    {
        DestroyTrail();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.isMine)
            return;

        PhotonView target = collision.gameObject.GetComponent<PhotonView>();
        if(target != null && (!target.isMine || target.isSceneView))
        {
            if(target.tag == "Player")
            {
                target.RPC("DamagePlayer", PhotonTargets.AllBuffered, BulletDamage);
            }


        }
        Transform hitParticle = Instantiate(HitPrefab, transform.position, Quaternion.FromToRotation(Vector3.right, Vector3.left)) as Transform;
        Destroy(hitParticle.gameObject, 1f);
        Destroy(this.gameObject, 0f);
    }

    public void DestroyTrail()
    {
        Destroy(this.gameObject, 2f);
    }
}
