using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmRotation : MonoBehaviour
{
    public int rotationOffset = 0;
    public Joystick FireJoystick;
    void Awake() {
        FireJoystick = GameObject.FindWithTag("FireJoystick").GetComponent<FixedJoystick>();
        
    }
    // Update is called once per frame
    void Update()
    {
        
        // Vector3 posX = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width/10*9, Screen.height/4)) ;
        // Vector3 difference = Camera.main.ScreenToWorldPoint (new Vector3(FireJoystick.Horizontal, FireJoystick.Vertical)) - transform.position;
        // Debug.Log(FireJoystick.Horizontal);
        // Debug.Log(FireJoystick.Vertical);
        Vector3 difference = new Vector3(FireJoystick.Horizontal, FireJoystick.Vertical);

        // Vector3 difference = Camera.main.ScreenToWorldPoint (Input.mousePosition) - transform.position;
        // Debug.Log(Camera.main.ScreenToWorldPoint (Input.mousePosition) - transform.position);
        difference.Normalize ();


        float rotZ = Mathf.Atan2 (difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + rotationOffset);
    

    }
}
