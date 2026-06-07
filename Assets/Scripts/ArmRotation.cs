using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmRotation : Photon.MonoBehaviour
{
    public int rotationOffset = 0;
    public Joystick FireJoystick;
    public Joystick AbilityJoystick;
    public Vector3 difference;

    void Awake()
    {
        FireJoystick = GameObject.FindWithTag("FireJoystick").GetComponent<FixedJoystick>();
        AbilityJoystick = GameObject.FindWithTag("AbilityJoystick").GetComponent<FixedJoystick>();
    }

    void Update()
    {
        if (AbilityJoystick.Horizontal != 0f || AbilityJoystick.Vertical != 0f)
        {
            CheckInputAbility();
        }
        CheckInputArm();
    }

    void CheckInputArm()
    {
        difference = new Vector3(FireJoystick.Horizontal, FireJoystick.Vertical);
        difference.Normalize();

        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + rotationOffset);
    }

    void CheckInputAbility()
    {
        difference = new Vector3(AbilityJoystick.Horizontal, AbilityJoystick.Vertical);
        difference.Normalize();

        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + rotationOffset);
    }
}
