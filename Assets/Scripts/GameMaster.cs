using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster gm;
    AudioSource audioData;

    void Start (){
        if (gm == null){
            gm = GameObject.FindGameObjectWithTag ("GM").GetComponent<GameMaster>();
        }
    }

    public Transform playerPrefab;
    public Transform spawnPoint;
    public float spawnDelay;
    public Transform spawnPrefab;

    public IEnumerator RespawnPlayer (){
        audioData = GetComponent < AudioSource > ();
        audioData.Play (0);
        yield return new WaitForSeconds (spawnDelay);

        Instantiate (playerPrefab, spawnPoint.position, spawnPoint.rotation);
        Transform clone = Instantiate(spawnPrefab, spawnPoint.position, spawnPoint.rotation) as Transform;
        Destroy (clone.gameObject, 3f);
    }

    public static void KillPlayer (Player player){
        Destroy (player.gameObject);
        gm.StartCoroutine (gm.RespawnPlayer());
    }

    public static void KillEnemy (Enemy enemy){
        Destroy (enemy.gameObject);
    }

}
