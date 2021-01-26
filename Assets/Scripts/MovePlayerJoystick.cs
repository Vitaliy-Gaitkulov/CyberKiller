using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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


        private void FixedUpdate()
        {
            if (!m_Jump)
            {
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

