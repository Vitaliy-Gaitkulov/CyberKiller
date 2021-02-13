using System;
using UnityEngine;
using UnityEngine.UI;


public class PlayerMove : Photon.MonoBehaviour, IPunObservable
{

	[SerializeField] private float m_JumpForce = 300f;             
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f; 
	[SerializeField] private bool m_AirControl = false;                
	[SerializeField] private LayerMask m_WhatIsGround;
		
	[SerializeField]
	string landingSoundName = "LandingFootsteps";

	private Transform m_GroundCheck;    
	const float k_GroundedRadius = .2f; 
	private bool m_Grounded;            
	private Transform m_CeilingCheck;   
	const float k_CeilingRadius = .01f; 
	private Animator m_Anim;            
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  
	Transform playerGraphics;

	private Joystick joystick;
	private bool m_Jump;

	public PhotonView photonView;
	public Rigidbody2D rb;
	public Text PlayerNameText;

	public Transform armRotation;
	private int rotationOffset = 0;
	private Joystick FireJoystick;
	private Vector3 difference;


	private Quaternion correctArmRot;
	private Vector3 theScaleArm;
	Transform armGraphics;


	AudioManager audioManager;
	void Start() 
	{

		audioManager = AudioManager.instance;
	}

	private void Awake()
	{
    if (photonView.isMine)
    {
		//PlayerCamera.SetActive(true);
		PlayerNameText.text = PhotonNetwork.playerName;
		FireJoystick = GameObject.FindWithTag("FireJoystick").GetComponent<FixedJoystick>();
	}
	else
    {
		PlayerNameText.text = photonView.owner.name;
		PlayerNameText.color = Color.cyan;
    }

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

			difference = new Vector3(FireJoystick.Horizontal, FireJoystick.Vertical);
			difference.Normalize();
			CheckInputArm();
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
				photonView.RPC("Flip", PhotonTargets.AllBuffered);
			}
			else if (move < 0 && m_FacingRight)
			{
				photonView.RPC("Flip", PhotonTargets.AllBuffered);
			}
		}

		if (m_Grounded && jump && m_Anim.GetBool("Ground"))
		{
			m_Grounded = false;
			m_Anim.SetBool("Ground", false);
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce), ForceMode2D.Impulse);
		}
	}


	[PunRPC]
	private void Flip()
	{
		m_FacingRight = !m_FacingRight;
		Vector3 theScale = playerGraphics.localScale;
		theScale.x *= -1;
		playerGraphics.localScale = theScale;
	}


	void CheckInputArm()
	{
		float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
		armRotation.transform.rotation = Quaternion.Euler(0f, 0f, rotZ + rotationOffset);
		theScaleArm = armRotation.transform.localScale;
		if (Mathf.Abs(rotZ) > 90)
		{
			if (theScaleArm.y > 0)
			{
				theScaleArm.y *= -1;
				armRotation.transform.localScale = theScaleArm;
			}
		}
		else
		{
				theScaleArm.y = Mathf.Abs(theScaleArm.y);
				armRotation.transform.localScale = theScaleArm;
			
		};
	}

	void Update()
    {
		if (!photonView.isMine)
		{
			//armRotation.transform.rotation = Quaternion.Lerp(armRotation.transform.rotation, this.correctArmRot, Time.deltaTime * 5);
		}
	}

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
		if (stream.isWriting)
        {
			//stream.SendNext(armRotation.transform.rotation);

        }
		else if(stream.isReading)
        {
			//this.correctArmRot = (Quaternion)stream.ReceiveNext();
		}
    }
}
