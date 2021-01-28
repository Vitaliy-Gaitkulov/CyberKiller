using System;
using UnityEngine;
using UnityEngine.UI;

// #pragma warning disable 649
// namespace UnityStandardAssets._2D
// {
    public class PlayerMove : Photon.MonoBehaviour
    {

    
    
    // The fastest the player can travel in the x axis.
        [SerializeField] private float m_JumpForce = 300f;                  // Amount of force added when the player jumps.
        [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask m_WhatIsGround;
        
        [SerializeField]
        string landingSoundName = "LandingFootsteps";

        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
        private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
        private bool m_FacingRight = true;  // For determining which way the player is currently facing.
        Transform playerGraphics;

        private Joystick joystick;
        private bool m_Jump;

    public PhotonView photonView;
    public Rigidbody2D rb;
    public Animation anim;
    public GameObject PlayerCamera;
    public SpriteRenderer sr;
    public Text PlayerNameText;


    AudioManager audioManager;

        void Start() 
        {
            audioManager = AudioManager.instance;
        }

        private void Awake()
        {
/*        if(photonView.isMine)
        {
            PlayerCamera.SetActive(true);
        }*/

        joystick = GameObject.FindWithTag("joystick").GetComponent<FixedJoystick>();
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            playerGraphics = transform.Find ("Graphics");
            if (playerGraphics == null)
            {
                Debug.LogError("error 'Graphics'");
            }
        }

    private void FixedUpdate()
    {
        if(photonView.isMine)
        {
            CheckInput();
        }
    }

    private void CheckInput()
    {
            if(!m_Jump)
            {
                if(joystick.Vertical > 0.4)
                {
                    m_Jump = true;
                }
            }

        bool crounh = Input.GetKey(KeyCode.LeftControl);
        float h = joystick.Horizontal;
        Move(h, crounh, m_Jump);
        m_Jump = false;

            m_Grounded = false;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                    m_Grounded = true;
            }
            if (!m_Grounded && !inAir)
            {
                inAir = true;
            }
            m_Anim.SetBool("Ground", m_Grounded);

            if (m_Grounded && inAir)
            {
                inAir = false;
                audioManager.PlaySound(landingSoundName);
            }

            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);
    }


        private bool inAir = false;


        public void Move(float move, bool crouch, bool jump)
        {
            if (!crouch && m_Anim.GetBool("Crouch"))
            {
                if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
                {
                    crouch = true;
                }
            }

            m_Anim.SetBool("Crouch", crouch);

            if (m_Grounded || m_AirControl)
            {
                move = (crouch ? move*m_CrouchSpeed : move);

                m_Anim.SetFloat("Speed", Mathf.Abs(move));

                m_Rigidbody2D.velocity = new Vector2(move*PlayerStats.instance.movementSpeed, m_Rigidbody2D.velocity.y);

                if (move > 0 && !m_FacingRight)
                {
                    Flip();
                }
                else if (move < 0 && m_FacingRight)
                {
                    Flip();
                }
            }

            if (m_Grounded && jump && m_Anim.GetBool("Ground"))
            {
                m_Grounded = false;
                m_Anim.SetBool("Ground", false);
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce), ForceMode2D.Impulse);
            }
        }
        private void Flip()
        {
            m_FacingRight = !m_FacingRight;
            Vector3 theScale = playerGraphics.localScale;
            theScale.x *= -1;
            playerGraphics.localScale = theScale;
        }
    }
