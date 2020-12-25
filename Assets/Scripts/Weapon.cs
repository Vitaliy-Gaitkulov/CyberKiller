using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public float fireRate = 0;
    public int Damage = 10;
    public LayerMask whatToHit;

    public Transform BulletTrailPrefab;
    public Transform HitPrefab;
    public Transform MuzzleFlashPrefab;
    float timeToSpawnEffect = 0;
    public float effectSpawnRate = 10;

    float timeToFire = 0;
    public Transform firePoint;
    public Joystick fire;
    
    // Start is called before the first frame update
    void Awake()
    {
        firePoint = transform.Find("FirePoint");
        if (firePoint == null){
            Debug.LogError ("No firePoint?");
        }
        fire = GameObject.FindWithTag("FireJoystick").GetComponent<FixedJoystick>();
        // fire.onClick.AddListener(Shoot);
    }

    // Update is called once per frame

    void Update() {
        var tapCount = Input.touchCount;
        if(tapCount > 1) {
            var touch1 = Input.GetTouch(0);
            var touch2 = Input.GetTouch(1);
        }
    }

    void FixedUpdate()
    {
        if(fire.Horizontal >= 0.5 || fire.Vertical >= 0.5 || fire.Horizontal <= -0.5 || fire.Vertical <= -0.5){
            Shoot();
        }

        // else
        // {
        //     if (fire.Horizontal == 1 || fire.Vertical == 1 && Time.time > timeToFire){
        //         timeToFire = Time.time +1/fireRate;
        //         Shoot();
        //     }
        // }
        // if (fireRate == 0){
        //     // if (Input.GetButtonDown ("Fire1")){
        //     //     Shoot();
        //     // }
        //     fire.onClick.AddListener(Shoot);
        // }
        // else
        // {
        //     if (Input.GetButton ("Fire1") && Time.time > timeToFire){
        //         timeToFire = Time.time +1/fireRate;
        //         Shoot();
        //     }
        // }
    }

    void Shoot(){
        Vector2 mousePosition = new Vector2 (Camera.main.ScreenToWorldPoint (Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        Vector2 firePointPosition = new Vector2 (firePoint.position.x, firePoint.position.y);
        RaycastHit2D hit = Physics2D.Raycast (firePointPosition, mousePosition-firePointPosition, 100, whatToHit);

        Debug.DrawLine (firePointPosition, (mousePosition-firePointPosition), Color.white);
        if (hit.collider != null) {
            Debug.DrawLine (firePointPosition, hit.point, Color.red);
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null){
                enemy.DamageEnemy (Damage);
                Debug.Log (" We hit " + hit.collider.name + " and did " + Damage + " damage. ");
            }
        }

        if (Time.time >= timeToSpawnEffect)
        {
            Vector3 hitPos;
            Vector3 hitNormal;

            if(hit.collider == null)
            {
                hitPos = (mousePosition - firePointPosition) * 30;
                hitNormal = new Vector3(9999, 9999, 9999);
            }else
            {
                hitPos = hit.point;
                hitNormal = hit.normal;
            }

            Effect(hitPos, hitNormal);
            timeToSpawnEffect = Time.time + 1/effectSpawnRate;
        }
    }

    void Effect (Vector3 hitPos, Vector3 hitNormal){
        Transform trail = Instantiate (BulletTrailPrefab, firePoint.position, firePoint.rotation) as Transform;
        LineRenderer lr = trail.GetComponent<LineRenderer>();

        if (lr != null)
        {
            lr.SetPosition(0, firePoint.position);
            lr.SetPosition(1, hitPos);
        }

        Destroy (trail.gameObject, 0.04f);

        if (hitNormal != new Vector3(9999, 9999, 9999))
        {
            Transform hitParticle = Instantiate(HitPrefab, hitPos, Quaternion.FromToRotation (Vector3.right, hitNormal)) as Transform;
            Destroy(hitParticle.gameObject, 1f);
        }

        Transform clone = Instantiate (MuzzleFlashPrefab, firePoint.position, firePoint.rotation) as Transform;
        float size = Random.Range (0.6f, 0.9f);
        clone.localScale = new Vector3 (size, size, size);
        Destroy (clone.gameObject, 0.02f);
    }
}
