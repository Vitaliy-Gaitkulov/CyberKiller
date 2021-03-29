using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    //public Camera mainCam;

    float shakeAmount = 0;

/*    void Awake() {
        if(mainCam == null){
            Debug.Log("no mainCam");
            mainCam = Camera.main;
         }
    }*/

    public void Shake(float amt, float length){
        shakeAmount = amt;
        InvokeRepeating("DoShake", 0, 0.01f);
        Invoke("StopShake", length);
    }

    void DoShake()
    {
        if (shakeAmount > 0){
            Vector3 camPos = this.transform.position;

            float offsetX = Random.value * shakeAmount * 2 -shakeAmount;
            float offsetY = Random.value * shakeAmount * 2 -shakeAmount;

            camPos.x += offsetX;
            camPos.y += offsetY;
            Debug.Log(camPos);
            this.transform.position = camPos;
        }
    }

    void StopShake()
    {
        CancelInvoke("DoShake");
        this.transform.localPosition = Vector3.zero;
    }
}
