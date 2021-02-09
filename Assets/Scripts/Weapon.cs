using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : Photon.MonoBehaviour, IPunObservable
{

    public Transform pistolGun;

    public float fireRate = 0;
    public int Damage = 10;
    public LayerMask whatToHit;

    public Transform Arm;

    //public GameObject BulletTrailPrefab;
    public Transform HitPrefab;
    public Transform MuzzleFlashPrefab;
    float timeToSpawnEffect = 0;
    public float effectSpawnRate = 10;

    public float camShakeAmt = 0.05f;
    public float camShakeLength = 0.1f;
    CameraShake camShake;

    public string weaponShootSound = "DefaultShot";

    float timeToFire = 0;
    public Transform firePoint;
    public Transform firePointHelper;
    public Transform aim;


    private Joystick fire;

    AudioManager audioManager;

    //public PhotonView photonView;


    void Awake()
    {
        //firePoint = transform.Find("FirePoint");
        if (firePoint == null){
            Debug.LogError ("No firePoint?");
        }
        Arm = GameObject.FindWithTag("Arm").GetComponent<Transform>();
        fire = GameObject.FindWithTag("FireJoystick").GetComponent<FixedJoystick>();
    }

    void Start(){
        camShake = GameMaster.gm.GetComponent<CameraShake>();
        if(camShake == null){
            Debug.LogError("No CameraShake script found on GM object.");
        }

        audioManager = AudioManager.instance;
        if(audioManager == null)
        {
            Debug.LogError("no audimanager found");
        }
    }

    void Update() {
        if (photonView.isMine)
        {
            var tapCount = Input.touchCount;
            if(tapCount > 1) {
                var touch1 = Input.GetTouch(0);
                var touch2 = Input.GetTouch(1);
            }
        }

    }

    void FixedUpdate()
    {
        if (photonView.isMine)
        {
            if (Mathf.Abs(fire.Horizontal) + Mathf.Abs(fire.Vertical) > 0.2){
                aim.gameObject.SetActive(true);
            }else{
                aim.gameObject.SetActive(false);
            }

            if(Mathf.Abs(fire.Horizontal) + Mathf.Abs(fire.Vertical) > 0.5 && Time.time > timeToFire){
                timeToFire = Time.time +1/fireRate;
                photonView.RPC("Shoot", PhotonTargets.AllBuffered);
            }
        }

    }
    
    [PunRPC]
    void Shoot(){
        Vector2 mousePosition = new Vector2 (firePoint.position.x, firePoint.position.y);

        Vector2 aimPointPosition = new Vector2 (aim.position.x, aim.position.y);
        Vector2 helperPointPosition = new Vector2 (firePointHelper.position.x, firePointHelper.position.y);
        RaycastHit2D hit = Physics2D.Raycast (helperPointPosition, aimPointPosition-mousePosition, 100, whatToHit);

        if (Time.time >= timeToSpawnEffect)
        {
            Vector3 hitPos;
            Vector3 hitNormal;
           /*
            if (hit.collider != null) {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null){
                    enemy.DamageEnemy (Damage);
                }
            }

            if(hit.collider == null)
            {
                hitPos = (aimPointPosition - mousePosition) * 100;
                hitNormal = new Vector3(9999, 9999, 9999);
            }else
            {
                hitPos = hit.point;
                hitNormal = hit.normal;
            }

            */

            //Effect(hitPos, hitNormal);
            timeToSpawnEffect = Time.time + 1/effectSpawnRate;




            GameObject trail = PhotonNetwork.Instantiate("BulletTrail", mousePosition, firePoint.rotation, 0) as GameObject;
            trail.GetComponent<Rigidbody2D>().AddForce((aimPointPosition - mousePosition) * 100f);

            //photonView.RPC("DestroyTrail", PhotonTargets.AllBuffered);
        }
    }

    /*
    void Effect (Vector3 hitPos, Vector3 hitNormal){
        trail = PhotonNetwork.Instantiate(BulletTrailPrefab.name, firePoint.position, firePoint.rotation, 0) as GameObject;
        LineRenderer lr = trail.GetComponent<LineRenderer>();

        if (lr != null)
        {
            lr.SetPosition(0, firePoint.position);
            lr.SetPosition(1, hitPos);
        }

        photonView.RPC("DestroyTrail", PhotonTargets.AllBuffered);

        if (hitNormal != new Vector3(9999, 9999, 9999))
        {
            Transform hitParticle = Instantiate(HitPrefab, hitPos, Quaternion.FromToRotation (Vector3.right, hitNormal)) as Transform;
            Destroy(hitParticle.gameObject, 1f);
        }

        Transform clone = Instantiate (MuzzleFlashPrefab, firePoint.position, firePoint.rotation) as Transform;
        float size = Random.Range (0.6f, 0.9f);
        clone.localScale = new Vector3 (size, size, size);
        Destroy (clone.gameObject, 0.02f);

        camShake.Shake(camShakeAmt, camShakeLength);

        audioManager.PlaySound(weaponShootSound);

    }
    

    [PunRPC]
    public void DestroyTrail()
    {
        Destroy (trail.gameObject, .1f);
    }
*/
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
           // if (trail.transform.position != null)
           // {
                //stream.SendNext(trail.transform.position.x);
                //stream.SendNext(trail.transform.position.y);
           // }
        }
        else if (stream.isReading)
        {
            
                //trail.transform.position = new Vector3((float)stream.ReceiveNext(), (float)stream.ReceiveNext(), 0);
        }
    }
}
