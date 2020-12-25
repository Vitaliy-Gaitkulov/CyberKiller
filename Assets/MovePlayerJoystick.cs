using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets._2D{

public class MovePlayerJoystick : MonoBehaviour
{
    public Joystick joystick;


    private PlatformerCharacter2D m_Character;
    private bool m_Jump;
    // Start is called before the first frame update
    private void Awake()
        {
            m_Character = GetComponent<PlatformerCharacter2D>();
            joystick = GameObject.FindWithTag("joystick").GetComponent<FixedJoystick>();
        }


        private void Update()
        {

        }


        private void FixedUpdate()
        {
            if (!m_Jump)
            {
                // Read the jump input in Update so button presses aren't missed.
                // m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
                // m_Jump = joystick.Vertical;
                if(joystick.Vertical > 0.4){
                    m_Jump = true;
                }
            }
            
            // Read the inputs.
            bool crouch = Input.GetKey(KeyCode.LeftControl);
            float h = joystick.Horizontal;
            // Pass all parameters to the character control script.
            m_Character.Move(h, crouch, m_Jump);
            m_Jump = false;
        }
    }
}

