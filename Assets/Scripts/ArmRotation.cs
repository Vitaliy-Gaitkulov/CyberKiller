using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmRotation : Photon.MonoBehaviour
{
    public int rotationOffset = 0;
    public Joystick FireJoystick;
    public Vector3 difference;

    void Awake() 
    {
        if (true)
        {
            FireJoystick = GameObject.FindWithTag("FireJoystick").GetComponent<FixedJoystick>();
        }
    }
    void Update()
    {
        if (true)
        {
            CheckInputArm();
        }

    }

    void CheckInputArm()
    {
        difference = new Vector3(FireJoystick.Horizontal, FireJoystick.Vertical);
        difference.Normalize ();

        float rotZ = Mathf.Atan2 (difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + rotationOffset);
    }
}
